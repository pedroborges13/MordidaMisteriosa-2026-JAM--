using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
    [SerializeField] private Transform animalTransform;
    [SerializeField] private GameObject snakeJumpscare;

    public int ActionPoints => actionPoints;
    public int MaxStress => maxStress;
    public GameObject SpawnedAnimalObject { get; private set; }

    [Header("Current Status")]
    [SerializeField] private float currentStress;
    [SerializeField] private AnimalData currentAnimal;
    public float CurrentStress => currentStress;    
    public AnimalData CurrentAnimal => currentAnimal; //Read only

    [Header("Box Animation")]
    [SerializeField] private Animator boxAnimator;

    [Header("Database")]
    [SerializeField] private List<AnimalData> animalList;

    //Events
    public event Action OnGameOver;
    public event Action<int> OnActionPointChanged;
    public event Action<float> OnStressChanged;
    public event Action<string> OnFeedbackReceived;
    public event Action<int> OnActionCost;
    public event Action<int> OnStressAdded;

    void Awake()
    {
        if (Instance == null) Instance = this;
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

        int randomIndex = Random.Range(0, possibleAnimals.Count);
        currentAnimal = possibleAnimals[randomIndex];

        SpawnedAnimalObject = Instantiate(currentAnimal.AnimalPrefab, animalTransform);
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

    private void AddStress(int amount)
    {
        currentStress += amount;

        currentStress = Mathf.Clamp(currentStress, 0, maxStress);

        OnStressChanged?.Invoke(currentStress);
        OnStressAdded?.Invoke(amount);

        if (currentStress >= maxStress)
        {
            Debug.Log("O animal ficou estressado demais e você perdeu!");

            GameManager.Instance.SetGameOver();
            UIManager.Instance.ShowGameOverScreen();

            OnGameOver?.Invoke();
        }
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
            if (currentAnimal is DogData dog)
            {
                if (dog.Size == Size.Small) AudioManager.Instance.PlaySFX(AudioManager.Instance.smallDogWhining);
                if (dog.Size == Size.Big) AudioManager.Instance.PlaySFX(AudioManager.Instance.bigDogWhining);
            }
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
                StartCoroutine(BoxOpenCloseAnimationRoutine());
                if (dog != null)
                {
                    if (dog.Size == Size.Small)
                    {
                        if (dog.Temperament == Temperament.Docile)
                        {
                            reaction = BoxReaction.SmallShake;
                            feedback = "Alguma coisa se sacudiu lá dentro... Porém a caixa não se moveu muito";
                            stressToAdd = 25;
                        }
                        else if (dog.Temperament == Temperament.Restless || dog.Temperament == Temperament.Agressive)
                        {
                            reaction = BoxReaction.BigShake;
                            feedback = "Meu Deus! Esse animalzinho com certeza tem força... Será que ele ama água? não entendi se quer brincar ou se odiou";
                            stressToAdd = 35;
                        }
                    }
                    else if (dog.Size == Size.Big)
                    {
                        if(dog.Temperament == Temperament.Docile)
                        {
                            reaction = BoxReaction.BigShake;
                            feedback = "Acho que é um animal grande. Mas não sei se gostou ou não da água";
                            stressToAdd = 25;
                        }
                        else if (dog.Temperament == Temperament.Restless || dog.Temperament == Temperament.Agressive)
                        {
                            reaction = BoxReaction.BigShake;
                            feedback = "Meu Deus! A caixa quase caiu.... Mas não sei se gostou ou não da água";
                            if (dog.Temperament == Temperament.Restless) stressToAdd = 25;
                            else if (dog.Temperament == Temperament.Agressive) stressToAdd = 35;
                        }
                    }
                }
                else if (snake != null)
                {
                    reaction = BoxReaction.SmallShake;
                    feedback = "Alguma coisa se sacudiu lá dentro... Porém a caixa não se moveu muito";
                    stressToAdd = 25;
                }
                break;

            case ActionType.Hand:
                StartCoroutine(BoxOpenCloseAnimationRoutine());

                if (snake != null)
                {
                    reaction = BoxReaction.Attack;
                    feedback = "";

                    snakeJumpscare.SetActive(true);
                    Animator snakeAnim = snakeJumpscare.GetComponent<Animator>();   

                    if (snakeAnim != null) snakeAnim.SetTrigger("HandAttack");

                    StartJumpScareRoutine();

                    return;
                }
                else if (dog != null)
                {
                    if (dog.Temperament == Temperament.Docile)
                    {
                        reaction = BoxReaction.SmallShake;
                        feedback = "Que fofo! Ele gostou do carinho, parece um animal bonzinho";
                        stressToAdd = 15;
                    }
                    else if (dog.Temperament == Temperament.Restless)
                    {
                        reaction = BoxReaction.SmallShake;
                        feedback = "Humm, esse bicho está meio agitado. Será que quer brincar ou foi um aviso para me manter longe?";
                        stressToAdd = 35;
                    }
                    else if (dog.Temperament == Temperament.Agressive)
                    {
                        reaction = BoxReaction.BigShake;
                        feedback = "AIII! Alguma coisa peluda me mordeu!!";
                        stressToAdd = 65;
                    }
                }
                break;

            case ActionType.Shake:
                if (snake != null)
                {
                    reaction = BoxReaction.BigShake;
                    feedback = "Não parece ser um bicho pesado, até que está leve a caixa... Com certeza ele se incomodou com meu chacoalhão";
                    stressToAdd = 35;
                }
                else if (dog != null)
                {
                    if (dog.Size == Size.Small)
                    {
                        if (dog.Temperament == Temperament.Docile)
                        {
                            reaction = BoxReaction.SmallShake;
                            feedback = "Não parece ser um bicho pesado, até que está leve a caixa... Aparentemente ele não se incomodou muito com o chacoalhão";
                            stressToAdd = 15;
                        }
                        else if (dog.Temperament == Temperament.Restless)
                        {
                            reaction = BoxReaction.BigShake;
                            feedback = "Não parece ser um bicho pesado, até que está leve a caixa... Aparentemente ele se incomodou com meu chacoalhão";
                            stressToAdd = 15;
                        }
                        else if (dog.Temperament == Temperament.Agressive)
                        {
                            reaction = BoxReaction.BigShake;
                            feedback = "Não parece ser um bicho pesado, até que está leve a caixa... Com certeza ele se incomodou com meu chacoalhão";
                            stressToAdd = 35;
                        }
                    }
                    else if (dog.Size == Size.Big)
                    {

                        if (dog.Temperament == Temperament.Docile)
                        {
                            reaction = BoxReaction.SmallShake;
                            feedback = "Que caixa pesada! Mas o grandão ai está bem tranquilo";
                            stressToAdd = 15;
                        }
                        else if (dog.Temperament == Temperament.Restless)
                        {
                            reaction = BoxReaction.BigShake;
                            feedback = "Que caixa pesada! Acho que o grandão quer brincar comigo... ou não gostou muito do meu chacoalhão";
                            stressToAdd = 25;
                        }
                        else if (dog.Temperament == Temperament.Agressive)
                        {
                            reaction = BoxReaction.BigShake;
                            feedback = "Que caixa pesada! Acho que o grandão quer brincar comigo... ou não gostou muito do meu chacoalhão";
                            stressToAdd = 35;
                        }
                    }
                }
                break;
        }

        AddStress(stressToAdd);
        OnFeedbackReceived?.Invoke(feedback);

        if (boxAnimator != null && reaction != BoxReaction.None) boxAnimator.SetTrigger(reaction.ToString());

        Debug.Log($"Stress Atual: {currentStress}/{maxStress} | Feedback: {feedback}");
    }   

    IEnumerator BoxOpenCloseAnimationRoutine()
    {
        if (boxAnimator != null)
        {
            boxAnimator.SetTrigger("Open");

            yield return new WaitForSeconds(4f);

            boxAnimator.SetTrigger("Close");
        }
    }

    public void StartJumpScareRoutine()
    {
        StartCoroutine(DelayedGameOver());
    }

    IEnumerator DelayedGameOver()
    {
        //snakeJumpscare.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.pianoJumpScare);
        yield return new WaitForSeconds(0.6f);
        snakeJumpscare.SetActive(false);
        UIManager.Instance.ShowGameOverScreen();
        OnGameOver?.Invoke();
    }
}
