using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/GoToPosition")]
public class GoToPositionBehavior : FlockBehavior
{
    //Vector2 center;
    //Radio de aceptación
    public float radius = 15f;
     public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Calculo la dirección a la que ir
        Vector3 targetOffset = /*center*/ flock.targetPosition - agent.transform.position;
        //
        float t = targetOffset.magnitude / radius;

        //Si estoy dentro del área objetivo, me quedo rondando el área
        //  ¿o no hace falta moverme más?
        if(t<0.5f)
        {
            //Rondando
            flock.targetPosition = Vector3.zero;
            return Vector3.zero;
        }

        Vector3 final = targetOffset * (t * t);
        return new Vector3(final.x, 0, final.z);
    }
}
