using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    public float defaultFOV = 95f;
    public float smoothSpeed = 8f;

    private Camera cam;
    private float targetFOV;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetFOV = defaultFOV;
        cam.fieldOfView = defaultFOV;
    }

    void Update()
    {
        // smooth transition to target FOV
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * smoothSpeed);
    }

    public void SetFOV(float newFOV)
    {
        targetFOV = newFOV;
    }

    public void ResetFOV()
    {
        targetFOV = defaultFOV;
    }
}