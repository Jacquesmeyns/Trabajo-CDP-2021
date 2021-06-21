using UnityEngine;

/// <summary>
/// Devuelve SUCCESS cuando está dentro de la zona en la que quiere cavar. Y cava.
/// </summary>
public class GoToDigNode : Node
{
    private FlockAgentRabbit _agent;
    private float burrowRadius = 2f;    //Radio de la zona a cavar

    public GoToDigNode(FlockAgentRabbit agent)
    {
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
        Vector3 targetOffset = _agent.burrowPosition - _agent.transform.position;

        float t = targetOffset.magnitude / burrowRadius;

        if (t < 0.9)
        {
            _agent.DigBurrow();
            return NodeState.SUCCESS;
        }

        _agent.ResetDiggingPosition();
        return NodeState.RUNNING;
    }
}
