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
        "Ai nên uống vòng này nhất?",
        "Ai là người lụy người yêu cũ nhất?",
        "Ai là người có nhiều người yêu cũ nhất ở đây?",
        "Ai có vẻ ngoài ngây thơ nhất nhưng \"quái vật\" ngầm khi yêu?",
        "Ai dễ đồng ý quay lại với người yêu cũ nhất nếu được rủ rê?",
        "Ai dễ nảy sinh tình cảm/say nắng nhất khi say?",
        "Ai hay xem phim \"hấp dẫn\" nhiều nhất?",
        "Ai là người giữ nhiều bí mật thầm kín nhất?",
        "Ai là người lãng mạn (hoặc sến sẩm) nhất khi yêu?",
        "Ai dễ có xu hướng \"nhìn trộm\" điện thoại của người yêu nhất?",
        "Ai có gu chọn người yêu độc lạ nhất?",
        "Ai dễ bị thu hút bởi người lớn tuổi hơn mình nhiều nhất?",
        "Ai là người có khả năng nhắn tin thả thính cùng lúc nhiều người nhất?",
        "Ai dễ \"cảm nắng\" bạn thân nhất?",
        "Ai là người giữ thể diện nhất trước mặt người yêu cũ?",
        "Ai dễ tin vào những lời thề non hẹn biển nhất?",
        "Ai là người thích kiểm soát người yêu nhất?",
        "Ai dễ quên sinh nhật hay ngày kỷ niệm của người yêu nhất?",
        "Ai là người hay ghen tuông vô cớ nhất?",
        "Ai dễ bị dụ dỗ bằng đồ ăn hoặc một chầu nhậu nhất?",
        "Ai có khả năng giấu chuyện mình đang yêu giỏi nhất?",
        "Ai là người hay stalk trang cá nhân của nyc nhiều nhất?",
        "Ai dễ đồng ý đi du lịch riêng với người mới quen nhất?",
        "Ai là người chi tiêu hào phóng nhất khi hẹn hò?",
        "Ai dễ giận dỗi người yêu vì những lý do vô lý nhất?",
        "Ai là người giỏi che giấu cảm xúc khi buồn nhất?",
        "Ai có khả năng trở thành \"quân sư tình yêu\" giỏi nhất cho người khác?",
        "Ai dễ yêu xa giỏi nhất mà không sợ cô đơn?",
        "Ai là người hay có những phát ngôn \"triết lý tình yêu\" sến sẩm nhất?",
        "Ai dễ nói lời chia tay trước nhất khi có mâu thuẫn nhỏ?",
        "Ai là người thích được người yêu nuông chiều như em bé nhất?",
        "Ai có khả năng say xỉn xong gọi điện khóc lóc với nyc cao nhất?",
        "Ai dễ tha thứ cho sự lừa dối trong tình yêu nhất?",
        "Ai có nụ cười tỏa nắng/thu hút người khác phái nhất ở đây?",
        "Ai dễ bị thu hút bởi ngoại hình hơn là tính cách nhất?",
        "Ai là người kén chọn nhất trong việc tìm kiếm bạn đời?",
        "Ai dễ yêu từ cái nhìn đầu tiên nhất?",
        "Ai là người hay thả thính bằng những câu thơ sến nhất?",
        "Ai dễ có biểu cảm \"mê gái/mê trai\" lộ liễu nhất khi gặp người đẹp?",
        "Ai là người thích những trò đùa tinh nghịch/quyến rũ nhẹ nhàng nhất?",
        "Ai có khả năng ngủ quên luôn trong lúc đang nhắn tin lãng mạn?",
        "Ai dễ giận dỗi nhưng cũng dễ dỗ dành nhất?",
        "Ai là người thích gây bất ngờ cho nửa kia bằng những món quà độc lạ nhất?",
        "Ai dễ bị đỏ mặt nhất khi được ai đó thì thầm vào tai?",
        "Ai có phong thái tự tin nhất khi đi tán tỉnh người khác?",
        "Ai là người thích sờ tóc hoặc nắm tay người yêu mọi lúc mọi nơi nhất?",
        "Ai dễ đồng ý tham gia một trò chơi thử thách mạo hiểm/táo bạo nhất?"
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
                txt.fontSize = playerCount > 12 ? 12 : (playerCount > 8 ? 14 : 18);
                
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
