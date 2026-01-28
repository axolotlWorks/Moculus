using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System;

public class MouseAgentSimple : Agent

{

    [SerializeField] private float _moveSpeed = 2f; //can be slow

    float time = 0;




   

    public int currentEpisode = 0;
    public float cumulativeReward = 0f;

    public override void Initialize() 
    { 
        Debug.Log("MouseAgent initialized");
        currentEpisode = 0;
        cumulativeReward = 0f;
    }
    public override void OnEpisodeBegin() 
    { 
        Debug.Log("Episode begun");

        currentEpisode++;
        cumulativeReward = 0f;
   

        ResetPosition();
    }

    private void ResetPosition()
    {
        transform.localPosition = new Vector3(0, 2.43f, -37.59f);
        transform.localRotation = Quaternion.identity;
        time = 0f;
        

    }
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
    public override void OnActionReceived(ActionBuffers actionBuffers) 
    { 
        MoveAgent(actionBuffers.DiscreteActions);
        AddReward(-2f/MaxStep); // small negative reward each step to encourage faster solutions

        cumulativeReward = GetCumulativeReward();
    }

    private void MoveAgent(ActionSegment<int> discreteActions)
    {
        
        var action = discreteActions[0];

        switch (action)
        {
            //0 : Do Nothing
            case 1: // Move Forward
                transform.localPosition += transform.forward * _moveSpeed * Time.deltaTime;
                break;
            case 2: // Turn Right
                transform.localPosition -= transform.forward * _moveSpeed * Time.deltaTime;
                break;
         
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndGoal"))
        {
            AddReward(1.0f);
            cumulativeReward = GetCumulativeReward();
            //EndEpisode();
        }



    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EndGoal"))
        {
            AddReward(0.2f* Time.deltaTime);
            time += Time.deltaTime;
            //Debug.Log(time);
            if (time >= 50f)
            {
                Debug.Log("Goal reached for 5 seconds, ending episode.");
                EndEpisode();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EndGoal"))
        {
            AddReward(-0.5f);
        }
    }



    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        discreteActionsOut[0] = 0; // Do Nothing

        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = 1; // Move Forward
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] = 3; // Turn Right
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = 2; // Turn Left
        }
    }
}
