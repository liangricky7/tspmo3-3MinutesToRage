using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimDrinkCommand : MonoBehaviour
{

    public void Drink()
    {
        Drinking.Instance.Drink();
    }

    public void EndDrink()
    {
        Drinking.Instance.EndDrink();
    }
}
