using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System;
using System.Security.AccessControl;

public class MouseAgentSimple : Agent

{

    [SerializeField] private float _moveSpeed = 5f; //Modife from editor to optimize 

    
    public int lickCount = 0;
    private bool inTheZone = false;
    private int lickInZone = 0;

    public int currentEpisode = 0;
    public float cumulativeReward = 0f;

    /// <summary>
    /// Initializes the agent.
    /// </summary>
    public override void Initialize() 
    { 
        Debug.Log("MouseAgent initialized");
        currentEpisode = 0;
        cumulativeReward = 0f;
        lickInZone= 0;
        lickCount = 0;
    }

    /// <summary>
    /// Resets the agent's state and environment at the beginning of each new episode.
    /// </summary>
    /// <remarks>This method is called automatically at the start of every episode to prepare the agent for a
    /// new training or evaluation cycle. Override this method to implement custom reset logic as needed.</remarks>
    public override void OnEpisodeBegin() 
    { 
        Debug.Log("Episode begun");

        currentEpisode++;
        cumulativeReward = 0f;
        lickInZone = 0;
        lickCount = 0;


        ResetPosition();
    }

    private void ResetPosition()
    {
        transform.localPosition = new Vector3(0, 2.43f, -37.59f);
        transform.localRotation = Quaternion.identity;
        
    }

    // Observations are not used in this implementation, because we is CameraSensor for observation, but this is an example to use Vector Observation.
    /*
    public override void CollectObservations(VectorSensor sensor) 
    { 
        float goalPoxX_norm = goal.localPosition.normalized.x;
        float goalPoxZ_norm = goal.localPosition.normalized.z;

        float mousePosX_norm = transform.localPosition.normalized.x; 
        float mousePosZ_norm = transform.localPosition.normalized.z;

        float mouseRotY_norm = (transform.localRotation.eulerAngles.y / 360f)*2f-1f;

        sensor.AddObservation(goalPoxX_norm);
        sensor.AddObservation(goalPoxZ_norm);
        sensor.AddObservation(mousePosX_norm);
        sensor.AddObservation(mousePosZ_norm);
        sensor.AddObservation(mouseRotY_norm);
    }*/

    /// <summary>
    /// Processes the actions received from the agent's policy or heuristic and updates the agent's state accordingly.
    /// </summary>
    /// <remarks>This method is called at each simulation step to apply the received actions.</remarks>
    /// <param name="actionBuffers">The actions to be applied to the agent, typically provided by the policy or heuristic. Contains discrete or
    /// continuous action values as defined by the agent's action space.</param>
    public override void OnActionReceived(ActionBuffers actionBuffers) 
    { 
        MoveAgent(actionBuffers.DiscreteActions);
        AddReward(-2f/MaxStep); // small negative reward each step to encourage faster solutions

        cumulativeReward = GetCumulativeReward();
    }


    /// <summary>
    /// Executes an action for the agent based on the provided discrete action input.
    /// </summary>
    /// <remarks>The method interprets the first value in the action segment to determine the agent's movement
    /// or interaction. If the lick action is performed while the agent is in the designated zone, a reward is granted
    /// every third successful lick.
    /// NOTE: Backwards movement is currently unused, because the branch size i set to 3 in the editor. This helps the mouse to move forward and reach the goal faster for testing.</remarks>
    /// <param name="discreteActions">A segment containing the discrete action to perform. The first element specifies the action: 0 to do nothing, 1
    /// to move forward, 2 to perform a lick action, or 3 to move backward.</param>
    private void MoveAgent(ActionSegment<int> discreteActions)
    {
        
        var action = discreteActions[0];

        switch (action)
        {
            //0 : Do Nothing
            case 1: // Move Forward
                transform.localPosition += transform.forward * _moveSpeed * Time.deltaTime;
                break;
            case 3: // move back
                transform.localPosition -= transform.forward * _moveSpeed * Time.deltaTime;
                break;
            case 2: // lick
                lickCount++; 
                if(inTheZone) 
                { 
                    lickInZone++;
                    if (lickInZone %3 == 0)
                    {
                        AddReward(1.5f);

                    }
                }
                break;

        }
    }
     /// <summary>
     /// Handles the event when another collider enters the trigger collider attached to this object.
     /// </summary>
     /// <remarks>If the entering collider is tagged as "EndGoal", this method updates the agent's state and
     /// rewards accordingly. This method is typically used in Unity to detect when an agent reaches a specific goal
     /// area.
     /// We reward the mouse for entering the goal area.</remarks>
     /// <param name="other">The other collider that enters the trigger collider.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndGoal"))
        {
            inTheZone= true;
            AddReward(1.0f);
            cumulativeReward = GetCumulativeReward();
            //EndEpisode();
        }
    }
     /// <summary>
     /// Handles logic when another collider exits the trigger collider attached to this object.
     /// </summary>
     /// <remarks>This method is typically used in Unity to detect when an object leaves a designated trigger
     /// zone. It is called automatically by the Unity engine when a collider exits the trigger.
     /// We punish the mouse for leaving the goal area.</remarks>
     /// <param name="other">The collider that has exited the trigger area.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EndGoal"))
        {
            inTheZone = false;
            AddReward(-0.5f);
        }
    }


    /// <summary>
    /// Provides a heuristic for selecting actions based on user keyboard input, typically for manual agent control
    /// during testing or debugging.
    /// </summary>
    /// <remarks>This method enables manual control of the agent using the keyboard when running in heuristic
    /// mode. The Up, Down, and Left arrow keys correspond to specific discrete actions. If no relevant key is pressed,
    /// the agent performs a 'Do Nothing' action.
    /// To use this set the behavior type to heuristic. Useful for testing that the mouse moves as intended.</remarks>
    /// <param name="actionsOut">The output structure that receives the selected discrete action, which is determined by the current state of the
    /// arrow keys.</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        discreteActionsOut[0] = 0; // Do Nothing

        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = 1; // Move Forward
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = 2; // Lick
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActionsOut[0] = 3; // Move Backward
        }
    }
}
