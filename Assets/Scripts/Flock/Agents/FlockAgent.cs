using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgent : MonoBehaviour
{
    BoxCollider agentCollider;
    
    Vector3 currentVelocity;

    //Ángulos desde los que se lanzan los rayos del raycast
    public int[] angulosVision = {-2, -1, 1, 2};

    //Salud de inicio
    [SerializeField] internal float startingHealth;
    
    //Medidor de hambre
    [Range(0,1)] public float hungerThreshold = 0.3f;
    [Range(0,1)] public float flockHungerThreshold = 0.97f;
    internal float startingHunger = 100f;
    internal float _hunger;     //Cuando llega a cero, empieza a perder vida
    public float hunger
    {
        get { return _hunger;}
        set
        {
            _hunger = Mathf.Clamp(value, 0, startingHunger);
            if (_hunger/startingHunger < hungerThreshold)
            {
                //GoAlone();
            }

            //Si el hambre está a 0, va perdiendo vida poco a poco
            if (_hunger <= 0)
            {
                currentHealth -= 2*gluttony * startingHealth;
            }
        }
    }
    //Cuánto hambre baja por turno
    internal float gluttony = 0.0001f;


    private int _foodBites;

    internal int foodBites
    {
        get
        {
            return _foodBites;
        }
        set
        {
            _foodBites = value;
            if(_foodBites<=0)
                Dissappear();
        }
        
    }

    //Salud mínima para no considerarse la bandada sana
    [Range(0,1)] public float _flockLowHealthThreshold;

    public float flockLowHealthThreshold
    {
        get { return _flockLowHealthThreshold; }
    }
    
    //Salud mínima para no considerarse sano
    [Range(0,1)] public float _lowHealthThreshold;
    public float lowHealthThreshold{
        get {return _lowHealthThreshold;}
        set { _lowHealthThreshold = value;}
    }

    //Ratio de sanación por tiempo
    [SerializeField] internal float healthRestoreRate;

    //Si está con el grupo o va por su cuenta
    [SerializeField] private bool _inFlock = false;

    //Si ha criado o no
    [SerializeField] internal bool _hasBreeded = false;
    public bool inFlock {
        get { return _inFlock;}
        set { _inFlock = value;}
    }

    //Agente con el que criar 
    private FlockAgent _partner = null;
    public FlockAgent partner {
        get { return _partner;}
        set { _partner = value;}
    }

    //Comportamiento de criar
    [SerializeField] public FlockBehavior breedingBehavior;
    [SerializeField] public FlockBehavior preBreedingBehavior;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Cover[] availableCovers;

    internal Material material;

    //Tipo de animal
    [SerializeField] private AnimalKind _kind = AnimalKind.NULL;
    public AnimalKind kind {
        get { return _kind;}
        set { _kind = value;}
    }

    //Radio de conciencia
    [SerializeField] private float _awarenessRadius = 10f;
    public float awarenessRadius
    { 
        get{ return _awarenessRadius;}
        set{ _awarenessRadius = value;}
    }


    //Empieza el Behavior Tree
    internal Node topNode;

    //Nivel de salud actual
    [SerializeField] private float _currentHealth;
    public float currentHealth {
        get {return _currentHealth;}
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, startingHealth);
        }
    }

    public BoxCollider AgentCollider { get { return agentCollider; } }
    
    //public VisionCone ConoVision { get { return conoVision; } }

    //internal Transform _targetLocation = null;

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<BoxCollider>();
        //conoVision = GetComponentInChildren<VisionCone>();
    }

    //Los árboles se construyen en código leyéndo el grafo de derecha a izquierda y de abajo a arriba
    //  Primero haz todos los nodos (no importa el orden), y luego monta las secuencias y los selectores
    //  en el orden que he dicho al principio
    private void ConstructBehaviorTree()
    {
        Debug.LogError("Necesita implementación de árbol");
    }

    //Aplico movimientos con la velocidad
    public void Move(Vector3 velocity)
    {
        transform.forward = velocity;
        transform.position += velocity * Time.deltaTime;
        
        
        /*Vector3 velocidad = new Vector3(velocity.x, 0, velocity.y);
        transform.up = velocity;
        transform.position += (Vector3)velocidad * Time.deltaTime;
        */
    }

    public virtual void GoAlone()
    {
        Debug.LogError("Metodo a implementar en su clase");
    }

    public virtual void Regroup()
    {
        Debug.LogError("Metodo a implementar en su clase");
    }

    //Para controlar el movimiento
    public bool IsDead(){
        return currentHealth <= 0;
    }

    public bool CanBreed()
    {
        return !_hasBreeded;
        
        //Si ha tenido crías no puede tener más
        /*if(_hasBreeded)
        {
            return false;
        }
        //Si no ha tenido, puede tener sólo una vez
        else
        {
            _hasBreeded = true;
            return true;
        }*/
    }

    public void Dissappear()
    {
        Debug.Log("Me destruyo: " + this.ToString());
        Destroy(gameObject);
    }

    
    //True cuando está con bocados disponibles
    public bool CanBeEaten()
    {
        return foodBites > 0;
    }
    
    //Se le quita un contador de bocados
    public void TakeBite()
    {
        foodBites--;
    }

    public void UpdateHunger()
    {
        hunger -= gluttony*startingHunger;
    }
    
    internal void RegenerateHealth(){
        currentHealth += Time.deltaTime * healthRestoreRate;
    }

    public bool PartnerWith(FlockAgent agent)
    {
        //Si alguno ya tiene compañero, no puede elegirse otro
        if (partner != null || agent.partner != null)
            return false;

        partner = agent;
        agent.partner = this;
        
        return true;
    }

    //Crea hijo o hijos
    public virtual void SpawnChilds()
    {
        if(!CanBreed())
            return;
        _hasBreeded = true;
        partner._hasBreeded = true;
        
        GameObject child = Instantiate(gameObject, GetComponentInParent<FlockWolf>().transform);
        GetComponentInParent<FlockWolf>().agents.Add(child.GetComponent<FlockAgentWolf>());
    }

    internal bool InNestWithPartner(Vector3 nestPosition)
    {
        float minDistance = 4f;
        Vector3 distanceToNest = transform.position - nestPosition;
        Vector3 partnerDistanceToNest = partner.transform.position - nestPosition;
        //float t = targetOffset.magnitude / 10f;

        //Si ambos están a menos de 10 unidades del nido, están dentro
        return distanceToNest.magnitude < minDistance && partnerDistanceToNest.magnitude < minDistance;
    }
}

public enum AnimalKind{
    WOLF, RABBIT, NULL,
}
