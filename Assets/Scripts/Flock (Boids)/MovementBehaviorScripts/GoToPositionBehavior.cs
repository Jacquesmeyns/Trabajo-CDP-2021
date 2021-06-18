using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/GoToPosition")]
public class GoToPositionBehavior : FlockBehavior
{
    //Vector2 center;
    //Radio de aceptación
    public float radius = 5f;
     public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Calculo la dirección a la que ir
        Vector3 targetOffset = /*center*/ flock.targetPosition - agent.transform.position;
        //Debug.Log("Target position: " + flock.targetPosition);
        float t = targetOffset.magnitude / radius;

        //Si estoy dentro del área objetivo, me quedo rondando el área
        if(t<0.5f)
        {
            //Rondando
            return Vector3.zero;
        }

        Vector3 final = targetOffset * (t * t);
        return new Vector3(final.x, 0, final.z);
    }
}
