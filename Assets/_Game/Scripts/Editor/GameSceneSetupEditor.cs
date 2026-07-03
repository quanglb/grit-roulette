#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameSceneSetupEditor : EditorWindow
{
    private static GameObject LoadGameCanvasPrefab()
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/GameCanvas.prefab");
    }

    private static void ClearScene()
    {
        var rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            if (obj.name != "Main Camera" && obj.name != "Directional Light")
            {
                GameObject.DestroyImmediate(obj);
            }
        }
    }

    private static (GameObject canvas, GameObject setupPanel, GameObject gameplayPanel, GameObject resultPanel, Transform titleText, Transform instText) SetupCommonUI(string title)
    {
        Color bgGameColor = new Color(0.0784f, 0.0902f, 0.1216f, 1f); // #14171F

        // Cập nhật background camera thành #14171F
        var mainCam = GameObject.Find("Main Camera")?.GetComponent<Camera>();
        if (mainCam != null)
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = bgGameColor;
        }

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

        GameObject prefab = LoadGameCanvasPrefab();
        GameObject canvas = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        canvas.name = "GameCanvas";

        Transform setupPanel = canvas.transform.Find("SetupPanel");
        Transform gameplayPanel = canvas.transform.Find("GameplayPanel");
        Transform resultPanel = canvas.transform.Find("ResultPanel");
        Transform titleText = canvas.transform.Find("TitleText");
        Transform instText = canvas.transform.Find("InstructionText");

        titleText.GetComponent<TextMeshProUGUI>().text = title;

        return (canvas, setupPanel.gameObject, gameplayPanel.gameObject, resultPanel.gameObject, titleText, instText);
    }

    private static void BindBaseProperties(SerializedObject so, GameObject setupPanel, GameObject gameplayPanel, GameObject resultPanel, Transform titleText, Transform instText)
    {
        so.FindProperty("setupPanel").objectReferenceValue = setupPanel;
        so.FindProperty("gameplayPanel").objectReferenceValue = gameplayPanel;
        so.FindProperty("resultPanel").objectReferenceValue = resultPanel;
        so.FindProperty("gameTitleText").objectReferenceValue = titleText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("instructionText").objectReferenceValue = instText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("loserText").objectReferenceValue = resultPanel.transform.Find("LoserText").GetComponent<TextMeshProUGUI>();
        so.FindProperty("startButton").objectReferenceValue = setupPanel.transform.Find("StartButton").GetComponent<Button>();
        so.FindProperty("restartButton").objectReferenceValue = resultPanel.transform.Find("RestartButton").GetComponent<Button>();
        so.FindProperty("backToMenuButton").objectReferenceValue = resultPanel.transform.Find("BackToMenuButton").GetComponent<Button>();
    }

    private static TMP_InputField CreateInputField(GameObject parent, string name, string defaultValue, Vector2 pos, string labelText)
    {
        Color textWhite = Color.white;
        Color inputBgColor = new Color(0.12f, 0.13f, 0.17f, 1f); // Tối màu Slate
        Color neonCyan = new Color(0f, 0.9f, 1f, 1f); // #00E5FF

        // Label (Chữ màu trắng)
        GameObject labelObj = new GameObject(name + "_Label");
        labelObj.transform.SetParent(parent.transform, false);
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = labelText;
        label.fontSize = 18;
        label.color = textWhite;
        label.alignment = TextAlignmentOptions.Center;
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(pos.x, pos.y + 40);
        labelRect.sizeDelta = new Vector2(200, 30);

        // Input Box
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent.transform, false);
        Image img = inputObj.AddComponent<Image>();
        img.color = inputBgColor;
        
        // Outline cho Input
        var outline = inputObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.25f, 0.27f, 0.38f, 1f);
        outline.effectDistance = new Vector2(1, 1);

        RectTransform rect = inputObj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(180, 45);

        TMP_InputField input = inputObj.AddComponent<TMP_InputField>();
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform, false);
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.color = textWhite; // Chữ trắng trong Input
        text.fontSize = 20;
        text.alignment = TextAlignmentOptions.Left;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-15, -5);

        input.textComponent = text;
        return input;
    }

    private static UnityEngine.UI.Dropdown CreateDropdown(GameObject parent, string name, List<string> options, Vector2 pos, string labelText)
    {
        Color textWhite = Color.white;
        Color inputBgColor = new Color(0.12f, 0.13f, 0.17f, 1f);
        Color neonCyan = new Color(0f, 0.9f, 1f, 1f); // #00E5FF

        // 1. Tạo Label phía trên Dropdown
        GameObject labelObj = new GameObject(name + "_Label");
        labelObj.transform.SetParent(parent.transform, false);
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = labelText;
        label.fontSize = 18;
        label.color = textWhite;
        label.alignment = TextAlignmentOptions.Center;
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(pos.x, pos.y + 40);
        labelRect.sizeDelta = new Vector2(200, 30);

        // 2. Tạo Dropdown chuẩn bằng DefaultControls
        DefaultControls.Resources resources = new DefaultControls.Resources();
        GameObject ddObj = DefaultControls.CreateDropdown(resources);
        ddObj.name = name;
        ddObj.transform.SetParent(parent.transform, false);

        // Cập nhật vị trí và kích thước
        RectTransform rect = ddObj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(180, 45);

        // Đổi màu sắc hình ảnh background và viền
        Image img = ddObj.GetComponent<Image>();
        if (img != null) img.color = inputBgColor;
        
        var outline = ddObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.25f, 0.27f, 0.38f, 1f);
        outline.effectDistance = new Vector2(1, 1);

        UnityEngine.UI.Dropdown dropdown = ddObj.GetComponent<UnityEngine.UI.Dropdown>();

        // 3. Tinh chỉnh Font và màu sắc hiển thị Caption Text (Chữ đang chọn)
        if (dropdown.captionText != null)
        {
            dropdown.captionText.color = textWhite;
            dropdown.captionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            dropdown.captionText.fontSize = 20;
            dropdown.captionText.alignment = TextAnchor.MiddleLeft;
            dropdown.captionText.verticalOverflow = VerticalWrapMode.Overflow;
            dropdown.captionText.horizontalOverflow = HorizontalWrapMode.Overflow;
        }

        // 4. Tăng chiều cao của Item Prototype trong Template để chữ hiển thị thoải mái
        Transform itemTrans = ddObj.transform.Find("Template/Viewport/Content/Item");
        if (itemTrans != null)
        {
            RectTransform itemRect = itemTrans.GetComponent<RectTransform>();
            itemRect.sizeDelta = new Vector2(0, 45); // Set chiều cao item là 45
        }

        // 5. Tinh chỉnh Font và màu sắc của Item Label trong Template
        if (dropdown.itemText != null)
        {
            dropdown.itemText.color = textWhite;
            dropdown.itemText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            dropdown.itemText.fontSize = 18;
            dropdown.itemText.alignment = TextAnchor.MiddleLeft;
            dropdown.itemText.verticalOverflow = VerticalWrapMode.Overflow;
            dropdown.itemText.horizontalOverflow = HorizontalWrapMode.Overflow;
        }

        // Thay đổi màu nền của Template List đổ xuống
        Transform templateTrans = ddObj.transform.Find("Template");
        if (templateTrans != null)
        {
            Image templateImg = templateTrans.GetComponent<Image>();
            if (templateImg != null) templateImg.color = inputBgColor;
            
            // Thay đổi màu nền của từng item khi hover/chọn
            Transform itemBgTrans = templateTrans.Find("Viewport/Content/Item/Item Background");
            if (itemBgTrans != null)
            {
                Image itemBgImg = itemBgTrans.GetComponent<Image>();
                if (itemBgImg != null) itemBgImg.color = new Color(0.15f, 0.16f, 0.2f, 1f);
            }
        }

        // 6. Thêm các Options
        dropdown.ClearOptions();
        List<UnityEngine.UI.Dropdown.OptionData> optionDatas = new List<UnityEngine.UI.Dropdown.OptionData>();
        foreach (var opt in options)
        {
            optionDatas.Add(new UnityEngine.UI.Dropdown.OptionData(opt));
        }
        dropdown.AddOptions(optionDatas);

        return dropdown;
    }

    private static Button CreateButton(GameObject parent, string name, string text, Vector2 pos, Color color)
    {
        Color neonCyan = new Color(0f, 0.9f, 1f, 1f); // #00E5FF
        Color cardSlate = new Color(0.15f, 0.16f, 0.2f, 0.75f);

        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);
        Image img = btnObj.AddComponent<Image>();
        
        // Nếu là nút đỏ đặc thù (như Trigger), thì có thể dùng màu đỏ nhấp nháy, nếu không thì dùng Slate mờ với Outline Cyan
        if (color == Color.red)
        {
            img.color = new Color(0.6f, 0.1f, 0.15f, 0.9f);
            var outline = btnObj.AddComponent<Outline>();
            outline.effectColor = Color.red;
            outline.effectDistance = new Vector2(2, 2);
        }
        else
        {
            img.color = cardSlate;
            var outline = btnObj.AddComponent<Outline>();
            outline.effectColor = neonCyan;
            outline.effectDistance = new Vector2(2, 2);
        }

        Button btn = btnObj.AddComponent<Button>();
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(220, 65); // Nút to rõ dễ bấm

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = 22;
        txt.fontStyle = FontStyles.Bold;
        txt.color = (color == Color.red) ? Color.white : neonCyan; // Chữ xanh neon hoặc trắng
        txt.alignment = TextAlignmentOptions.Center;
        RectTransform txtRect = txtObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;

        return btn;
    }

    [MenuItem("Tools/Drinking Game/Setup Game UI (All Scenes)")]
    public static void SetupAllGameScenesUI()
    {
        SetupRussianRoulette();
        SetupBombPass();
        SetupLuckyNumber();
        SetupSpinBottle();
        SetupReactionDuel();
        SetupHotPotato();
        SetupHigherLower();
        SetupDiceBattle();
        SetupSecretVote();
        SetupColorTrap();
        SetupMemoryChain();
        Debug.Log("Successfully setup all mini game scenes UI!");
    }

    private static void SetupRussianRoulette()
    {
        // 1. Mở Scene gốc SampleScene
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        
        GameObject sampleGameManager = GameObject.Find("GameManager");
        if (sampleGameManager == null)
        {
            Debug.LogError("Not found GameManager in SampleScene!");
            return;
        }

        // Đảm bảo có script RussianRouletteGame gốc của họ
        var gameScript = sampleGameManager.GetComponent<RussianRouletteGame>();
        if (gameScript == null)
        {
            gameScript = sampleGameManager.AddComponent<RussianRouletteGame>();
        }

        // 2. Cập nhật camera background sang #14171F
        var mainCam = GameObject.Find("Main Camera")?.GetComponent<Camera>();
        if (mainCam != null)
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0.0784f, 0.0902f, 0.1216f, 1f); // #14171F
        }

        // 3. Tinh chỉnh màu chữ của các Text TMPro hiện có trong scene sang trắng và xanh neon
        var texts = GameObject.FindObjectsOfType<TextMeshProUGUI>();
        Color textWhite = Color.white;
        Color neonCyan = new Color(0f, 0.9f, 1f, 1f); // #00E5FF

        foreach (var txt in texts)
        {
            if (txt.gameObject.name.Contains("Title") || txt.gameObject.name.Contains("Turn") || txt.gameObject.name.Contains("Result"))
            {
                txt.color = neonCyan;
            }
            else
            {
                txt.color = textWhite;
            }
        }

        var normalTexts = GameObject.FindObjectsOfType<UnityEngine.UI.Text>();
        foreach (var txt in normalTexts)
        {
            txt.color = textWhite;
        }

        // 4. Tạo nút BackToMenuButton ở Canvas gốc nếu chưa có
        var canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
        {
            Transform backBtnTrans = canvasObj.transform.Find("BackToMenuButton");
            Button backBtn = null;
            if (backBtnTrans == null)
            {
                GameObject backBtnObj = new GameObject("BackToMenuButton");
                backBtnObj.transform.SetParent(canvasObj.transform, false);
                Image backImg = backBtnObj.AddComponent<Image>();
                backImg.color = new Color(0.15f, 0.16f, 0.2f, 0.75f);
                var backOutline = backBtnObj.AddComponent<Outline>();
                backOutline.effectColor = neonCyan;
                backOutline.effectDistance = new Vector2(2, 2);

                backBtn = backBtnObj.AddComponent<Button>();
                RectTransform backRect = backBtnObj.GetComponent<RectTransform>();
                backRect.anchorMin = new Vector2(0f, 1f);
                backRect.anchorMax = new Vector2(0f, 1f);
                backRect.pivot = new Vector2(0f, 1f);
                backRect.anchoredPosition = new Vector2(50, -50); // Góc trên bên trái
                backRect.sizeDelta = new Vector2(180, 60);

                GameObject txtObj = new GameObject("Text");
                txtObj.transform.SetParent(backBtnObj.transform, false);
                TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
                txt.text = "Menu";
                txt.fontSize = 22;
                txt.fontStyle = FontStyles.Bold;
                txt.color = neonCyan;
                txt.alignment = TextAlignmentOptions.Center;
                
                RectTransform txtRect = txtObj.GetComponent<RectTransform>();
                txtRect.anchorMin = Vector2.zero;
                txtRect.anchorMax = Vector2.one;
                txtRect.sizeDelta = Vector2.zero;
            }
            else
            {
                backBtn = backBtnTrans.GetComponent<Button>();
            }

            // Gán backToMenuButton vào RussianRouletteGame
            if (backBtn != null && gameScript != null)
            {
                SerializedObject so = new SerializedObject(gameScript);
                so.FindProperty("backToMenuButton").objectReferenceValue = backBtn;
                so.ApplyModifiedProperties();
            }
        }

        // Đảm bảo EventSystem của SampleScene sử dụng InputSystemUIInputModule
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

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupBombPass()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/BombPassScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<BombPassGameManager>();
        
        var ui = SetupCommonUI("Bomb Pass");
        
        List<string> options = new List<string>();
        for (int i = 2; i <= 18; i++) options.Add($"{i} Players");
        var playersDd = CreateDropdown(ui.setupPanel, "PlayerCountDropdown", options, new Vector2(0, 20), "Player Count:");
        
        GameObject bombObj = new GameObject("BombImage");
        bombObj.transform.SetParent(ui.gameplayPanel.transform, false);
        Image bombImg = bombObj.AddComponent<Image>();
        bombImg.color = Color.black;
        RectTransform bombRect = bombObj.GetComponent<RectTransform>();
        bombRect.anchoredPosition = new Vector2(0, 100);
        bombRect.sizeDelta = new Vector2(180, 180);

        var passBtn = CreateButton(ui.gameplayPanel, "PassButton", "Chuyền Bom (Pass)", new Vector2(0, -80), Color.blue);

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("playerCountDropdown").objectReferenceValue = playersDd;
        so.FindProperty("passButton").objectReferenceValue = passBtn;
        so.FindProperty("bombImage").objectReferenceValue = bombImg;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupLuckyNumber()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/LuckyNumberScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<LuckyNumberGameManager>();
        
        var ui = SetupCommonUI("Lucky Number");
        
        List<string> options = new List<string>();
        for (int i = 3; i <= 18; i++) options.Add($"{i} Players");
        var playersDd = CreateDropdown(ui.setupPanel, "PlayerCountDropdown", options, new Vector2(-200, 20), "Player Count:");
        
        var minInput = CreateInputField(ui.setupPanel, "MinRangeInput", "1", new Vector2(0, 20), "Min Range:");
        var maxInput = CreateInputField(ui.setupPanel, "MaxRangeInput", "10", new Vector2(200, 20), "Max Range:");

        var choiceInput = CreateInputField(ui.gameplayPanel, "NumberChoiceInput", "", new Vector2(0, 70), "Nhập số của bạn:");
        var submitBtn = CreateButton(ui.gameplayPanel, "SubmitButton", "Xác Nhận (Submit)", new Vector2(0, -20), Color.green);
        var revealBtn = CreateButton(ui.gameplayPanel, "RevealButton", "Xem Kết Quả (Reveal)", new Vector2(0, -20), Color.red);

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("playerCountDropdown").objectReferenceValue = playersDd;
        so.FindProperty("minRangeInput").objectReferenceValue = minInput;
        so.FindProperty("maxRangeInput").objectReferenceValue = maxInput;
        so.FindProperty("numberChoiceInput").objectReferenceValue = choiceInput;
        so.FindProperty("submitButton").objectReferenceValue = submitBtn;
        so.FindProperty("revealButton").objectReferenceValue = revealBtn;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupSpinBottle()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SpinBottleScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<SpinBottleGameManager>();
        
        var ui = SetupCommonUI("Spin Bottle");
        
        List<string> options = new List<string>();
        for (int i = 2; i <= 18; i++) options.Add($"{i} Players");
        var playersDd = CreateDropdown(ui.setupPanel, "PlayerCountDropdown", options, new Vector2(0, 20), "Player Count:");

        GameObject containerObj = new GameObject("PlayersContainer");
        containerObj.transform.SetParent(ui.gameplayPanel.transform, false);
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(0, 40);
        containerRect.sizeDelta = new Vector2(400, 400);

        GameObject bottleObj = new GameObject("BottleImage");
        bottleObj.transform.SetParent(ui.gameplayPanel.transform, false);
        Image bottleImg = bottleObj.AddComponent<Image>();
        bottleImg.color = new Color(0f, 0.9f, 1f, 1f); // Neon Cyan bottle
        RectTransform bottleRect = bottleObj.GetComponent<RectTransform>();
        bottleRect.anchoredPosition = new Vector2(0, 40);
        bottleRect.sizeDelta = new Vector2(30, 180);

        // Tạo PointerArrow chỉ người thua
        GameObject arrowObj = new GameObject("PointerArrow");
        arrowObj.transform.SetParent(ui.gameplayPanel.transform, false);
        Image arrowImg = arrowObj.AddComponent<Image>();
        arrowImg.color = new Color(1f, 0.09f, 0.27f, 1f); // Neon Crimson Red
        
        RectTransform arrowRect = arrowObj.GetComponent<RectTransform>();
        arrowRect.pivot = new Vector2(0f, 0.5f); // Quay quanh đuôi (tâm vòng tròn)
        arrowRect.anchoredPosition = new Vector2(0, 40); // Trùng tâm với cái chai
        arrowRect.sizeDelta = new Vector2(90, 20);

        // Tip đầu nhọn hình thoi xoay 45 độ
        GameObject tipObj = new GameObject("Tip");
        tipObj.transform.SetParent(arrowObj.transform, false);
        Image tipImg = tipObj.AddComponent<Image>();
        tipImg.color = new Color(1f, 0.09f, 0.27f, 1f);
        RectTransform tipRect = tipObj.GetComponent<RectTransform>();
        tipRect.anchorMin = new Vector2(1f, 0.5f);
        tipRect.anchorMax = new Vector2(1f, 0.5f);
        tipRect.anchoredPosition = Vector2.zero;
        tipRect.sizeDelta = new Vector2(30, 30);
        tipRect.localRotation = Quaternion.Euler(0, 0, 45);

        arrowObj.SetActive(false); // Ẩn lúc khởi đầu

        var spinBtn = CreateButton(ui.gameplayPanel, "SpinButton", "Quay Chai (Spin)", new Vector2(0, -180), Color.blue);

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("playerCountDropdown").objectReferenceValue = playersDd;
        so.FindProperty("bottleTransform").objectReferenceValue = bottleRect;
        so.FindProperty("spinButton").objectReferenceValue = spinBtn;
        so.FindProperty("playersContainer").objectReferenceValue = containerRect;
        so.FindProperty("pointerArrow").objectReferenceValue = arrowRect;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupReactionDuel()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ReactionDuelScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<ReactionDuelGameManager>();
        
        var ui = SetupCommonUI("Reaction Duel");
        
        var p1Input = CreateInputField(ui.setupPanel, "P1NameInput", "Player 1", new Vector2(-150, 20), "P1 Name:");
        var p2Input = CreateInputField(ui.setupPanel, "P2NameInput", "Player 2", new Vector2(150, 20), "P2 Name:");

        GameObject signalObj = new GameObject("SignalImage");
        signalObj.transform.SetParent(ui.gameplayPanel.transform, false);
        Image signalImg = signalObj.AddComponent<Image>();
        signalImg.color = Color.gray;
        RectTransform signalRect = signalObj.GetComponent<RectTransform>();
        signalRect.anchoredPosition = new Vector2(0, 70);
        signalRect.sizeDelta = new Vector2(180, 180);

        var p1Btn = CreateButton(ui.gameplayPanel, "P1Button", "TAP P1", new Vector2(-180, -100), Color.blue);
        var p2Btn = CreateButton(ui.gameplayPanel, "P2Button", "TAP P2", new Vector2(180, -100), Color.red);

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("p1NameInput").objectReferenceValue = p1Input;
        so.FindProperty("p2NameInput").objectReferenceValue = p2Input;
        so.FindProperty("p1Button").objectReferenceValue = p1Btn;
        so.FindProperty("p2Button").objectReferenceValue = p2Btn;
        so.FindProperty("signalImage").objectReferenceValue = signalImg;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupHotPotato()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/HotPotatoScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<HotPotatoGameManager>();
        
        var ui = SetupCommonUI("Hot Potato");
        
        List<string> options = new List<string>();
        for (int i = 2; i <= 18; i++) options.Add($"{i} Players");
        var playersDd = CreateDropdown(ui.setupPanel, "PlayerCountDropdown", options, new Vector2(0, 20), "Player Count:");

        GameObject potatoObj = new GameObject("PotatoImage");
        potatoObj.transform.SetParent(ui.gameplayPanel.transform, false);
        Image potatoImg = potatoObj.AddComponent<Image>();
        potatoImg.color = new Color(0.6f, 0.4f, 0.2f, 1f);
        RectTransform potatoRect = potatoObj.GetComponent<RectTransform>();
        potatoRect.anchoredPosition = new Vector2(0, 90);
        potatoRect.sizeDelta = new Vector2(180, 120);

        var nextBtn = CreateButton(ui.gameplayPanel, "NextButton", "Chuyền Khoai (Next)", new Vector2(0, -90), Color.green);

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("playerCountDropdown").objectReferenceValue = playersDd;
        so.FindProperty("nextButton").objectReferenceValue = nextBtn;
        so.FindProperty("potatoImage").objectReferenceValue = potatoImg;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupHigherLower()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/HigherLowerScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<HigherLowerGameManager>();
        
        var ui = SetupCommonUI("Higher or Lower");
        
        var pInput = CreateInputField(ui.setupPanel, "PlayerNameInput", "Player", new Vector2(0, 20), "Player Name:");

        GameObject numObj = new GameObject("CurrentNumberText");
        numObj.transform.SetParent(ui.gameplayPanel.transform, false);
        TextMeshProUGUI numTxt = numObj.AddComponent<TextMeshProUGUI>();
        numTxt.text = "7";
        numTxt.fontSize = 80;
        numTxt.fontStyle = FontStyles.Bold;
        numTxt.color = new Color(0f, 0.9f, 1f, 1f); // Neon Cyan chữ
        numTxt.alignment = TextAlignmentOptions.Center;
        RectTransform numRect = numObj.GetComponent<RectTransform>();
        numRect.anchoredPosition = new Vector2(0, 90);
        numRect.sizeDelta = new Vector2(250, 120);

        var higherBtn = CreateButton(ui.gameplayPanel, "HigherButton", "Cao Hơn (Higher)", new Vector2(-180, -50), Color.blue);
        var lowerBtn = CreateButton(ui.gameplayPanel, "LowerButton", "Thấp Hơn (Lower)", new Vector2(180, -50), Color.red);
        var passBtn = CreateButton(ui.gameplayPanel, "PassButton", "Dừng Lại (Pass)", new Vector2(0, -135), Color.green);

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("playerNameInput").objectReferenceValue = pInput;
        so.FindProperty("currentNumberText").objectReferenceValue = numTxt;
        so.FindProperty("higherButton").objectReferenceValue = higherBtn;
        so.FindProperty("lowerButton").objectReferenceValue = lowerBtn;
        so.FindProperty("passButton").objectReferenceValue = passBtn;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupDiceBattle()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/DiceBattleScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<DiceBattleGameManager>();
        
        var ui = SetupCommonUI("Dice Battle");
        
        List<string> options = new List<string>();
        for (int i = 2; i <= 18; i++) options.Add($"{i} Players");
        var playersDd = CreateDropdown(ui.setupPanel, "PlayerCountDropdown", options, new Vector2(0, 20), "Player Count:");

        GameObject resObj = new GameObject("DiceResultsText");
        resObj.transform.SetParent(ui.gameplayPanel.transform, false);
        TextMeshProUGUI resTxt = resObj.AddComponent<TextMeshProUGUI>();
        resTxt.text = "";
        resTxt.fontSize = 24;
        resTxt.color = Color.white; // Chữ trắng
        resTxt.alignment = TextAlignmentOptions.Center;
        RectTransform resRect = resObj.GetComponent<RectTransform>();
        resRect.anchoredPosition = new Vector2(0, 60);
        resRect.sizeDelta = new Vector2(500, 220);

        var rollBtn = CreateButton(ui.gameplayPanel, "RollButton", "Đổ Xúc Xắc (Roll)", new Vector2(0, -90), Color.blue);

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("playerCountDropdown").objectReferenceValue = playersDd;
        so.FindProperty("rollButton").objectReferenceValue = rollBtn;
        so.FindProperty("diceResultsText").objectReferenceValue = resTxt;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupSecretVote()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SecretVoteScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<SecretVoteGameManager>();
        
        var ui = SetupCommonUI("Secret Vote");
        
        List<string> options = new List<string>();
        for (int i = 3; i <= 18; i++) options.Add($"{i} Players");
        var playersDd = CreateDropdown(ui.setupPanel, "PlayerCountDropdown", options, new Vector2(0, 20), "Player Count:");

        GameObject containerObj = new GameObject("VotingButtonsContainer");
        containerObj.transform.SetParent(ui.gameplayPanel.transform, false);
        GridLayoutGroup grid = containerObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(110, 35);
        grid.spacing = new Vector2(10, 10);
        grid.childAlignment = TextAnchor.MiddleCenter;
        RectTransform containerRect = containerObj.GetComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(0, 50);
        containerRect.sizeDelta = new Vector2(400, 280);

        var showBtn = CreateButton(ui.gameplayPanel, "ShowVoteListButton", "Xem Danh Sách Vote", new Vector2(0, -110), Color.blue);

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("playerCountDropdown").objectReferenceValue = playersDd;
        so.FindProperty("votingButtonsContainer").objectReferenceValue = containerObj;
        so.FindProperty("showVoteListButton").objectReferenceValue = showBtn;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupColorTrap()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ColorTrapScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<ColorTrapGameManager>();
        
        var ui = SetupCommonUI("Color Trap");
        
        var pInput = CreateInputField(ui.setupPanel, "PlayerNameInput", "Player", new Vector2(0, 20), "Player Name:");

        GameObject wordObj = new GameObject("WordDisplay");
        wordObj.transform.SetParent(ui.gameplayPanel.transform, false);
        TextMeshProUGUI wordTxt = wordObj.AddComponent<TextMeshProUGUI>();
        wordTxt.text = "RED";
        wordTxt.fontSize = 72;
        wordTxt.fontStyle = FontStyles.Bold;
        wordTxt.alignment = TextAlignmentOptions.Center;
        RectTransform wordRect = wordObj.GetComponent<RectTransform>();
        wordRect.anchoredPosition = new Vector2(0, 110);
        wordRect.sizeDelta = new Vector2(400, 100);

        GameObject barBgObj = new GameObject("TimeBarBackground");
        barBgObj.transform.SetParent(ui.gameplayPanel.transform, false);
        Image bgImg = barBgObj.AddComponent<Image>();
        bgImg.color = new Color(0.12f, 0.13f, 0.17f, 1f);
        RectTransform bgRect = barBgObj.GetComponent<RectTransform>();
        bgRect.anchoredPosition = new Vector2(0, 35);
        bgRect.sizeDelta = new Vector2(400, 15);

        GameObject barObj = new GameObject("TimeBar");
        barObj.transform.SetParent(barBgObj.transform, false);
        Image barImg = barObj.AddComponent<Image>();
        barImg.color = new Color(0f, 0.9f, 1f, 1f); // Neon Cyan
        barImg.type = Image.Type.Filled;
        barImg.fillMethod = Image.FillMethod.Horizontal;
        RectTransform barRect = barObj.GetComponent<RectTransform>();
        barRect.anchorMin = Vector2.zero;
        barRect.anchorMax = Vector2.one;
        barRect.sizeDelta = Vector2.zero;

        GameObject containerObj = new GameObject("AnswersContainer");
        containerObj.transform.SetParent(ui.gameplayPanel.transform, false);
        GridLayoutGroup grid = containerObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(150, 50);
        grid.spacing = new Vector2(20, 20);
        grid.childAlignment = TextAnchor.MiddleCenter;
        RectTransform containerRect = containerObj.GetComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(0, -60);
        containerRect.sizeDelta = new Vector2(360, 140);

        Button[] ansBtns = new Button[4];
        string[] colors = { "Đỏ", "Xanh Lá", "Xanh Dương", "Vàng" };
        Color[] btnColors = { Color.red, Color.green, Color.blue, Color.yellow };
        for (int i = 0; i < 4; i++)
        {
            int idx = i;
            ansBtns[i] = CreateButton(containerObj, $"AnsButton_{idx}", colors[idx], Vector2.zero, btnColors[idx]);
        }

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("playerNameInput").objectReferenceValue = pInput;
        so.FindProperty("wordDisplay").objectReferenceValue = wordTxt;
        so.FindProperty("answerButtons").ClearArray();
        for (int i = 0; i < 4; i++)
        {
            so.FindProperty("answerButtons").InsertArrayElementAtIndex(i);
            so.FindProperty("answerButtons").GetArrayElementAtIndex(i).objectReferenceValue = ansBtns[i];
        }
        so.FindProperty("timeBar").objectReferenceValue = barImg;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void SetupMemoryChain()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MemoryChainScene.unity");
        ClearScene();
        
        GameObject mgrObj = new GameObject("GameManager");
        var manager = mgrObj.AddComponent<MemoryChainGameManager>();
        
        var ui = SetupCommonUI("Memory Chain");
        
        var pInput = CreateInputField(ui.setupPanel, "PlayerNameInput", "Player", new Vector2(0, 20), "Player Name:");

        GameObject containerObj = new GameObject("ColorsGrid");
        containerObj.transform.SetParent(ui.gameplayPanel.transform, false);
        GridLayoutGroup grid = containerObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(130, 130);
        grid.spacing = new Vector2(25, 25);
        grid.childAlignment = TextAnchor.MiddleCenter;
        RectTransform containerRect = containerObj.GetComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(0, 45);
        containerRect.sizeDelta = new Vector2(320, 320);

        Button[] colBtns = new Button[4];
        Color[] btnColors = { new Color(0.8f, 0, 0), new Color(0, 0.8f, 0), new Color(0, 0, 0.8f), new Color(0.8f, 0.8f, 0) };
        for (int i = 0; i < 4; i++)
        {
            int idx = i;
            colBtns[i] = CreateButton(containerObj, $"ColorButton_{idx}", "", Vector2.zero, btnColors[idx]);
        }

        SerializedObject so = new SerializedObject(manager);
        BindBaseProperties(so, ui.setupPanel, ui.gameplayPanel, ui.resultPanel, ui.titleText, ui.instText);
        so.FindProperty("playerNameInput").objectReferenceValue = pInput;
        so.FindProperty("colorButtons").ClearArray();
        for (int i = 0; i < 4; i++)
        {
            so.FindProperty("colorButtons").InsertArrayElementAtIndex(i);
            so.FindProperty("colorButtons").GetArrayElementAtIndex(i).objectReferenceValue = colBtns[i];
        }
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }
}
#endif
