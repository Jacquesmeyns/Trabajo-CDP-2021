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
        this._agent = agent;
        _partner = agent.partner;
    }

    public override NodeState Evaluate()
    {
        _partner = _agent.partner;
        if (_partner == null)
        {
            _agent.Regroup();
            return NodeState.FAILURE;
        }

        float distance = Vector3.Distance(_partner.transform.position, _agent.transform.position);
        if(distance > 0.2f)
        {
            return NodeState.RUNNING;
        }
        else
        {
            _agent.SpawnChilds();
            _agent.Regroup();
            return NodeState.SUCCESS;
        }
    }
}