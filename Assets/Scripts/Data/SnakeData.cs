using UnityEngine;

[CreateAssetMenu(fileName = "NewSnake", menuName = "Animal/SnakeData")]

public class SnakeData : AnimalData
{
    [Header("Snake Specifics")]
    [SerializeField] bool isInstakill;

    public bool IsInstakill => isInstakill;
}