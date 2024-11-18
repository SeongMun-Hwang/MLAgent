using UnityEngine;

public class BallController : MonoBehaviour
{
    public SoccerController soccerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BlueGoalPost")
        {
            soccerController.GoalScored(Team.Red);
        }        
        else if (other.tag == "RedGoalPost")
        {
            soccerController.GoalScored(Team.Blue);
        }
    }
}
