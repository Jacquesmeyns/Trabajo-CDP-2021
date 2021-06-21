using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla que el agente vaya hacia la dirección donde tiene que cavar.
/// </summary>
[CreateAssetMenu(menuName = "Flock/Behavior/GoDig")]
public class GoDigBehavior : FlockBehavior
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Calculo la dirección a la que ir
        Vector3 targetOffset = ((FlockAgentRabbit)agent).burrowPosition - agent.transform.position;
        //Cuán cerca estoy del centro (si t == 0, estoy justo en el centro, si t==1, estoy justo en el borde del radio)
        float t = targetOffset.magnitude / flock.nestRadius;

        //Si está a menos de 1/10 parte del radio del nido, se para
        if(t<0.1)
        {
            //Se queda cerca del centro
            return Vector3.zero;
        }

        Vector3 final = targetOffset * (t * t); //Cuanto más lejos está, más escala
        return new Vector3(final.x, 0, final.z);
    }
}
