using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgentWolf : FlockAgent
{
    private void Awake() {
        //awarenessRadius = 10f;
        currentHealth = startingHealth;
        foodBytes = (int) startingHealth;
        ConstructBehaviorTree();
    }

    [SerializeField] public FlockBehavior huntingBehavior;

    private FlockAgentRabbit _prey = new FlockAgentRabbit();
    public FlockAgentRabbit prey
    { 
        get{ return _prey;}
        set{ _prey = value;}
    }

    public Transform targetLocation{
        get {return _prey.transform;}
    }


    //Los árboles se construyen en código leyéndo el grafo de derecha a izquierda y de abajo a arriba
    //  Primero haz todos los nodos (no importa el orden), y luego monta las secuencias y los selectores
    //  en el orden que he dicho al principio
    private void ConstructBehaviorTree()
    {
        IsFlockHealthyNode isFlockHealthyNode = new IsFlockHealthyNode(this, lowHealthThreshold);
        HealthNode healthNode = new HealthNode(this, lowHealthThreshold);
        SearchPreyNode searchPreyNode = new SearchPreyNode(this);
        ChaseNode chaseNode = new ChaseNode(this);
        AttackNode attackNode = new AttackNode(prey);
        EatNode eatNode = new EatNode(this);
        BreedNode breedNode = new BreedNode(this);
        Inverter isFlockHungryNode = new Inverter(isFlockHealthyNode);

        Sequence mateSequence = new Sequence(new List<Node> {isFlockHealthyNode, isFlockHungryNode, breedNode});

        Sequence defendSequence = new Sequence(new List<Node>{searchPreyNode, chaseNode, attackNode});

        //Sequence huntSequence = new Sequence(new List<Node>{healthNode, searchPreyNode, chaseNode, eatNode});
        Sequence surviveSequence = new Sequence(new List<Node>{ isFlockHealthyNode, isFlockHungryNode, healthNode, searchPreyNode, chaseNode, eatNode});

        topNode = new Selector(new List<Node>{ surviveSequence, defendSequence, mateSequence});
    }

    private void Update() {
        
        topNode.Evaluate();
        if(topNode.nodeState == NodeState.FAILURE)
        {
            //GetComponentInChildren<Material>().SetColor("_Color",Color.red);
            //Debug.Log("TODO MAL");
        }
        //regenerateHealth();
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
    
    public void GoAlone()
    {
        inFlock = false;
        this.tag = "LoneWolf";
    }

    public void Regroup()
    {
        inFlock = true;
        this.tag = "Wolf";
    }

    public bool IsPreyDead()
    {
        if(prey!=null)
            return prey.IsDead();
        else
            return false;
    }

    public void Attack()
    {
        prey.currentHealth -= 5;
    }

    private void RegenerateHealth(){
        currentHealth += Time.deltaTime * healthRestoreRate;
    }
    
    
    
}
