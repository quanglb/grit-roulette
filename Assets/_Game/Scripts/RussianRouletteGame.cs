using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Chứa logic cốt lõi của game Russian Roulette, tách biệt khỏi UI.
/// </summary>
public class RussianRouletteLogic
{
    private int maxBullets;
    private int bulletsLoaded;
    private bool[] chamber;
    private int currentChamberIndex;

    public int MaxBullets => maxBullets;
    public int BulletsLoaded => bulletsLoaded;
    public int CurrentChamberIndex => currentChamberIndex;

    public RussianRouletteLogic(int maxBullets, int bulletsLoaded)
    {
        this.maxBullets = Mathf.Max(1, maxBullets);
        // Đảm bảo số đạn không vượt quá số lỗ
        this.bulletsLoaded = Mathf.Clamp(bulletsLoaded, 0, this.maxBullets);
        
        SetupChamber();
    }

    private void SetupChamber()
    {
        chamber = new bool[maxBullets];
        for (int i = 0; i < maxBullets; i++)
        {
            chamber[i] = false;
        }

        // Random vị trí các viên đạn trong ổ
        int bulletsPlaced = 0;
        while (bulletsPlaced < bulletsLoaded)
        {
            int randomSlot = Random.Range(0, maxBullets);
            if (!chamber[randomSlot])
            {
                chamber[randomSlot] = true;
                bulletsPlaced++;
            }
        }

        // Random vị trí bắt đầu của ổ đạn (Optional rule)
        currentChamberIndex = Random.Range(0, maxBullets);
    }

    /// <summary>
    /// Thực hiện bóp cò.
    /// </summary>
    /// <returns>Trả về true nếu trúng đạn (lose), false nếu safe.</returns>
    public bool PullTrigger()
    {
        bool hasBullet = chamber[currentChamberIndex];
        
        // Nếu an toàn thì ổ đạn chuyển sang slot tiếp theo
        if (!hasBullet)
        {
            currentChamberIndex = (currentChamberIndex + 1) % maxBullets;
        }
        
        return hasBullet;
    }
}

/// <summary>
/// Script điều khiển giao diện UI và luồng game chính (Player vs Bot).
/// </summary>
public class RussianRouletteGame : MonoBehaviour
{
    [Header("Setup UI")]
    [Tooltip("Panel hiển thị khi chuẩn bị bắt đầu game")]
    public GameObject setupPanel;
    public TMP_InputField maxBulletsInput;
    public TMP_InputField bulletsLoadedInput;
    public Button startGameButton;

    [Header("Gameplay UI")]
    [Tooltip("Panel hiển thị trong lúc chơi")]
    public GameObject gameplayPanel;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI resultText;
    public Button pullTriggerButton;
    public Button restartButton;

    [Header("Settings")]
    public float botDelaySeconds = 1.0f;

    private RussianRouletteLogic gameLogic;
    private bool isPlayerTurn = true;
    private bool gameOver = false;

    private void Start()
    {
        // Gán giá trị mặc định cho input
        maxBulletsInput.text = "6";
        bulletsLoadedInput.text = "1";

        // Gán sự kiện cho các nút
        startGameButton.onClick.AddListener(OnStartGameClicked);
        pullTriggerButton.onClick.AddListener(OnPullTriggerClicked);
        restartButton.onClick.AddListener(ShowSetup);

        ShowSetup();
    }

    /// <summary>
    /// Hiển thị màn hình setup trước khi chơi
    /// </summary>
    private void ShowSetup()
    {
        setupPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        gameOver = false;
        StopAllCoroutines(); // Dừng tất cả hành động của bot nếu đang chạy
    }

    /// <summary>
    /// Bắt đầu game với các thông số từ giao diện
    /// </summary>
    private void OnStartGameClicked()
    {
        // Validate input cho Max Bullets
        if (!int.TryParse(maxBulletsInput.text, out int maxBullets) || maxBullets <= 0)
        {
            maxBullets = 6;
            maxBulletsInput.text = "6";
        }

        // Validate input cho Bullets Loaded
        if (!int.TryParse(bulletsLoadedInput.text, out int bulletsLoaded) || bulletsLoaded < 0)
        {
            bulletsLoaded = 1;
            bulletsLoadedInput.text = "1";
        }

        if (bulletsLoaded > maxBullets)
        {
            bulletsLoaded = maxBullets;
            bulletsLoadedInput.text = bulletsLoaded.ToString();
        }

        // Khởi tạo logic game
        gameLogic = new RussianRouletteLogic(maxBullets, bulletsLoaded);

        // Chuyển UI sang màn chơi
        setupPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        
        isPlayerTurn = true;
        gameOver = false;
        resultText.text = "Trò chơi bắt đầu!";
        pullTriggerButton.interactable = true;
        restartButton.gameObject.SetActive(false);

        UpdateStatusText();
        UpdateTurnUI();
    }

    private void UpdateStatusText()
    {
        statusText.text = $"Chamber Size: {gameLogic.MaxBullets} | Bullets Loaded: {gameLogic.BulletsLoaded}\n" +
                          $"Current Slot: {gameLogic.CurrentChamberIndex + 1}/{gameLogic.MaxBullets}";
    }

    private void UpdateTurnUI()
    {
        if (gameOver) return;

        if (isPlayerTurn)
        {
            turnText.text = "Turn: <color=green>Player</color>";
            pullTriggerButton.interactable = true;
        }
        else
        {
            turnText.text = "Turn: <color=red>Bot</color>";
            pullTriggerButton.interactable = false; // Ngăn user bấm nút khi tới lượt bot
            StartCoroutine(BotTurnRoutine());
        }
    }

    private void OnPullTriggerClicked()
    {
        if (isPlayerTurn && !gameOver)
        {
            ExecuteTurn();
        }
    }

    /// <summary>
    /// Xử lý hành động bóp cò chung cho cả Player và Bot
    /// </summary>
    private void ExecuteTurn()
    {
        if (gameOver) return;

        pullTriggerButton.interactable = false; // Vô hiệu hóa nút để tránh spam
        
        bool isHit = gameLogic.PullTrigger();
        
        if (isHit)
        {
            // Nếu có đạn -> Thua
            gameOver = true;
            if (isPlayerTurn)
            {
                resultText.text = "<b><color=red>Bang... Player lose!</color></b>";
            }
            else
            {
                resultText.text = "<b><color=green>Bang... Bot lose! Player won!</color></b>";
            }
            
            turnText.text = "Game Over";
            UpdateStatusText(); // Cập nhật lại slot hiện tại để biết slot có đạn
            restartButton.gameObject.SetActive(true);
        }
        else
        {
            // Không có đạn -> An toàn, qua lượt
            if (isPlayerTurn)
                resultText.text = "Player: Click... safe";
            else
                resultText.text = "Bot: Click... safe";
            
            UpdateStatusText();

            // Đổi lượt
            isPlayerTurn = !isPlayerTurn;
            UpdateTurnUI();
        }
    }

    /// <summary>
    /// Coroutine xử lý lượt đi tự động của Bot sau một khoảng delay
    /// </summary>
    private IEnumerator BotTurnRoutine()
    {
        yield return new WaitForSeconds(botDelaySeconds);
        
        if (!gameOver)
        {
            ExecuteTurn();
        }
    }
}
