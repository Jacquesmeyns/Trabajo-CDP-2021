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
        Vector3 targetOffset = Vector3.zero;
        //Calculo la dirección a la que ir
        if(agent.kind == AnimalKind.WOLF)
            targetOffset = CalculatePreyPosition((FlockAgentWolf) agent);
        else if (agent.kind == AnimalKind.RABBIT)
        {
            radius = 1/3f;
            targetOffset = CalculateFoodPosition((FlockAgentRabbit) agent);
        }

        float t = targetOffset.magnitude / radius;

        //Si estoy dentro del área objetivo, no hace falta moverme más
        if(t<3f)
        {
            //Rondando
            return Vector2.zero;
        }

        //Debug.Log("Esta no es la buena, primo");
        return targetOffset * (t*t);
    }

     public Vector3 CalculatePreyPosition(FlockAgentWolf agent)
    {
        //vvvvvvvvvvvvvvvvvvvvv--------------------->>>>>>>>>>>>>>>>>>>>>Esto debería ir en un nodo
        if (agent.prey == null)
        {
            agent.Regroup();
            
            return Vector3.zero;
        }
            
        //Calculo la dirección a la que ir
        Vector3 targetOffset = agent.prey.transform.position - agent.transform.position;

        //Va hacia esa posición
        return targetOffset;
    }

     public Vector3 CalculateFoodPosition(FlockAgentRabbit agent)
     {
         if (agent.food == null)
         {
             agent.Regroup();
            
             return Vector3.zero;
         }
            
         //Calculo la dirección a la que ir
         Vector3 targetOffset = agent.food.transform.position - agent.transform.position;
         
         //Va hacia esa posición
         return targetOffset;
     }

    
}
