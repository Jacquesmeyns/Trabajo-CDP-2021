using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Comportamiento que evita los obstáculos calculando el movimiento de esquive.
/// </summary>
[CreateAssetMenu(menuName = "Flock/Behavior/EvadeObstacle")]
public class EvadeObstacleBehavior : FlockBehavior
{
     public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 evadeMove = Vector3.zero;

        //Comprobar obstáculos lanzando rayos con Raycast
            
        //Desplazamiento de bit para obtener la máscara de capa que queremos utilizar
        //  En este caso, la capa "Obstacle"
        int layerMask = 1 << 8;
            
        //Lanza los rayos, definidos en el agente
        RaycastHit[] hits = new RaycastHit[agent.angulosVision.Length];

        for (int i = 0; i < hits.Length; i++)
        {
            Vector3 anguloVision;
            float maxDistance = 5f;
            anguloVision = Quaternion.Euler(0, agent.angulosVision[i], 0) * agent.transform.forward;

            //Compruebo colisiones con objetos que estén en la máscara de capa seleccionada
            if (Physics.Raycast(agent.transform.position,anguloVision, out hits[i], maxDistance,layerMask))
            {
                Vector3 direccion = Vector3.zero;
                //Si es un conejo sólo interesan los que tengan el tag Obstacle
                if (agent.kind == AnimalKind.RABBIT && hits[i].transform.CompareTag("Obstacle"))
                {
                    Debug.DrawRay(agent.transform.position,hits[i].point-agent.transform.position,Color.red);
                    direccion = agent.transform.position - hits[i].point;
                    evadeMove += direccion;
                    break;
                }

                if (agent.kind == AnimalKind.WOLF)
                {
                    //Para los lobos se queda todos los de la capa
                    Debug.DrawRay(agent.transform.position, hits[i].point - agent.transform.position, Color.red);
                    direccion = agent.transform.position - hits[i].point;
                    evadeMove += direccion;
                    break;
                }
            }
            else
            {
                Debug.DrawRay(agent.transform.position,anguloVision * maxDistance,Color.green);
            }
        }

        Debug.DrawRay(agent.transform.position,evadeMove,Color.cyan);     //La dirección para esquivar
        return new Vector3(evadeMove.x, 0, evadeMove.z).normalized;             //y = 0 para que no salgan volando
    }
}
