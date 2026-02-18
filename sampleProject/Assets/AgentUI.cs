using UnityEngine;

public class AgentUI : MonoBehaviour
{
    [SerializeField] private MouseAgentSimple agent;
    [SerializeField] private TMPro.TextMeshProUGUI episodeText;
    [SerializeField] private TMPro.TextMeshProUGUI rewardText;
    [SerializeField] private TMPro.TextMeshProUGUI lickCount;

    private void Update()
    {
        episodeText.text = $"Episode: {agent.currentEpisode}" + " - Step: "+ agent.StepCount;
        rewardText.text = $"Cumulative Reward: "+agent.cumulativeReward.ToString();
        lickCount.text = $"Lick Count: "+agent.lickCount.ToString();
    }
}
