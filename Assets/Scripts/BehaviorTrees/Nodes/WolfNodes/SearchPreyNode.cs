using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchPreyNode : Node
{
    private FlockAgentWolf _agent;
    List<FlockAgentRabbit> conejos = new List<FlockAgentRabbit>();

    public SearchPreyNode(FlockAgentWolf agent)
    {
        this._agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Reiniciamos la lista cada vez
        conejos = new List<FlockAgentRabbit>();
        //Buscando presa

        Collider[] contextColliders = Physics.OverlapSphere(_agent.transform.position, _agent.awarenessRadius);
        //Guardamos las posiciones de todos los conejos dentro de su radio de búsqueda (los agentes 
        //  que estén dentro de su área)
        foreach (Collider c in contextColliders)
        {
            //No queremos guardar la posición del propio agente, ni la de agentes que no sean conejos
            if(c!= _agent.AgentCollider && (c.CompareTag("Rabbit") || c.CompareTag("FleeingRabbit")) )
            {
                if(!c.gameObject.GetComponent<FlockAgentRabbit>().predated)
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
            _agent.prey = closestAgent();
            //if (_agent.prey != null)
            //{
                _agent.prey.predated = true;
                _agent.GoAlone();
                return NodeState.SUCCESS;
            //}

            return NodeState.FAILURE;
        }
    }

    private FlockAgentRabbit closestAgent()
    {
        float closestDistance = 99999999f;
        FlockAgentRabbit closestRabbit = null;

        foreach (FlockAgentRabbit conejo in conejos)
        {
            float distance = Vector3.Distance(_agent.transform.position, conejo.transform.position);
                if (distance < closestDistance/* && !conejo.predated*/)
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
