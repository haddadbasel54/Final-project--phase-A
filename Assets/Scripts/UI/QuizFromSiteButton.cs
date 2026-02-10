using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizFromSiteButton : MonoBehaviour
{
    [Header("Quiz Button Reference")]
    public Button quizButton;

    [Header("Quiz Scene")]
    [Tooltip("The single quiz scene that serves all sites.")]
    public string quizSceneName = "QuizScene";

    void Awake()
    {
        // Auto-find button under this Canvas if not assigned
        if (quizButton == null)
            quizButton = GetComponentInChildren<Button>(true);

        if (quizButton == null)
        {
            Debug.LogError("[QuizFromSiteButton] ❌ No Button found. Assign it in the inspector.");
            return;
        }

        // We own the click logic
        quizButton.onClick.RemoveAllListeners();
        quizButton.onClick.AddListener(OnQuizPressed);

        Debug.Log($"[QuizFromSiteButton] ✅ Wired to '{quizButton.gameObject.name}' on Canvas '{name}'.");
    }

    public void OnQuizPressed()
    {
        // Remember from which scene we came (TowerOfFliesScene, Khan scene, WallCannons scene, etc.)
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("lastMenuScene", currentScene);
        PlayerPrefs.Save();

        // Get the current siteId
        string siteId = PlayerPrefs.GetString("siteId", "tower_of_flies");

        // --- NORMALIZATION ---
        // Ensure the ID matches what the QuizController expects
        if (siteId == "khan_al_hameer") siteId = "khan_el_hamir";
        if (siteId == "wall_cannons_site") siteId = "wall_cannons"; // ADDED THIS

        Debug.Log($"[QuizFromSiteButton] siteId='{siteId}' → Quiz → '{quizSceneName}' from '{currentScene}'");

        // Jump to the shared quiz scene (QuizController will pick questions by siteId)
        SceneManager.LoadScene(quizSceneName);
    }
}