using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNestNode : Node
{
    private FlockAgent _agent;
    // Start is called before the first frame update
    void Start(FlockAgent agent)
    {
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
        throw new System.NotImplementedException();
    }
}
