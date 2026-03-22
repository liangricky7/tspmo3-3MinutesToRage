using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimGunCommand : MonoBehaviour
{
    [SerializeField] private ShootBehavior shootBehavior;

    public void Attack()
    {
        shootBehavior.Fire();
    }

    public void EndAttack()
    {
        shootBehavior.EndShoot();
    }
}
