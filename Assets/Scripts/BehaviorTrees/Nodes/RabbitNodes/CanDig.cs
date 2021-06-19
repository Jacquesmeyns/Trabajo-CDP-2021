using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanDig : Node
{
    private FlockAgentRabbit _agent;
    
    public CanDig(FlockAgentRabbit agent)
    {
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (_agent.hasDug)
        {
            //_agent.searchingWhereToDig = false;           ?¿?¿?¿
            return NodeState.FAILURE;
        }

        return NodeState.SUCCESS;
    }
}
