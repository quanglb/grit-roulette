using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorTrapGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_Text wordDisplay;
    [SerializeField] private Button[] answerButtons; // 0: Red, 1: Green, 2: Blue, 3: Yellow
    [SerializeField] private Image timeBar;

    private string playerName = "Player";
    private string[] colorNames = { "RED", "GREEN", "BLUE", "YELLOW" };
    private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };

    private int targetWordIndex;
    private int targetColorIndex;
    private bool isTextMode; // true: trả lời theo Chữ hiển thị, false: trả lời theo Màu thực tế
    private int score = 0;
    private float maxTime = 3f;
    private float timeRemaining;

    protected override void Start()
    {
        base.Start();
        if (answerButtons != null)
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                int idx = i;
                if (answerButtons[i] != null)
                {
                    answerButtons[i].onClick.AddListener(() => SubmitAnswer(idx));
                }
            }
        }
    }

    protected override void OnInitGame()
    {
        score = 0;
        NextQuestion();
    }

    protected override bool ValidateInputs()
    {
        playerName = (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text)) ? playerNameInput.text : "Player";
        return true;
    }

    private void NextQuestion()
    {
        if (score >= 5)
        {
            if (instructionText != null) instructionText.text = $"{playerName} đã vượt qua thử thách Bẫy Màu Sắc an toàn!";
            EndGame("Không có (Chiến thắng)");
            return;
        }

        targetWordIndex = Random.Range(0, 4);
        targetColorIndex = Random.Range(0, 4);
        isTextMode = Random.value > 0.5f;

        if (wordDisplay != null)
        {
            wordDisplay.text = colorNames[targetWordIndex];
            wordDisplay.color = colors[targetColorIndex];
        }

        if (instructionText != null)
        {
            instructionText.text = isTextMode ? "HÃY CHỌN THEO CHỮ HIỂN THỊ!" : "HÃY CHỌN THEO MÀU SẮC THỰC TẾ!";
        }
        timeRemaining = maxTime;
    }

    private void Update()
    {
        if (currentState != GameState.Playing) return;

        timeRemaining -= Time.deltaTime;
        if (timeBar != null)
        {
            timeBar.fillAmount = timeRemaining / maxTime;
        }

        if (timeRemaining <= 0)
        {
            EndGame(playerName);
        }
    }

    public void SubmitAnswer(int buttonIndex)
    {
        if (currentState != GameState.Playing) return;

        int correctIndex = isTextMode ? targetWordIndex : targetColorIndex;

        if (buttonIndex == correctIndex)
        {
            score++;
            NextQuestion();
        }
        else
        {
            EndGame(playerName);
        }
    }
}
