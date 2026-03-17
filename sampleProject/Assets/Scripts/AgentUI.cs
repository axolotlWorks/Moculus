using UnityEngine;

public class AgentUI : MonoBehaviour
{
    [SerializeField] private MouseAgentSimple agent;
    [SerializeField] private TMPro.TextMeshProUGUI episodeText;
    [SerializeField] private TMPro.TextMeshProUGUI rewardText;
    [SerializeField] private TMPro.TextMeshProUGUI lickCount;
    [SerializeField] private TMPro.TextMeshProUGUI lickStamina;

    /// <summary>
    /// Updates the displayed episode, step, cumulative reward, and lick count information in the user interface.
    /// </summary>
    /// <remarks>Call this method to refresh the UI elements with the latest values from the agent. This
    /// method should be invoked whenever the agent's state changes to ensure the displayed information remains
    /// accurate.</remarks>
    private void Update()
    {
        episodeText.text = $"Episode: {agent.currentEpisode}" + " - Step: "+ agent.StepCount;
        rewardText.text = $"Cumulative Reward: "+agent.cumulativeReward.ToString();
        lickCount.text = $"Lick Count: "+agent.lickCount.ToString();
        lickStamina.text = $"Lick Stamina: "+agent.lickStamina.ToString("F2");
    }
}
