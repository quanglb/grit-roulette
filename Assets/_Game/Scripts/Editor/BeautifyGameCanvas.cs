#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeautifyGameCanvas
{
    [MenuItem("Tools/Drinking Game/Beautify Game Canvas Prefab")]
    public static void CreateBeautifiedPrefab()
    {
        GameObject canvasObj = new GameObject("GameCanvasTmp");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // Responsive 2160x1080 Landscape configuration
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(2160, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // Bảng màu yêu cầu:
        // Background game màu: #14171F (RGB: 20, 23, 31)
        Color bgGameColor = new Color(0.0784f, 0.0902f, 0.1216f, 1f); 
        Color cardSlate = new Color(0.15f, 0.16f, 0.2f, 0.75f); // Slate mờ để lộ nền #14171F
        Color neonGreen = new Color(0f, 0.9f, 0.46f, 1f); // #00E676
        Color neonCyan = new Color(0f, 0.9f, 1f, 1f); // #00E5FF (Màu xanh neon từ MainMenu)
        Color textWhite = Color.white;

        // 0. Nền Background Image toàn màn hình (#14171F)
        GameObject bgObj = new GameObject("BackgroundImage");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = bgGameColor;
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // 1. Title Text (Màu xanh neon #00E5FF)
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Game Title";
        titleText.enableAutoSizing = true;
        titleText.fontSize = 80;
        titleText.fontSizeMin = 24;
        titleText.fontSizeMax = 55;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = neonCyan;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -90);
        titleRect.sizeDelta = new Vector2(1000, 80);

        // 2. Instruction Text (Màu trắng)
        GameObject instObj = new GameObject("InstructionText");
        instObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI instText = instObj.AddComponent<TextMeshProUGUI>();
        instText.text = "Instruction goes here...";
        instText.enableAutoSizing = true;
        instText.fontSizeMin = 20; // Larger instruction text
        instText.fontSizeMax = 48; // Larger instruction text max size
        instText.color = textWhite;
        instText.alignment = TextAlignmentOptions.Center;
        RectTransform instRect = instObj.GetComponent<RectTransform>();
        instRect.anchorMin = new Vector2(0.5f, 1f);
        instRect.anchorMax = new Vector2(0.5f, 1f);
        instRect.anchoredPosition = new Vector2(0, -200);
        instRect.sizeDelta = new Vector2(1200, 120);

        System.Func<string, GameObject> createPanel = (name) => {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(canvasObj.transform, false);
            Image img = panel.AddComponent<Image>();
            img.color = cardSlate;
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.15f, 0.15f);
            rect.anchorMax = new Vector2(0.85f, 0.75f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            return panel;
        };

        // 3. SetupPanel
        GameObject setupPanel = createPanel("SetupPanel");

        // StartButton
        GameObject startBtnObj = new GameObject("StartButton");
        startBtnObj.transform.SetParent(setupPanel.transform, false);
        Image startImg = startBtnObj.AddComponent<Image>();
        startImg.color = cardSlate; // Đổi sang Slate mờ đồng bộ
        // Thêm viền xanh neon
        var outline = startBtnObj.AddComponent<Outline>();
        outline.effectColor = neonCyan;
        outline.effectDistance = new Vector2(2, 2);

        Button startBtn = startBtnObj.AddComponent<Button>();
        RectTransform startRect = startBtnObj.GetComponent<RectTransform>();
        startRect.anchorMin = new Vector2(0.5f, 0f);
        startRect.anchorMax = new Vector2(0.5f, 0f);
        startRect.anchoredPosition = new Vector2(0, 50);
        startRect.sizeDelta = new Vector2(200, 60);

        GameObject startTxtObj = new GameObject("Text");
        startTxtObj.transform.SetParent(startBtnObj.transform, false);
        TextMeshProUGUI startTxt = startTxtObj.AddComponent<TextMeshProUGUI>();
        startTxt.text = "START";
        startTxt.fontSize = 28;
        startTxt.fontStyle = FontStyles.Bold;
        startTxt.alignment = TextAlignmentOptions.Center;
        startTxt.color = neonCyan; // Màu chữ xanh neon
        RectTransform startTxtRect = startTxtObj.GetComponent<RectTransform>();
        startTxtRect.anchorMin = Vector2.zero;
        startTxtRect.anchorMax = Vector2.one;
        startTxtRect.sizeDelta = Vector2.zero;

        // 4. GameplayPanel
        GameObject gameplayPanel = createPanel("GameplayPanel");
        gameplayPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0); // Gameplay Panel trong suốt để hiện súng 3D / gameplay

        // 5. ResultPanel
        GameObject resultPanel = createPanel("ResultPanel");
        resultPanel.GetComponent<Image>().color = cardSlate;

        // LoserText (Màu xanh neon hoặc trắng nổi bật)
        GameObject loserObj = new GameObject("LoserText");
        loserObj.transform.SetParent(resultPanel.transform, false);
        TextMeshProUGUI loserText = loserObj.AddComponent<TextMeshProUGUI>();
        loserText.text = "Loser: Player — Drink!";
        loserText.enableAutoSizing = true;
        loserText.fontSizeMin = 18;
        loserText.fontSizeMax = 45;
        loserText.fontStyle = FontStyles.Bold;
        loserText.color = neonCyan; // Đổi sang màu xanh neon
        loserText.alignment = TextAlignmentOptions.Center;
        RectTransform loserRect = loserObj.GetComponent<RectTransform>();
        loserRect.anchoredPosition = new Vector2(0, 80);
        loserRect.sizeDelta = new Vector2(800, 80);

        // RestartButton
        GameObject restartBtnObj = new GameObject("RestartButton");
        restartBtnObj.transform.SetParent(resultPanel.transform, false);
        Image restartImg = restartBtnObj.AddComponent<Image>();
        restartImg.color = cardSlate;
        var restartOutline = restartBtnObj.AddComponent<Outline>();
        restartOutline.effectColor = neonCyan;
        restartOutline.effectDistance = new Vector2(2, 2);

        Button restartBtn = restartBtnObj.AddComponent<Button>();
        RectTransform restartRect = restartBtnObj.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0f);
        restartRect.anchorMax = new Vector2(0.5f, 0f);
        restartRect.anchoredPosition = new Vector2(-180, 60);
        restartRect.sizeDelta = new Vector2(220, 75);

        GameObject restartTxtObj = new GameObject("Text");
        restartTxtObj.transform.SetParent(restartBtnObj.transform, false);
        TextMeshProUGUI restartTxt = restartTxtObj.AddComponent<TextMeshProUGUI>();
        restartTxt.text = "Restart";
        restartTxt.fontSize = 26;
        restartTxt.fontStyle = FontStyles.Bold;
        restartTxt.alignment = TextAlignmentOptions.Center;
        restartTxt.color = textWhite; // Chữ trắng
        RectTransform restartTxtRect = restartTxtObj.GetComponent<RectTransform>();
        restartTxtRect.anchorMin = Vector2.zero;
        restartTxtRect.anchorMax = Vector2.one;
        restartTxtRect.sizeDelta = Vector2.zero;

        // BackToMenuButton
        GameObject backBtnObj = new GameObject("BackToMenuButton");
        backBtnObj.transform.SetParent(resultPanel.transform, false);
        Image backImg = backBtnObj.AddComponent<Image>();
        backImg.color = cardSlate;
        var backOutline = backBtnObj.AddComponent<Outline>();
        backOutline.effectColor = neonCyan;
        backOutline.effectDistance = new Vector2(2, 2);

        Button backBtn = backBtnObj.AddComponent<Button>();
        RectTransform backRect = backBtnObj.GetComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0.5f, 0f);
        backRect.anchorMax = new Vector2(0.5f, 0f);
        backRect.anchoredPosition = new Vector2(180, 60);
        backRect.sizeDelta = new Vector2(220, 75);

        GameObject backTxtObj = new GameObject("Text");
        backTxtObj.transform.SetParent(backBtnObj.transform, false);
        TextMeshProUGUI backTxt = backTxtObj.AddComponent<TextMeshProUGUI>();
        backTxt.text = "Menu";
        backTxt.fontSize = 24;
        backTxt.fontStyle = FontStyles.Bold;
        backTxt.alignment = TextAlignmentOptions.Center;
        backTxt.color = neonCyan; // Chữ xanh
        RectTransform backTxtRect = backTxtObj.GetComponent<RectTransform>();
        backTxtRect.anchorMin = Vector2.zero;
        backTxtRect.anchorMax = Vector2.one;
        backTxtRect.sizeDelta = Vector2.zero;

        System.IO.Directory.CreateDirectory("Assets/_Game/Prefabs");
        string prefabPath = "Assets/_Game/Prefabs/GameCanvas.prefab";
        PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
        GameObject.DestroyImmediate(canvasObj);
        
        Debug.Log("Beautified GameCanvas Prefab with #14171F Background and Cyan/White Text successfully!");
    }
}
#endif
