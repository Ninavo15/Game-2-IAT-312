using UnityEngine;

// the wipping sprite.
public class WipeCursor : MonoBehaviour
{
    public Camera cam;
    [Range(0.5f, 1f)] public float pressedScale = 0.85f;

    Vector3 baseScale;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        baseScale = transform.localScale;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = -cam.transform.position.z;
        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        world.z = 0f;
        transform.position = world;

        transform.localScale = Input.GetMouseButton(0) ? baseScale * pressedScale : baseScale;
    }

    void OnDestroy()
    {
        Cursor.visible = true;
    }

    public Vector3 GetWorldPosition() => transform.position;
}
