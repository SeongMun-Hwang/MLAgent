using UnityEngine;
using Unity.MLAgents;
using System.Collections.Generic;
using TMPro;

public enum Team
{
    Blue,
    Red
}
public class SoccerController : MonoBehaviour
{
    public int maxSteps = 10000;
    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRb;
    Vector3 ballStartPos;
    public List<SoccerAgent> agents = new List<SoccerAgent>();
    public float agentSpeed = 1;

    SimpleMultiAgentGroup blueAgentGroup;
    SimpleMultiAgentGroup redAgentGroup;

    int stepCounter;

    public TextMeshPro scoreText;
    int redTeamScore = 0;
    int blueTeamScore = 0;

    private void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        ballStartPos = ball.transform.position;

        blueAgentGroup = new SimpleMultiAgentGroup();
        redAgentGroup = new SimpleMultiAgentGroup();

        foreach (var agent in agents)
        {
            agent.moveSpeed = agentSpeed;
            if (agent.team == Team.Blue)
            {
                blueAgentGroup.RegisterAgent(agent);
            }
            else
            {
                redAgentGroup.RegisterAgent(agent);
            }
        }
        InitEnvironment();
    }

    private void FixedUpdate()
    {
        stepCounter++;
        if (stepCounter > maxSteps && maxSteps > 0) //�ð����� ������ ������
        {
            blueAgentGroup.GroupEpisodeInterrupted(); //��ȿ
            redAgentGroup.GroupEpisodeInterrupted();
            InitEnvironment();
        }
        scoreText.text = blueTeamScore + " : " + redTeamScore;
    }
    public void GoalScored(Team scoredTeam)
    {
        if (scoredTeam == Team.Blue)
        {
            //���� ����
            blueAgentGroup.AddGroupReward(1 - (float)stepCounter / maxSteps); //�� �ִµ� �ɸ� �ð� ���
            blueTeamScore++;
            redAgentGroup.AddGroupReward(-1);
        }
        else
        {
            redAgentGroup.AddGroupReward(1 - (float)stepCounter / maxSteps);
            redTeamScore++;
            blueAgentGroup.AddGroupReward(-1);
        }
        redAgentGroup.EndGroupEpisode();
        blueAgentGroup.EndGroupEpisode();

        InitEnvironment();
    }
    public void InitEnvironment() //���Ǽҵ� ���� �� ���� ��ġ ����
    {
        stepCounter = 0;
        foreach (var agent in agents)
        {
            agent.transform.position = agent.startingPos + new Vector3(0, 0, Random.Range(-0.2f, 0.2f));
            float rot = (agent.team == Team.Blue) ? Random.Range(80f, 100f) : Random.Range(-80f, -100f);
            agent.transform.rotation = Quaternion.Euler(0, rot, 0);
            agent.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        float bx = Random.Range(-1f, 1f);
        float by = Random.Range(-1f, 1f);
        ball.transform.position = ballStartPos + new Vector3(bx, 0.4f, by);
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }
}
