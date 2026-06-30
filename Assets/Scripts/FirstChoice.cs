using UnityEngine;

public class FirstChoice : MonoBehaviour
{
    public CarMovement car;
    public ChoicePopup popup;
    private bool triggered = false; // prevents re-triggering


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            car.stopped = true;
            triggered = true;
            popup.ShowChoice();
        }
    }
}
