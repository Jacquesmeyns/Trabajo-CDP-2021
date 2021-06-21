using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    #region Variables
    
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
    
    internal bool called;
    
    
    
    [Header("Prefab de nido")]
    [SerializeField] internal GameObject nestPrefab;
    
    [Header("Umbrales")]
    [Range(0,1)] public float _flockLowHungerThreshold;
    public float flockLowHungerThreshold
    {
        get { return _flockLowHungerThreshold; }
    }
    [Range(0,1)] public float _flockLowHealthThreshold;       //Salud mínima para no considerarse la bandada sana
    public float flockLowHealthThreshold
    {
        get { return _flockLowHealthThreshold; }
    }
    
    [Header("Nest attributes")]
    internal Vector3 nestPosition;
    [Range(1,20)] public float nestRadius;
    
    //Para llevar la cuenta del total de agentes
    internal int total;
    
    #endregion

    #region ClassMethods

    /// <summary>
    /// Devuelve una lista con los objetos cercanos al agente.
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Cada 15 segundos cambia la posición en la que se centran las bandadas.
    /// Otorga dinamismo a la simulación
    /// </summary>
    /// <returns></returns>
    internal IEnumerator ChangeTargetPosition()
    {
        called = true;
        Vector3 newPos = Random.insideUnitSphere;
        targetPosition = new Vector3(newPos.x, 0, newPos.z)*65;
        yield return new WaitForSeconds(15);
        called = false;
    }
    
    #endregion
}
