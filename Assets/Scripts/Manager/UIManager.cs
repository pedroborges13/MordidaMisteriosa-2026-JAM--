using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] private Slider stressBar;

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
        }

        UpdateActionPointUI(RoundManager.Instance.ActionPoints);

        stressBar.minValue = 0;
        stressBar.maxValue = RoundManager.Instance.MaxStress;
        stressBar.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateActionPointUI(int actionPoint)
    {
        actionPointText.text = "Action Points: " + actionPoint.ToString();
        Debug.Log("Water button");
    }

    void UpdateStressBar(float currentStress)
    {
        stressBar.value = currentStress;
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
        }
    }
}
