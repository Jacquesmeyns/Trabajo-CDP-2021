using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BreedNode : Node
{
    //private NavMeshAgent agent;
    private FlockAgent agent;
    List<Collider2D> agents = new List<Collider2D>();

    public BreedNode(FlockAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Cuando es CIAN, está buscando compañero para tener crías
        //agent.GetComponentInChildren<Material>().SetColor("_Color",Color.cyan);

        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, agent.awarenessRadius);
        
        //Guardamos las posiciones de todos los agentes dentro de su radio de búsqueda (los agentes 
        //  que estén dentro de su área)
        foreach (Collider2D c in contextColliders)
        {
            //No queremos guardar la posición del propio agente, 
            //  ni la de agentes que no sean de su tipo ni de los que no puedan criar
            if(c!= agent.AgentCollider 
                && c.gameObject.GetComponent<FlockAgent>().kind == agent.kind
                && c.gameObject.GetComponent<FlockAgent>().CanBreed() )
            {
                agents.Add(c);
            }
        }

        //Si hay, busco el que más cerca esté
        if(agents.Count !=0){
            agent.partnerPosition = closestBreedableAgent();
            return NodeState.SUCCESS;
        }
        else
            return NodeState.RUNNING;

        
    }

    private Transform closestBreedableAgent()
    {
        float closestDistance = 99999999f;
        Transform closestPosition = null;

        foreach (Collider2D c in agents)
        {
            float distance = Vector3.Distance(agent.transform.position, c.transform.position);
            if( distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = c.transform;
            }
        }

        return closestPosition;
    }
}
