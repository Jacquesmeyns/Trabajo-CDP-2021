using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    private FlockAgentWolf agent;
    private FlockAgent targetAgent;
    private Transform targetLocation;

    public ChaseNode(FlockAgentWolf agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        
        if(agent.prey == null)
            return NodeState.FAILURE;
        targetAgent = agent.prey;

        float distance = Vector3.Distance(targetAgent.transform.position, agent.transform.position);
        if(distance < 0.4f)
        {
            //Seguir moviéndose

            //Si la presa se ha ocultado
            if( agent.IsPreyHidden())
            {
                //Dejo de buscar
                return NodeState.FAILURE;
            }
            //Le ataco
            agent.Attack();
            
            if (agent.IsPreyDead())
            {
                //Presa muerta
                return NodeState.SUCCESS;
            }

            //La presa sigue viva
            return NodeState.RUNNING;
        }
        return NodeState.RUNNING;
    }
}
