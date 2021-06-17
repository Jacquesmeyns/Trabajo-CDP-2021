using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgentWolf : FlockAgent
{
    private void Awake() {
        //awarenessRadius = 10f;
        currentHealth = startingHealth;   //************************QUITAR EL /9. SÓLO DEBUG
        hunger = startingHunger;
        foodBites = (int) startingHealth;
        ConstructBehaviorTree();
        _hasBreeded = true;
        StartCoroutine(GrowUp()); //Corrutina, tiempo que tarda en crecer para poder criar
    }

    [SerializeField] public FlockBehavior huntingBehavior;

    private FlockAgentRabbit _prey = new FlockAgentRabbit();
    public FlockAgentRabbit prey
    { 
        get{ return _prey;}
        set{ _prey = value;}
    }
    
    private bool attacked = false;
    private bool eating = false;
    

    public Transform targetLocation{
        get {return _prey.transform;}
    }


    //Los árboles se construyen en código leyendo el grafo de derecha a izquierda y de abajo a arriba
    //  Primero se hacen todos los nodos (no importa el orden), y luego se montan las secuencias y los selectores
    //  en el orden dicho al principio
    private void ConstructBehaviorTree()
    {
        IsFlockHealthyNode isFlockHealthyNode = new IsFlockHealthyNode(this, flockLowHealthThreshold);
        HealthNode healthNode = new HealthNode(this, lowHealthThreshold);
        SearchPreyNode searchPreyNode = new SearchPreyNode(this);
        ChaseAttackNode chaseAttackNode = new ChaseAttackNode(this);
        AttackNode attackNode = new AttackNode(prey);
        EatNode eatNode = new EatNode(this);
        SeekPartnerNode seekPartnerNode = new SeekPartnerNode(this);
        IsFlockHungryNode isFlockHungryNode = new IsFlockHungryNode(this, flockHungerThreshold);
        GoToPartnerNode goToPartnerNode = new GoToPartnerNode(this);

        Sequence mateSequence = new Sequence(new List<Node> {isFlockHealthyNode, new Inverter(isFlockHungryNode), seekPartnerNode, goToPartnerNode});

        Sequence defendSequence = new Sequence(new List<Node>{searchPreyNode, chaseAttackNode, attackNode});

        //Sequence huntSequence = new Sequence(new List<Node>{healthNode, searchPreyNode, chaseNode, eatNode});
        Sequence surviveSequence = new Sequence(new List<Node>{ new Inverter(isFlockHealthyNode), isFlockHungryNode, /*healthNode, */searchPreyNode, chaseAttackNode, eatNode});

        topNode = new Selector(new List<Node>{ /*surviveSequence, defendSequence, */mateSequence});
    }

    private void Update() {
        if (!IsDead())
        {
            //Controlo el comportamiento a través del arbol
            topNode.Evaluate();

            if (topNode.nodeState == NodeState.FAILURE)
            {
                //GetComponentInChildren<Material>().SetColor("_Color",Color.red);
                //Debug.Log("TODO MAL");
            }

            //Actualizo la vida y el hambre
            //RegenerateHealth();
            UpdateHunger();
        }
    }

    public bool IsPreyHidden()
    {
        if(!prey && prey.isSafe())
        {
            Regroup();
            prey = null;
        }
        return inFlock;
    }
    
    public override void GoAlone()
    {
        inFlock = false;
        this.tag = "LoneWolf";
    }

    public override void Regroup()
    {
        inFlock = true;
        this.tag = "Wolf";
    }


    public bool IsPreyDead()
    {
        return prey.IsDead();
        
        /*
        if(prey!=null)
            return prey.IsDead();
        else
            return false;*/
    }

    public void Attack()
    {
        if(!attacked)
            StartCoroutine(AttackCoolDown());
    }

    

    public bool CanTakeBite()
    {
        if (prey == null)
        {
            Debug.LogError("No tiene una presa a la que morder");
            return false;
        }
        else
        {
            return prey.CanBeEaten();
        }
    }

    public void Eat()
    {
        if(!eating)
            StartCoroutine(BiteCoolDown());
    }
    
    //Crea hijo o hijos
    public void SpawnChilds()
    {
        if(!CanBreed())
            return;
        _hasBreeded = true;
        partner._hasBreeded = true;
        
        GameObject child = Instantiate(gameObject, GetComponentInParent<FlockWolf>().transform);
        GetComponentInParent<FlockWolf>().agents.Add(child.GetComponent<FlockAgentWolf>());
    }
        
    IEnumerator AttackCoolDown()
    {
        attacked = true;
        prey.currentHealth -= 5;
        yield return new WaitForSeconds(3);
        attacked = false;
    }
    
    IEnumerator BiteCoolDown()
    {
        eating = true;
        prey.TakeBite();
        currentHealth += 10;
        yield return new WaitForSeconds(1.5f);
        eating = false;
    }
    
    IEnumerator GrowUp()
    {
        yield return new WaitForSeconds(3f);
        _hasBreeded = false;
    }
}
