using UnityEngine;

/// <summary>
/// Devuelve SUCCESS si la presa/comida está dentro del rango determinado.
/// </summary>
public class RangeNode : Node
{
    private float range;
    private FlockAgent origin;

    public RangeNode(float range, FlockAgent origin)
    {
        this.range = range;
        this.origin = origin;
    }

    public override NodeState Evaluate()
    {
        //GameObject target;
        if (origin.kind == AnimalKind.WOLF)
        {
            FlockAgentRabbit target = ((FlockAgentWolf) origin).prey;
            float distance = Vector3.Distance(target.transform.position, origin.transform.position);
            _nodeState = distance <= range ? NodeState.SUCCESS : NodeState.FAILURE;
        }
        else
        {
            GameObject target = ((FlockAgentRabbit) origin).food;
            float distance = Vector3.Distance(target.transform.position, origin.transform.position);
            _nodeState = distance <= range ? NodeState.SUCCESS : NodeState.FAILURE;
        }

        
        if (nodeState == NodeState.FAILURE && origin.kind == AnimalKind.WOLF)
        {
            //Se actualiza el estado. La presa y el depredador dejan de serlo
            ((FlockAgentWolf) origin).prey.predator = null;
            ((FlockAgentWolf) origin).prey = null;
        }
        return _nodeState;
    }
}
