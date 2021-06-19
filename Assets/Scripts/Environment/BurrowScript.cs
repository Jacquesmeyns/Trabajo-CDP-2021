using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowScript : MonoBehaviour
{
    private int maxCapacity;

    private int currentRabbits;

    private void Awake()
    {
        currentRabbits = 0;
    }

    /// <summary>
    /// Devuelve true si puede entrar otro conejo más
    /// </summary>
    /// <returns></returns>
    public bool EnterBurrow()
    {
        if (currentRabbits < maxCapacity)
        {
            currentRabbits++;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Disminuye en 1 la cantidad de conejos que hay dentro
    /// </summary>
    /// <returns></returns>
    public void ExitBurrow()
    {
        currentRabbits--;
    }
}
