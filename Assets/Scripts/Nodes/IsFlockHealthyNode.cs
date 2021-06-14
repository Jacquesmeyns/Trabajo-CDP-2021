using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsFlockHealthyNode : Node
{
    private FlockAgent agent;
    private float flockThreshold;

    public IsFlockHealthyNode(FlockAgent agent, float flockThreshold)
    {
        this.agent = agent;
        this.flockThreshold = flockThreshold;
    }

    //Se hace la media de la vida de la bandada y si supera el límite establecido
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
        flockHealth /= quantity;
        _nodeState = flockHealth >= flockThreshold ? NodeState.SUCCESS : NodeState.FAILURE;
        return _nodeState;
    }
}
