using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

public class ThreePointsAgent : Agent
{
    [SerializeField] List<GameObject> points;
    [SerializeField] Renderer floorRenderer;
    [SerializeField] Material winMaterial;
    [SerializeField] Material loseMaterial;

    public int nextPoint;
    public int interactionPoint;

    int inThisArea = -1;

    bool pressedInteractionInThisFrame = false;

    public override void OnEpisodeBegin()
    {
        nextPoint = 0;
        interactionPoint = -1;
        inThisArea = -1;
        transform.localPosition = new Vector3(0, 0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            pressedInteractionInThisFrame = true;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = (pressedInteractionInThisFrame ? 1 : 0);
        pressedInteractionInThisFrame = false;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 3f;

        transform.Translate(new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime);

        int interaction = actions.DiscreteActions[0];

        if (interaction == 1)
        {
            Interact();
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(inThisArea);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(points[nextPoint].transform.localPosition);
        sensor.AddObservation(points[nextPoint].transform.localPosition - transform.localPosition);
    }

    void Interact() // ��ȣ�ۿ�Ű�� ���ȴµ�
    {
        if (interactionPoint == nextPoint) // ���� ���ͷ����� �ؾ��ϴ� ��Ȳ�̶��
        {
            Collider[] overlapped = Physics.OverlapBox(transform.position, Vector3.one / 2);
            Debug.Log("overlapped : " + overlapped.Length);
            if (overlapped.Length > 0)
            {
                foreach (var collider in overlapped)
                {
                    if (collider.isTrigger)
                    {
                        if (collider.gameObject == points[interactionPoint]) // �׸��� �´� �������� ��ȣ�ۿ��� �ߴٸ�
                        {
                            Debug.Log("Pillar Touch");
                            AddReward(4f);
                            nextPoint++;
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("No need to toucj");
                AddReward(-0.1f);
            }
        }
        else // ������ �׳� �� ������ ��ȣ�ۿ�
        {
            Debug.Log("Where is here");
            AddReward(-0.1f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Point") // ����Ʈ�� �ε�������
        {
            inThisArea = points.IndexOf(other.gameObject);

            if (other.gameObject == points[nextPoint]) // ���� ��ǥ�����̸�
            {
                if (nextPoint == interactionPoint) // �ٵ� �̹� ���Դ� ���̸�
                {
                    Debug.Log("Already entered");
                    AddReward(-0.5f);
                }
                else // ó�� �� ���°Ŷ��
                {
                    Debug.Log("First touch");
                    AddReward(1f);
                    interactionPoint = nextPoint;

                    if (nextPoint == points.Count - 1) // ������ ����Ʈ��
                    {
                        Debug.Log("Mission Clear");
                        AddReward(4f);
                        floorRenderer.material = winMaterial;
                        EndEpisode();
                    }
                }
            }
            else // �׳� �߸��� ������
            {
                Debug.Log("Wrong Order");
                AddReward(-1f);
                floorRenderer.material = loseMaterial;
                EndEpisode();
            }
        }
        else if (other.tag == "Wall")  // ���̸�
        {
            Debug.Log("Cube in wall");
            AddReward(-4);
            floorRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Point")
        {
            inThisArea = -1;
        }
    }
}
