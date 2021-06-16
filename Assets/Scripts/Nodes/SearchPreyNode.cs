using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchPreyNode : Node
{
    private FlockAgentWolf agent;
    List<FlockAgentRabbit> conejos = new List<FlockAgentRabbit>();

    public SearchPreyNode(FlockAgentWolf agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Buscando presa

        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, agent.awarenessRadius);
        //Guardamos las posiciones de todos los conejos dentro de su radio de búsqueda (los agentes 
        //  que estén dentro de su área)
        foreach (Collider c in contextColliders)
        {
            //No queremos guardar la posición del propio agente, ni la de agentes que no sean conejos
            if(c!= agent.AgentCollider && (c.CompareTag("Rabbit") || c.CompareTag("FleeingRabbit")) )
            {
                conejos.Add(c.gameObject.GetComponent<FlockAgentRabbit>());
            }
        }
        
        if(conejos.Count == 0)
        {
            //Seguir buscando
            //Debug.Log("Buscando conejos");
            return NodeState.FAILURE;
        }
        else
        {
            //Asignamos la presa a la que perseguir y atacar en el siguiente nodo
            agent.prey = closestAgent();
            agent.GoAlone();
            return NodeState.SUCCESS;
        }
    }

        private FlockAgentRabbit closestAgent()
    {
        float closestDistance = 99999999f;
        FlockAgentRabbit closestRabbit = null;

        foreach (FlockAgentRabbit conejo in conejos)
        {
            float distance = Vector3.Distance(agent.transform.position, conejo.transform.position);
            if( distance < closestDistance)
            {
                closestDistance = distance;
                closestRabbit = conejo;
            }
        }
        
        if(closestRabbit == null)
            Debug.LogError("ESTE CONEJO NO ES UN CONEJO");

        return closestRabbit;
    }
}
