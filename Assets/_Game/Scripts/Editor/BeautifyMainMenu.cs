#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeautifyMainMenu
{
    [MenuItem("Tools/Drinking Game/Beautify Main Menu Scene")]
    public static void SetupBeautifiedMainMenu()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenuScene.unity");

        // Clean scene, giữ lại Camera, Light và EventSystem
        var rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            if (obj.name != "Main Camera" && obj.name != "Directional Light" && obj.name != "EventSystem")
            {
                GameObject.DestroyImmediate(obj);
            }
        }

        // Đảm bảo luôn có EventSystem hoạt động chuẩn với New Input System
        GameObject esObj = GameObject.Find("EventSystem");
        if (esObj == null)
        {
            esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        }

        var oldInput = esObj.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        if (oldInput != null)
        {
            GameObject.DestroyImmediate(oldInput);
        }

        var newInput = esObj.GetComponent("InputSystemUIInputModule");
        if (newInput == null)
        {
            System.Type inputSystemType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemType != null)
            {
                esObj.AddComponent(inputSystemType);
            }
            else
            {
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        // Camera background game màu #14171F (RGB: 20, 23, 31)
        Color bgGameColor = new Color(0.0784f, 0.0902f, 0.1216f, 1f);
        Color cardSlate = new Color(0.15f, 0.16f, 0.2f, 0.75f);
        Color neonCyan = new Color(0f, 0.9f, 1f, 1f); // #00E5FF
        Color textWhite = Color.white;

        var mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        mainCam.clearFlags = CameraClearFlags.SolidColor;
        mainCam.backgroundColor = bgGameColor;

        // Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(2160, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        // Background Image để đảm bảo hiển thị đúng màu ở chế độ 2D UI overlay
        GameObject bgObj = new GameObject("BackgroundImage");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = bgGameColor;
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Title
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "DRINKING PARTY MINI GAMES";
        titleText.fontSize = 60;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = neonCyan; // Xanh neon
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -100);
        titleRect.sizeDelta = new Vector2(1200, 100);

        // Grid container
        GameObject gridObj = new GameObject("ButtonsGrid");
        gridObj.transform.SetParent(canvasObj.transform, false);
        GridLayoutGroup grid = gridObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(320, 120);
        grid.spacing = new Vector2(30, 30);
        grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        grid.constraintCount = 3;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.childAlignment = TextAnchor.MiddleCenter;

        RectTransform gridRect = gridObj.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.5f, 0.5f);
        gridRect.anchorMax = new Vector2(0.5f, 0.5f);
        gridRect.anchoredPosition = new Vector2(0, -60);
        gridRect.sizeDelta = new Vector2(1500, 480);

        string[] scenes = {
            "SampleScene", "BombPassScene", "LuckyNumberScene", 
            "SpinBottleScene", "ReactionDuelScene", "HotPotatoScene", 
            "HigherLowerScene", "DiceBattleScene", "SecretVoteScene", 
            "ColorTrapScene", "MemoryChainScene"
        };

        string[] gameNames = {
            "Russian Roulette", "Bomb Pass", "Lucky Number", 
            "Spin Bottle", "Reaction Duel", "Hot Potato", 
            "Higher or Lower", "Dice Battle", "Secret Vote", 
            "Color Trap", "Memory Chain"
        };

        for (int i = 0; i < scenes.Length; i++)
        {
            GameObject btnObj = new GameObject(scenes[i]);
            btnObj.transform.SetParent(gridObj.transform, false);
            Image img = btnObj.AddComponent<Image>();
            img.color = cardSlate;
            
            // Thêm viền xanh neon
            var btnOutline = btnObj.AddComponent<Outline>();
            btnOutline.effectColor = neonCyan;
            btnOutline.effectDistance = new Vector2(2, 2);

            Button btn = btnObj.AddComponent<Button>();

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = gameNames[i];
            txt.fontSize = 24;
            txt.fontStyle = FontStyles.Bold;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = textWhite; // Chữ trắng

            RectTransform txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;
        }

        // MainMenuController
        GameObject controllerObj = new GameObject("MainMenuController");
        controllerObj.AddComponent<MainMenuController>();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("MainMenuScene beautified with #14171F BG successfully!");
    }
}
#endif
