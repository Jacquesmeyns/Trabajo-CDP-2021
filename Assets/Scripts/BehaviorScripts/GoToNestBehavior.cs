using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/GoToNest")]
public class GoToNestBehavior : FlockBehavior
{
    public float radius = 5f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Calculo la dirección a la que ir
        Vector3 targetOffset = flock.nestPosition - agent.transform.position;
        //
        float t = targetOffset.magnitude / radius;
        
        /*if(t<0.5f)
        {
            //Rondando
            return Vector3.zero;
        }*/

        return new Vector3(targetOffset.x, 0, targetOffset.z);
    }
}
