using UnityEngine;

public class CamState : MonoBehaviour
{
    [SerializeField] Transform car;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Camera.main.transform.parent == null)
            {
                Camera.main.transform.SetParent(car);
            } else
            {
                Camera.main.transform.SetParent(null);
            }
        }
    }
}
