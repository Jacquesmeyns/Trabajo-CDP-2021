using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla la alineación de los agentes respecto a la manada
/// </summary>
[CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]
public class AlignmentBehavior : FlockBehavior
{
     public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Si no hay vecinos, mantener direccion actual
        if(context.Count == 0)
            return agent.transform.forward;

        //Suma todas los direcciones y haz la media
        Vector3 alignmentMove = Vector3.zero;
        foreach (Transform item in context)
        {
            alignmentMove += item.transform.forward;
        }

        //La media
        alignmentMove /= context.Count;

        return alignmentMove;
    }
}
