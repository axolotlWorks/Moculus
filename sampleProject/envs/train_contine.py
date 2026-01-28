import os
import time
import gym
from gym import spaces

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.envs.unity_gym_env import UnityToGymWrapper

from stable_baselines3 import PPO
from stable_baselines3.common.vec_env import DummyVecEnv


class TwoCamToDict(gym.Wrapper):
    """
    Converts UnityToGymWrapper multi-obs output (list/tuple: [cam0, cam1])
    into Dict: {"cam0": cam0, "cam1": cam1} on reset() and step().
    """
    def __init__(self, env: gym.Env):
        super().__init__(env)

        if isinstance(env.observation_space, spaces.Tuple) and len(env.observation_space.spaces) == 2:
            cam0_space, cam1_space = env.observation_space.spaces
        else:
            obs = env.reset()
            cam0, cam1 = obs[0], obs[1]
            cam0_space = spaces.Box(low=0, high=255, shape=cam0.shape, dtype=cam0.dtype)
            cam1_space = spaces.Box(low=0, high=255, shape=cam1.shape, dtype=cam1.dtype)

        self.observation_space = spaces.Dict({"cam0": cam0_space, "cam1": cam1_space})
        self.action_space = env.action_space

    def reset(self, **kwargs):
        obs = self.env.reset(**kwargs)  # UnityToGymWrapper: returns obs only
        return {"cam0": obs[0], "cam1": obs[1]}

    def step(self, action):
        obs, reward, done, info = self.env.step(action)
        obs = {"cam0": obs[0], "cam1": obs[1]}
        return obs, reward, done, info


# ---------------------- CONFIG ----------------------
ENV_PATH = r"D:\Projects\Moculus\sampleProject\envs\MiceGramophone.exe"
MODEL_PATH = r"ppo_two_cam_model.zip"

# Set to an integer to stop after N episodes, or None to run forever
NUM_EPISODES = None

# Slow down display a bit (Unity can run very fast). Set 0 for max speed.
SLEEP_SECONDS_PER_STEP = 0.0

# If True, use deterministic actions (less random, better for evaluation)
DETERMINISTIC = True
# ----------------------------------------------------


def make_env():
    unity = UnityEnvironment(file_name=ENV_PATH, no_graphics=False)
    env = UnityToGymWrapper(
        unity,
        uint8_visual=True,
        allow_multiple_obs=True,
        flatten_branched=True,  # your branch size 2 -> Discrete(2)
    )
    env = TwoCamToDict(env)
    return env


def main():
    if not os.path.exists(MODEL_PATH):
        raise FileNotFoundError(f"Model not found: {MODEL_PATH}")

    # SB3 expects a VecEnv
    vec_env = DummyVecEnv([make_env])

    # Load trained model
    model = PPO.load(MODEL_PATH, env=vec_env)

    ep = 0
    try:
        while True:
            obs = vec_env.reset()
            done = False
            ep_reward = 0.0
            ep_steps = 0

            while not done:
                action, _ = model.predict(obs, deterministic=DETERMINISTIC)
                obs, reward, done, info = vec_env.step(action)

                # DummyVecEnv returns arrays with shape (n_envs,)
                ep_reward += float(reward[0])
                ep_steps += 1

                if SLEEP_SECONDS_PER_STEP > 0:
                    time.sleep(SLEEP_SECONDS_PER_STEP)

            ep += 1
            print(f"Episode {ep} | reward={ep_reward:.3f} | steps={ep_steps}")

            if NUM_EPISODES is not None and ep >= NUM_EPISODES:
                break

    except KeyboardInterrupt:
        print("\nStopped by user (Ctrl+C).")

    finally:
        vec_env.close()


if __name__ == "__main__":
    main()