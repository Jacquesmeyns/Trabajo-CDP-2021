using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName= "Flock/Behavior/Composite")]
public class CompositeBehavior : FlockBehavior
{
    Vector3 currentVelocity;
    public float agentSmoothTime = 10f;
    public FlockBehavior[] behaviors;
    public float[] weights;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //Si hay distinto número de peso y conductas, error: Debe haber una relación 1 a 1
        //  No se mueve
        if(behaviors.Length != weights.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        //Preparar movimiento
        Vector3 move = Vector3.zero;

        //Itera sobre las conductas o comportamientos (behaviors)
        for (int i = 0; i < behaviors.Length; i++)
        {
            //Calculamos cada movimiento parcial con su correspondiente peso
            Vector3 partialMove = behaviors[i].CalculateMove(agent, context, flock) * weights[i];
            
            //if(!partialMove.y.Equals(0.0f))
            //    Debug.Log("Movimiento jodido en: " + behaviors[i].ToString() + " -->  " + partialMove);

            //Si se mueve
            if(partialMove != Vector3.zero)
            {
                //Si el movimiento parcial es mayor que su peso, se capa (se normaliza y se ajusta al peso)
                if(partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                //Si no sobrepasa al peso está bien y se asigna tal cual
                move += partialMove;
            }
        }

        //move += new EvadeObstacleBehavior().CalculateMove(agent, context, flock);
        
        //move = Vector3.SmoothDamp(agent.transform.forward, move, ref currentVelocity, agentSmoothTime);
        //Debug.DrawRay(agent.transform.position,move*15,Color.white);
        return move;

    }
}
