using System.Collections;
using UnityEngine;

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
    [Range(0,10)] public int minCumulus;    //Min-Max de nodos de obstáculos. Cada nodo tiene un árbol y varias piedras.
    [Range(10,30)] public int maxCumulus;
    [Range(0,2)] public int minStones;
    [Range(2,10)] public int maxStones;
    [Range(1, 5)] public float stonesRadius;    //Radio de aparición de las piedras alrededor del árbol.
    [Range(0, 30)] public int startingRabbitFood;
    [Range(1, 120)] public int grassSpawnTime;  //Segundos que pasan entre instanciaciones de césped.

    internal bool called;   //Para controlar las llamadas de la corrutina
    private GameObject scene;   //Referencia a la escena
    
    // Start is called before the first frame update
    private void Start()
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

    }

    // Update is called once per frame
    void Update()
    {
        //Genera césped cada x tiempo
        if (!called)
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
