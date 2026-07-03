using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    // Reference to the SceneLoader component (assign via inspector or find at runtime)
    public SceneLoader sceneLoader;

    // Assign your UI Buttons in the inspector. The button name should exactly match the target scene name (e.g., "SpinBottleScene").
    public Button buttonRusian;
    public Button buttonBombPass;
    public Button buttonLuckyNumber;
    public Button buttonSpinBottle;
    public Button buttonReactionDuel;
    public Button buttonHotPotato;
    public Button buttonHigherLower;
    public Button buttonDiceBattle;
    public Button buttonSecretVote;
    public Button buttonColorTrap;
    public Button buttonMemoryChain;

    private void Awake()
    {
        // Ensure we have a SceneLoader instance in the scene.
        if (sceneLoader == null)
        {
            GameObject loaderObj = new GameObject("SceneLoader");
            sceneLoader = loaderObj.AddComponent<SceneLoader>();
        }
        
        Vibration.Init();
    }

    private void Start()
    {
        // Play background music for the main menu.
        AudioManager.Instance.PlayBGM("BGM_MainMenu");

        // Register each button individually
        RegisterButton(buttonRusian, "SampleScene");
        RegisterButton(buttonBombPass, "BombPassScene");
        RegisterButton(buttonLuckyNumber, "LuckyNumberScene");
        RegisterButton(buttonSpinBottle, "SpinBottleScene");
        RegisterButton(buttonReactionDuel, "ReactionDuelScene");
        RegisterButton(buttonHotPotato, "HotPotatoScene");
        RegisterButton(buttonHigherLower, "HigherLowerScene");
        RegisterButton(buttonDiceBattle, "DiceBattleScene");
        RegisterButton(buttonSecretVote, "SecretVoteScene");
        RegisterButton(buttonColorTrap, "ColorTrapScene");
        RegisterButton(buttonMemoryChain, "MemoryChainScene");
    }

    private void RegisterButton(Button btn, string sceneName)
    {
        if (btn == null) return;

        btn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX("SFX_Click");
            Vibration.VibratePop();
            sceneLoader.LoadScene(sceneName);
        });
    }
}
