using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DiceBattleGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private UnityEngine.UI.Dropdown playerCountDropdown;
    [SerializeField] private Button rollButton;
    [SerializeField] private TMP_Text diceResultsText;

    private int playerCount = 4;
    private List<string> players = new List<string>();
    private int currentPlayerIndex = 0;
    private Dictionary<string, int> playerScores = new Dictionary<string, int>();

    protected override void Start()
    {
        base.Start();
        if (rollButton != null) rollButton.onClick.AddListener(RollDice);
    }

    protected override void OnInitGame()
    {
        players.Clear();
        playerScores.Clear();
        for (int i = 1; i <= playerCount; i++) players.Add($"Player {i}");

        currentPlayerIndex = 0;
        if (diceResultsText != null) diceResultsText.text = "Bắt đầu đổ xúc xắc!";
        if (rollButton != null) rollButton.interactable = true;
        AskNextPlayerRoll();
    }

    protected override bool ValidateInputs()
    {
        if (playerCountDropdown != null)
        {
            playerCount = playerCountDropdown.value + 2;
        }
        else
        {
            playerCount = 4;
        }
        return true;
    }

    private void AskNextPlayerRoll()
    {
        if (instructionText != null)
        {
            instructionText.text = $"Lượt của: {players[currentPlayerIndex]}. Hãy đổ xúc xắc!";
        }
    }

    public void RollDice()
    {
        if (currentState != GameState.Playing) return;

        int score = Random.Range(1, 7);
        playerScores[players[currentPlayerIndex]] = score;
        
        string resultStr = "";
        for (int i = 0; i <= currentPlayerIndex; i++)
        {
            resultStr += $"{players[i]}: {playerScores[players[i]]}\n";
        }
        if (diceResultsText != null) diceResultsText.text = resultStr;

        currentPlayerIndex++;

        if (currentPlayerIndex < playerCount)
        {
            AskNextPlayerRoll();
        }
        else
        {
            EvaluateResults();
        }
    }

    private void EvaluateResults()
    {
        if (rollButton != null) rollButton.interactable = false;

        int minScore = 7;
        List<string> losers = new List<string>();

        foreach (var score in playerScores)
        {
            if (score.Value < minScore)
            {
                minScore = score.Value;
                losers.Clear();
                losers.Add(score.Key);
            }
            else if (score.Value == minScore)
            {
                losers.Add(score.Key);
            }
        }

        EndGame(string.Join(", ", losers));
    }
}
