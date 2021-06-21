using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla que el agente se mueva hacia su compañero para reproducirse.
/// </summary>
[CreateAssetMenu(menuName = "Flock/Behavior/GoToPartner")]
public class GoToPartnerBehavior : FlockBehavior
{
    //Radio de aceptación
    public float radius = 1f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Calculo la dirección a la que ir
        Vector3 targetOffset = agent.partner.transform.position - agent.transform.position;
        //Cuán cerca estoy del centro (si t == 0, estoy justo en el centro, si t==1, estoy justo en el borde del radio)
        float t = targetOffset.magnitude / radius;

        //Si estoy dentro del área objetivo, me quedo en el área
        if(t<0.5f)
        {
            return Vector3.zero;
        }
        
        Vector3 final = targetOffset * (t * t); //Cuanto más lejos está, más escala
        return new Vector3(final.x, 0, final.z);
    }
}
