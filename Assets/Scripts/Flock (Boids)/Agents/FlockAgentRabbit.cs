using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgentRabbit : FlockAgent
{
    internal GameObject food;
    private bool _safe;
    public bool panic;
    public FlockAgentWolf predator;
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
        currentHealth = startingHealth;
        hunger = startingHunger;
        foodBites = (int) startingHealth/8;
        ConstructBehaviorTree();
        //Para controlar que no haya demasiadas madrigueras, tienen sólo un 25% de poder cavar
        if (Random.value < 0.65)
            hasDug = true;
        //Hasta que no crezca no puede reproducirse
        _hasBreeded = true;
        StartCoroutine(GrowUp());
        
    }



    //Los árboles se construyen en código leyéndo el grafo de derecha a izquierda y de abajo a arriba
    //  Primero haz todos los nodos (no importa el orden), y luego monta las secuencias y los selectores
    //  en el orden que he dicho al principio
    private void ConstructBehaviorTree()
    {
        IsPredatorNearNode isPredatorNearNode = new IsPredatorNearNode(this);
        HealthNode isHealthy = new HealthNode(this, lowHealthThreshold);
        IsFlockHealthyNode isFlockHealthyNode =
            new IsFlockHealthyNode(this, transform.GetComponentInParent<FlockRabbit>().flockLowHealthThreshold);
        CanDigNode canDigNode = new CanDigNode(this);
        GoToDigNode goToDigNode = new GoToDigNode(this);
        IsFlockFedNode isFlockFedNode =
            new IsFlockFedNode(this, transform.GetComponentInParent<FlockRabbit>().flockLowHungerThreshold);
        SearchFoodNode searchFoodNode = new SearchFoodNode(this);
        GoToEatNode goToEatNode = new GoToEatNode(this);
        EatNode eatNode = new EatNode(this);
        SeekPartnerNode seekPartnerNode = new SeekPartnerNode(this);
        GoToPartnerNode goToPartnerNode = new GoToPartnerNode(this);
        
        Sequence isHealthySequence = new Sequence(new List<Node> { isHealthy, isFlockHealthyNode, isFlockFedNode});

        Sequence mateSequence = new Sequence(new List<Node> {seekPartnerNode, goToPartnerNode});
        
        Sequence eatSequence = new Sequence(new List<Node>{new Inverter(isHealthySequence),searchFoodNode,goToEatNode,eatNode});
        
        Sequence digSafetyZoneSequence = new Sequence(new List<Node>{canDigNode, isHealthy, goToDigNode});
        
        Selector surviveSelector = new Selector(new List<Node> {digSafetyZoneSequence,isPredatorNearNode,eatSequence});
        
        topNode = new Selector(new List<Node>{surviveSelector, mateSequence});
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

    public void Eat()
    {
        currentHealth += 7;
        hunger +=24;
        Destroy(food);
        food = null;
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
        burrowPosition = (Random.insideUnitSphere * 70);// - (Vector3.one*70);
        burrowPosition.y = 0.42f;//0.42 es la altura para que quede bonito
        yield return new WaitForSeconds(60);
        calledThread = false;
    }

    public override void SpawnChilds()
    {
        if(!CanBreed())
            return;
        _hasBreeded = true;
        partner._hasBreeded = true;
        FlockRabbit pack = GetComponentInParent<FlockRabbit>();
        
        //Crían de 3 a 4 conejos
        int tope = Mathf.CeilToInt(Random.value*2 + 2);
        for (int i = 0; i < tope; i++)
        {
            //Se instancia el prefab del conejo
            GameObject child = Instantiate(pack.agentPrefabRabbit, pack.transform);
            //Se le asigna un nombre
            child.name = "Conejo " + pack.total;
            pack.total++;
            //Se escala al 50% porque es una cría (más tarde crece en GrowUp() )        //<<<<<<<<<<<<<<-----
            child.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //Se añade a la manada
            pack.agents.Add(child.GetComponent<FlockAgentRabbit>());
        }
        Regroup();
        partner.Regroup();
    }
    
    IEnumerator GrowUp()
    {
        //float startTime = Time.time;
        Vector3 speed = Vector3.one;
        //transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref speed, 2f);
        yield return new WaitForSeconds(Random.value *10f + 10f);
        transform.localScale = Vector3.one;
        _hasBreeded = false;
    }
}
