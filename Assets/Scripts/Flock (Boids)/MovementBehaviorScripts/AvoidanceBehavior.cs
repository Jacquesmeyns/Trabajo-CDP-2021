using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla que los agentes se eviten si se acercan demasiado entre ellos
/// </summary>
[CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]
public class AvoidanceBehavior : FlockBehavior
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Si no hay vecinos, no retornes nada
        if(context.Count == 0)
            return Vector3.zero;

        //Suma todas sus posiciones y haz la media de la dirección para evitarlos a todos
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        foreach (Transform item in context)
        {
            // Si la distancia entre dos agentes es menor que el radio de evitar
            if(Vector3.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                //Se añade que hay que evitar un agente más, en la dirección opuesta al mismo
                nAvoid++;
                //avoidanceMove += (Vector2)(agent.transform.position - item.position);
                avoidanceMove += agent.transform.position - item.position;
            }
        }

        //La media
        if(nAvoid > 0)
            avoidanceMove /= nAvoid;

        Debug.DrawRay(agent.transform.position, avoidanceMove, Color.red);
        
        return new Vector3(avoidanceMove.x, 0, avoidanceMove.z);    //y = 0 para que los agentes no salgan volando
    }
}
