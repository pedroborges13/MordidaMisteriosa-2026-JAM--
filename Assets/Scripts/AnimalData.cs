using UnityEngine;

//All animals
public enum Species { Cat, Dog, Snake}
public enum Age { Baby, Adult }
public enum Temperament { Docile, Restless, Agressive } //Cats and Dogs
//public enum Weight { Light, Medium, Heavy}

//Specific to cats
public enum CatVisual { Orange, Black, Hairless}

//Specific to dogs
public enum DogVisual { Caramelo, Pinscher, Golden}

//Specific to snakes
//public enum SnakeType { Coral, Cascavel , Jiboia}

public abstract class AnimalData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string animalName;
    [SerializeField] private Species species;

    public string AnimalName => animalName;
    public Species Species => species;  
}

[CreateAssetMenu(fileName = "NewCat", menuName = "Animal/CatData")]
public class CatData : AnimalData
{
    [Header("Cat Specifics")]
    [SerializeField] private CatVisual catColor;
    [SerializeField] private Temperament temperament;
    [SerializeField] private Age age;

    public CatVisual CatColor => catColor;
    public Temperament Temperament => temperament;
    public Age Age => age;
}

[CreateAssetMenu(fileName = "NewDog", menuName = "Animal/DogData")]
public class DogData : AnimalData
{
    [Header("Dog Specifics")]
    [SerializeField] private DogVisual dogBreed;
    [SerializeField] private Temperament temperament;
    [SerializeField] private Age age;

    public DogVisual DogBreed => dogBreed;
    public Temperament Temperament => temperament;
    public Age Age => age;
}

[CreateAssetMenu(fileName = "NewSnake", menuName = "Animal/SnakeData")]
public class SnakeData : AnimalData
{
    [Header("Snake Specifics")]
    [SerializeField] bool isInstakill;
    
    public bool IsInstakill => isInstakill;
}


