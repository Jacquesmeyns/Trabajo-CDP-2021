using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlockAgentRabbit : FlockAgent
{
    private bool _safe;
    public bool safe{ 
        get{ return _safe;}
        set{ _safe = value;}
    }

    private void Awake() {
        //awarenessRadius = 15f;
        currentHealth = startingHealth;
        foodBites = (int) startingHealth;
    }

    //Los árboles se construyen en código leyéndo el grafo de derecha a izquierda y de abajo a arriba
    //  Primero haz todos los nodos (no importa el orden), y luego monta las secuencias y los selectores
    //  en el orden que he dicho al principio
    private void ConstructBehaviorTree()
    {
        Debug.LogError("Conejo necesita implementación de árbol");
    }

    public bool isSafe(){
        return _safe;
    }
    
    public void GoAlone()
    {
        inFlock = false;
        this.tag = "FleeingRabbit";
    }

    public void Regroup()
    {
        inFlock = true;
        this.tag = "Rabbit";
    }

    //Devuelve true cuando ya no queda más que comer
    //  Cada vez reduce en uno los bocados disponibles
    
}
