using System.IO;
using System.Text;
using UnityEngine;

public class CSVExporter : MonoBehaviour
{
    [SerializeField] MouseAgentSimple mouseAgent;

    public bool StartNewFile = true;

    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "learning.csv");
    }


    private void Update()
    {
        string line="";

       

                
        if (StartNewFile)
        {
            // If the file doesn’t exist yet, write a header firs
            File.WriteAllText(filePath, string.Empty, Encoding.UTF8);
            var sb = new StringBuilder();
            sb.AppendLine("Step,Episode,ActionChoice,CommulativeReward,LickCount,InEndZone");
            //sb.Append(line);
            File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
            StartNewFile = false;

        }
        else
        {
            switch (mouseAgent.TakenAction)
            {

                case 1: // Move Forward
                    line = mouseAgent.StepCount.ToString() + "," + mouseAgent.currentEpisode + "," + "Forward" + "," + mouseAgent.cumulativeReward + "," + mouseAgent.lickCount + "," + mouseAgent.inTheZone + "\n";
                    break;
                case 0: // do nothing
                    line = mouseAgent.StepCount.ToString() + "," + mouseAgent.currentEpisode + "," + "Stay" + "," + mouseAgent.cumulativeReward + "," + mouseAgent.lickCount + "," + mouseAgent.inTheZone + "\n";
                    break;
                case 2: // lick
                    line = mouseAgent.StepCount.ToString() + "," + mouseAgent.currentEpisode + "," + "Lick" + "," + mouseAgent.cumulativeReward + "," + mouseAgent.lickCount + "," + mouseAgent.inTheZone + "\n";
                    break;
            }
            File.AppendAllText(filePath, line, Encoding.UTF8);

        }
    }
}
