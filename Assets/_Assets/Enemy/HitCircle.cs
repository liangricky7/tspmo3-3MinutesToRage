using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCircle : MonoBehaviour
{
    [SerializeField] private GameObject ringUIPrefab;
    [SerializeField] private float triggerDistance;


    private bool isResolved = false;
    private GameObject ringInstance;
    private float startScale;
    private Transform player;
    private float maxDistance;

    void Start()
    {
        triggerDistance = GetComponent<EnemyBehavior>().stoppingDistance;
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        ringInstance = Instantiate(ringUIPrefab, transform.position, Quaternion.identity);
        startScale = GetScaleForDistance();
        ringInstance.transform.localScale = Vector3.one * startScale;

        maxDistance = Vector3.Distance(transform.position, player.position);
    }

    float GetScaleForDistance()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        return distance * 0.006f;
    }

    void Update()
    {
        if (isResolved || player == null) return;

        ringInstance.transform.position = transform.position + (-Camera.main.transform.forward * 0.7f);
        ringInstance.transform.rotation = Camera.main.transform.rotation;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        float progress = Mathf.InverseLerp(triggerDistance, maxDistance, distToPlayer);
        ringInstance.transform.localScale = Vector3.one * (startScale * progress);

        if (distToPlayer <= triggerDistance)
            Expire();
    }

    public bool TryHit()
    {
        if (isResolved) return false;
        isResolved = true;
        Destroy(ringInstance);
        return true;
    }

    public void DestroyEnemy()
    {
        if (ringInstance != null) Destroy(ringInstance);
        Destroy(gameObject); // destroys the root enemy
    }

    void Expire()
    {
        isResolved = true;
        if (ringInstance != null) Destroy(ringInstance);
        EnergyMeter.Instance.AddEnergy(-20f);
        Destroy(gameObject);
    }
}