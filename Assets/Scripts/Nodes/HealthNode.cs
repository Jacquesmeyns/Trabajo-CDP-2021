using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        _nodeState = agent.currentHealth/agent.startingHealth <= threshold ? NodeState.SUCCESS : NodeState.FAILURE;
        //Si no la vida está por debajo del umbral, me pongo a buscar comida
        if(_nodeState == NodeState.SUCCESS)
            agent.Regroup();
        return _nodeState;
    }
}
