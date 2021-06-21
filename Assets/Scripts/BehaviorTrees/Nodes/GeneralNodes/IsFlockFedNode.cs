/// <summary>
/// Devuelve SUCCESS cuando el hambre media de la manada está por encima del umbral de la manada.
/// </summary>
public class IsFlockFedNode : Node
{
    private FlockAgent agent;
    //Umbral de hambre
    private float flockHungerThreshold;

    public IsFlockFedNode(FlockAgent agent, float flockThreshold)
    {
        this.agent = agent;
        this.flockHungerThreshold = flockThreshold;
    }

    //Se hace la media del hambre de la bandada y si es mayor al límite establecido
    //  decimos que es una bandada bien alimentada
    public override NodeState Evaluate()
    {
        float flockHunger = 0f;
        int quantity = 0;
        foreach (FlockAgent _agent in agent.GetComponentInParent<Flock>().agents)
        {
            flockHunger += _agent.hunger;
            quantity++;
        }
        flockHunger /= (quantity*agent.startingHunger);
        _nodeState = flockHunger >= flockHungerThreshold ? NodeState.SUCCESS : NodeState.FAILURE;
        return _nodeState;
    }
}