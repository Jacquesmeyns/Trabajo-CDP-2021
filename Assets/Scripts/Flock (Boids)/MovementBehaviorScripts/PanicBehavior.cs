using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Panic")]
public class PanicBehavior : FlockBehavior
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Reiniciamos la lista cada vez
        //_predators = new List<FlockAgentWolf>();
        int nPredators = 0;
        Vector3 panicMove = Vector3.zero;
        
        //Busca todos los colliders en su radio de consciencia          ?¿?¿??¿¿??¿¿?¿?¿¿¿?¿??¿¿?¿
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, agent.awarenessRadius);
        //Guarda las posiciones de todos los lobos dentro de su radio de búsqueda (los agentes 
        //  que estén dentro de su área)
        foreach (Collider c in contextColliders)
        {
            //No queremos guardar la posición del propio agente, ni la de agentes que no sean lobos
            if(c!= agent.AgentCollider && (c.CompareTag("LoneWolf") || c.CompareTag("Wolf")))
            {
                //_predators.Add(c.gameObject.GetComponent<FlockAgentWolf>());
                panicMove += agent.transform.position - c.transform.position;
                nPredators++;
            }
        }
        
        //Calculo la dirección hacia la que huir como la media de las posiciones
        if (nPredators > 0)
            panicMove /= nPredators;

        Debug.DrawRay(agent.transform.position, panicMove, Color.blue);
        return new Vector3(panicMove.x, 0f, panicMove.z);
    }
}
