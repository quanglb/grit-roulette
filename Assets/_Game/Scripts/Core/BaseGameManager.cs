using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public abstract class BaseGameManager : MonoBehaviour
{
    public enum GameState { Setup, Playing, Ended }
    
    [Header("Common UI Panels")]
    [SerializeField] protected GameObject setupPanel;
    [SerializeField] protected GameObject gameplayPanel;
    [SerializeField] protected GameObject resultPanel;

    [Header("Common UI Texts & Buttons")]
    [SerializeField] protected TMP_Text gameTitleText;
    [SerializeField] protected TMP_Text instructionText;
    [SerializeField] protected TMP_Text loserText;
    [SerializeField] protected Button startButton;
    [SerializeField] protected Button restartButton;
    [SerializeField] protected Button backToMenuButton;

    protected GameState currentState;
    protected SceneLoader sceneLoader;

    protected virtual void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader == null)
        {
            GameObject loaderObj = new GameObject("SceneLoader");
            sceneLoader = loaderObj.AddComponent<SceneLoader>();
        }

        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (backToMenuButton != null) backToMenuButton.onClick.AddListener(BackToMenu);

        SetupGame();
    }

    public virtual void SetupGame()
    {
        currentState = GameState.Setup;
        if (setupPanel != null) setupPanel.SetActive(true);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    public virtual void StartGame()
    {
        if (!ValidateInputs()) return;

        currentState = GameState.Playing;
        if (setupPanel != null) setupPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        if (resultPanel != null) resultPanel.SetActive(false);

        OnInitGame();
    }

    public virtual void EndGame(string loserName)
    {
        currentState = GameState.Ended;
        if (setupPanel != null) setupPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(true);

        if (loserText != null)
        {
            loserText.text = $"Loser: {loserName} — Drink!";
        }
    }

    public virtual void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        sceneLoader.LoadScene(currentSceneName);
    }

    public virtual void BackToMenu()
    {
        sceneLoader.LoadMainMenu();
    }

    // Hàm ảo để lớp con bắt đầu logic game riêng biệt
    protected abstract void OnInitGame();

    // Hàm ảo để kiểm tra dữ liệu đầu vào trong Setup
    protected virtual bool ValidateInputs()
    {
        return true;
    }
}
