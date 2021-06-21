using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// Instancia los scripts que instanciarán los agentes y prepara el mapa de forma aleatoria.
/// </summary>
public class Spawner : MonoBehaviour
{
    //Todos los prefabs a instanciar
    [SerializeField] private FlockWolf wolfPackPrefab;
    [SerializeField] private FlockRabbit rabbitFlockPrefab;
    [Header("Environment prefabs")] 
    [SerializeField] private GameObject treeObstacle;
    [SerializeField] private GameObject stoneObstacle;
    [SerializeField] private GameObject grass;

    [Header("Environment generation variables")]
    [Range(0,10)] public int minCumulus = 0;    //Min-Max de nodos de obstáculos. Cada nodo tiene un árbol y varias piedras.
    [Range(10,30)] public int maxCumulus = 10;
    [Range(0,2)] public int minStones = 0;
    [Range(2,10)] public int maxStones = 2;
    [Range(1, 5)] public float stonesRadius = 1;    //Radio de aparición de las piedras alrededor del árbol.
    [Range(0, 30)] public int startingRabbitFood = 0;
    [Range(1, 120)] public int grassSpawnTime = 1;  //Segundos que pasan entre instanciaciones de césped.

    internal bool called; //Para controlar las llamadas de la corrutina
    internal bool started;
    private GameObject scene;   //Referencia a la escena
    
    /// <summary>
    /// Comienza la simulación
    /// </summary>
    public void StartSimulation()
    {
        scene = GameObject.Find("Escenario");
        //Los flocks se van a instanciar en posiciones aleatorias dentro de un radio (dentro del mapa)
        GameObject wolfPackSpawnPositionsObject = new GameObject();
        GameObject rabbitFlockSpawnPositionsObject = new GameObject();
        Vector3 ranPos = Random.insideUnitSphere*60;
        wolfPackSpawnPositionsObject.transform.position = new Vector3(ranPos.x, 0.6f, ranPos.z);
        ranPos = Random.insideUnitSphere*60;
        rabbitFlockSpawnPositionsObject.transform.position= new Vector3(ranPos.x, 0.6f, ranPos.z);
        
        //Creamos las manadas de lobos y conejos
        Instantiate(wolfPackPrefab, wolfPackSpawnPositionsObject.transform);
        Instantiate(rabbitFlockPrefab, rabbitFlockSpawnPositionsObject.transform);
        
        //Creo los obstáculos del mapa
        int nObstaculos = Random.Range(minCumulus, maxCumulus);   //De min a max cúmulos de obstáculos
        for (int i = 0; i < nObstaculos; i++)
        {
            ranPos = Random.insideUnitSphere*60;
            ranPos.y = 0f;
            GameObject cumulo = new GameObject();
            cumulo.name = "Obstacle cumulus " + i;
            cumulo.transform.position = ranPos;
            
            //Instancia del centro del cúmulo. Un árbol
            Instantiate(treeObstacle, 
                cumulo.transform.position, 
                Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
                cumulo.transform);
            //Por cada cúmulo se instancian alrededor del mismo una serie de obstáculos en posiciones aleatorias
            int nStoneObstacles = Random.Range(minStones, maxStones);
            for (int j = 0; j < nStoneObstacles; j++)
            {
                //Usa la posición del cúmulo como centro
                ranPos = Random.insideUnitSphere * stonesRadius * 10;
                ranPos += cumulo.transform.position;
                ranPos.y = 0f;
                //cumulo.transform.position = ranPos;
                Instantiate(stoneObstacle, 
                    ranPos,
                    Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
                    cumulo.transform);
            }
        }
        
        //Spawn del césped inicial para los conejos
        for (int i = 0; i < startingRabbitFood; i++)
        {
            ranPos = Random.insideUnitSphere * 70;
            ranPos.y = 0.6f;
            Instantiate(grass,
                ranPos,
                Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
                scene.transform);
        }

        started = true;
    }

    /// <summary>
    /// Reinicia la escena
    /// </summary>
    public void ResetSimulation()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    void Update()
    {
        //Genera césped cada x tiempo
        if (!called && started)
            StartCoroutine(SpawnGrass());
    }

    /// <summary>
    /// Instancia una mata de césped cada vez que es llamada
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnGrass()
    {
        called = true;
        Vector3 ranPos = Random.insideUnitSphere * 70;
        ranPos.y = 0.6f;
        Instantiate(grass,
            ranPos,
            Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
            scene.transform);

        yield return new WaitForSeconds(grassSpawnTime);
        called = false;
    }
}
