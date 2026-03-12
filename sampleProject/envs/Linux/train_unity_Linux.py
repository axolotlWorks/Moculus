from pathlib import Path
ENV_PATH = Path.home() / "Mice.x86_64"

MODEL_NAME = "ppo_two_cam_model"
MODEL_FILE = MODEL_NAME + ".zip"
import gymnasium as gym
from gym import spaces

import gymnasium as gymn
from shimmy.openai_gym_compatibility import GymV21CompatibilityV0 

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.envs.unity_gym_env import UnityToGymWrapper

from stable_baselines3 import PPO
from stable_baselines3.common.vec_env import DummyVecEnv, VecMonitor
import os
from stable_baselines3.common.callbacks import CheckpointCallback


class TwoCamToDict(gym.Wrapper):
    """
    Force UnityToGymWrapper multi-obs output (list/tuple: [cam0, cam1])
    to Dict: {"cam0": cam0, "cam1": cam1} on BOTH reset() and step().
    """
    def __init__(self, env: gym.Env):
        super().__init__(env)

        # Build Dict observation space
        if isinstance(env.observation_space, spaces.Tuple) and len(env.observation_space.spaces) == 2:
            cam0_space, cam1_space = env.observation_space.spaces
        else:
            # Fallback: infer from a sample reset
            obs = env.reset()
            cam0, cam1 = obs[0], obs[1]
            cam0_space = spaces.Box(low=0, high=255, shape=cam0.shape, dtype=cam0.dtype)
            cam1_space = spaces.Box(low=0, high=255, shape=cam1.shape, dtype=cam1.dtype)

        self.observation_space = spaces.Dict({"cam0": cam0_space, "cam1": cam1_space})
        self.action_space = env.action_space

    def reset(self, **kwargs):
        obs = self.env.reset(**kwargs)
        # UnityToGymWrapper returns obs (gym), not (obs, info)
        return {"cam0": obs[0], "cam1": obs[1]}

    def step(self, action):
        obs, reward, done, info = self.env.step(action)
        obs = {"cam0": obs[0], "cam1": obs[1]}
        return obs, reward, done, info




def make_gymnasium_env() -> gymn.Env:
    unity = UnityEnvironment(
        file_name=str(ENV_PATH),
        no_graphics=False,
        worker_id=0,
        timeout_wait=120,)
       
    env_gym = UnityToGymWrapper(
        unity,
        uint8_visual=True,
        allow_multiple_obs=True,   # because you have 2 cameras
        flatten_branched=True,     # your branch size 2 -> Discrete(2)
    )
    env_gym = TwoCamToDict(env_gym)  # <-- IMPORTANT: use the new wrapper
    env_gymn = GymV21CompatibilityV0(env=env_gym)

    return env_gymn

vec_env = DummyVecEnv([make_gymnasium_env])
vec_env = VecMonitor(vec_env)

if os.path.exists(MODEL_FILE):
    print("✅ Continuing training...")
    model = PPO.load(MODEL_FILE, env=vec_env)
else:
    print("🆕 Starting new training...")
    model = PPO(
        "MultiInputPolicy",
        vec_env,
        verbose=1,
        tensorboard_log="./tb_logs/",
        n_steps=1024,
        batch_size=256,
        device="cpu"
    )

checkpoint_callback = CheckpointCallback(
    save_freq=5_000,
    save_path="./checkpoints/",
    name_prefix="ppo_two_cam"
)


try:
    print("🚀 Training started... Press Ctrl+C to stop safely.")
    model.learn(total_timesteps=50_000, tb_log_name="ppo_two_cam",reset_num_timesteps=False)

except KeyboardInterrupt:
    print("\n⛔ Training interrupted! Saving model...")

    model.save(MODEL_NAME)
    print("✅ Model saved safely:", MODEL_FILE)

finally:
    vec_env.close()
    print("✅ Environment closed.")
