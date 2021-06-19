using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Panic")]
public class PanicBehavior : FlockBehavior
{
    private float burrowRadius = 5f;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Reiniciamos la lista cada vez
        //_predators = new List<FlockAgentWolf>();
        int nPredators = 0;
        int nBurrows = 0;
        Vector3 panicMove = Vector3.zero;
        Vector3 fleeMove = Vector3.zero;
        Vector3 toBurrowMove = Vector3.zero;
        float t;
        
        
        //Busca todos los colliders en su radio de consciencia
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, agent.awarenessRadius);
        //Guarda las posiciones de todos los lobos dentro de su radio de búsqueda (los agentes 
        //  que estén dentro de su área)
        foreach (Collider c in contextColliders)
        {
            
            
            
            //No queremos guardar la posición del propio agente, ni la de agentes que no sean lobos
            if(c!= agent.AgentCollider && 
               (c.CompareTag("LoneWolf") || c.CompareTag("Wolf"))
               
               )
            {
                //_predators.Add(c.gameObject.GetComponent<FlockAgentWolf>());
                fleeMove += agent.transform.position - c.transform.position;
                nPredators++;
            }
            else if (c.CompareTag("Burrow"))    //Si encuentra madrigueras, va hacia ellas
            {
                Vector3 distanceToBurrow = c.transform.position - agent.transform.position;
                Debug.DrawRay(agent.transform.position, distanceToBurrow, Color.black);
                //Si está cerca de un mínimo de la madriguera, se esconde
                t = distanceToBurrow.magnitude / burrowRadius;
                if (t < 0.9f)
                {
                    ((FlockAgentRabbit) agent).safe = true;
                    return Vector3.zero;
                }
                
                toBurrowMove += distanceToBurrow;
                nBurrows++;
            }
        }
        
        //Calculo la dirección hacia la que huir como la media de las posiciones
        if (nPredators > 0)
            fleeMove /= nPredators;
        
        if (nBurrows > 0)
            toBurrowMove /= nBurrows;

        panicMove = fleeMove + toBurrowMove;

        Debug.DrawRay(agent.transform.position, panicMove, Color.blue);
        return new Vector3(panicMove.x, 0f, panicMove.z);
    }
}
