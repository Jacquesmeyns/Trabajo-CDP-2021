using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/EvadeObstacle")]
public class EvadeObstacleBehavior : FlockBehavior
{
     public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 evadeMove = Vector3.zero;

        //Comprobar obstáculos lanzando 5 rayos con Raycast
        //  Del 0 al 4, de izquierda a derecha siendo 2 el transform.forward
        //  para dibujar el raycast
        //Debug.DrawLine(Camera.main.ScreenPointToRay(Input.mousePosition),hit.point,Color.green);
            
        //Desplazamiento de bit para obtener la máscara de capa que queremos utilizar
        //  En este caso, la capa "Obstacle"
        int layerMask = 1 << 8;
            
        //Voy a lanzar cinco rayos
        RaycastHit[] hits = new RaycastHit[agent.angulosVision.Length];

        for (int i = 0; i < hits.Length; i++)
        {
            Vector3 anguloVision;
            float maxDistance = 1f;
            anguloVision = Quaternion.Euler(0, agent.angulosVision[i], 0) * agent.transform.forward;

            //Compruebo colisiones con objetos que estén en la máscara de capa seleccionada
            if (Physics.Raycast(agent.transform.position,anguloVision, out hits[i], maxDistance,layerMask))
            {
                //Debug.Log(hits[i].point);
                Debug.DrawRay(agent.transform.position,hits[i].point-agent.transform.position,Color.red);
                Vector3 direccion = agent.transform.position - hits[i].point;
                //El movimento escala con la cercanía al objetivo a evadir
                //direccion = direccion * (1 - Vector3.Magnitude(direccion)/maxDistance);
                evadeMove += direccion;
                break;
                //Debug.Log("EVITA ESTO: " + evadeMove);
            }
            else
            {
                Debug.DrawRay(agent.transform.position,anguloVision * maxDistance,Color.green);
            }
        }

        Debug.DrawRay(agent.transform.position,evadeMove,Color.cyan);
        return new Vector3(evadeMove.x, 0, evadeMove.z).normalized;
    }
}
