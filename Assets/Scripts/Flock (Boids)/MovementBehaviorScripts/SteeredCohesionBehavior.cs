using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla que se mantengan en la manada de una forma más suave.
/// </summary>
[CreateAssetMenu(menuName = "Flock/Behavior/Steered Cohesion")]
public class SteeredCohesionBehavior : FlockBehavior
{

    Vector3 currentVelocity;
    public float agentSmoothTime = 0.2f;

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

        //Para suavizar el movimiento. Cambia lentamente el vector hacia la dirección
        //    deseada en un tiempo determinado
        cohesionMove = Vector3.SmoothDamp(agent.transform.forward, cohesionMove, ref currentVelocity, agentSmoothTime); //.normalized?¿
        cohesionMove.y = 0f;

        return cohesionMove;
    }
}
