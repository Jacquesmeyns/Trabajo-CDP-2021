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
        Vector3 panicMove = Vector3.zero;
        Vector3 fleeMove = Vector3.zero;
        Vector3 toBurrowMove = Vector3.zero;
        List<Collider> burrows = new List<Collider>();
        float t;
        
        
        //Busca todos los colliders en su radio de consciencia
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, agent.awarenessRadius);
        //Guarda las posiciones de todos los lobos dentro de su radio de búsqueda (los agentes 
        //  que estén dentro de su área)
        foreach (Collider c in contextColliders)
        {
            //No queremos guardar la posición del propio agente, ni la de agentes que no sean lobos
            if(c!= agent.AgentCollider && 
               (c.CompareTag("LoneWolf") || c.CompareTag("Wolf")))
            {
                //_predators.Add(c.gameObject.GetComponent<FlockAgentWolf>());
                fleeMove += agent.transform.position - c.transform.position;
                nPredators++;
            }
            else if (c.CompareTag("Burrow"))    //Si encuentra madrigueras, las añade a una lista para luego ir a la más cercana
            {
                burrows.Add(c);
                
                //Elige la más cercana y va hacia ella
                Collider nearestBrrow = NearestBurrow(burrows, agent);
                
                Vector3 distanceToBurrow = nearestBrrow.transform.position - agent.transform.position;
                Debug.DrawRay(agent.transform.position, distanceToBurrow, Color.black);

                toBurrowMove += distanceToBurrow;
            }
        }
        
        //Calculo la dirección hacia la que huir como la media de las posiciones
        if (nPredators > 0)
            fleeMove /= nPredators;

        panicMove = fleeMove + toBurrowMove;

        Debug.DrawRay(agent.transform.position, panicMove, Color.blue);
        return new Vector3(panicMove.x, 0f, panicMove.z);
    }

    private Collider NearestBurrow(List<Collider> burrows, FlockAgent _agent)
    {
        float closestDistance = 99999999f;
        Collider closestBurrow = null;
        
        foreach (Collider burrow in burrows)
        {
            float distance = Vector3.Distance(_agent.transform.position, burrow.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBurrow = burrow;
            }
        }

        return closestBurrow;
    }
}
