﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgentRabbit : FlockAgent
{
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
        foodBites = (int) startingHealth/8;
        ConstructBehaviorTree();
        if (Random.value < 0.65)
            hasDug = true;
        //Para controlar que no haya demasiadas madrigueras, tienen sólo un 25% de poder cavar
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
        
        topNode = new Selector(new List<Node>{digSafetyZoneSequence, surviveSelector});
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
        burrowPosition = (Random.insideUnitSphere * 70);// - (Vector3.one*70);
        burrowPosition.y = 0.42f;//0.42 es la altura para que quede bonito
        yield return new WaitForSeconds(60);
        calledThread = false;
    }
}
