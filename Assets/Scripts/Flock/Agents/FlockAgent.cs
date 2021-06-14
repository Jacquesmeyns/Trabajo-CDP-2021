using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgent : MonoBehaviour
{
    BoxCollider agentCollider;

    //Ángulos desde los que se lanzan los rayos del raycast
    public int[] angulosVision = {-2, -1, 0, 1, 2};

    //Salud de inicio
    [SerializeField] internal float startingHealth;
    internal int foodBytes;

    //Salud mínima para no considerarse sano
    [SerializeField] private float _lowHealthThreshold;
    public float lowHealthThreshold{
        get {return _lowHealthThreshold;}
        set { _lowHealthThreshold = value;}
    }

    //Ratio de sanación por tiempo
    [SerializeField] internal float healthRestoreRate;

    //Si está con el grupo o va por su cuenta
    [SerializeField] private bool _inFlock = false;

    //Si ha criado o no
    [SerializeField] private bool _hasBreeded = false;
    public bool inFlock {
        get { return _inFlock;}
        set { _inFlock = value;}
    }

    //Posición del agente con el que criar 
    private Transform _partnerPosition = null;
    public Transform partnerPosition {
        get { return _partnerPosition;}
        set { _partnerPosition = value;}
    }

    //Comportamiento de criar
    [SerializeField] public FlockBehavior breedingBehavior;

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
        set { _currentHealth = Mathf.Clamp(value, 0, startingHealth);}
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

    public virtual void GoAlone(){}

    public virtual void Regroup(){}

    //Para controlar el movimiento
    public bool IsDead(){
        return currentHealth <= 0;
    }

    public bool CanBreed()
    {
        //Si ha tenido crías no puede tener más
        if(_hasBreeded)
        {
            return false;
        }
        //Si no ha tenido, puede tener sólo una vez
        else
        {
            _hasBreeded = true;
            return true;
        }
    }

    public void Dissappear()
    {
        Destroy(this);
    }

    
    //True cuando está con bocados disponibles
    public bool CanBeEaten()
    {
        return --foodBytes > 0;
    }
}

public enum AnimalKind{
    WOLF, RABBIT, NULL,
}
