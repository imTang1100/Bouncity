using UnityEngine;

public class CamCp : MonoBehaviour
{
    private new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        camera.projectionMatrix = Camera.main.projectionMatrix;
    }
}
