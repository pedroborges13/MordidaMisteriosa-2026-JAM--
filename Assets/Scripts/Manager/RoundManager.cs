using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int actionPoints;
    [SerializeField] private int maxStress;
    [SerializeField] private float snakeProbability;

    public int ActionPoints => actionPoints;
    public int MaxStress => maxStress;

    [Header("Current Status")]
    [SerializeField] private float currentStress;
    [SerializeField] private AnimalData currentAnimal;

    [Header("Database")]
    [SerializeField] private List<AnimalData> animalList;

    public AnimalData CurrentAnimal => currentAnimal; //Read only

    //Events
    public event Action OnGameOver;
    public event Action<int> OnActionPointChanged;
    public event Action<float> OnStressChanged;
    public event Action<string> OnFeedbackReceived;
    public event Action<int> OnActionCost;
    public event Action<int> OnStressAdded;
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
        if (animalList == null || animalList.Count == 0) return;

        float roll = Random.Range(0f, 100f);
        
        List<AnimalData> possibleAnimals = new List<AnimalData>();

        if (roll <= snakeProbability)
        {
            foreach (var animal in animalList)
            {
                if (animal is SnakeData) possibleAnimals.Add(animal);
            }
        }
        else
        {
            foreach (var animal in animalList)
            {
                if (animal is DogData) possibleAnimals.Add(animal);
            }
        }

        int randomIndex = Random.Range(0,possibleAnimals.Count);
        currentAnimal = possibleAnimals[randomIndex];

        AnimalDebug();

    }

    void AnimalDebug()
    {
        string info = $"The animal is: {currentAnimal.AnimalName} ({currentAnimal.Species})";

        //if (currentAnimal is CatData cat) info += $"Age: {cat.Age} | Color: {cat.CatColor} | Temp: {cat.Temperament}";
        if (currentAnimal is DogData dog) info += $"Size: {dog.Size} | Temp: {dog.Temperament}";
        else if (currentAnimal is SnakeData snake) info += $" Instakill: {snake.IsInstakill}";

        Debug.Log(info);

    }

    public void ExecuteAction(ActionData action)
    {
        if (actionPoints < action.Cost)
        {
            Debug.Log("Out of Action Points!");
            return;
        }

        actionPoints -= action.Cost;
        OnActionCost?.Invoke(action.Cost);
        OnActionPointChanged?.Invoke(actionPoints);

        Reaction(action.Type);

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

        DogData dog = currentAnimal as DogData;
        SnakeData snake = currentAnimal as SnakeData;

        switch (actionType)
        {
            case ActionType.Water:
                if (dog != null)
                {
                    feedback = "Algo se sacudiu lá dentro...";

                    if (dog.Size == Size.Small)
                    {
                        reaction = BoxReaction.SmallShake;
                    }
                    else if (dog.Size == Size.Big)
                    {
                        reaction = BoxReaction.BigShake;
                    }

                    if (dog.Temperament == Temperament.Docile)
                    {
                        stressToAdd = 10;
                    }
                    else if (dog.Temperament == Temperament.Restless || dog.Temperament == Temperament.Agressive)
                    {
                        stressToAdd = 20;
                    }
                    
                }
                else if (snake != null)
                {
                    reaction = BoxReaction.None;
                    feedback = "Nenhuma reacão...";
                    stressToAdd = 0;
                }
                break;

            case ActionType.Hand:
                if (snake != null)
                {
                    reaction = BoxReaction.Attack;
                    feedback = "Você foi atacado!";
                    OnGameOver?.Invoke();
                    return;
                }
                else if (dog != null)
                {
                    if (dog.Temperament == Temperament.Docile)
                    {
                        reaction = BoxReaction.SmallShake;
                        feedback = "Humm, parece um animal bonzinho";
                        stressToAdd = 0;
                    }
                    else if (dog.Temperament == Temperament.Restless)
                    {
                        reaction = BoxReaction.SmallShake;
                        feedback = "Humm, ele está meio agitado";
                        stressToAdd = 20;
                    }
                    else if (dog.Temperament == Temperament.Agressive)
                    {
                        reaction = BoxReaction.BigShake;
                        feedback = "Fui mordido!!";
                        stressToAdd = 50;
                    }
                }
                break;

            case ActionType.Shake:
                if (snake != null)
                {
                    reaction = BoxReaction.BigShake;
                    feedback = "Não parece ser um bicho pesado, mas ele não gostou disso";
                    stressToAdd = 30;
                }
                else if (dog != null)
                {
                    if (dog.Size == Size.Small)
                    {
                        if (dog.Temperament == Temperament.Docile)
                        {
                            reaction = BoxReaction.SmallShake;
                            feedback = "A caixa é razoavelmente leve";
                            stressToAdd = 10;
                        }
                        else if (dog.Temperament == Temperament.Restless)
                        {
                            reaction = BoxReaction.BigShake;
                            feedback = "A caixa é leve";
                            stressToAdd = 10;
                        }
                        else if (dog.Temperament == Temperament.Agressive)
                        {
                            reaction = BoxReaction.BigShake;
                            feedback = "A caixa é leve";
                            stressToAdd = 30;
                        }
                    }
                    else if (dog.Size == Size.Big)
                    {
                        feedback = "Que caixa pesada!";

                        if (dog.Temperament == Temperament.Docile)
                        {
                            reaction = BoxReaction.SmallShake;
                            stressToAdd = 10;
                        }
                        else if (dog.Temperament == Temperament.Restless)
                        {
                            reaction = BoxReaction.BigShake;
                            stressToAdd = 10;
                        }
                        else if (dog.Temperament == Temperament.Agressive)
                        {
                            reaction = BoxReaction.BigShake;
                            stressToAdd = 30;
                        }
                    }
                }
                break;
        }

        currentStress += stressToAdd;
        OnStressChanged?.Invoke(currentStress);
        OnFeedbackReceived?.Invoke(feedback);
        OnStressAdded?.Invoke(stressToAdd);
        //OnBoxReact?.Invoke(reaction);

        Debug.Log($"Stress Atual: {currentStress}/{maxStress} | Feedback: {feedback}");
    }
}
