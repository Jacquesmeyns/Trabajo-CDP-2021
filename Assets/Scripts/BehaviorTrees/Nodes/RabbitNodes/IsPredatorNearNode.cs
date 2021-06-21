using UnityEngine;

/// <summary>
/// Devuelve SUCCESS cuando hay un depredador cerca del radio de consciencia del agente (conejo).
/// </summary>
public class IsPredatorNearNode : Node
{
    private FlockAgentRabbit _agent;
    
    public IsPredatorNearNode(FlockAgentRabbit agent)
    {
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
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
                //_agent.safe = false;
                _agent.panic = true;
                return NodeState.SUCCESS;
            }
        }
        
        //No hay lobos cerca
        _agent.panic = false;
        return NodeState.FAILURE;
    }
}
