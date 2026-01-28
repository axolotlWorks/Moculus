using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System;

public class MouseAgent : Agent

{

    [SerializeField] private Transform goal;
    [SerializeField] private float _moveSpeed = 2f; //can be slow
    [SerializeField] private float _rotationSpeed = 180f;

    [SerializeField] GameObject states ;

    [SerializeField] private Renderer body;

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
        body.material.color = Color.white;

        ResetPosition();
    }

    private void ResetPosition()
    {
        transform.localPosition = new Vector3(-2.472406f, -3.66071f, -49.12957f);
        transform.localRotation = Quaternion.identity;
        
        for (int i = 0; i < states.transform.childCount; i++)
        {
            var state = states.transform.GetChild(i);
            state.gameObject.SetActive(true);
        }
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
                transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime,0f);
                break;
            case 3: // Turn Left
                transform.Rotate(Vector3.up, -_rotationSpeed * Time.deltaTime,0f);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndGoal"))
        {
            AddReward(2.0f);
            cumulativeReward = GetCumulativeReward();
            EndEpisode();
        }

        if (other.CompareTag("Goal"))
        {
            AddReward(1.0f);
            cumulativeReward = GetCumulativeReward();
            // EndEpisode();
            other.gameObject.SetActive(false);
            Debug.Log("Goal");
            Debug.Log(cumulativeReward);
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.05f);
            
            body.material.color = Color.red;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.01f* Time.deltaTime);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            body.material.color = Color.white;
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
