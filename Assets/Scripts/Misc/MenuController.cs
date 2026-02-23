using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject menuButtons;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen1;
    [SerializeField] private GameObject creditsScreen2;
    [SerializeField] private GameObject titleObj;

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OptionsMenu()
    {
        menuButtons.SetActive(false);
        optionsScreen.SetActive(true);
        creditsScreen1.SetActive(false);
        creditsScreen2.SetActive(false);
        titleObj.SetActive(false);
    }
    public void CreditsMenu()
    {
        menuButtons.SetActive(false);
        optionsScreen.SetActive(false);
        creditsScreen1.SetActive(true);
        creditsScreen2.SetActive(true);
        titleObj.SetActive(false);
    }

    public void BackToMenu()
    {
        menuButtons.SetActive(true);
        optionsScreen.SetActive(false);
        creditsScreen1.SetActive(false);
        creditsScreen2.SetActive(false);
        titleObj.SetActive(true);
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
