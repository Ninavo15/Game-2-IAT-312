using UnityEngine;
using UnityEngine.UI;

public class ClickableImage : MonoBehaviour
{
    public Button button;

    void Start()
    {
        button.onClick.AddListener(OnImageClicked);
    }

    void OnImageClicked()
    {
        Debug.Log("Image clicked!");
    }
}