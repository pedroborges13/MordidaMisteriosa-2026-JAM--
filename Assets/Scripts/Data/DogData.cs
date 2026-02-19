using UnityEngine;

[CreateAssetMenu(fileName = "NewDog", menuName = "Animal/DogData")]
public class DogData : AnimalData
{
    [Header("Dog Specifics")]
    [SerializeField] private Size size;
    [SerializeField] private Temperament temperament;

    public Size Size => size;
    public Temperament Temperament => temperament;
}
