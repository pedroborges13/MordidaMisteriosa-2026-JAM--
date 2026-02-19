using UnityEngine;

public enum GameState { Playing, Paused, GameOver, Victory}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
