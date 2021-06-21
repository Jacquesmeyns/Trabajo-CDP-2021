using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgentWolf : FlockAgent
{

    #region Variables

    //El comportamiento de caza de los lobos
    [SerializeField] public FlockBehavior huntingBehavior;

    //Referencia a la presa
    private FlockAgentRabbit _prey = new FlockAgentRabbit();
    public FlockAgentRabbit prey
    { 
        get{ return _prey;}
        set{ _prey = value;}
    }
    
    //Para controlar si ha atacado o si está comiendo
    private bool attacked;
    private bool eating;
    
    #endregion

    #region MonobehaviorMethods
    
    private void Awake() {
        currentHealth = startingHealth;
        hunger = startingHunger;
        foodBites = (int) startingHealth /10;   //Para escalar los bocados en función al 10% de la vida máxima
        ConstructBehaviorTree();
        _hasBreeded = true;
        StartCoroutine(GrowUp()); //Corrutina, tiempo que tarda en crecer para poder criar (10-20seg)
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
            RegenerateHealth();
            UpdateHunger();
        }
    }
    
    #endregion

    #region ClassMethods
    
    public override void ConstructBehaviorTree()
    {
        //Nodos
        IsFlockHealthyNode isFlockHealthyNode = new IsFlockHealthyNode(this, transform.GetComponentInParent<FlockWolf>().flockLowHealthThreshold);
        HealthNode healthNode = new HealthNode(this, lowHealthThreshold);
        SearchPreyNode searchPreyNode = new SearchPreyNode(this);
        ChaseAttackNode chaseAttackNode = new ChaseAttackNode(this);
        AttackNode attackNode = new AttackNode(prey);
        EatNode eatNode = new EatNode(this);
        SeekPartnerNode seekPartnerNode = new SeekPartnerNode(this);
        IsFlockFedNode isFlockFedNode = new IsFlockFedNode(this, transform.GetComponentInParent<FlockWolf>().flockLowHungerThreshold);
        GoToPartnerNode goToPartnerNode = new GoToPartnerNode(this);
        RangeNode isInRangeNode = new RangeNode(awarenessRadius * 1.5f, this);

        //Secuencias
        Sequence mateSequence = new Sequence(new List<Node> {isFlockFedNode, seekPartnerNode, goToPartnerNode});
        
        Selector notHealtthySelector = new Selector(new List<Node>{new Inverter(isFlockFedNode), new Inverter(healthNode)});

        Sequence surviveSequence = new Sequence(new List<Node>{notHealtthySelector, searchPreyNode, isInRangeNode,chaseAttackNode, eatNode});

        //Nodo raíz
        topNode = new Selector(new List<Node>{ surviveSequence,mateSequence});
    }

    
    /// <summary>
    /// Devuelve true si la presa está oculta.
    /// </summary>
    /// <returns></returns>
    public bool IsPreyHidden()
    {
        if(prey.isSafe() && prey.panic)
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

    /// <summary>
    /// Devuelve true si la presa ha muerto.
    /// </summary>
    /// <returns></returns>
    public bool IsPreyDead()
    {
        return prey.IsDead();
    }

    /// <summary>
    /// Hace daño a la presa cada cierto tiempo (corrutina).
    /// </summary>
    public void Attack()
    {
        if(!attacked)
            StartCoroutine(AttackCoolDown());
    }

    /// <summary>
    /// True si puede comer de la presa.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Se come un bocado de la presa cada cierto tiempo (corrutina).
    /// </summary>
    public void Eat()
    {
        if(!eating)
            StartCoroutine(BiteCoolDown());
    }
    
    public override void SpawnChilds()
    {
        if(!CanBreed())
            return;
        _hasBreeded = true;
        partner._hasBreeded = true;
        FlockWolf pack = GetComponentInParent<FlockWolf>();
        
        //Crían de 1 a 2 lobos (puede salir 0 aunque tiene muy poca probabilidad)
        int tope = Mathf.CeilToInt(Random.value*2f);
        for (int i = 0; i < tope; i++)
        {
            //Se instancia el prefab del lobo
            GameObject child = Instantiate(pack.agentPrefabWolf, pack.transform);
            //Se le asigna un nombre
            child.name = "Lobo " + pack.total;
            pack.total++;
            //Se escala al 50% porque es una cría (más tarde crece en GrowUp() )
            child.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //Se añade a la manada
            pack.agents.Add(child.GetComponent<FlockAgentWolf>());
        }
        Regroup();          //Ambos vuelven con la manada
        partner.Regroup();
    }
    
    #endregion
        
    #region IEnumerators
    
    /// <summary>
    /// Le quita una cantidad de evida a la presa cada cinco segundos.
    /// </summary>
    /// <returns></returns>
    IEnumerator AttackCoolDown()
    {
        attacked = true;
        prey.currentHealth -= 5;
        yield return new WaitForSeconds(3);
        attacked = false;
    }
    
    /// <summary>
    /// Toma un bocado de la presa que le cura cada 2 segundos.
    /// </summary>
    /// <returns></returns>
    IEnumerator BiteCoolDown()
    {
        eating = true;
        prey.TakeBite();
        currentHealth += 5;
        hunger += 20;
        yield return new WaitForSeconds(2f);
        eating = false;
    }
    
    /// <summary>
    /// Crece a los 10-20 segundos. Actualiza su escala y pone _hasBreed a false.
    /// </summary>
    /// <returns></returns>
    IEnumerator GrowUp()
    {
        yield return new WaitForSeconds(Random.value *10f + 10f);
        transform.localScale = Vector3.one;
        _hasBreeded = false;
    }
    
    #endregion
}
