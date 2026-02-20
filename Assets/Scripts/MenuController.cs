using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject menuButtons;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OptionsMenu()
    {
        menuButtons.SetActive(false);
        optionsScreen.SetActive(true);
        creditsScreen.SetActive(false);
    }
    public void CreditsMenu()
    {
        menuButtons.SetActive(false);
        optionsScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void BackToMenu()
    {
        menuButtons.SetActive(true);
        optionsScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_WEBGL
        SceneManager.LoadScene("MainMenu");
        #else
        Application.Quit();
        #endif
    }
}
