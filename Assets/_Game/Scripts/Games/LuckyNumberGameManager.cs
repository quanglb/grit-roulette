using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LuckyNumberGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private UnityEngine.UI.Dropdown playerCountDropdown;
    [SerializeField] private TMP_InputField minRangeInput;
    [SerializeField] private TMP_InputField maxRangeInput;
    [SerializeField] private TMP_InputField numberChoiceInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button revealButton;

    private int playerCount = 3;
    private int minRange = 1;
    private int maxRange = 10;
    private int secretNumber;
    private int currentPlayerIndex = 0;
    
    private List<string> players = new List<string>();
    private Dictionary<string, int> playerChoices = new Dictionary<string, int>();

    protected override void Start()
    {
        base.Start();
        if (submitButton != null) submitButton.onClick.AddListener(SubmitChoice);
        if (revealButton != null) revealButton.onClick.AddListener(RevealResult);
    }

    protected override void OnInitGame()
    {
        players.Clear();
        playerChoices.Clear();
        for (int i = 1; i <= playerCount; i++) players.Add($"Player {i}");

        secretNumber = Random.Range(minRange, maxRange + 1);
        currentPlayerIndex = 0;
        
        if (submitButton != null) submitButton.gameObject.SetActive(true);
        if (revealButton != null) revealButton.gameObject.SetActive(false);
        if (numberChoiceInput != null)
        {
            numberChoiceInput.gameObject.SetActive(true);
            numberChoiceInput.text = "";
        }

        AskNextPlayerChoice();
    }

    protected override bool ValidateInputs()
    {
        if (playerCountDropdown != null)
        {
            playerCount = playerCountDropdown.value + 3; // 3-6 players
        }
        else
        {
            playerCount = 3;
        }

        if (minRangeInput != null)
        {
            int.TryParse(minRangeInput.text, out minRange);
        }
        if (maxRangeInput != null)
        {
            int.TryParse(maxRangeInput.text, out maxRange);
        }

        if (minRange >= maxRange) maxRange = minRange + 10;
        return true;
    }

    private void AskNextPlayerChoice()
    {
        if (instructionText != null)
        {
            instructionText.text = $"{players[currentPlayerIndex]}, hãy nhập số của bạn từ {minRange} đến {maxRange}:";
        }
        if (numberChoiceInput != null) numberChoiceInput.text = "";
    }

    public void SubmitChoice()
    {
        if (currentState != GameState.Playing) return;

        int choice;
        if (numberChoiceInput == null || !int.TryParse(numberChoiceInput.text, out choice) || choice < minRange || choice > maxRange)
        {
            if (instructionText != null)
            {
                instructionText.text = $"Số nhập không hợp lệ! {players[currentPlayerIndex]} vui lòng nhập lại số trong khoảng [{minRange}-{maxRange}]:";
            }
            return;
        }

        playerChoices[players[currentPlayerIndex]] = choice;
        currentPlayerIndex++;

        if (currentPlayerIndex < playerCount)
        {
            AskNextPlayerChoice();
        }
        else
        {
            if (instructionText != null) instructionText.text = "Tất cả đã chọn số xong! Hãy mở số bí mật.";
            if (submitButton != null) submitButton.gameObject.SetActive(false);
            if (numberChoiceInput != null) numberChoiceInput.gameObject.SetActive(false);
            if (revealButton != null) revealButton.gameObject.SetActive(true);
        }
    }

    public void RevealResult()
    {
        int maxDiff = -1;
        List<string> losers = new List<string>();

        string detailText = $"Số bí mật là: {secretNumber}\n\n";

        foreach (var choice in playerChoices)
        {
            int diff = Mathf.Abs(choice.Value - secretNumber);
            detailText += $"{choice.Key} chọn {choice.Value} (Chênh lệch: {diff})\n";
            
            if (diff > maxDiff)
            {
                maxDiff = diff;
                losers.Clear();
                losers.Add(choice.Key);
            }
            else if (diff == maxDiff)
            {
                losers.Add(choice.Key);
            }
        }

        if (instructionText != null) instructionText.text = detailText;
        if (revealButton != null) revealButton.gameObject.SetActive(false);

        EndGame(string.Join(", ", losers));
    }
}
