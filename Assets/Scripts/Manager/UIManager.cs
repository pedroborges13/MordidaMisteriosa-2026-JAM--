using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] private Slider stressBar;

    [Header("Screens")]
    [SerializeField] private GameObject HUDobject;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private TextMeshProUGUI victoryMessageText;
    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private TextMeshProUGUI gameoverMessageText;

    [Header("Feedback UI")]
    [SerializeField] private GameObject actionButtonsGroup;
    [SerializeField] private GameObject feedbackBackground;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float feedbackDuration;
    [SerializeField] private float typingSpeed;
    [SerializeField] private float barAnimationSpeed;
    private Coroutine feedbackCoroutine;
    private Coroutine stressBarCoroutine;

    [Header("Floating numbers")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Transform mainCanvasTransform;
    [SerializeField] private Transform stressBarTransform;
    [SerializeField] private Transform actionPointsTransform;

    [Header("Revelation System")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject revelationCamera;
    [SerializeField] private float fadeDuration;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.OnActionPointChanged += UpdateActionPointUI;
            RoundManager.Instance.OnStressChanged += UpdateStressBar;
            RoundManager.Instance.OnFeedbackReceived += ShowFeedback;
            RoundManager.Instance.OnStressAdded += HandleStressGain;
            RoundManager.Instance.OnActionCost += HandleActionCost;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChange;
        }

        UpdateActionPointUI(RoundManager.Instance.ActionPoints);

        if (!HUDobject.activeSelf) EnableHUD();

        stressBar.minValue = 0;
        stressBar.maxValue = RoundManager.Instance.MaxStress;
        stressBar.value = 0;
    }

    void HandleStateChange(GameState newState)
    {
        pauseScreen.SetActive(false);
        victoryScreen.SetActive(false);
        gameoverScreen.SetActive(false);

        if (newState == GameState.Paused) pauseScreen.SetActive(true);
    }

    void ShowFeedback(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);

        feedbackCoroutine = StartCoroutine(FeedbackLettersAnimation(message));   
    }

    IEnumerator FeedbackLettersAnimation(string message)
    {
        actionButtonsGroup.SetActive(false);
        feedbackText.text = "";    
        feedbackBackground.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        foreach (char letter in message.ToCharArray())
        {
            feedbackText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(feedbackDuration);

        actionButtonsGroup.SetActive(true);
        feedbackBackground.SetActive(false);
    }

    void HandleActionCost(int amount)
    {
        SpawnFloatingText("-" + amount.ToString(), Color.black, actionPointsTransform);
    }

    void HandleStressGain(int amount)
    {
        //Only spawns the text if the added stress if greater than zero
        if (amount > 0)
        {
            SpawnFloatingText("+" + amount.ToString(), Color.orange, stressBarTransform);
        }
    }
    void SpawnFloatingText(string message, Color color, Transform targetTransform)
    {
        if (floatingTextPrefab == null || mainCanvasTransform == null) return;

        GameObject textInstance = Instantiate(floatingTextPrefab, mainCanvasTransform);

        textInstance.transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y - 100f, targetTransform.position.z - 1f); 

        textInstance.GetComponent<FloatingText>().Setup(message, color);
    }

    void UpdateActionPointUI(int actionPoint)
    {
        actionPointText.text = "Pontos de Ação: " + actionPoint.ToString();
    }

    void UpdateStressBar(float currentStress)
    {
        if (stressBarCoroutine != null) StopCoroutine(stressBarCoroutine);

        stressBarCoroutine = StartCoroutine(StressBarAnimation(currentStress));
    }

    IEnumerator StressBarAnimation(float targetValue)
    {
        float startValue = stressBar.value;
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime * barAnimationSpeed;

            stressBar.value = Mathf.Lerp(startValue, targetValue, time);

            yield return null;
        }

        stressBar.value = targetValue;
    }

    public void OnClickButton(ActionData data)
    {
        RoundManager.Instance.ExecuteAction(data);
    }

    public void StartRevelationTransition()
    {
        DisableHUD();
        StartCoroutine(RevelationTransitionRoutine());
    }

    IEnumerator RevelationTransitionRoutine()
    {
        //Fade out: Screen becomes black
        yield return StartCoroutine(FadeRoutine(1f));

        //Switch cameras
        mainCamera.SetActive(false);
        revelationCamera.SetActive(true);

        // --- SNAKE ATTACK LOGIC ---
        if (GameManager.Instance.CurrentState == GameState.GameOver)
        {
            if (RoundManager.Instance.CurrentAnimal is SnakeData)
            {
                GameObject animalObject = RoundManager.Instance.SpawnedAnimalObject;

                if (animalObject != null)
                {
                    Animator snakeAnim = animalObject.GetComponent<Animator>();

                    if (snakeAnim != null) snakeAnim.SetTrigger("Attack");
                }
            }
            else
            {
                //You lost, but it was a dog (characteristics didn't match)
                yield return new WaitForSeconds(5f);
            }
        }
        // ------------------------

        //Dramatic pause while the screen is black
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeRoutine(0f));

        yield return new WaitForSeconds(5f);

        ShowFinalResultScreen();
    }

    IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time/fadeDuration);

            yield return null;  
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    void DisableHUD() => HUDobject.SetActive(false);

    void EnableHUD() => HUDobject.SetActive(true);

    void ShowFinalResultScreen()
    {
        GameState currentState = GameManager.Instance.CurrentState;
        GuessManager guessManager = FindAnyObjectByType<GuessManager>();

        if (currentState == GameState.Victory)
        {
            victoryScreen.SetActive(true);
            victoryMessageText.text = guessManager.FinalMessage;
        }
        else if (currentState == GameState.GameOver)
        {
            gameoverScreen.SetActive(true);
            gameoverMessageText.text = guessManager.FinalMessage;
        }
    }

    private void OnDestroy()
    {
        if (RoundManager.Instance != null)
        {
            RoundManager.Instance.OnActionPointChanged -= UpdateActionPointUI;
            RoundManager.Instance.OnStressChanged -= UpdateStressBar;
            RoundManager.Instance.OnFeedbackReceived -= ShowFeedback;
            RoundManager.Instance.OnStressAdded -= HandleStressGain;
            RoundManager.Instance.OnActionCost -= HandleActionCost;
        }

        if (GameManager.Instance != null) GameManager.Instance.OnStateChanged -= HandleStateChange;
    }
}
