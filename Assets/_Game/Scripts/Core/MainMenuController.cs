using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private SceneLoader sceneLoader;

    private void Start()
    {
        // Phát nhạc nền main menu
        AudioManager.Instance.PlayBGM("BGM_MainMenu");

        sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader == null)
        {
            GameObject loaderObj = new GameObject("SceneLoader");
            sceneLoader = loaderObj.AddComponent<SceneLoader>();
        }

        // Tìm và gán sự kiện cho các Button theo tên Scene tương ứng
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (var btn in buttons)
        {
            // Phát sfx click cho mọi button
            btn.onClick.AddListener(() => {
                AudioManager.Instance.PlaySFX("SFX_Click");
            });

            string btnName = btn.gameObject.name;
            // Nếu tên Button trùng với tên Scene, tự động gán sự kiện LoadScene
            if (btnName.EndsWith("Scene"))
            {
                btn.onClick.AddListener(() => {
                    sceneLoader.LoadScene(btnName);
                });
            }
        }
    }
}
