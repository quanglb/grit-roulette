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
        "Chỉ định một người khác uống cùng bạn vòng này."
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
