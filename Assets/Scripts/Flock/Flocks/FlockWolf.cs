using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlockWolf : Flock
{
    private void Start() {
        
    }
    void Awake()
    {
        nestPosition = transform.position;
        
        //Calculamos los cuadrados
        //  Se usan cuadrados para ahorrar un poco de cálculos, en lugar de usar raíces cuadradas
        //  cada vez que use sqrMagnitude
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        //Creamos los agentes de la manada
        for (var i = 0; i < startingCount; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle;
            //Se instancia el agente en una posición aleatoria y teniendo en cuenta la densidad
            //  También se hace que apunte a una dirección aleatoria del plano
            var newAgent = Instantiate(
                agentPrefabWolf,
                //Aseguro que siempre hay una distancia aceptable entre agentes
                 transform.position + new Vector3(randomPos.x,0f,randomPos.y)*  startingCount * AgentDensity,
                //Que mire a un punto aleatorio con respecto al plano
                Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
                GetComponentInParent<Transform>()
            );
            
            
            newAgent.name = "Lobo " + i;
            //Se guarda el nuevo agente en la bandada
            agents.Add(newAgent);
        }

        Instantiate( GameObject.Find("Posicion target"), nestPosition, GameObject.Find("Posicion target").transform.rotation, gameObject.transform);
    }

    internal void create(){
    }

    // Update is called once per frame
    void Update()
    {
        //Se calcula el movimiento de cada agente
        foreach (FlockAgentWolf agent in agents)
        {
            if (!agent.IsDead()){

                Vector3 move;

                //Se recogen todos los agentes dentro del radio
                List<Transform> context = GetNearbyObjects(agent);


                if (agent.inFlock)
                {
                    //Debug.Log("Flock Behavior");
                    //Se calcula el movimiento de cada agente de la bandada en función del comportamiendo definido
                    move = behavior.CalculateMove(agent, context, this);

                }
                //Si no esta en grupo y puede aparearse, se aparea
                else if (agent.partner != null && agent.CanBreed())
                {
                    Debug.Log("Breeding Behavior" + agent.name);
                    //Si ambos están en el nido, pueden tener a la cría
                    if(agent.InNestWithPartner(nestPosition))
                        move = agent.breedingBehavior.CalculateMove(agent, context, this);
                    else
                    //Se calcula el movimiento de cada agente de la bandada en función del comportamiendo definido
                        move = agent.preBreedingBehavior.CalculateMove(agent, context, this);
                }
                else
                {
                    Debug.Log("Hunting Behavior");
                    //Se calcula el movimiento de cada agente de la bandada en función del comportamiendo definido
                    move = agent.huntingBehavior.CalculateMove(agent, context, this);
                    //move = Vector3.zero;
                }
                
                //Con esto se suavizan los giros, para que no haga movimientos bruscos
                move *= driveFactor;

                if (move.sqrMagnitude > squareMaxSpeed)
                {
                    //Si el cuadrado del movimiento es mayor que el cuadrado la velocidad máxima
                    //  capo la velocidad con el máximo definido
                    move = move.normalized * maxSpeed;
                }
                if(move == Vector3.zero)
                    Debug.Log("ZERO");
                
                //Aplico el movimiento
                agent.Move(move);
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
            //No queremos guardar la posición del propio agente
            if(c!= agent.AgentCollider && (c.CompareTag("Wolf") || c.CompareTag("LoneWolf")))
            {
                //Debug.Log(c.gameObject.ToString());
                context.Add(c.transform);
            }
        }

        //Devolvemos los agentes circundantes
        return context;
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    /*
    List<Transform> GetSeenObjects(FlockAgent agent)
    {
        
        //Lista de posiciones
        List<Transform> context = new List<Transform>();
        //Array de todos los colliders que estén dentro del collider del cono de vision
        Collider[] contextColliders = agent.ConoVision.OverlappingColliders().ToArray();
        
        //Para cada agente guardamos toda las posiciones de los obstáculos 
        //  que colisionen con él (estén dentro de su área de visión)
        foreach (Collider c in contextColliders)
        {
            //No queremos guardar la posición del propio agente
            if(c!= agent.AgentCollider && c.CompareTag("Obstacle"))
            {
                context.Add(c.transform);
            }
        }

        //Devolvemos los obstáculos circundantes
        return context;
    }*/
}
