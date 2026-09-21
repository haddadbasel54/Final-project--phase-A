using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMenuButton : MonoBehaviour
{
    [Header("Back Button Reference")]
    public Button backButton;

    [Header("Scene Names per Site")]
    public string towerMenuScene = "MainMenu";
    public string khanMenuScene = "MainMenu";
    public string wallCannonsMenuScene = "MainMenu"; // ✅ NEW
    public string fallbackScene = "MainMainMenu";

    void Awake()
    {
        if (backButton == null)
            backButton = GetComponentInChildren<Button>(true);

        if (backButton == null)
        {
            Debug.LogError("[BackToMenuButton] ❌ No Button found under Canvas. Assign it in the inspector.");
            return;
        }

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBackPressed);

        Debug.Log($"[BackToMenuButton] ✅ Wired to '{backButton.gameObject.name}' on Canvas '{name}'.");
    }

    static string Normalize(string id)
    {
        return id == "khan_al_hameer" ? "khan_el_hamir" : id;
    }

    public void OnBackPressed()
    {
        string siteId = Normalize(PlayerPrefs.GetString("siteId", "tower_of_flies"));
        string target = fallbackScene;

        // 🔓 Unlock bronze trophy for this site FOR THE ACTIVE PLAYER ONLY
        string bronzeKey = $"BronzeUnlocked_{siteId}";
        PlayerProfileManager.SetInt(bronzeKey, 1);
        PlayerProfileManager.Save();

        switch (siteId)
        {
            case "tower_of_flies":
                target = towerMenuScene;
                break;

            case "khan_el_hamir":
                target = khanMenuScene;
                break;

            case "wall_cannons": // ✅ NEW SITE
                target = wallCannonsMenuScene;
                break;

            default:
                target = fallbackScene;
                break;
        }

        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            Debug.LogError($"[BackToMenuButton] Scene '{target}' not in Build Settings → falling back to '{fallbackScene}'.");
            target = fallbackScene;
        }

        Debug.Log($"[BackToMenuButton] 🔙 siteId='{siteId}' → Load '{target}'");
        SceneManager.LoadScene(target);
    }
}
