using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimMeleeCommand : MonoBehaviour
{
    [SerializeField] private BatBehavior batBehavior;

    public void Attack()
    {
        batBehavior.Attack();
    }
    
    public void EndAttack()
    {
        batBehavior.EndAttack();
    }
}
