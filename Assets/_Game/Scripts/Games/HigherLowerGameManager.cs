using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HigherLowerGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_Text currentNumberText;
    [SerializeField] private Button higherButton;
    [SerializeField] private Button lowerButton;
    [SerializeField] private Button passButton;

    private string playerName = "Player";
    private int currentNumber;
    private int nextNumber;
    private int streak = 0;

    protected override void Start()
    {
        base.Start();
        if (higherButton != null) higherButton.onClick.AddListener(() => Guess(true));
        if (lowerButton != null) lowerButton.onClick.AddListener(() => Guess(false));
        if (passButton != null) passButton.onClick.AddListener(PassTurn);
    }

    protected override string GetBGMName()
    {
        return "BGM_Puzzle_Loop";
    }

    protected override void OnInitGame()
    {
        currentNumber = Random.Range(1, 14);
        streak = 0;
        UpdateUI();
        if (higherButton != null) higherButton.interactable = true;
        if (lowerButton != null) lowerButton.interactable = true;
        if (passButton != null) passButton.interactable = false;
    }

    protected override bool ValidateInputs()
    {
        playerName = (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text)) ? playerNameInput.text : "Player";
        return true;
    }

    private void UpdateUI()
    {
        if (currentNumberText != null)
        {
            currentNumberText.text = GetCardName(currentNumber);
        }
        if (instructionText != null)
        {
            instructionText.text = $"{playerName} đang có {streak} lượt đúng. Số tiếp theo sẽ Cao hay Thấp hơn?";
        }
        if (passButton != null)
        {
            passButton.interactable = streak >= 3; // Cho phép dừng an toàn sau 3 lần đoán đúng liên tục
        }
    }

    public void Guess(bool isHigher)
    {
        if (currentState != GameState.Playing) return;

        // Lật bài
        AudioManager.Instance.PlaySFX("SFX_CardFlip");

        // Sinh số ngẫu nhiên mới khác số hiện tại
        do
        {
            nextNumber = Random.Range(1, 14);
        } while (nextNumber == currentNumber);

        bool isCorrect = (isHigher && nextNumber > currentNumber) || (!isHigher && nextNumber < currentNumber);

        if (isCorrect)
        {
            AudioManager.Instance.PlaySFX("SFX_Correct");
            currentNumber = nextNumber;
            streak++;
            UpdateUI();
        }
        else
        {
            AudioManager.Instance.PlaySFX("SFX_Wrong");
            AudioManager.Instance.PlaySFX("SFX_LoseFanfare");
            EndGame(playerName);
        }
    }

    public void PassTurn()
    {
        // Phát tiếng chuyền lượt
        AudioManager.Instance.PlaySFX("SFX_Pass");

        // Điểm danh sách an toàn, nạp số mới cho người kế tiếp
        streak = 0;
        currentNumber = Random.Range(1, 14);
        UpdateUI();
    }

    private string GetCardName(int val)
    {
        if (val == 1) return "A";
        if (val == 11) return "J";
        if (val == 12) return "Q";
        if (val == 13) return "K";
        return val.ToString();
    }
}
