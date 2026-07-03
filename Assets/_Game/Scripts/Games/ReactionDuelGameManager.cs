using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ReactionDuelGameManager : BaseGameManager
{
    [Header("Game Specific UI")]
    [SerializeField] private TMP_InputField p1NameInput;
    [SerializeField] private TMP_InputField p2NameInput;
    [SerializeField] private Button p1Button;
    [SerializeField] private Button p2Button;
    [SerializeField] private Image signalImage;

    private string p1Name = "Player 1";
    private string p2Name = "Player 2";
    private bool isSignalActive = false;
    private bool isWaitingForSignal = false;

    protected override void Start()
    {
        base.Start();
        if (p1Button != null) p1Button.onClick.AddListener(() => PlayerTap(1));
        if (p2Button != null) p2Button.onClick.AddListener(() => PlayerTap(2));
    }

    protected override string GetBGMName()
    {
        return "BGM_Duel_Loop";
    }

    protected override void OnInitGame()
    {
        if (p1Button != null) p1Button.interactable = true;
        if (p2Button != null) p2Button.interactable = true;
        if (signalImage != null) signalImage.color = Color.gray;
        if (instructionText != null) instructionText.text = "Hãy sẵn sàng! Đợi tín hiệu TAP NOW.";
        
        StartCoroutine(StartDuelCoroutine());
    }

    protected override bool ValidateInputs()
    {
        p1Name = (p1NameInput != null && !string.IsNullOrEmpty(p1NameInput.text)) ? p1NameInput.text : "Player 1";
        p2Name = (p2NameInput != null && !string.IsNullOrEmpty(p2NameInput.text)) ? p2NameInput.text : "Player 2";
        return true;
    }

    private IEnumerator StartDuelCoroutine()
    {
        isSignalActive = false;
        isWaitingForSignal = true;

        float delay = Random.Range(2f, 5f);
        yield return new WaitForSeconds(delay);

        if (currentState == GameState.Playing && isWaitingForSignal)
        {
            isWaitingForSignal = false;
            isSignalActive = true;
            if (signalImage != null) signalImage.color = Color.green;
            if (instructionText != null) instructionText.text = "TAP NOW!!!";
            
            // Phát còi báo hiệu tap ngay
            AudioManager.Instance.PlaySFX("SFX_DuelSignal");
        }
    }

    public void PlayerTap(int playerNum)
    {
        if (currentState != GameState.Playing) return;

        if (isWaitingForSignal)
        {
            // Bấm sớm khi chưa có tín hiệu: Thua ngay lập tức
            isWaitingForSignal = false;
            StopAllCoroutines();
            
            AudioManager.Instance.PlaySFX("SFX_Wrong");
            AudioManager.Instance.PlaySFX("SFX_LoseFanfare");
            
            EndGame(playerNum == 1 ? p1Name : p2Name);
        }
        else if (isSignalActive)
        {
            isSignalActive = false;
            
            // Tát người đối diện và thua kèn
            AudioManager.Instance.PlaySFX("SFX_Slap");
            AudioManager.Instance.PlaySFX("SFX_LoseFanfare");
            
            // Người bấm trước thắng -> Người bấm sau/người còn lại thua
            EndGame(playerNum == 1 ? p2Name : p1Name);
        }
    }
}
