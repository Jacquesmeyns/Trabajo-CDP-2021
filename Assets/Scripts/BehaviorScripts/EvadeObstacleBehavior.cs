using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/EvadeObstacle")]
public class EvadeObstacleBehavior : FlockBehavior
{
     public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Si no hay vecinos, mantener direccion actual
        if(context.Count == 0)
            return agent.transform.forward;
        
        Vector3 evadeMove = Vector3.zero;

        //Comprobar obstáculos lanzando 5 rayos con Raycast
        //  Del 0 al 4, de izquierda a derecha siendo 2 el transform.forward
        //  para dibujar el raycast
        //Debug.DrawLine(Camera.main.ScreenPointToRay(Input.mousePosition),hit.point,Color.green);
            
        //Desplazamiento de bit para obtener la máscara de capa que queremos utilizar
        //  En este caso, la capa "Obstacle"
        int layerMask = 1 << 8;
            
        RaycastHit[] hits = new RaycastHit[5];

        for (int i = 0; i < hits.Length; i++)
        {
            Vector3 anguloVision;
            float scale = 5f;
            anguloVision = Quaternion.Euler(0, agent.angulosVision[i], 0) * agent.transform.forward;
                
            if (Physics.Raycast(agent.transform.position,anguloVision, scale,layerMask))
            {
                Debug.DrawRay(agent.transform.position,anguloVision * scale,Color.red);
                Vector3 direccion = hits[i].point - agent.transform.position;
                direccion = direccion * 0.005f;
                evadeMove += direccion;
            }
            else
            {
                Debug.DrawRay(agent.transform.position,anguloVision * scale,Color.green);
            }
        }


        return evadeMove;
    }
}
