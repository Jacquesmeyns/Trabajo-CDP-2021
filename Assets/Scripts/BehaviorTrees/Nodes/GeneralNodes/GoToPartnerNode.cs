using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToPartnerNode : Node
{
    //private NavMeshAgent agent;
    private FlockAgent _agent;
    private FlockAgent _partner;

    public GoToPartnerNode(FlockAgent agent)
    {
        _agent = agent;
        _partner = agent.partner;
    }

    public override NodeState Evaluate()
    {
        _partner = _agent.partner;
        /*if (_partner == null )
        {
            _agent.Regroup();
            return NodeState.FAILURE;
        }*/

        float distance = Vector3.Distance(_partner.transform.position, _agent.transform.position);
        //Si ambos están en el nido, pueden tener a la cría
        if (_agent.InNestWithPartner(_agent.GetComponentInParent<Flock>().nestPosition))
        {
            _agent.SpawnChilds();
            _agent.Regroup();
            return NodeState.SUCCESS;
        }
        
        //Si el compañero no se ha enterado, le avisa
        if(_partner.inFlock && _partner.CanBreed())
            _partner.GoAlone();
        
        return NodeState.FAILURE;
    }
}