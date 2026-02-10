using UnityEngine;
using UnityEngine.SceneManagement;

public class SiteRouter : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void ChooseSite(string siteId)
    {
        if (string.IsNullOrEmpty(siteId))
        {
            Debug.LogError("[SiteRouter] siteId is null or empty!");
            return;
        }

        // --- NORMALIZATION ---
        // 1. Force to lowercase to prevent "Cannons" vs "cannons" bugs
        string cleanId = siteId.Trim().ToLower();

        // 2. Map variations to standard IDs
        if (cleanId == "khan_al_hameer")
            cleanId = "khan_el_hamir";

        if (cleanId == "wall_cannons_site" || cleanId == "cannons" || cleanId == "wallcannons")
            cleanId = "wall_cannons";

        // Remember selection
        PlayerPrefs.SetString("siteId", cleanId);
        PlayerPrefs.Save();

        Debug.Log($"[SiteRouter] Site Selected: {cleanId}. Preparing to load {mainMenuSceneName}...");

        // --- SAFETY CHECK ---
        // Ensure the MainMenu scene is actually in your Build Settings
        if (Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError($"[SiteRouter] Scene '{mainMenuSceneName}' not found in Build Settings! Did you add it?");
        }
    }
}