using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Devuelve SUCCESS cuando la presa ha muerto.
/// </summary>
public class AttackNode : Node
{
    private FlockAgent targetAgent = new FlockAgent();

    public AttackNode(FlockAgent targetAgent)
    {
        this.targetAgent = targetAgent;
    }

    public override NodeState Evaluate()
    {
        if(targetAgent.IsDead()){
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
