using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EatNode : Node
{
    //private NavMeshAgent agent;
    private FlockAgent agent;
    private bool eaten = false;

    public EatNode(FlockAgent agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        switch(agent.kind)
        {
            case AnimalKind.WOLF:
                //if(((FlockAgentWolf)agent).prey.IsDead())
                //{
                //Si sigue con bocados y no ha desaparecido
                if (((FlockAgentWolf) agent).CanTakeBite() && ((FlockAgentWolf) agent).prey != null)
                {
                    //Come
                    ((FlockAgentWolf) agent).Eat();
                    return NodeState.FAILURE;
                }
                else
                {
                    //((FlockAgentWolf) agent).prey.Dissappear();
                    ((FlockAgentWolf) agent).Regroup();
                    return NodeState.SUCCESS;
                }
                //}
                break;

            case AnimalKind.RABBIT:
                //Se borra el gameobject y el conejo come
                ((FlockAgentRabbit) agent).Eat();
                return NodeState.SUCCESS;

            case AnimalKind.NULL:
            default:
                Debug.LogError("Define the AnimalKind variable of the agent");
                return NodeState.FAILURE;
        }

        return NodeState.RUNNING;
    }
}
