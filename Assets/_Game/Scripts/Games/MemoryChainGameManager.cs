using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MemoryChainGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button[] colorButtons; // 4 buttons

    private string playerName = "Player";
    private List<int> sequence = new List<int>();
    private int playerInputIndex = 0;
    private bool isReplaying = false;

    protected override void Start()
    {
        base.Start();
        if (colorButtons != null)
        {
            for (int i = 0; i < colorButtons.Length; i++)
            {
                int idx = i;
                if (colorButtons[i] != null)
                {
                    colorButtons[i].onClick.AddListener(() => PlayerButtonClick(idx));
                }
            }
        }
    }

    protected override void OnInitGame()
    {
        sequence.Clear();
        StartNewRound();
    }

    protected override bool ValidateInputs()
    {
        playerName = (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text)) ? playerNameInput.text : "Player";
        return true;
    }

    private void StartNewRound()
    {
        if (sequence.Count >= 5)
        {
            if (instructionText != null) instructionText.text = $"{playerName} đã vượt qua 5 chuỗi nhớ thành công! Chiến thắng!";
            EndGame("Không có (Chiến thắng)");
            return;
        }

        sequence.Add(Random.Range(0, 4));
        StartCoroutine(ReplaySequenceCoroutine());
    }

    private IEnumerator ReplaySequenceCoroutine()
    {
        isReplaying = true;
        SetButtonsInteractable(false);
        if (instructionText != null) instructionText.text = "Hãy ghi nhớ chuỗi màu...";

        yield return new WaitForSeconds(0.8f);

        for (int i = 0; i < sequence.Count; i++)
        {
            int colorIdx = sequence[i];
            yield return StartCoroutine(FlashButtonCoroutine(colorIdx));
            yield return new WaitForSeconds(0.3f);
        }

        isReplaying = false;
        SetButtonsInteractable(true);
        playerInputIndex = 0;
        if (instructionText != null) instructionText.text = "Đến lượt bạn bấm lại chuỗi:";
    }

    private IEnumerator FlashButtonCoroutine(int idx)
    {
        if (colorButtons != null && idx < colorButtons.Length && colorButtons[idx] != null && colorButtons[idx].image != null)
        {
            Color originalColor = colorButtons[idx].image.color;
            colorButtons[idx].image.color = Color.white; // Làm sáng nút
            yield return new WaitForSeconds(0.4f);
            colorButtons[idx].image.color = originalColor;
        }
    }

    private void SetButtonsInteractable(bool active)
    {
        if (colorButtons != null)
        {
            foreach (var btn in colorButtons)
            {
                if (btn != null) btn.interactable = active;
            }
        }
    }

    public void PlayerButtonClick(int idx)
    {
        if (currentState != GameState.Playing || isReplaying) return;

        StartCoroutine(FlashButtonCoroutine(idx));

        if (idx == sequence[playerInputIndex])
        {
            playerInputIndex++;
            if (playerInputIndex >= sequence.Count)
            {
                // Hoàn thành chuỗi, sang round tiếp theo
                StartNewRound();
            }
        }
        else
        {
            EndGame(playerName);
        }
    }
}
