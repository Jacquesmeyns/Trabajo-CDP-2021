using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Flock : MonoBehaviour
{
    //Prefabs de los agentes lobo y conejo y de las manadas de cada uno
    [Header("Prefabs")]
    private FlockWolf wolfPackPrefab;
    [SerializeField] internal GameObject agentPrefabWolf;
    private FlockRabbit rabbitFlockPrefab;
    [SerializeField] internal GameObject agentPrefabRabbit;
    //Lista de agentes
    internal  List<FlockAgent> agents = new List<FlockAgent>();
    [Header("")]
    //Comportamiento
    public FlockBehavior defaultBehavior;

    [Header("Variables para aparición de agentes")]
    //Variables para la aparición de agentes
    [Range(1,500)] public int startingCount;
    internal const float AgentDensity = 0.08f;

    [Header("Características boids")]
    [Range(1f,100f)] public float driveFactor = 10f;
    [Range(1f,100f)] public float maxSpeed = 5f;
    [Range(1f,100f)] public float neighborRadius = 1.5f;
    [Range(0f,10f)] public float avoidanceRadiusMultiplier = 0.8f;

    internal float squareMaxSpeed;
    internal float squareNeighborRadius;
    internal float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
    [SerializeField] internal Vector3 targetPosition = Vector3.zero;
    
    
    
    [Header("Posiciones de aparición")]
    private readonly List<Transform> _wolfSpawnPoints = new List<Transform>();
    private readonly List<Transform> _rabbitSpawnPoints = new List<Transform>();
    
    [Header("Umbrales")]
    [Range(0,1)] public float _flockLowHungerThreshold;
    public float flockLowHungerThreshold
    {
        get { return _flockLowHungerThreshold; }
    }
    [Range(0,1)] public float _flockLowHealthThreshold;         //Salud mínima para no considerarse la bandada sana
    public float flockLowHealthThreshold
    {
        get { return _flockLowHealthThreshold; }
    }
    
    [Header("Nest attributes")]
    internal Vector3 nestPosition;
    [Range(1,20)] public float nestRadius;
    
    //Para llevar la cuenta del total de agentes
    internal int total;
    
    // Start is called before the first frame update
    void Start()
    {
       /* total = 0;
        nestPosition = transform.position;
        //Calculamos los cuadrados
        //  Se usan cuadrados para ahorrar un poco de cálculos, en lugar de usar raíces cuadradas
        //  cada vez que use sqrMagnitude
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        //Cargo las posibles posiciones de aparición
        GameObject wolfPackSpawnPositionsObject = GameObject.Find("WolvesSpawnPoints");
        GameObject rabbitFlockSpawnPositionsObject = GameObject.Find("RabbitSpawnPoints");
*/
/*        foreach (Transform child in wolfPackSpawnPositionsObject.GetComponent<Transform>())
        {
            //Debug.Log(child.ToString());
            _wolfSpawnPoints.Add(child);
        }
        
        foreach (Transform child in rabbitFlockSpawnPositionsObject.GetComponent<Transform>())
        {
            _rabbitSpawnPoints.Add(child);
        }*/

        //Creamos las manadas de lobos y conejos
//        FlockWolf wolvesFlock = Instantiate(wolfPackPrefab, _wolfSpawnPoints[Random.Range(0,_wolfSpawnPoints.Count)]);
//        FlockRabbit rabbitFlock = Instantiate(rabbitFlockPrefab, _rabbitSpawnPoints[Random.Range(0,_rabbitSpawnPoints.Count)]);

    }

    // Update is called once per frame
    void Update()
    {
        /*
        //Se calcula el movimiento de cada agente
        foreach(FlockAgent agent in agents)
        {
            //Se recogen todos los agentes dentro del radio
            List<Transform> context = GetNearbyObjects(agent);
            //agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, context.Count/6f);
            
            //Se calcula el movimiento de cada agente de la bandada en función del comportamiendo definido
            Vector2 move = behavior.CalculateMove(agent, context, this);
            //Con esto se suavizan los giros, para que no haga movimientos bruscos
            move *= driveFactor;

            
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                //Si el cuadrado del movimiento es mayor que el cuadrado la velocidad máxima
                //  capo la velocidad con el máximo definido
                move = move.normalized * maxSpeed;
            }

            //Muevo el agente mientras no haya llegado a la posición objetivo
            if(targetPosition!=Vector2.zero){
                agent.Move(move);
            }
                
            
        }*/
    }

    internal virtual List<Transform> GetNearbyObjects(FlockAgent agent)
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
            if(c!= agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }

        //Devolvemos los agentes circundantes
        return context;
    }
}
