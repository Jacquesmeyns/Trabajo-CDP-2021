using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToDig : Node
{
//private NavMeshAgent agent;
    private FlockAgentRabbit _agent;
    private float burrowRadius = 2f;

    public GoToDig(FlockAgentRabbit agent)
    {
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
        Vector3 targetOffset = _agent.burrowLocation.position - _agent.transform.position;

        float t = targetOffset.magnitude / burrowRadius;

        if (t < 0.9)
        {
            _agent.DigBurrow();
            return NodeState.SUCCESS;
        }

        _agent.ResetDiggingPosition();
        return NodeState.RUNNING;
    }
}
