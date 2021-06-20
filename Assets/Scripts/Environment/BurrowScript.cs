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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("A");
        if ((other.CompareTag("Rabbit") || other.CompareTag("FleeingRabbit")) && EnterBurrow())
        {
            other.transform.GetComponent<FlockAgentRabbit>().safe = true;
            other.transform.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rabbit") || other.CompareTag("FleeingRabbit"))
        {
            other.transform.GetComponent<FlockAgentRabbit>().safe = false;
            other.transform.GetComponentInChildren<MeshRenderer>().enabled = true;
            ExitBurrow();
        }
    }
}
