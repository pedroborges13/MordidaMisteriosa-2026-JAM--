using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int actionPoints;
    [SerializeField] private int maxStress;

    [Header("Current Status")]
    [SerializeField] private int currentStress;
    [SerializeField] private AnimalData currentAnimal;

    [Header("Database")]
    [SerializeField] private List<AnimalData> animalList;

    public AnimalData CurrentAnimal => currentAnimal; //read only

    //Events
    public event Action OnGameOver;
    //public event Action OnBoxReaction

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentStress = 0;
        RandomAnimal();
    }

    void RandomAnimal()
    {
        if (animalList != null && animalList.Count > 0)
        {
            int randomIndex = Random.Range(0, animalList.Count);
            currentAnimal = animalList[randomIndex];

            AnimalDebug();
        }
    }

    void AnimalDebug()
    {
        string info = $"The animal is: {currentAnimal.AnimalName} ({currentAnimal.Species})";

        if (currentAnimal is CatData cat) info += $"Age: {cat.Age} | Color: {cat.CatColor} | Temp: {cat.Temperament}";
        else if (currentAnimal is DogData dog) info += $"Age: {dog.Age} | Breed: {dog.DogBreed} | Temp: {dog.Temperament}";
        else if (currentAnimal is SnakeData snake) info += $" Instakill: {snake.IsInstakill}";

    }

    public void ExecuteAction(ActionData action)
    {
        if (actionPoints < action.Cost)
        {
            Debug.Log("Out of Action Points!");
            return;
        }

        actionPoints -= action.Cost;

        if (currentStress >= maxStress)
        {
            Debug.Log("You stressed the animal and it ran away :( ");
            OnGameOver?.Invoke();
        }
    }

    void Reaction(ActionType actionType)
    {
        BoxReaction reaction = BoxReaction.None;
        string feedback = "";
        int stressToAdd = 0;

        switch (actionType)
        {
            case ActionType.Water:
                if (currentAnimal is CatData)
                {
                    reaction = BoxReaction.BigShake;
                    feedback = "Parece que o animal nao gostou...";
                    stressToAdd = 30;
                }
                else if (currentAnimal is DogData)
                {
                    reaction = BoxReaction.SmallShake;
                    feedback = "Algo se sacudiu lá dentro...";
                    stressToAdd = 10;
                }
                else if (currentAnimal is SnakeData)
                {
                    reaction = BoxReaction.None;
                    feedback = "Nenhuma reacão...";
                    stressToAdd = 0;
                }
                break;

            case ActionType.Hand:
                if (currentAnimal is CatData)
                {
                    reaction = BoxReaction.SmallShake;
                    feedback = "";
                    stressToAdd = 0;
                }
                else if (currentAnimal is DogData)
                {
                    reaction = BoxReaction.SmallShake;
                    feedback = "";
                    stressToAdd = 0;
                }
                else if (currentAnimal is SnakeData)
                {
                    reaction = BoxReaction.Attack;
                    feedback = "Você foi atacado!";
                    stressToAdd = 0;
                }
                break;
        }
    }
}
