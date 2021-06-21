/// <summary>
/// Clase nodo. Puede tener tres estados (RUNNING, SUCCESS, FAILURE) y debe tener un método Evaluate que decida
/// el estado a devolver.
/// </summary>
[System.Serializable]
public abstract class Node
{
    protected NodeState _nodeState;
    public NodeState nodeState{ get {return _nodeState;} }

    public abstract NodeState Evaluate();
}

public enum NodeState
{
    RUNNING, SUCCESS, FAILURE,
}