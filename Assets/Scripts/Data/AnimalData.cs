using UnityEngine;

//All animals
public enum Species { Cat, Dog, Snake}
public enum Age { Baby, Adult }
public enum Temperament { Docile, Restless, Agressive } //Cats and Dogs
//public enum Weight { Light, Medium, Heavy}

//Specific to cats
public enum CatVisual { Orange, Black, Hairless}

//Specific to dogs
public enum Size { Small, Big}
//public enum DogVisual { Caramelo, Pinscher, Golden}

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



