using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Devuelve SUCCESS cuando ha encontrado comida.
/// </summary>
public class SearchFoodNode : Node
{

    private FlockAgentRabbit _agent;
    private List<GameObject> comidas;

    public SearchFoodNode(FlockAgentRabbit agent)
    {
        this._agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Compruebo si ya tiene comida asignada
        if (_agent.food != null)
            return NodeState.SUCCESS;

        //Reiniciamos la lista cada vez
        comidas = new List<GameObject>();
        //Buscando Comida
        
        Collider[] contextColliders = Physics.OverlapSphere(_agent.transform.position, _agent.awarenessRadius);
        //Guardamos todos los gameobjects dentro de su radio de búsqueda que tengan
        //  el tag de comida "GreenFood"
        foreach (Collider c in contextColliders)
        {
            //No queremos guardar al propio agente
            if(c!= _agent.AgentCollider && c.CompareTag("GreenFood"))
            {
                comidas.Add(c.gameObject);
            }
        }
        
        if(comidas.Count == 0)
        {
            //Seguir buscando
            return NodeState.FAILURE;
        }

        //Asignamos la comida a la que perseguir y comer en el siguiente nodo
        _agent.food = closestAgent();
        //_agent.prey.predator = _agent;
        _agent.GoAlone();
        return NodeState.SUCCESS;
   
    }

    /// <summary>
    /// Devuelve el gameobject más cercano.
    /// </summary>
    /// <returns></returns>
    private GameObject closestAgent()
    {
        float closestDistance = 99999999f;
        GameObject closestFood = null;

        foreach (GameObject comida in comidas)
        {
            float distance = Vector3.Distance(_agent.transform.position, comida.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFood = comida;
                }
        }

        return closestFood;
    }
}
