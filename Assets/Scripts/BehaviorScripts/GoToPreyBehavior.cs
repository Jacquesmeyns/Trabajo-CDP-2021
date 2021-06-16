using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/GoToPrey")]
public class GoToPreyBehavior : FlockBehavior
{
    //Radio de aceptación
    public float radius = 1f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Calculo la dirección a la que ir
        Vector3 targetOffset = flock.targetPosition - agent.transform.position;
        //
        float t = targetOffset.magnitude / radius;

        //Si estoy dentro del área objetivo, me quedo rondando el área
        //  ¿o no hace falta moverme más?
        if(t<0.2f)
        {
            //Rondando
            flock.targetPosition = Vector3.zero;
            //return Vector2.zero;
        }

        targetOffset = calculatePreyPosition((FlockAgentWolf) agent);
        
        //Debug.Log("Esta no es la buena, primo");
        return targetOffset * (t*t);
    }

     public Vector3 calculatePreyPosition(FlockAgentWolf agent)
    {
        if (agent.prey == null)
        {
            agent.Regroup();
            return Vector3.zero;
        }
            
        //Calculo la dirección a la que ir
        Vector3 targetOffset = agent.prey.transform.position - agent.transform.position;
        //
        float t = targetOffset.magnitude / radius;

        //Debug.Log("Posicion perseguida: " + agent.prey.transform.position.ToString());

        //Va hacia esa posición
        return targetOffset;
    }

    
}
