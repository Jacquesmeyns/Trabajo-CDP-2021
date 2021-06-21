using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Devuelve SUCCESS cuando la vida del agente está por encima del umbral
/// </summary>
public class HealthNode : Node
{
    private FlockAgent agent;
    private float threshold;

    public HealthNode(FlockAgent agent, float threshold)
    {
        this.agent = agent;
        this.threshold = threshold;
    }
    public override NodeState Evaluate()
    {
        _nodeState = agent.currentHealth/agent.startingHealth >= threshold ? NodeState.SUCCESS : NodeState.FAILURE;
        
        //Si la vida está por debajo del umbral, no está sano
        if(_nodeState == NodeState.FAILURE)
            agent.Regroup();
        return _nodeState;
    }
}
