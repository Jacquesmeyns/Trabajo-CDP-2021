using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockRabbit : Flock
{

private void Start() {
    
}

    void Awake(){
        
        //  Se usan cuadrados para ahorrar un poco de cálculos, en lugar de usar raíces cuadradas
        //  cada vez que use sqrMagnitude
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        //Creamos los agentes de la manada
        for (int i = 0; i < startingCount; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle;
            //Se instancia el agente en una posición aleatoria y teniendo en cuenta la densidad
            //  También se hace que apunte a una dirección aleatoria del plano
            FlockAgentRabbit newAgent = Instantiate(
                agentPrefabRabbit,
                //Aseguro que siempre hay una distancia aceptable entre agentes
                this.transform.position + new Vector3(randomPos.x,0f,randomPos.y)*  startingCount * AgentDensity,
                //Que mire a un punto aleatorio con respecto al plano
                Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
                this.GetComponentInParent<Transform>()
            );
            newAgent.name = "Conejo " + i;
            //newAgent.kind = AnimalKind.RABBIT;
            //Se guarda el nuevo agente en la bandada
            agents.Add(newAgent);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Se calcula el movimiento de cada agente
        
        for(int i = agents.Count-1; i >= 0; i--)
        {
            if (agents[i] == null)
                agents.RemoveAt(i);
            if (!agents[i].IsDead())
            {
                //Se recogen todos los agentes dentro del radio
                List<Transform> context = GetNearbyObjects(agents[i]);
                //agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, context.Count/6f);

                //Se calcula el movimiento de cada agente de la bandada en función del comportamiendo definido
                Vector3 move = behavior.CalculateMove(agents[i], context, this);

                /*
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
                        move += direccion;
                    }
                    else
                    {
                        Debug.DrawRay(agent.transform.position,anguloVision * scale,Color.green);
                    }
                }
                */
                //Con esto se suavizan los giros, para que no haga movimientos bruscos
                move *= driveFactor;

                if (move.sqrMagnitude > squareMaxSpeed)
                {
                    //Si el cuadrado del movimiento es mayor que el cuadrado la velocidad máxima
                    //  capo la velocidad con el máximo definido
                    move = move.normalized * maxSpeed;
                }

                //Muevo el agente mientras no haya llegado a la posición objetivo
                //if(targetPosition!=Vector2.zero){
                //    Debug.Log("Tengo una posición a la que ir");
                
                //agents[i].Move(move);
                //  ------------> SÓLO DEBUG, PARA EVITAR QUE SE MUEVA <--------------------
                
                
                //}
            }

        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        //Lista de posiciones
        List<Transform> context = new List<Transform>();
        //Array de todos los colliders que estén dentro del área circular, 
        // de centro el origen local del agente y con el radio de vecindad definido
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position,neighborRadius);
        
        //Para cada agente guardamos toda las posiciones de los demás agentes 
        //  que colisionen con él (estén dentro de su área)
        foreach (Collider c in contextColliders)
        {
            //No queremos guardar la posición del propio agente, ni la de otro tipo de agentes
            if(c!= agent.AgentCollider && c.CompareTag("Rabbit"))
            {
                context.Add(c.transform);
            }
        }

        //Devolvemos los agentes circundantes
        return context;
    }
}
