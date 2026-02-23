using UnityEngine;
using TMPro;
using System.Globalization;
using System;

public class GuessManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject clipboardPanel;
    [SerializeField] private GameObject dogOptionsPanel;

    [Header("Dropdown")]
    [SerializeField] private TMP_Dropdown speciesDropdown;
    [SerializeField] private TMP_Dropdown sizeDropdown;
    [SerializeField] private TMP_Dropdown temperamentDropdown;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI resultText;

    void Start()
    {
        clipboardPanel.SetActive(false);
        resultText.text = "";

        speciesDropdown.onValueChanged.AddListener(OnSpeciesChanged);

        OnSpeciesChanged(speciesDropdown.value);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleClipboard();
        }
    }

    void ToggleClipboard()
    {
        bool isActive = clipboardPanel.activeSelf;
        clipboardPanel.SetActive(!isActive);
    }

    //Return button
    public void CloseClipboard()
    {
        clipboardPanel.SetActive(false);
    }

    void OnSpeciesChanged(int index)
    {
        //Dog
        if (index == 0) dogOptionsPanel.SetActive(true); //Refers to the UI dropdown index value
        //Snake
        else if (index == 1) dogOptionsPanel.SetActive(false); //Refers to the UI dropdown index value
    }

    public void ConfirmGuess()
    {
        AnimalData currentAnimal = RoundManager.Instance.CurrentAnimal;

        bool guessedDog = (speciesDropdown.value == 0);

        if (currentAnimal is SnakeData)
        {
            if (!guessedDog) WinGame();
            else LoseGame();
        }
        else if (currentAnimal is DogData dog)
        {
            if (!guessedDog)
            {
                LoseGame();
            }

            //Size: 0 = Small, 1 = Big
            Size guessedSize;
            if (sizeDropdown.value == 0) guessedSize = Size.Small;
            else guessedSize = Size.Big;

            //Takes the dropdown int value and casts it to Temperament enum
            //Temperament: 0 = Docile, 1 = Restless, 2 = Agressive
            Temperament guessedTemp = (Temperament)temperamentDropdown.value;

            if (dog.Size == guessedSize && dog.Temperament == guessedTemp) WinGame();
            else LoseGame();
        }

        UIManager.Instance.StartRevelationTransition();
    }

    void WinGame()
    {
        CloseClipboard();
        GameManager.Instance.SetVictory();
    }

    void LoseGame()
    {
        CloseClipboard();
        GameManager.Instance.SetGameOver();
    }

}
