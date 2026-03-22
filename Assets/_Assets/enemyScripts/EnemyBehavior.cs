using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float stoppingDistance = 1.5f;

    [HideInInspector] public bool isGrappled = false;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || isGrappled) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > stoppingDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}