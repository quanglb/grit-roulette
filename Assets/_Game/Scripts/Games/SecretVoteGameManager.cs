using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SecretVoteGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private UnityEngine.UI.Dropdown playerCountDropdown;
    [SerializeField] private GameObject votingButtonsContainer;
    [SerializeField] private Button showVoteListButton;

    private int playerCount = 4;
    private List<string> players = new List<string>();
    private int votingPlayerIndex = 0;
    private Dictionary<string, int> votes = new Dictionary<string, int>();

    private string[] questions = {
        "Ai dễ say nhất?",
        "Ai hay đến muộn nhất?",
        "Ai hay quên nhất?",
        "Ai là người nói nhiều nhất?",
        "Ai nên uống vòng này nhất?"
    };
    private string currentQuestion;

    protected override void Start()
    {
        base.Start();
        if (showVoteListButton != null) showVoteListButton.onClick.AddListener(ShowVoteList);
    }

    protected override void OnInitGame()
    {
        players.Clear();
        votes.Clear();
        for (int i = 1; i <= playerCount; i++)
        {
            players.Add($"Player {i}");
            votes[players[i - 1]] = 0;
        }

        currentQuestion = questions[Random.Range(0, questions.Length)];
        votingPlayerIndex = 0;
        if (showVoteListButton != null) showVoteListButton.gameObject.SetActive(true);
        if (votingButtonsContainer != null) votingButtonsContainer.SetActive(false);

        AskNextPlayerVote();
    }

    protected override bool ValidateInputs()
    {
        if (playerCountDropdown != null)
        {
            playerCount = playerCountDropdown.value + 3; // 3-8 players
        }
        else
        {
            playerCount = 4;
        }
        return true;
    }

    private void AskNextPlayerVote()
    {
        if (instructionText != null)
        {
            instructionText.text = $"CÂU HỎI: {currentQuestion}\n\nĐến lượt: {players[votingPlayerIndex]} vote. (Người khác hãy quay mặt đi!)";
        }
        if (showVoteListButton != null) showVoteListButton.gameObject.SetActive(true);
        if (votingButtonsContainer != null) votingButtonsContainer.SetActive(false);
    }

    public void ShowVoteList()
    {
        if (showVoteListButton != null) showVoteListButton.gameObject.SetActive(false);
        if (votingButtonsContainer != null)
        {
            votingButtonsContainer.SetActive(true);

            // Tạo các nút vote trong Container
            foreach (Transform child in votingButtonsContainer.transform) Destroy(child.gameObject);

            for (int i = 0; i < playerCount; i++)
            {
                string targetPlayer = players[i];
                GameObject btnObj = new GameObject($"VoteButton_{targetPlayer}");
                btnObj.transform.SetParent(votingButtonsContainer.transform, false);
                
                var img = btnObj.AddComponent<Image>();
                img.color = Color.white;
                var btn = btnObj.AddComponent<Button>();
                
                GameObject txtObj = new GameObject("Text");
                txtObj.transform.SetParent(btnObj.transform, false);
                var txt = txtObj.AddComponent<TextMeshProUGUI>();
                txt.text = targetPlayer;
                txt.color = Color.black;
                txt.alignment = TextAlignmentOptions.Center;
                
                RectTransform txtRect = txtObj.GetComponent<RectTransform>();
                txtRect.anchorMin = Vector2.zero;
                txtRect.anchorMax = Vector2.one;
                txtRect.sizeDelta = Vector2.zero;

                btn.onClick.AddListener(() => CastVote(targetPlayer));
            }
        }
    }

    private void CastVote(string targetPlayer)
    {
        votes[targetPlayer]++;
        votingPlayerIndex++;

        if (votingPlayerIndex < playerCount)
        {
            AskNextPlayerVote();
        }
        else
        {
            EvaluateVotes();
        }
    }

    private void EvaluateVotes()
    {
        if (votingButtonsContainer != null) votingButtonsContainer.SetActive(false);
        if (showVoteListButton != null) showVoteListButton.gameObject.SetActive(false);

        int maxVotes = -1;
        List<string> losers = new List<string>();

        string resultStr = $"KẾT QUẢ VOTE ({currentQuestion}):\n\n";
        foreach (var player in votes)
        {
            resultStr += $"{player.Key}: {player.Value} vote(s)\n";
            if (player.Value > maxVotes)
            {
                maxVotes = player.Value;
                losers.Clear();
                losers.Add(player.Key);
            }
            else if (player.Value == maxVotes)
            {
                losers.Add(player.Key);
            }
        }

        if (instructionText != null) instructionText.text = resultStr;
        EndGame(string.Join(", ", losers));
    }
}
