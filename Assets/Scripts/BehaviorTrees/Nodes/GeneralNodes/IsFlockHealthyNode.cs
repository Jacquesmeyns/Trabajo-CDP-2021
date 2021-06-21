using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Devuelve SUCCESS cuando la vida media de la manada está por encima del umbral de la manada
/// </summary>
public class IsFlockHealthyNode : Node
{
    private FlockAgent agent;
    private float flockHealthThreshold;

    public IsFlockHealthyNode(FlockAgent agent, float flockThreshold)
    {
        this.agent = agent;
        this.flockHealthThreshold = flockThreshold;
    }

    //Se hace la media de la vida de la bandada y si supera el umbral establecido
    //  decimos que es una bandada con buena salud
    public override NodeState Evaluate()
    {
        float flockHealth = 0f;
        int quantity = 0;
        foreach (FlockAgent _agent in agent.GetComponentInParent<Flock>().agents)
        {
            flockHealth += _agent.currentHealth;
            quantity++;
        }
        flockHealth /= (quantity*agent.startingHealth);
        _nodeState = flockHealth >= flockHealthThreshold ? NodeState.SUCCESS : NodeState.FAILURE;
        return _nodeState;
    }
}
