using UnityEngine;

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
