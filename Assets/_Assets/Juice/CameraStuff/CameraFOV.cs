using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    public float defaultFOV = 95f;
    public float sprintFOV  = 110f;
    public float slideFOV   = 120f;
    public float smoothSpeed = 8f;

    private Camera cam;
    private float targetFOV;
    private FirstPersonController fpc;

    void Start()
    {
        cam = GetComponent<Camera>();
        fpc = FindObjectOfType<FirstPersonController>();
        targetFOV = defaultFOV;
        cam.fieldOfView = defaultFOV;
    }

    void Update()
    {
        // sprint and slide FOV
        if (fpc._isSliding)
            targetFOV = slideFOV;
        else if (Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            targetFOV = sprintFOV;
        else
            targetFOV = defaultFOV;

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