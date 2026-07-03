using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BombPassGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private UnityEngine.UI.Dropdown playerCountDropdown;
    [SerializeField] private Button passButton;
    [SerializeField] private Image bombImage;

    private int playerCount = 4;
    private List<string> players = new List<string>();
    private int currentPlayerIndex = 0;
    private float bombTimer = 10f;
    private bool isRunning = false;
    private AudioSource tickAudioSource;

    protected override void Start()
    {
        base.Start();
        if (passButton != null) passButton.onClick.AddListener(PassBomb);
    }

    protected override string GetBGMName()
    {
        return "BGM_Tense_Loop";
    }

    protected override void OnInitGame()
    {
        players.Clear();
        for (int i = 1; i <= playerCount; i++)
        {
            players.Add($"Player {i}");
        }

        currentPlayerIndex = Random.Range(0, playerCount);
        bombTimer = Random.Range(5f, 20f);
        isRunning = true;
        if (passButton != null)
        {
            passButton.gameObject.SetActive(true);
            passButton.interactable = true;
        }

        if (tickAudioSource != null)
        {
            AudioManager.Instance.StopSFX(tickAudioSource);
        }
        tickAudioSource = AudioManager.Instance.PlaySFX("SFX_BombTick", true);

        UpdateTurnUI();
    }

    protected override bool ValidateInputs()
    {
        if (playerCountDropdown != null)
        {
            playerCount = playerCountDropdown.value + 2; // Value 0 = 2 players, Value 1 = 3 players...
        }
        else
        {
            playerCount = 4;
        }
        return true;
    }

    private void Update()
    {
        if (!isRunning || currentState != GameState.Playing) return;

        bombTimer -= Time.deltaTime;

        // Hiệu ứng nhấp nháy quả bom placeholder theo nhịp đếm ngược
        if (bombImage != null)
        {
            float scale = 1f + Mathf.PingPong(Time.time * (20f / Mathf.Max(bombTimer, 1f)), 0.3f);
            bombImage.transform.localScale = new Vector3(scale, scale, 1f);
            bombImage.color = Color.Lerp(Color.red, Color.black, Mathf.PingPong(Time.time * (15f / Mathf.Max(bombTimer, 1f)), 1f));
        }

        if (bombTimer <= 0)
        {
            ExplodeBomb();
        }
    }

    public void PassBomb()
    {
        if (currentState != GameState.Playing) return;
        currentPlayerIndex = (currentPlayerIndex + 1) % playerCount;
        
        // Phát tiếng chuyền bom
        AudioManager.Instance.PlaySFX("SFX_Pass");
        
        UpdateTurnUI();
    }

    private void UpdateTurnUI()
    {
        if (instructionText != null)
        {
            instructionText.text = $"QUẢ BOM ĐANG Ở CHỖ: {players[currentPlayerIndex]}! HÃY CHUYỀN MAU!";
        }
    }

    private void ExplodeBomb()
    {
        isRunning = false;
        if (passButton != null) passButton.interactable = false;

        // Dừng tiếng tíc tắc
        if (tickAudioSource != null)
        {
            AudioManager.Instance.StopSFX(tickAudioSource);
        }

        // Phát tiếng nổ và tiếng thua cuộc
        AudioManager.Instance.PlaySFX("SFX_Explosion");
        AudioManager.Instance.PlaySFX("SFX_LoseFanfare");

        EndGame(players[currentPlayerIndex]);
    }
}
