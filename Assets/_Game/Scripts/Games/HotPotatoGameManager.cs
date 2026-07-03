using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HotPotatoGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private UnityEngine.UI.Dropdown playerCountDropdown;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image potatoImage;

    private int playerCount = 4;
    private List<string> players = new List<string>();
    private int currentPlayerIndex = 0;
    private float potatoTimer = 10f;
    private bool isRunning = false;

    protected override void Start()
    {
        base.Start();
        if (nextButton != null) nextButton.onClick.AddListener(PassPotato);
    }

    protected override void OnInitGame()
    {
        players.Clear();
        for (int i = 1; i <= playerCount; i++) players.Add($"Player {i}");

        currentPlayerIndex = Random.Range(0, playerCount);
        // Ẩn thời gian nổ ngầm
        potatoTimer = Random.Range(5f, 20f);
        isRunning = true;
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.interactable = true;
        }
        UpdateTurnUI();
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

    private void Update()
    {
        if (!isRunning || currentState != GameState.Playing) return;

        potatoTimer -= Time.deltaTime;

        // Hiệu ứng phình củ khoai nhẹ nhàng không lộ thời gian chính xác
        if (potatoImage != null)
        {
            float scale = 1f + Mathf.PingPong(Time.time * 2f, 0.1f);
            potatoImage.transform.localScale = new Vector3(scale, scale, 1f);
        }

        if (potatoTimer <= 0)
        {
            ExplodePotato();
        }
    }

    public void PassPotato()
    {
        if (currentState != GameState.Playing) return;
        currentPlayerIndex = (currentPlayerIndex + 1) % playerCount;
        UpdateTurnUI();
    }

    private void UpdateTurnUI()
    {
        if (instructionText != null)
        {
            instructionText.text = $"KHOAI TÂY NÓNG ĐANG Ở CHỖ: {players[currentPlayerIndex]}! CHUYỀN NGAY!";
        }
    }

    private void ExplodePotato()
    {
        isRunning = false;
        if (nextButton != null) nextButton.interactable = false;
        EndGame(players[currentPlayerIndex]);
    }
}
