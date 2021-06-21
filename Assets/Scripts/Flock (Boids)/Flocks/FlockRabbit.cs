using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockRabbit : Flock
{
    
    void Awake()
    {
        Instantiate( 
            nestPrefab, 
            nestPosition, 
            nestPrefab.transform.rotation, 
            transform);
        
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
            var newAgent = Instantiate(
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
            agents.Add(newAgent.GetComponent<FlockAgentRabbit>());
            total++;
        }

        targetPosition = transform.position;
        
        
        //nestPrefab.GetComponent<SpriteRenderer>().color = Color.blue;
        /*nestPrefab.transform.position =
            new Vector3(nestPrefab.transform.position.x, 0, nestPrefab.transform.position.z);*/
        
    }

    // Update is called once per frame
    void Update()
    {
        //Se calcula el movimiento de cada agente
        
        for(int i = agents.Count-1; i >= 0; i--)
        {
            if (agents[i] == null)
                agents.RemoveAt(i);
            else 
            if (!agents[i].IsDead())
            {
                //Se recogen todos los agentes dentro del radio
                List<Transform> context = GetNearbyObjects(agents[i]);

                Vector3 move = new Vector3();
                //Se calcula el movimiento de cada agente de la bandada en función del comportamiendo definido
                    //Pánico
                if (((FlockAgentRabbit)agents[i]).panic)
                {
                    //Si no está a salvo y sigue cerca, se sigue moviendo
                    if(!((FlockAgentRabbit)agents[i]).safe)
                        move = ((FlockAgentRabbit)agents[i]).panicBehavior.CalculateMove(agents[i], context, this);
                }   //Cavar madriguera
                else if (!((FlockAgentRabbit) agents[i]).hasDug)
                {
                    Debug.Log("DIG BEHAVIOR");
                    move = ((FlockAgentRabbit)agents[i]).digBehavior.CalculateMove(agents[i], context, this);
                }   //Hambre
                else if(((FlockAgentRabbit)agents[i]).hunger < ((FlockAgentRabbit)agents[i]).hungerThreshold && 
                        ((FlockAgentRabbit)agents[i]).food != null)
                {
                    move = ((FlockAgentRabbit)agents[i]).eatBehavior.CalculateMove(agents[i], context, this);
                }
                else if(agents[i].CanBreed() && agents[i].partner != null)//Reproducción
                {
                    if (!agents[i].InNestWithPartner(nestPosition))
                    {
                        //Pre-breeding behavior. Se esperan en el nido
                        move = agents[i].preBreedingBehavior.CalculateMove(agents[i], context, this);
                    }
                    else
                        move = ((FlockAgentRabbit)agents[i]).breedingBehavior.CalculateMove(agents[i], context, this);
                }
                else//Movimiento normal
                {
                    move = defaultBehavior.CalculateMove(agents[i], context, this);
                }
                    


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
                
                agents[i].Move(move);
                //  ---------------> SÓLO DEBUG, PARA EVITAR QUE SE MUEVA <--------------------
                
                
                //}
            }

        }

        /*if (!called)
            StartCoroutine(ChangeTargetPosition());*/
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
