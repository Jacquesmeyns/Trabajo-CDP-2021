﻿using System;
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
        //transform.forward = velocity;
        transform.position += velocity * Time.deltaTime;
        
        //tDelta += Time.deltaTime / tSeconds;
        transform.forward = Vector3.SmoothDamp(transform.forward, velocity, ref velocity, 1f);

        //transform.position = Vector3.Lerp(transform.position, velocity , Time.deltaTime);
        //transform.position = Vector3.MoveTowards(transform.position, velocity * Time.deltaTime, velocity.magnitude);

        /*Vector3 velocidad = new Vector3(velocity.x, 0, velocity.y);
        transform.up = velocity;
        transform.position += (Vector3)velocidad * Time.deltaTime;
        */
    }

    public virtual void GoAlone()
    {
        throw new NotImplementedException();
    }

    public virtual void Regroup()
    {
        throw new NotImplementedException();
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
        //Debug.Log("Me destruyo: " + this.ToString());
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
        //Si alguno ya tiene compañero, no puede elegirse otro. Tampoco si el compañero no puede criar aún
        if (partner != null || agent.partner != null || !agent.CanBreed() )
            return false;

        partner = agent;
        agent.partner = this;
        
        agent.GoAlone();
        agent.partner.GoAlone();
        
        return true;
    }

    //Crea hijo o hijos
    public virtual void SpawnChilds()
    {
        throw new NotImplementedException();
        /*if(!CanBreed())
            return;
        _hasBreeded = true;
        partner._hasBreeded = true;
        
        GameObject child = Instantiate(gameObject, GetComponentInParent<FlockWolf>().transform);
        GetComponentInParent<FlockWolf>().agents.Add(child.GetComponent<FlockAgentWolf>());*/
    }

    //Si ambos están dentro del radio del nido, true
    internal bool InNestWithPartner(Vector3 nestPosition)
    {
        float minDistance = transform.GetComponentInParent<Flock>().nestRadius;
        Vector3 distanceToNest = transform.position - nestPosition;
        Vector3 partnerDistanceToNest = partner.transform.position - nestPosition;
 
        return distanceToNest.magnitude < minDistance && partnerDistanceToNest.magnitude < minDistance;
    }
    
    #endregion
}

public enum AnimalKind{
    WOLF, RABBIT, NULL,
}
