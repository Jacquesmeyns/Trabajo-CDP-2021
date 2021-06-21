using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Devuelve el estado inverso del nodo pasado por parámetro. Si dicho nodo devuelve Running, también devuelve Running.
/// </summary>
public class Inverter : Node
{
    protected Node node;

    public Inverter(Node node)
    {
        this.node = node;
    }
    public override NodeState Evaluate()
    {
        switch (node.Evaluate())
        {
            case NodeState.RUNNING:
                _nodeState = NodeState.RUNNING;
                break;
            case NodeState.SUCCESS:
                _nodeState = NodeState.FAILURE;
                break;
            case NodeState.FAILURE:
                _nodeState = NodeState.SUCCESS;
                break;
            default:
                break;
        }
        return _nodeState;
    }
}
