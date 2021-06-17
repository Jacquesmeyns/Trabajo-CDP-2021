using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/GoToPartner")]
public class GoToPartnerBehavior : FlockBehavior
{
    //Radio de aceptación
    public float radius = 1f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Calculo la dirección a la que ir
        Vector3 targetOffset = agent.partner.transform.position - agent.transform.position;
        //
        float t = targetOffset.magnitude / radius;

        //Si estoy dentro del área objetivo, me quedo rondando el área
        //  ¿o no hace falta moverme más?
        /*if(t<3f)
        {
            //Rondando
            //flock.targetPosition = Vector3.zero;
            return Vector3.zero;
        }*/
        
        return new Vector3(targetOffset.x, 0, targetOffset.z);
    }

     /*public Vector3 CalculateMove(FlockAgentWolf agent)
    {
        //Calculo la dirección a la que ir
        Vector3 targetOffset = agent.prey.transform.position - agent.transform.position;
        //
        float t = targetOffset.magnitude / radius;

        //Va hacia esa posición
        return targetOffset;
    }*/

    
}
