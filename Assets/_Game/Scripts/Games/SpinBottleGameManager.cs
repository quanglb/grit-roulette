using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SpinBottleGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private UnityEngine.UI.Dropdown playerCountDropdown;
    [SerializeField] private RectTransform bottleTransform;
    [SerializeField] private Button spinButton;
    [SerializeField] private RectTransform playersContainer;
    [SerializeField] private RectTransform pointerArrow;

    private int playerCount = 4;
    private List<string> players = new List<string>();
    private bool isSpinning = false;

    private string[] challenges = {
        "Uống 1 hớp cùng người đối diện!",
        "Kể một bí mật nhậu nhẹt đáng xấu hổ nhất của bạn.",
        "Thực hiện 5 cái chống đẩy hoặc uống 1 hớp.",
        "Uống 2 hớp hoặc cho người bên trái véo má.",
        "Chỉ định một người khác uống cùng bạn vòng này.",
        "Uống 2 hớp hoặc kể tên người yêu cũ gần đây nhất của bạn.",
        "Kể một thói quen xấu của người yêu cũ mà bạn từng ghét nhất, hoặc uống 2 hớp.",
        "Uống 1 hớp nếu bạn từng nhắn tin cho người yêu cũ lúc say.",
        "Uống 1 hớp hoặc trả lời: Bạn thích ôm từ phía trước hay phía sau hơn khi ngủ?",
        "Kể tên bộ phận quyến rũ nhất của người bên cạnh khiến bạn chú ý, hoặc uống 1 hớp.",
        "Uống 2 hớp hoặc trả lời thật lòng: Bạn đã bao giờ làm \"chuyện ấy\" ở một nơi không phải giường ngủ chưa?",
        "Cho cả bàn xem tin nhắn gần nhất của bạn với người yêu cũ (nếu còn giữ), hoặc uống 3 hớp.",
        "Kể về một kỷ niệm hài hước/trớ trêu nhất xảy ra khi đang hẹn hò lãng mạn.",
        "Uống 1 hớp nếu bạn từng giả vờ ngủ để tránh \"làm chuyện ấy\".",
        "Thực hiện một điệu nhảy quyến rũ (sexy dance) trong 10 giây hoặc uống 2 hớp.",
        "Nhắm mắt lại, sờ tay đoán xem người bên cạnh là ai, nếu đoán sai uống 2 hớp.",
        "Uống 2 hớp hoặc cho người bên phải xem hình ảnh/album ảnh riêng tư nhất của bạn.",
        "Thú nhận một điều bạn từng làm lén lút sau lưng người yêu cũ nhưng chưa bao giờ kể.",
        "Kể về buổi hẹn hò tồi tệ nhất bạn từng trải qua.",
        "Bạn có từng \"vượt rào\" trong buổi hẹn hò đầu tiên không? Trả lời thật lòng hoặc uống 2 hớp.",
        "Uống 1 hớp nếu bạn vẫn còn giữ quà lưu niệm của bất kỳ người yêu cũ nào.",
        "Nhắn tin cho người yêu cũ câu: \"Hôm nay thời tiết đẹp nhỉ\" hoặc uống 3 hớp.",
        "Kể về gu thời trang phòng ngủ của bạn: bạn thích mặc gì nhất khi đi ngủ?",
        "Kể một điều lãng mạn nhất bạn từng làm cho một người, hoặc uống 2 hớp.",
        "Uống 2 hớp hoặc thực hiện thử thách: Cho người khác chọn một người ở đây để bạn gửi một lời khen ngọt ngào nhất.",
        "Bạn đã từng hẹn hò với hai người cùng một lúc chưa? Trả lời hoặc uống 2 hớp.",
        "Uống 1 hớp và tiết lộ: Bạn thích nụ hôn kiểu Pháp hay nụ hôn nhẹ nhàng lên trán hơn?",
        "Kể về nụ hôn đầu tiên của bạn: diễn ra ở đâu và cảm xúc lúc đó thế nào?",
        "Uống 2 hớp hoặc kể tên một người trong bàn này mà bạn có ấn tượng đầu tiên tốt nhất.",
        "Cho phép người đối diện đặt một câu hỏi bất kỳ về đời sống tình cảm của bạn, bạn phải trả lời thật lòng hoặc uống 3 hớp.",
        "Kể về lời nói dối ngớ ngẩn nhất bạn từng dùng để chia tay hoặc từ chối ai đó.",
        "Uống 1 hớp nếu bạn từng khóc vì người yêu cũ trong vòng 6 tháng qua.",
        "Kể về tư thế ngủ kỳ lạ nhất của bạn khi ngủ chung giường với người khác.",
        "Bạn nghĩ gì về mối quan hệ \"Friends with benefits\" (FWB)? Trả lời thật lòng hoặc uống 2 hớp.",
        "Uống 2 hớp hoặc thú nhận: Bạn từng có suy nghĩ lãng mạn nào với bất cứ ai đang ngồi trong bàn này không?",
        "Gửi một tin nhắn thoại thì thầm vào tai người bên cạnh hoặc uống 1 hớp.",
        "Kể về trải nghiệm lãng mạn ngượng ngùng nhất của bạn, hoặc uống 2 hớp.",
        "Uống 1 hớp nếu bạn từng kiểm tra điện thoại của người yêu cũ mà không để họ biết.",
        "Kể về điều điên rồ nhất bạn từng làm vì tình yêu.",
        "Hãy cho người bên cạnh cù lét trong 10 giây mà không được cười, nếu cười thì uống 1 hớp.",
        "Trả lời thật lòng: Bạn có tin vào tình yêu sét đánh không? Không trả lời uống 1 hớp.",
        "Uống 2 hớp hoặc tiết lộ tần suất lý tưởng cho chuyện phòng ngủ của bạn trong một tuần.",
        "Nếu được quay lại thời gian, bạn có muốn thay đổi bất cứ điều gì về mối tình đầu của mình không?",
        "Uống 1 hớp nếu bạn từng bấm nhầm nút thích/tim ảnh rất cũ của nyc khi đang \"stalk\".",
        "Kể về một nơi kỳ lạ nhất mà bạn từng nảy sinh ham muốn lãng mạn.",
        "Trả lời thật lòng: Bạn thích ánh sáng đèn mờ ảo hay tắt đèn hoàn toàn khi lãng mạn?",
        "Uống 2 hớp hoặc chia sẻ suy nghĩ thật lòng nhất của bạn về người yêu cũ của người bên trái.",
        "Nói một câu bằng giọng điệu quyến rũ/sexy nhất hướng về phía người đối diện hoặc uống 2 hớp.",
        "Uống 1 hớp nếu bạn từng đồng ý hẹn hò chỉ vì cô đơn chứ không thực sự thích người đó.",
        "Chia sẻ một điểm yếu hoặc \"nút nhạy cảm\" trên cơ thể của bạn (ví dụ: cổ, tai...) khi được chạm vào.",
        "Uống 2 hớp hoặc kể về lần bạn say xỉn và làm chuyện ngớ ngẩn nhất trước mặt người mình thích."
    };

    protected override void Start()
    {
        base.Start();
        if (spinButton != null) spinButton.onClick.AddListener(Spin);
    }

    protected override void OnInitGame()
    {
        players.Clear();
        for (int i = 1; i <= playerCount; i++) players.Add($"Player {i}");

        if (pointerArrow != null)
        {
            pointerArrow.gameObject.SetActive(true);
            pointerArrow.localScale = Vector3.one;
            float initialAngle = bottleTransform != null ? bottleTransform.eulerAngles.z + 90f : 90f;
            pointerArrow.rotation = Quaternion.Euler(0, 0, initialAngle);
        }

        // Setup các Text hiển thị tên người chơi xoay tròn
        if (playersContainer != null)
        {
            foreach (Transform child in playersContainer) Destroy(child.gameObject);

            for (int i = 0; i < playerCount; i++)
            {
                GameObject txtObj = new GameObject($"PlayerName_{i}");
                txtObj.transform.SetParent(playersContainer, false);
                var textComp = txtObj.AddComponent<TextMeshProUGUI>();
                textComp.text = players[i];
                textComp.alignment = TextAlignmentOptions.Center;
                textComp.color = Color.white; // Chữ màu trắng dễ nhìn
                textComp.fontSize = playerCount > 12 ? 14 : (playerCount > 8 ? 18 : 24);

                // Tính toán vị trí theo vòng tròn bán kính phù hợp với số lượng người chơi
                float radius = playerCount > 8 ? 175f : 160f;
                float angle = i * Mathf.PI * 2f / playerCount;
                var rect = txtObj.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            }
        }

        if (spinButton != null) spinButton.interactable = true;
        if (instructionText != null) instructionText.text = "Bấm SPIN để bắt đầu quay chai!";
    }

    protected override bool ValidateInputs()
    {
        if (playerCountDropdown != null)
        {
            playerCount = playerCountDropdown.value + 2; // 2-8 players
        }
        else
        {
            playerCount = 4;
        }
        return true;
    }

    public void Spin()
    {
        if (isSpinning || currentState != GameState.Playing) return;
        StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;
        if (spinButton != null) spinButton.interactable = false;
        if (pointerArrow != null) pointerArrow.gameObject.SetActive(false);

        float spinTime = 10f; // Xoay liên tục đến lúc dừng (10s)
        float elapsed = 0f;
        float startSpeed = Random.Range(700f, 1100f);
        float currentSpeed = startSpeed;

        while (elapsed < spinTime)
        {
            elapsed += Time.deltaTime;
            float rotAmount = currentSpeed * Time.deltaTime;
            if (bottleTransform != null)
            {
                bottleTransform.Rotate(0, 0, rotAmount);
            }
            
            // Giảm tốc độ quay tuyến tính mượt mà trong 10 giây
            currentSpeed = Mathf.Lerp(startSpeed, 0f, elapsed / spinTime);
            yield return null;
        }

        isSpinning = false;

        // Tính toán xem mũi tên chai chỉ vào ai dựa vào góc xoay Z hiện tại
        float currentAngle = bottleTransform != null ? bottleTransform.eulerAngles.z : 0f;
        float angleStep = 360f / playerCount;
        
        // Chuyển đổi góc từ 0-360 độ sang Index người chơi tương ứng, cộng thêm 90 độ vì mặc định đầu chai hướng lên trên (90 độ)
        float bottleMouthAngle = (currentAngle + 90f) % 360f;
        if (bottleMouthAngle < 0f) bottleMouthAngle += 360f;
        int selectedIndex = Mathf.RoundToInt(bottleMouthAngle / angleStep) % playerCount;
        string selectedPlayer = players[selectedIndex];
        string selectedChallenge = challenges[Random.Range(0, challenges.Length)];

        // Góc của player tương ứng trong Radian
        float targetAngleRad = selectedIndex * Mathf.PI * 2f / playerCount;
        float targetAngleDeg = targetAngleRad * Mathf.Rad2Deg;

        // Xoay chai dừng chính xác ở góc chỉ vào player
        if (bottleTransform != null)
        {
            bottleTransform.rotation = Quaternion.Euler(0, 0, targetAngleDeg - 90f);
        }

        // Hiện mũi tên chỉ vào player thua và nhấp nháy
        if (pointerArrow != null)
        {
            pointerArrow.gameObject.SetActive(true);
            pointerArrow.rotation = Quaternion.Euler(0, 0, targetAngleDeg);
            StartCoroutine(PulseArrowCoroutine());
        }

        if (instructionText != null)
        {
            instructionText.text = $"Chai đã chỉ vào: <color=yellow>{selectedPlayer}</color>!\nThử thách: {selectedChallenge}";
        }
        
        yield return new WaitForSeconds(3.5f);
        EndGame(selectedPlayer);
    }

    private IEnumerator PulseArrowCoroutine()
    {
        float t = 0f;
        Vector3 baseScale = Vector3.one;
        while (currentState == GameState.Playing && pointerArrow != null && pointerArrow.gameObject.activeSelf)
        {
            t += Time.deltaTime * 8f;
            float scale = 1f + Mathf.PingPong(t, 0.3f);
            pointerArrow.localScale = baseScale * scale;
            yield return null;
        }
    }
}
