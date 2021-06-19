using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgentRabbit : FlockAgent
{
    private bool _safe;
    public bool panic;
    public FlockAgentWolf predator;     //<<<<<<<<<<<-------------PENDIENTE DE HACER
    public bool _hasDug;
    public bool searchingWhereToDig;
    internal Transform burrowLocation;
    private GameObject burrowPrefab;
    
    [SerializeField] public FlockBehavior panicBehavior;
    [SerializeField] public FlockBehavior digBehavior;
    [SerializeField] public FlockBehavior eatBehavior;
    public bool safe{ 
        get{ return _safe;}
        set{ _safe = value;}
    }
    public bool hasDug{ 
        get{ return _hasDug;}
        set{ _hasDug = value;}
    }

    private void Awake() {
        //awarenessRadius = 15f;
        safe = true;
        currentHealth = startingHealth;
        foodBites = (int) startingHealth/8;
        ConstructBehaviorTree();
    }

    //Los árboles se construyen en código leyéndo el grafo de derecha a izquierda y de abajo a arriba
    //  Primero haz todos los nodos (no importa el orden), y luego monta las secuencias y los selectores
    //  en el orden que he dicho al principio
    private void ConstructBehaviorTree()
    {
        IsPredatorNearNode isPredatorNearNode = new IsPredatorNearNode(this);
        HealthNode isHealthy = new HealthNode(this, lowHealthThreshold);
        CanDigNode canDigNode = new CanDigNode(this);

        Selector survive = new Selector(new List<Node> { });///////a medias
        topNode = isPredatorNearNode;
    }

    private void Update()
    {
        if (!IsDead())
        {
            topNode.Evaluate();
            
            RegenerateHealth();
            UpdateHunger();
        }
    }

    public bool isSafe(){
        return _safe;
    }
    
    //
    public override void GoAlone()
    {
        inFlock = false;
        this.tag = "FleeingRabbit";
    }

    public override void Regroup()
    {
        inFlock = true;
        this.tag = "Rabbit";
    }

    public void ResetDiggingPosition()
    {
        if (!searchingWhereToDig)
        {
            searchingWhereToDig = true;
            StartCoroutine(resetPosition());
        }
    }

    public void DigBurrow()
    {
        hasDug = true;
        Instantiate(burrowPrefab, burrowLocation.position, transform.rotation);
        //Instanciar la madriguera
    }

    //Cada minuto se cambia la posición, para darle dinamismo
    IEnumerator resetPosition()
    {
        Vector2 location = Random.insideUnitCircle;
        burrowLocation.position = new Vector3(location.x, 0, location.y)*100;
        yield return new WaitForSeconds(60);
        searchingWhereToDig = false;
    }
}
