using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToPartnerNode : Node
{
    //private NavMeshAgent agent;
    private FlockAgent _agent;
    private FlockAgent _partner;

    public GoToPartnerNode(FlockAgent agent)
    {
        _agent = agent;
        _partner = agent.partner;
    }
    
    public GoToPartnerNode(FlockAgentWolf agent)
    {
        _agent = agent;
        _partner = agent.partner;
    }

    public override NodeState Evaluate()
    {
        _partner = _agent.partner;
        if (_partner == null || !_agent.CanBreed())
        {
            _agent.Regroup();
            return NodeState.FAILURE;
        }

        float distance = Vector3.Distance(_partner.transform.position, _agent.transform.position);
        if(distance > 3f)
        {
            return NodeState.FAILURE;
        }
        else
        {
            _agent.SpawnChilds();
            ((FlockAgentWolf)_agent).Regroup();
            return NodeState.SUCCESS;
        }
    }
}