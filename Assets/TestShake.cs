using UnityEngine;

public class TestShake : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CameraEvents.TriggerShake(0.3f, 0.2f);
        }
    }
}