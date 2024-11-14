using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToBallAgent : Agent
{
    [SerializeField] Transform targetTransform;
    [SerializeField] Material winMaterial;
    [SerializeField] Material lostMaterial;
    [SerializeField] Renderer floorRenderer;

    //���Ǽҵ� : ����/���� ����� ������ �ϳ��� ���� �Ⱓ
    public override void OnEpisodeBegin()
    {
        do
        {
            transform.localPosition = new Vector3(Random.Range(-3.5f,3.5f), 0.5f, Random.Range(-3.5f, 3.5f));
            targetTransform.localPosition = new Vector3(Random.Range(-3.5f,3.5f), 0.5f, Random.Range(-3.5f, 3.5f));
        } while (Vector3.Distance(transform.localPosition, targetTransform.localPosition) < 1.5f);
    }
    
    public override void CollectObservations(VectorSensor sensor) //������ ����
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions) //������ ������ �޾ƿ�
    {
        float x = actions.ContinuousActions[0];
        float y = actions.ContinuousActions[1];

        float moveSpeed = 2f;

        //�޾ƿ� ������� �̵�
        transform.Translate(new Vector3(x,0,y)*Time.deltaTime*moveSpeed);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //�޸���ƽ - �� ��ġ�� �����ϴ� �뵵�� ���
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("You did it!!!");
            SetReward(1f); //���� ����
            floorRenderer.material = winMaterial;
            EndEpisode(); //���Ǽҵ� ����
        }
        else if (other.CompareTag("Wall"))
        {
            Debug.Log("You did it...");
            SetReward(-1f);
            floorRenderer.material = lostMaterial;
            EndEpisode();
        }
    }
}
