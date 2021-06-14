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
                if(((FlockAgentWolf)agent).prey.IsDead())
                {
                    //Si sigue con bocados
                    if (((FlockAgentWolf) agent).prey.CanBeEaten())
                    {
                        return NodeState.RUNNING;
                    }
                    else
                    {
                        ((FlockAgentWolf) agent).prey.Dissappear();
                        return NodeState.SUCCESS;
                    }
                }
                break;

            case AnimalKind.RABBIT:
            //En un futuro poner zanahorias que respawneen con un CD
                agent.currentHealth+=10;
                return NodeState.SUCCESS;

            case AnimalKind.NULL:
            default:
                Debug.LogError("Define the AnimalKind variable of the agent");
                return NodeState.FAILURE;
        }

        return NodeState.RUNNING;
    }

    //Timer para que se quede unos segundos comiendo
    IEnumerator eat()
    {
        yield return new WaitForSeconds(3);
    }
    
}
