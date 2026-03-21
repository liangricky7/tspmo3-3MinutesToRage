using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCircle : MonoBehaviour
{
    [SerializeField] private float timeToExpire = 3f;
    [SerializeField] private GameObject ringUIPrefab;

    private float timer = 0f;
    private bool isResolved = false;
    private GameObject ringInstance;
    private float startScale;

    void Start()
    {
        ringInstance = Instantiate(ringUIPrefab, transform.position, Quaternion.identity);
        // no SetParent this time
        startScale = GetScaleForDistance();
        ringInstance.transform.localScale = Vector3.one * startScale;
    }

    float GetScaleForDistance()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        return distance * 0.006f;
    }

    void Update()
    {
        if (isResolved) return;

        // follow cube and always offset toward camera
        ringInstance.transform.position = transform.position + (-Camera.main.transform.forward * 0.7f);
        ringInstance.transform.rotation = Camera.main.transform.rotation;

        timer += Time.deltaTime;
        float progress = timer / timeToExpire;
        float scale = Mathf.Lerp(startScale, 0f, progress);
        ringInstance.transform.localScale = Vector3.one * scale;

        if (timer >= timeToExpire)
            Expire();
    }

    public bool TryHit()
    {
        if (isResolved) return false;
        isResolved = true;
        Destroy(ringInstance);
        return true;
    }

    void Expire()
    {
        isResolved = true;
        EnergyMeter.Instance.AddEnergy(-0.3f);
        Destroy(ringInstance);
    }
}