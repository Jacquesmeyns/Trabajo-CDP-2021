using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Spawner spawner;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Slider sliderMinCumulus;
    [SerializeField] private Slider sliderMaxCumulus;
    [SerializeField] private Slider sliderMinStones;
    [SerializeField] private Slider sliderMaxStones;
    [SerializeField] private Slider sliderStonesRadius;
    [SerializeField] private Slider sliderStartingRabbitFood;
    [SerializeField] private Slider sliderGrassSpawnTime;

    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;

    public void ChangeMinCumulus()
    {
        spawner.minCumulus = (int)sliderMinCumulus.value;
    }
    public void ChangeMaxCumulus()
    {
        spawner.maxCumulus = (int)sliderMaxCumulus.value;
    }
    
    public void ChangeMinStones()
    {
        spawner.minStones = (int)sliderMinStones.value;
    }
    public void ChangeMaxStones()
    {
        spawner.maxStones = (int)sliderMaxStones.value;
    }
    
    public void ChangeStonesRadius()
    {
        spawner.stonesRadius = (int)sliderStonesRadius.value;
    }
    
    public void ChangeRabbitStartingFood()
    {
        spawner.startingRabbitFood = (int)sliderStartingRabbitFood.value;
    }
    
    public void ChangeGrassSpawnTime()
    {
        spawner.grassSpawnTime = (int)sliderGrassSpawnTime.value;
    }

    public void StartButton()
    {
        if(!spawner.started)
            spawner.StartSimulation();

        mainMenu.SetActive(false);
        stopButton.gameObject.SetActive(true);
    }

    public void StopButton()
    {
        spawner.ResetSimulation();
    }
}
