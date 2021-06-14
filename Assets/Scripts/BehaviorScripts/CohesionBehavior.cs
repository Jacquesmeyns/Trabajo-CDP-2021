using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Cohesion")]
public class CohesionBehavior : FlockBehavior
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Si no hay vecinos, no retornes nada
        if(context.Count == 0)
            return Vector3.zero;

        //Suma todas sus posiciones y haz la media de todas ellas para mantenerte
        //  en la bandada
        Vector3 cohesionMove = Vector3.zero;
        foreach (Transform item in context)
        {
            cohesionMove += item.position;
        }

        //La media
        cohesionMove /= context.Count;

        //Crear offset de la posicion del agente
        cohesionMove -= agent.transform.position;

        return cohesionMove;
    }
}
