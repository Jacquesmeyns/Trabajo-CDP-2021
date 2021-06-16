using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackNode : Node
{
    private FlockAgent targetAgent = new FlockAgent();

    public AttackNode(FlockAgent targetAgent)
    {
        this.targetAgent = targetAgent;
    }

    public override NodeState Evaluate()
    {
        //Cuando lo mata: success
        if(targetAgent.IsDead()){
            //targetAgent.GetComponentInChildren<Material>().SetColor("_Color",Color.red);
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
