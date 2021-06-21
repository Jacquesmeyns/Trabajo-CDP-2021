/// <summary>
/// Devuelve SUCCESS cuando el agente puede cavar.
/// </summary>
public class CanDigNode : Node
{
    private FlockAgentRabbit _agent;
    
    public CanDigNode(FlockAgentRabbit agent)
    {
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (_agent.hasDug)
        {
            return NodeState.FAILURE;
        }

        return NodeState.SUCCESS;
    }
}
