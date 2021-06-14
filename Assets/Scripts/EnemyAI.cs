using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private float lowHealthThreshold;
    [SerializeField] private float healthRestoreRate;

    [SerializeField] private float chasingRange;
    [SerializeField] private float shootingRange;


    [SerializeField] private Transform playerTransform;
    [SerializeField] private Cover[] availableCovers;

    private Material material;
    private Transform bestCoverSpot;
    private NavMeshAgent agent;


    //Empieza el Behavior Tree
    private Node topNode;

    private float _currentHealth;
    public float currentHealth
    {
        get{ return _currentHealth; }
        set{ _currentHealth = Mathf.Clamp(value, 0, startingHealth); }
    }

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        currentHealth = startingHealth;
        
        ConstructBehaviorTree();
    }

    //Los árboles se construyen en código leyéndo el grafo de derecha a izquierda y de abajo a arriba
    //  Primero haz todos los nodos (no importa el orden), y luego monta las secuencias y los selectores
    //  en el orden que he dicho al principio
    private void ConstructBehaviorTree()
    {
        /*
        IsCoverAvailableNode coverAvailableNode = new IsCoverAvailableNode(availableCovers, playerTransform, this);
        GoToCoverNode goToCoverNode = new GoToCoverNode(agent, this);
        HealthNode healthNode = new HealthNode(this, lowHealthThreshold);
        IsCoveredNode isCoveredNode = new IsCoveredNode(playerTransform, transform);
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, this);
        RangeNode chasingRangeNode = new RangeNode(chasingRange, playerTransform, transform);
        RangeNode shootingRangeNode = new RangeNode(shootingRange, playerTransform, transform);
        ShootNode shootNode = new ShootNode(agent, this);

        Sequence chaseSequence = new Sequence(new List<Node> { chasingRangeNode, chaseNode});
        Sequence shootSequence = new Sequence(new List<Node> { shootingRangeNode, shootNode});
        Sequence goToCoverSequence = new Sequence(new List<Node> { coverAvailableNode , goToCoverNode});

        Selector findCoverSelector = new Selector(new List<Node> { goToCoverSequence, chaseSequence});
        Selector tryToTakeCoverSelector = new Selector(new List<Node> { isCoveredNode, findCoverSelector});
        Sequence mainCoverSequence = new Sequence(new List<Node> { healthNode, tryToTakeCoverSelector});

        topNode = new Selector (new List<Node> {mainCoverSequence, shootSequence, chaseSequence});
    */}


    private void Update()
    {
        topNode.Evaluate();
        if(topNode.nodeState == NodeState.FAILURE)
        {
            SetColor(Color.red);
        }
        currentHealth += Time.deltaTime * healthRestoreRate;
    }

    public void SetColor(Color color)
    {
        material.color = color;
    }

    public void SetBestCoverSpot(Transform bestCoverSpot)
    {
        this.bestCoverSpot = bestCoverSpot;
    }

    public Transform GetBestCoverSpot()
    {
        return bestCoverSpot;
    }
}
