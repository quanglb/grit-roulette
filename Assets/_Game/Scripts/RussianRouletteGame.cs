using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Chứa logic cốt lõi của game Russian Roulette.
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
        this.bulletsLoaded = Mathf.Clamp(bulletsLoaded, 0, this.maxBullets);

        SetupChamber();
    }

    private void SetupChamber()
    {
        chamber = new bool[maxBullets];

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

        currentChamberIndex = Random.Range(0, maxBullets);
    }

    /// <summary>
    /// true = trúng đạn
    /// false = an toàn
    /// </summary>
    public bool PullTrigger()
    {
        bool hasBullet = chamber[currentChamberIndex];

        if (!hasBullet)
        {
            currentChamberIndex = (currentChamberIndex + 1) % maxBullets;
        }

        return hasBullet;
    }
}

public enum GameMode
{
    PassAndPlay,
    VsBot
}

/// <summary>
/// UI + Gameplay
/// </summary>
public class RussianRouletteGame : MonoBehaviour
{
    [Header("Setup UI")]
    public GameObject setupPanel;
    public TMP_InputField maxBulletsInput;
    public TMP_InputField bulletsLoadedInput;
    public TMP_Dropdown modeDropdown;
    public Button startGameButton;

    [Header("Gameplay UI")]
    public GameObject gameplayPanel;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI resultText;
    public Button pullTriggerButton;
    public Button restartButton;
    public Button backToMenuButton;

    [Header("Settings")]
    public float botDelaySeconds = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bangClip;
    public AudioClip reloadClip;
    public ParticleSystem muzzleFx;

    private RussianRouletteLogic gameLogic;

    private GameMode currentMode;

    private bool isPlayerTurn = true;
    private bool gameOver = false;

    private int turnCount;
    private int maxTurns;

    private void Start()
    {
        Vibration.Init();
        
        maxBulletsInput.text = "6";
        bulletsLoadedInput.text = "1";

        if (modeDropdown != null)
        {
            modeDropdown.ClearOptions();
            modeDropdown.AddOptions(new System.Collections.Generic.List<string>()
            {
                "Pass & Play",
                "VS Bot"
            });

            modeDropdown.value = 0;
        }

        startGameButton.onClick.AddListener(OnStartGameClicked);
        pullTriggerButton.onClick.AddListener(OnPullTriggerClicked);
        restartButton.onClick.AddListener(ShowSetup);
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene"));
        }

        ShowSetup();
    }

    private void ShowSetup()
    {
        setupPanel.SetActive(true);
        gameplayPanel.SetActive(false);

        gameOver = false;
        StopAllCoroutines();
    }

    private void OnStartGameClicked()
    {
        if (!int.TryParse(maxBulletsInput.text, out int maxBullets) || maxBullets <= 0)
        {
            maxBullets = 6;
            maxBulletsInput.text = "6";
        }

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

        gameLogic = new RussianRouletteLogic(maxBullets, bulletsLoaded);

        currentMode = (GameMode)modeDropdown.value;

        setupPanel.SetActive(false);
        gameplayPanel.SetActive(true);

        isPlayerTurn = true;
        gameOver = false;

        turnCount = 1;
        maxTurns = maxBullets;

        resultText.text = "Game Started!";

        pullTriggerButton.interactable = true;
        restartButton.gameObject.SetActive(false);

        UpdateStatusText();
        UpdateTurnUI();
    }

    private void UpdateStatusText()
    {
        statusText.text =
            $"Chamber Size: {gameLogic.MaxBullets} | Bullets Loaded: {gameLogic.BulletsLoaded}\n" +
            $"Current Slot: {gameLogic.CurrentChamberIndex + 1}/{gameLogic.MaxBullets}";
    }

    private void UpdateTurnUI()
    {
        if (gameOver)
            return;

        if (currentMode == GameMode.VsBot)
        {
            if (isPlayerTurn)
            {
                turnText.text = "Turn : <color=green>Player</color>";
                pullTriggerButton.interactable = true;
            }
            else
            {
                turnText.text = "Turn : <color=red>Bot</color>";
                pullTriggerButton.interactable = false;
                StartCoroutine(BotTurnRoutine());
            }
        }
        else
        {
            turnText.text = $"Turn {turnCount}/{maxTurns}";
            pullTriggerButton.interactable = true;
        }
    }

    private void OnPullTriggerClicked()
    {
        if (gameOver)
            return;

        if (currentMode == GameMode.VsBot)
        {
            if (isPlayerTurn)
                ExecuteTurn();
        }
        else
        {
            ExecuteTurn();
        }
    }

    private void ExecuteTurn()
    {
        if (gameOver)
            return;

        pullTriggerButton.interactable = false;

        bool hit = gameLogic.PullTrigger();

        if (hit)
        {
            gameOver = true;

            if (currentMode == GameMode.VsBot)
            {
                if (isPlayerTurn)
                {
                    resultText.text = "<color=red><b>BANG!\nPlayer Lose!</b></color>";
                }
                else
                {
                    resultText.text = "<color=green><b>BANG!\nBot Lose!\nPlayer Wins!</b></color>";
                }
            }
            else
            {
                resultText.text =
                    $"<color=red><b>BANG!</b></color>\n\nTurn {turnCount} đã trúng đạn!";
            }

            if (bangClip != null)
                audioSource.PlayOneShot(bangClip);

            if(muzzleFx != null) muzzleFx.Play();
            
            Vibration.VibratePop();
            
            turnText.text = "Game Over";

            UpdateStatusText();

            restartButton.gameObject.SetActive(true);

            return;
        }

        // SAFE
        if (reloadClip != null)
            audioSource.PlayOneShot(reloadClip);
        
        Vibration.VibratePeek();

        if (currentMode == GameMode.VsBot)
        {
            resultText.text = isPlayerTurn
                ? "Player : Click... Safe"
                : "Bot : Click... Safe";

            isPlayerTurn = !isPlayerTurn;
        }
        else
        {
            resultText.text = "SAFE!\nNext Turn";

            turnCount++;

            if (turnCount > maxTurns)
                turnCount = maxTurns;
        }

        UpdateStatusText();
        UpdateTurnUI();
    }

    private IEnumerator BotTurnRoutine()
    {
        yield return new WaitForSeconds(botDelaySeconds);

        if (!gameOver)
            ExecuteTurn();
    }
}