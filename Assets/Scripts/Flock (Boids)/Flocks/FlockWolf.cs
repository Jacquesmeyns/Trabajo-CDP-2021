using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlockWolf : Flock
{
    #region MonobehaviorMethods

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
            agents.Add(newAgent.GetComponent<FlockAgentWolf>());
            total++;
        }
        
        //El círculo que marca el nido de la manada
        nestPrefab.GetComponent<SpriteRenderer>().color = Color.red;
        Instantiate( 
            nestPrefab, 
            nestPosition, 
            nestPrefab.transform.rotation, 
            transform);
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
                    //Se calcula el movimiento de cada agente de la bandada en función del comportamiendo definido
                    move = defaultBehavior.CalculateMove(agent, context, this);
                }
                //Si no esta en grupo y puede aparearse, se aparea
                else if (agent.partner != null && agent.CanBreed())
                {
                    if(!agent.InNestWithPartner(nestPosition)) 
                    {
                        //Se calcula el movimiento de cada agente de la bandada en función del comportamiendo definido
                         move = agent.preBreedingBehavior.CalculateMove(agent, context, this);
                    }
                    else
                    {
                        //Debug.Log("Breeding Behavior" + agent.name);
                        move = agent.breedingBehavior.CalculateMove(agent, context, this);
                    }
                }
                else
                {
                    //Se calcula el movimiento de cada agente de la bandada en función del comportamiendo definido
                    move = agent.huntingBehavior.CalculateMove(agent, context, this);
                }
                
                //Con esto se suavizan los giros, para que no haga movimientos bruscos
                move *= driveFactor;

                if (move.sqrMagnitude > squareMaxSpeed)
                {
                    //Si el cuadrado del movimiento es mayor que el cuadrado la velocidad máxima
                    //  capo la velocidad con el máximo definido
                    move = move.normalized * maxSpeed;
                }
                //Aplico el movimiento
                agent.Move(move);
            }
        }
        
        //Para dar dinamismo
        if (!called)
            StartCoroutine(ChangeTargetPosition());
    }

    #endregion

    #region ClassMethods
    
    internal override List<Transform> GetNearbyObjects(FlockAgent agent)
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
    
    #endregion
}
