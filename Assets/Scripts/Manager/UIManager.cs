using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] private Slider stressBar;

    [Header("Screens")]
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
    [SerializeField] private Transform stressBarTransform;
    [SerializeField] private Transform actionPointsTransform;

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

        stressBar.minValue = 0;
        stressBar.maxValue = RoundManager.Instance.MaxStress;
        stressBar.value = 0;
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
        if (floatingTextPrefab == null) return;

        GameObject textInstance = Instantiate(floatingTextPrefab, targetTransform.position, Quaternion.identity);

        textInstance.transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, targetTransform.position.z - 1f); 
        textInstance.GetComponentInChildren<FloatingText>().Setup(message, color);

        Debug.Log($"Spawnou: {textInstance.name} na posição {textInstance.transform.position}");
    }

    void HandleStateChange(GameState newState)
    {
        pauseScreen.SetActive(false);
        victoryScreen.SetActive(false);
        gameoverScreen.SetActive(false);

        if (newState == GameState.Paused) pauseScreen.SetActive(true);
        else if (newState == GameState.Victory)
        {
            Debug.Log("Victory");
            victoryScreen.SetActive(true);
            victoryMessageText.text = FindAnyObjectByType<GuessManager>().FinalMessage;
        }
        else if (newState == GameState.GameOver) 
        {
            gameoverScreen.SetActive(true);
            gameoverMessageText.text = FindAnyObjectByType<GuessManager>().FinalMessage;
        }
    }

    void UpdateActionPointUI(int actionPoint)
    {
        actionPointText.text = "Action Points: " + actionPoint.ToString();
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
