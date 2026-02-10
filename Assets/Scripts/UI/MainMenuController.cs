using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public Button quizButton;
    public GameObject exitDialog;

    [Header("Trophies (per site)")]
    public GameObject bronzeTrophyIcon;
    public GameObject silverTrophyIcon;

    [Header("Info Panels")]
    public GameObject playInfoPanel;
    public GameObject quizInfoPanel;

    [Header("Dynamic Logo")]
    public Image dynamicLogo;
    public Sprite towerOfFliesLogo;
    public Sprite khanElHamirLogo;
    public Sprite wallCannonsLogo; // NEW: Added for Wall Cannons

    [Serializable]
    public class SiteRoute
    {
        public string siteId;
        public string dataScene;
    }

    [Header("Routing per site (Play only)")]
    public SiteRoute[] routes;
    public string defaultSiteId = "tower_of_flies";
    public string defaultDataScene = "TowerOfFliesScene";

    [Header("Unified quiz scene")]
    public string quizSceneName = "QuizScene";

    private const string mainMainMenuScene = "MainMainMenu";

    string siteId;
    SiteRoute active;

    string PlayedKey => $"HasPlayed_{siteId}";
    string BronzeKey => $"BronzeUnlocked_{siteId}";
    string SilverKey => $"SilverUnlocked_{siteId}";

    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("siteId"))
            PlayerPrefs.SetString("siteId", defaultSiteId);

        siteId = NormalizeSiteId(PlayerPrefs.GetString("siteId", defaultSiteId));
        PlayerPrefs.SetString("siteId", siteId);
        PlayerPrefs.Save();

        active = FindRoute(siteId);

        RefreshQuizButtonState();
        RefreshTrophyIcons();

        if (playInfoPanel) playInfoPanel.SetActive(false);
        if (quizInfoPanel) quizInfoPanel.SetActive(false);

        SetDynamicLogo();
    }

    void OnEnable()
    {
        RefreshQuizButtonState();
        RefreshTrophyIcons();
        SetDynamicLogo();
    }

    static string NormalizeSiteId(string id)
    {
        if (id == "khan_al_hameer") return "khan_el_hamir";
        if (id == "wall_cannons_site") return "wall_cannons"; // NEW: Normalize for Wall Cannons
        return id;
    }

    SiteRoute FindRoute(string id)
    {
        if (routes != null)
        {
            foreach (var r in routes)
                if (!string.IsNullOrEmpty(r.siteId) && r.siteId == id)
                    return r;
        }
        return null;
    }

    void RefreshQuizButtonState()
    {
        if (!quizButton) return;
        bool hasPlayed = PlayerProfileManager.GetInt(PlayedKey, 0) == 1;
        quizButton.interactable = hasPlayed;
    }

    void RefreshTrophyIcons()
    {
        if (bronzeTrophyIcon)
        {
            bool bronzeUnlocked = PlayerProfileManager.GetInt(BronzeKey, 0) == 1;
            bronzeTrophyIcon.SetActive(bronzeUnlocked);
        }

        if (silverTrophyIcon)
        {
            bool silverUnlocked = PlayerProfileManager.GetInt(SilverKey, 0) == 1;
            silverTrophyIcon.SetActive(silverUnlocked);
        }
    }

    void SetDynamicLogo()
    {
        if (dynamicLogo == null) return;

        Sprite target = null;

        switch (siteId)
        {
            case "tower_of_flies":
                target = towerOfFliesLogo;
                break;
            case "khan_el_hamir":
                target = khanElHamirLogo;
                break;
            case "wall_cannons": // NEW: Logic for Wall Cannons logo
                target = wallCannonsLogo;
                break;
            default:
                target = towerOfFliesLogo;
                break;
        }

        if (target != null)
        {
            dynamicLogo.sprite = target;
            dynamicLogo.enabled = true;
        }
        else
        {
            dynamicLogo.enabled = false;
        }
    }

    public void ShowPlayInfo() { if (playInfoPanel) playInfoPanel.SetActive(true); }
    public void ShowQuizInfo() { if (quizInfoPanel) quizInfoPanel.SetActive(true); }
    public void HidePlayInfo() { if (playInfoPanel) playInfoPanel.SetActive(false); }
    public void HideQuizInfo() { if (quizInfoPanel) quizInfoPanel.SetActive(false); }

    public void OnPlayPressed()
    {
        PlayerProfileManager.SetInt(PlayedKey, 1);
        PlayerPrefs.SetString("lastMenuScene", SceneManager.GetActiveScene().name);
        PlayerProfileManager.Save();

        string target = (active != null && !string.IsNullOrEmpty(active.dataScene))
            ? active.dataScene
            : defaultDataScene;

        SceneManager.LoadScene(target);
    }

    public void OnQuizPressed()
    {
        PlayerPrefs.SetString("lastMenuScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene(quizSceneName);
    }

    public void OnExitPressed() => exitDialog?.SetActive(true);

    public void OnConfirmExit()
    {
        if (Application.CanStreamedLevelBeLoaded(mainMainMenuScene))
            SceneManager.LoadScene(mainMainMenuScene);
        else
            Application.Quit();
    }

    public void OnCancelExit() { if (exitDialog) exitDialog.SetActive(false); }
}