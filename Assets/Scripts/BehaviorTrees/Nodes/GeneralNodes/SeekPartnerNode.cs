using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Devuelve SUCCESS cuando encuentra un compañero con el que reproducirse
/// </summary>
public class SeekPartnerNode : Node
{
    //private NavMeshAgent agent;
    private FlockAgent agent;
    List<Collider> agents = new List<Collider>();

    public SeekPartnerNode(FlockAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Si ya tiene una pareja, exito
        if (agent.partner != null)
        {
            return NodeState.SUCCESS;
        }

        //Reiniciamos la lista de agentes
        agents = new List<Collider>();

        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, agent.awarenessRadius);
        
        //Guardamos las posiciones de todos los agentes dentro de su radio de búsqueda (los agentes 
        //  que estén dentro de su área)
        foreach (Collider c in contextColliders)
        {
            //No queremos guardar la posición del propio agente, 
            //  ni la de agentes que no sean de su tipo ni de los que no puedan criar
            if(c!= agent.AgentCollider && (c.tag.CompareTo(agent.tag) == 0)
                && c.gameObject.GetComponent<FlockAgent>().CanBreed() && c.gameObject != agent.gameObject)
            {
                agents.Add(c);
            }
        }

        //Si hay, busco el que más cerca esté
        if(agents.Count !=0)
        {
            if (MateClosestBreedableAgent())
            {
                //Los dos se salen de la manada y se buscan
                return NodeState.SUCCESS;
            }
            else
                return NodeState.FAILURE;
        }
        else
            return NodeState.FAILURE;

        
    }

    /// <summary>
    /// Devuelve al agente con el que se pueda reproducir más cercano
    /// </summary>
    /// <returns></returns>
    private bool MateClosestBreedableAgent()
    {
        Transform closestPosition = null;

        agents.Sort((agent1, agent2) => CompareDistance(agent1, agent2));
        
        foreach (Collider c in agents)
        {
            //Cuando encuentra un agente con el que tener crías, sale del bucle
            if (c.gameObject.GetComponent<FlockAgent>().PartnerWith(agent))
            {
                return true;
            }

            //Si no puede, prueba con el siguiente más cercano
        }

        //Si ninguno ha valido, no puede hacer nada
        return false;
    }

   /// <summary>
   ///  Compara la cercanía al agente de dos colliders
   /// </summary>
   /// <param name="trans1">El collider del primer agente a comparar</param>
   /// <param name="trans2">El collider del segundo agente a comparar</param>
   /// <returns></returns>
    private int CompareDistance(Collider trans1, Collider trans2)
    {
        float dist1 = Vector3.Distance(agent.transform.position, trans1.transform.position);
        float dist2 = Vector3.Distance(agent.transform.position, trans2.transform.position);
        if (dist1 < dist2)
            return -1;
        if (dist1 > dist2)
            return 1;
        return 0;
    }
}
