using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private FlockWolf wolfPackPrefab;
    [SerializeField] private FlockRabbit rabbitFlockPrefab;
    [Header("Posiciones de aparición")]
    private readonly List<Transform> _wolfSpawnPoints = new List<Transform>();
    private readonly List<Transform> _rabbitSpawnPoints = new List<Transform>();
    
    // Start is called before the first frame update
    private void Start()
    {
        //Cargo las posibles posiciones de aparición
        var wolfPackSpawnPositionsObject = GameObject.Find("WolvesSpawnPoints");
        var rabbitFlockSpawnPositionsObject = GameObject.Find("RabbitSpawnPoints");
        
        foreach (Transform child in wolfPackSpawnPositionsObject.GetComponent<Transform>())
        {
            _wolfSpawnPoints.Add(child);
        }
        
        foreach (Transform child in rabbitFlockSpawnPositionsObject.GetComponent<Transform>())
        {
            _rabbitSpawnPoints.Add(child);
        }

        //Creamos las manadas de lobos y conejos
        Instantiate(wolfPackPrefab, _wolfSpawnPoints[Random.Range(0,_wolfSpawnPoints.Count)]);
        //FlockRabbit rabbitFlock = Instantiate(rabbitFlockPrefab, _rabbitSpawnPoints[Random.Range(0,_rabbitSpawnPoints.Count)]);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
