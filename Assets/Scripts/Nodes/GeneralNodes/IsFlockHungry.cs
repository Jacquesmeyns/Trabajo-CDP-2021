using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsFlockHungryNode : Node
{
    private FlockAgent agent;
    //Umbral de hambre
    private float flockThreshold;

    public IsFlockHungryNode(FlockAgent agent, float flockThreshold)
    {
        this.agent = agent;
        this.flockThreshold = flockThreshold;
    }

    //Se hace la media del hambre de la bandada y si es menor al límite establecido
    //  decimos que es una bandada con hambre.
    public override NodeState Evaluate()
    {
        float flockHunger = 0f;
        int quantity = 0;
        foreach (FlockAgent _agent in agent.GetComponentInParent<Flock>().agents)
        {
            flockHunger += _agent.hunger;
            quantity++;
        }
        flockHunger /= (quantity*agent.startingHunger);
        _nodeState = flockHunger <= flockThreshold ? NodeState.SUCCESS : NodeState.FAILURE;
        return _nodeState;
    }
}