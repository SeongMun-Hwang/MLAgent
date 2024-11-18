using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Actuators;

public class SoccerAgent : Agent
{
    [HideInInspector]
    public Team team;
    [HideInInspector]
    public Vector3 startingPos;
    [HideInInspector]
    public float moveSpeed;

    float defaultKickPower = 2000;
    float kickPower;

    Rigidbody agentRb;

    public override void Initialize()
    {
        startingPos = transform.position;
        team = (Team)GetComponent<BehaviorParameters>().TeamId;
        agentRb = GetComponent<Rigidbody>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<int> discreteAction = actions.DiscreteActions;

        int forward = actions.DiscreteActions[0];
        int strafe = actions.DiscreteActions[1];
        int rotation = actions.DiscreteActions[2];

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        kickPower = 0;

        switch (forward)
        {
            case 1:
                kickPower = 1;
                dir = transform.forward;
                break;
            case 2:
                dir = -transform.forward;
                break;
        }

        switch (strafe)
        {
            case 1:
                dir += transform.right;
                break;
            case 2:
                dir -= transform.right;
                break;
        }

        dir.Normalize();

        switch (rotation)
        {
            case 1:
                rot = transform.up;
                break;
            case 2:
                rot = -transform.up;
                break;
        }

        transform.Rotate(rot, Time.deltaTime * 100);
        agentRb.AddForce(dir * moveSpeed, ForceMode.VelocityChange);

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
        // WS forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        // AD strafe
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 1;
        }
        // QE rotate
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[2] = 2;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            //AddReward(0.02f); //공 접근을 유도하기 위한 보상
            float force = kickPower * defaultKickPower;

            Vector3 dir = collision.contacts[0].point - transform.position;
            dir.Normalize();
            collision.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
    }
}