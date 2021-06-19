using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseAttackNode : Node
{
    private FlockAgentWolf agent;
    private FlockAgent targetAgent;
    private Transform targetLocation;

    public ChaseAttackNode(FlockAgentWolf agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Ya he encontrado mi presa
        if (agent.prey == null)
        {
            agent.Regroup();    //La presa ha desaparecido
            return NodeState.FAILURE;
        }

        targetAgent = agent.prey;

        //Rango a partir del cual ataca a la vez que persigue
        float distance = Vector3.Distance(targetAgent.transform.position, agent.transform.position);
        if(distance < 3f)
        {
            //Seguir moviéndose

            //Si la presa se ha ocultado
            if( agent.IsPreyHidden())
            {
                //Deja de buscar
                agent.prey = null;
                agent.Regroup();
                return NodeState.FAILURE;
            }
            
            //Ataca
            agent.Attack();
            
            // Si la mata
            if (agent.IsPreyDead())
            {
                //Presa muerta - ÉXITO
                return NodeState.SUCCESS;
            }

            //La presa sigue viva
            return NodeState.FAILURE;
        }
        return NodeState.FAILURE;
    }
}
