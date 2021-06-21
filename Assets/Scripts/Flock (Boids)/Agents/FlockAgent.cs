using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgent : MonoBehaviour
{
    #region Variables
    BoxCollider agentCollider;
    
    Vector3 currentVelocity;

    //Ángulos desde los que se lanzan los rayos del raycast
    public int[] angulosVision = {-2, -1, 1, 2};

    //Salud de inicio
    [SerializeField] internal float startingHealth;
    
    //Medidor de hambre
    [Range(0,1)] public float hungerThreshold = 0.3f;
    
    internal float startingHunger = 100f;
    internal float _hunger;     //Cuando llega a cero, empieza a perder vida
    
    //Cuánto hambre baja por turno
    public float gluttony = 0.0001f;
    
    //Salud mínima para no considerarse sano
    [Range(0,1)] public float _lowHealthThreshold;
    
    //Ratio de sanación por tiempo
    [SerializeField] internal float healthRestoreRate;
    
    //Si está con el grupo o va por su cuenta
    [SerializeField] private bool _inFlock = false;
    
    //Cantidad de bocados que puede morder un depredador de su cadáver
    private int _foodBites;
    
    //Si ha criado o no
    [SerializeField] internal bool _hasBreeded = false;
    
    //Agente con el que criar 
    private FlockAgent _partner = null;
    
    //Empieza el Behavior Tree
    internal Node topNode;

    //Nivel de salud actual
    [SerializeField] private float _currentHealth;
    
    //Tipo de animal
    [SerializeField] private AnimalKind _kind = AnimalKind.NULL;

    internal int foodBites
    {
        get
        {
            return _foodBites;
        }
        set
        {
            //Cuando se queda sin bocados, el cadáver desaparece
            _foodBites = value;
            if(_foodBites<=0)
                Dissappear();
        }
        
    }

    public float hunger
    {
        get { return _hunger;}
        set
        {
            _hunger = Mathf.Clamp(value, 0, startingHunger);

            //Si el hambre está a 0, va perdiendo vida poco a poco
            if (_hunger <= 0)
            {
                currentHealth -= 2*gluttony * startingHealth;
            }
        }
    }

    public float lowHealthThreshold{
        get {return _lowHealthThreshold;}
        set { _lowHealthThreshold = value;}
    }

    internal bool inFlock {
        get { return _inFlock;}
        set { _inFlock = value;}
    }
    
    public FlockAgent partner {
        get { return _partner;}
        set { _partner = value;}
    }

    //Comportamiento de criar
    [SerializeField] public FlockBehavior breedingBehavior;
    [SerializeField] public FlockBehavior preBreedingBehavior;

    public AnimalKind kind {
        get { return _kind;}
        set { _kind = value;}
    }

    //Radio de consciencia. Para ver a los demás agentes e interactuar con ellos (cazar/reproducirse)
    [SerializeField] private float _awarenessRadius = 10f;
    public float awarenessRadius
    { 
        get{ return _awarenessRadius;}
        //set{ _awarenessRadius = value;}
    }
    
    public float currentHealth {
        get {return _currentHealth;}
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, startingHealth);
            //Cuando se queda sin vida, se convierte en cadáver (se gira 90 grados en el eje Z)
            if (_currentHealth <= 0)
            {
                gameObject.transform.Rotate(0,0,90);
            }
        }
    }

    public BoxCollider AgentCollider { get { return agentCollider; } }
    #endregion

    #region MonoBehavior
    
    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<BoxCollider>();
        //conoVision = GetComponentInChildren<VisionCone>();
    }
    #endregion

    #region MyMethods
    
    /// <summary>
    /// Construye el árbol de comportamientos del agente.
    ///
    /// Los árboles se construyen en código leyendo el grafo de derecha a izquierda y de abajo a arriba.
    ///  Primero haz todos los nodos (no importa el orden), y luego monta las secuencias y los selectores
    ///  en el orden dicho al principio.
    /// </summary>
    public virtual void ConstructBehaviorTree()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Aplica movimiento con la velocidad.
    /// </summary>
    /// <param name="velocity"></param>
    public void Move(Vector3 velocity)
    {
        transform.position += velocity * Time.deltaTime;
        transform.forward = Vector3.SmoothDamp(transform.forward, velocity, ref velocity, 1f);
    }

    /// <summary>
    /// Separa al agente de la manada.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void GoAlone()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Devuelve al agente a la manada.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void Regroup()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Si el agente tiene puntos de vida o no.
    /// </summary>
    /// <returns></returns>
    public bool IsDead(){
        return currentHealth <= 0;
    }

    /// <summary>
    /// True si puede reproducirse.
    /// </summary>
    /// <returns></returns>
    public bool CanBreed()
    {
        return !_hasBreeded;
    }

    /// <summary>
    /// Elimina el gameobject del agente.
    /// </summary>
    public void Dissappear()
    {
        Destroy(gameObject);
    }
    
    /// <summary>
    /// True cuando está con bocados disponibles.
    /// </summary>
    /// <returns></returns>
    public bool CanBeEaten()
    {
        return foodBites > 0;
    }
    
    /// <summary>
    /// Se le quita un contador de bocados.
    /// </summary>
    public void TakeBite()
    {
        foodBites--;
    }

    /// <summary>
    /// Actualiza el nivel de hambre en base al valor gluttony y startingHunger.
    /// </summary>
    public void UpdateHunger()
    {
        hunger -= gluttony*startingHunger;
    }
    
    /// <summary>
    /// Regenera la vida con el tiempo en base al valor healthRestoreRate.
    /// </summary>
    internal void RegenerateHealth(){
        currentHealth += Time.deltaTime * healthRestoreRate;
    }

    /// <summary>
    /// Si puede hacerse compañero del agente pasado por parámetro se emparejan y devuelve true.
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public bool PartnerWith(FlockAgent agent)
    {
        //Si alguno ya tiene compañero, no puede elegirse otro. Tampoco si el compañero no puede criar aún
        if (partner != null || agent.partner != null || !agent.CanBreed() )
            return false;

        partner = agent;
        agent.partner = this;
        
        agent.GoAlone();
        agent.partner.GoAlone();
        
        return true;
    }

    /// <summary>
    /// Crea una o varias instancias de agentes hijos.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void SpawnChilds()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Devuelve true si ambos están dentro del radio del nido.
    /// </summary>
    /// <param name="nestPosition">Posición del nido.</param>
    /// <returns></returns>
    internal bool InNestWithPartner(Vector3 nestPosition)
    {
        float minDistance = transform.GetComponentInParent<Flock>().nestRadius;
        Vector3 distanceToNest = transform.position - nestPosition;
        Vector3 partnerDistanceToNest = partner.transform.position - nestPosition;
 
        return distanceToNest.magnitude < minDistance && partnerDistanceToNest.magnitude < minDistance;
    }
    
    #endregion
}

/// <summary>
/// Tipos de agentes animales.
/// </summary>
public enum AnimalKind{
    WOLF, RABBIT, NULL,
}
