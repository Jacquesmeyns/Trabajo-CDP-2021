using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgentRabbit : FlockAgent
{
    #region Variables
    
    //Referencia a la comida.
    internal GameObject food;
    
    //Si el conejo está o no a salvo.
    private bool _safe;
    
    //Si hay un lobo cerca o no.
    public bool panic;
    
    //Referencia al depredador que le persigue.
    public FlockAgentWolf predator;
    
    //Si ha cavado ya la madriguera.
    public bool _hasDug;
    
    //Para controlar que no se hagan varias llamadas a la corrutina. Sólo necesito una activa a la vez.
    private bool calledThread;
    
    //La posición de la madriguera en la que esconderse.
    internal Vector3 burrowPosition = Vector3.zero;
    
    //El prefab de la madriguera, para instanciarlo una vez haya cavado.
    [SerializeField] GameObject burrowPrefab;
    
    //Los comportamientos concretos que componen el movimiento del conejo.
    [SerializeField] public FlockBehavior panicBehavior;
    [SerializeField] public FlockBehavior digBehavior;
    [SerializeField] public FlockBehavior eatBehavior;
    
    //Getters - setters de las variables anteriores
    public bool safe{ 
        get{ return _safe;}
        set{ _safe = value;}
    }
    public bool hasDug{ 
        get{ return _hasDug;}
        set{ _hasDug = value;}
    }
    
    #endregion

    #region MonobehaviorMethods

    private void Awake() {
        currentHealth = startingHealth;
        hunger = startingHunger;
        foodBites = (int) startingHealth/8; //Para limitar el máximo de bocados en función a la vida máxima
        ConstructBehaviorTree();
        //Para controlar que no haya demasiadas madrigueras, tienen sólo un 40% de poder cavar
        if (Random.value < 0.60)
            hasDug = true;
        //Hasta que no crezca no puede reproducirse
        _hasBreeded = true;
        StartCoroutine(GrowUp());
        
    }
    
    private void Update()
    {
        if (!IsDead())
        {
            topNode.Evaluate();
            
            //Se actualizan vida y hambre cada vez
            RegenerateHealth();
            UpdateHunger();
        }
    }
    
    #endregion

    #region ClassMethods
    
    public override void ConstructBehaviorTree()
    {
        //Nodos
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

        //Secuencias y selectores
        Sequence mateSequence = new Sequence(new List<Node> {seekPartnerNode, goToPartnerNode});
        
        Sequence eatSequence = new Sequence(new List<Node>{new Inverter(isHealthy),searchFoodNode,goToEatNode,eatNode});
        
        Sequence digSafetyZoneSequence = new Sequence(new List<Node>{canDigNode, isHealthy, goToDigNode});
        
        Selector surviveSelector = new Selector(new List<Node> {digSafetyZoneSequence,isPredatorNearNode,eatSequence});
        
        //Nodo raíz
        topNode = new Selector(new List<Node>{surviveSelector, mateSequence});
    }

    public bool isSafe(){
        return _safe;
    }
    
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

    /// <summary>
    /// Actualiza la vida y el hambre cada vez que come. También elimina el gameobject comido.
    /// </summary>
    public void Eat()
    {
        currentHealth += 7;
        hunger +=24;
        Destroy(food);
        food = null;
    }

    /// <summary>
    /// Llama a una corrutina cada minuto que cambia la posición donde cavar. Por si el agente no llega a la
    /// posición actual.
    /// </summary>
    public void ResetDiggingPosition()
    {
        if (!calledThread)  //Para no llamar varias veces a la corrutina, lo controlo con un booleano.
        {
            calledThread = true;
            StartCoroutine(resetPosition());
        }
    }

    /// <summary>
    /// Cava la madriguera. Instancia el prefab de madriguera.
    /// </summary>
    public void DigBurrow()
    {
        hasDug = true;
        Instantiate(burrowPrefab, burrowPosition, transform.rotation);
    }

    public override void SpawnChilds()
    {
        if(!CanBreed())
            return;
        _hasBreeded = true;
        partner._hasBreeded = true;
        FlockRabbit pack = GetComponentInParent<FlockRabbit>();
        
        //Crían de 3 a 4 conejos
        int tope = Mathf.CeilToInt(Random.value*2 + 1);
        for (int i = 0; i < tope; i++)
        {
            //Se instancia el prefab del conejo
            GameObject child = Instantiate(pack.agentPrefabRabbit, pack.transform);
            //Se le asigna un nombre
            child.name = "Conejo " + pack.total;
            pack.total++;
            //Se escala al 50% porque es una cría (más tarde crece en GrowUp())
            child.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //Se añade a la manada
            pack.agents.Add(child.GetComponent<FlockAgentRabbit>());
        }
        Regroup();
        partner.Regroup();
    }
    
    #endregion

    #region IEnumerators
    
    /// <summary>
    /// Corrutina que al pasar un tiempo aleatorio de 10 a 20 segundos, hace crecer al agente.
    /// Aumenta su tamaño y puede reproducirse.
    /// </summary>
    /// <returns></returns>
    IEnumerator GrowUp()
    {
        yield return new WaitForSeconds(Random.value *10f + 10f);
        transform.localScale = Vector3.one;
        _hasBreeded = false;
    }
    
    /// <summary>
    /// Cada minuto cambia la posición en la que cavar.
    /// </summary>
    /// <returns></returns>
    IEnumerator resetPosition()
    {
        burrowPosition = (Random.insideUnitSphere * 70);
        burrowPosition.y = 0.42f;//0.42 es la altura para que quede bonito
        yield return new WaitForSeconds(60);
        calledThread = false;
    }
    
    #endregion
}
