using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehavior : ScriptableObject
{
    /// <summary>
    /// Calcula el movimiento a partir de la posición del agente, el contexto y la manada.
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="context"></param>
    /// <param name="flock"></param>
    /// <returns></returns>
    public abstract Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock);
}