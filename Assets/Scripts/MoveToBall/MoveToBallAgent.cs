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

    //에피소드 : 성공/실패 결과가 나오는 하나의 실행 기간
    public override void OnEpisodeBegin()
    {
        do
        {
            transform.localPosition = new Vector3(Random.Range(-3.5f,3.5f), 0.5f, Random.Range(-3.5f, 3.5f));
            targetTransform.localPosition = new Vector3(Random.Range(-3.5f,3.5f), 0.5f, Random.Range(-3.5f, 3.5f));
        } while (Vector3.Distance(transform.localPosition, targetTransform.localPosition) < 1.5f);
    }
    
    public override void CollectObservations(VectorSensor sensor) //관찰할 정보
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions) //관찰한 정보를 받아옴
    {
        float x = actions.ContinuousActions[0];
        float y = actions.ContinuousActions[1];

        float moveSpeed = 2f;

        //받아온 정보대로 이동
        transform.Translate(new Vector3(x,0,y)*Time.deltaTime*moveSpeed);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //휴리스틱 - 이 장치를 검증하는 용도로 사용
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("You did it!!!");
            SetReward(1f); //보상 설정
            floorRenderer.material = winMaterial;
            EndEpisode(); //에피소드 종료
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
