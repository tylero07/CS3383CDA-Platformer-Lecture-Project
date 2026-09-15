using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public GameObject camera;
    [Range(0f, 1f)] public float parallaxEffect = 0.05f;

    private Vector3 startPosition;
    private float cameraStartX;

    void Start()
    {
        if (camera == null)
        {
            enabled = false;
            return;
        }

        startPosition = transform.position;
        cameraStartX = camera.transform.position.x;
    }

    void LateUpdate()
    {
        float cameraMovement = camera.transform.position.x - cameraStartX;

        transform.position = new Vector3(
            startPosition.x + cameraMovement * parallaxEffect,
            startPosition.y,
            startPosition.z
        );
    }
}