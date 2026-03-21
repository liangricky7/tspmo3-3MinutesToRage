using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeRecognition : MonoBehaviour
{
    private string microphone;
    private AudioClip audioClip;

    public int sampleWindow = 1024; // how many samples to measure at once; range from 64 to 8192

    public float volume;

    // Start is called before the first frame update
    void Start()
    {
        microphone = Microphone.devices[0];
        // 1 sec looping buffer
        audioClip = Microphone.Start(microphone, loop: true, lengthSec: 1, 44100);
    }

    // Update is called once per frame
    void Update()
    {
        volume = GetRMSVolume();
    }

    float GetRMSVolume()
    {
        float[] samples = new float[sampleWindow];
        int startPosition = Microphone.GetPosition(microphone) - sampleWindow;
        if (startPosition < 0) return 0f;

        audioClip.GetData(samples, startPosition);

        float sum = 0f;
        foreach (float s in samples)
        {
            sum += s * s;
        }
        return Mathf.Sqrt(sum / sampleWindow);
    }

    void OnDestroy()
    {
        Microphone.End(microphone);
    }

}
