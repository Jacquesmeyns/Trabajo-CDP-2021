using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPredatorNearNode : Node
{
    private FlockAgentRabbit _agent;
    private List<FlockAgentWolf> _predators;
    
    public IsPredatorNearNode(FlockAgentRabbit agent)
    {
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Reiniciamos la lista cada vez
    //_predators = new List<FlockAgentWolf>();
        
        //Busca todos los colliders en su radio de consciencia
        Collider[] contextColliders = Physics.OverlapSphere(_agent.transform.position, _agent.awarenessRadius);
        //Guarda las posiciones de todos los lobos dentro de su radio de búsqueda (los agentes 
        //  que estén dentro de su área)
        foreach (Collider c in contextColliders)
        {
            //No queremos guardar la posición del propio agente, ni la de agentes que no sean lobos
            if(c!= _agent.AgentCollider && (c.CompareTag("LoneWolf") || c.CompareTag("Wolf")))
            {
                //_predators.Add(c.gameObject.GetComponent<FlockAgentWolf>());
                _agent.safe = false;
                _agent.panic = true;
                return NodeState.SUCCESS;
            }
        }
        
        //No hay lobos cerca
        _agent.panic = false;
        _agent.safe = true;
        return NodeState.FAILURE;
    }
}
