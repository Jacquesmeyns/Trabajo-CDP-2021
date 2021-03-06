using UnityEngine;

/// <summary>
/// Devuelve SUCCESS cuando el agente está dentro de la distancia mínima para comer.
/// </summary>
public class GoToEatNode : Node
{
    private FlockAgentRabbit _agent;
    private GameObject objective;
    private float eatingDistance = 2f;
    
    public GoToEatNode(FlockAgentRabbit agent)
    {
        _agent = agent;
        
    }

    public override NodeState Evaluate()
    {
        objective = _agent.food;
        
        //Se comprueba la distancia
        float distance = Vector3.Distance(_agent.transform.position, objective.transform.position);
        _nodeState = distance < eatingDistance ? NodeState.SUCCESS : NodeState.FAILURE;
        
        Debug.DrawRay(_agent.transform.position, objective.transform.position - _agent.transform.position, Color.yellow);

        return _nodeState;
    }
}
