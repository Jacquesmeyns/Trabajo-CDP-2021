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
    //public bool searchingWhereToDig;
    private bool calledThread;
    internal Vector3 burrowPosition = Vector3.zero;
    [SerializeField] GameObject burrowPrefab;
    
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
        GoToDigNode goToDigNode = new GoToDigNode(this);

        Sequence digSafetyZoneSequence = new Sequence(new List<Node>{canDigNode, isHealthy, goToDigNode});
        
        Selector surviveSelector = new Selector(new List<Node> {isPredatorNearNode});///////a medias
        
        topNode = new Selector(new List<Node>{surviveSelector, digSafetyZoneSequence});
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
        if (!calledThread)
        {
            calledThread = true;
            //searchingWhereToDig = true;
            StartCoroutine(resetPosition());
        }
    }

    public void DigBurrow()
    {
        hasDug = true;
        Instantiate(burrowPrefab, burrowPosition, transform.rotation);
        //Instanciar la madriguera
    }

    //Cada minuto se cambia la posición, para darle dinamismo
    IEnumerator resetPosition()
    {
        Vector3 location = Random.insideUnitSphere;
        burrowPosition = new Vector3(location.x, 0, location.z)*100;
        burrowPosition.y = 0.42f;//0.42 es la altura para que quede bonito
        yield return new WaitForSeconds(60);
        calledThread = false;
    }
}
