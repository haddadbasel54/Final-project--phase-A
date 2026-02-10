using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;
}

[System.Serializable]
public class SiteQuiz
{
    [Tooltip("Must match the siteId chosen in Main Menu (e.g., 'tower_of_flies', 'khan_el_hamir', 'wall_cannons')")]
    public string siteId;

    [Tooltip("Questions to use when this siteId is active")]
    public Question[] questions;
}

public class QuizController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text questionDisplay;
    public Button[] answerButtons;
    public GameObject nextButton;
    public GameObject resultPanel;
    public TMP_Text resultScoreText;

    [Header("Result UI Extras")]
    public TMP_Text resultTitleText;
    public TMP_Text resultDetailsText;

    [Header("Default/Legacy Quiz Data")]
    public Question[] questions;

    [Header("Per-Site Question Banks")]
    public SiteQuiz[] siteQuizzes;

    [Tooltip("Fallback siteId if none was stored yet.")]
    public string defaultSiteId = "tower_of_flies";

    [Header("Timer")]
    public TimerManager timerManager;
    public float timePerQuestion = 30f;

    [Header("Flow")]
    public bool autoAdvance = true;
    public float autoAdvanceDelay = 1.2f;

    // runtime state
    private string siteId;
    private Question[] activeQuestions;
    private int currentIndex = 0;
    private int score = 0;
    private bool answered = false;
    private Coroutine autoNextCoroutine;

    private void Start()
    {
        // 1. Read selected site from PlayerPrefs
        siteId = PlayerPrefs.GetString("siteId", defaultSiteId);

        // 2. Normalize IDs so they match your array entries
        if (siteId == "khan_al_hameer") siteId = "khan_el_hamir";
        if (siteId == "wall_cannons_site") siteId = "wall_cannons"; // NEW normalization for Wall Cannons

        // 3. Pick active question set for this site
        activeQuestions = ResolveQuestionsForSite(siteId);

        if (activeQuestions == null || activeQuestions.Length == 0)
        {
            // Fallback to legacy array so old setups still work
            activeQuestions = questions != null ? questions : new Question[0];
        }

        // Basic UI init
        resultPanel.SetActive(false);
        if (nextButton) nextButton.SetActive(!autoAdvance);

        if (timerManager != null)
            timerManager.OnTimerFinished += OnTimeExpired;

        if (activeQuestions.Length > 0) DisplayQuestion();
        else ShowEmptyQuizMessage();
    }

    private Question[] ResolveQuestionsForSite(string id)
    {
        if (siteQuizzes == null) return null;

        foreach (var sq in siteQuizzes)
        {
            if (!string.IsNullOrEmpty(sq.siteId) &&
                sq.siteId == id &&
                sq.questions != null &&
                sq.questions.Length > 0)
            {
                return sq.questions;
            }
        }
        return null;
    }

    private void DisplayQuestion()
    {
        answered = false;

        if (autoNextCoroutine != null)
        {
            StopCoroutine(autoNextCoroutine);
            autoNextCoroutine = null;
        }

        if (nextButton) nextButton.SetActive(!autoAdvance);

        Question q = activeQuestions[currentIndex];
        if (questionDisplay) questionDisplay.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            var btn = answerButtons[i];
            if (!btn) continue;

            var lbl = btn.GetComponentInChildren<TMP_Text>();
            btn.onClick.RemoveAllListeners();

            if (i < q.answers.Length)
            {
                btn.gameObject.SetActive(true);
                if (lbl) lbl.text = q.answers[i];
                int index = i;
                btn.onClick.AddListener(() => OnAnswerSelected(index));
                if (btn.image) btn.image.color = Color.white;
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }

        if (timerManager != null)
            timerManager.StartTimer(timePerQuestion);
    }

    private void OnAnswerSelected(int idx)
    {
        if (answered) return;
        answered = true;

        if (timerManager != null)
            timerManager.StartTimer(0); // stop timer

        Question q = activeQuestions[currentIndex];
        bool correct = (idx == q.correctAnswerIndex);

        if (correct)
        {
            score += 20;
            if (answerButtons[idx].image)
                answerButtons[idx].image.color = Color.green;
        }
        else
        {
            if (answerButtons[idx].image)
                answerButtons[idx].image.color = Color.red;

            if (answerButtons[q.correctAnswerIndex].image)
                answerButtons[q.correctAnswerIndex].image.color = Color.green;
        }

        HandlePostAnswerFlow();
    }

    private void OnTimeExpired(object sender, System.EventArgs e)
    {
        if (answered) return;
        answered = true;

        Question q = activeQuestions[currentIndex];

        foreach (var btn in answerButtons)
        {
            if (btn && btn.image) btn.image.color = Color.gray;
        }

        if (answerButtons[q.correctAnswerIndex].image)
            answerButtons[q.correctAnswerIndex].image.color = Color.green;

        HandlePostAnswerFlow();
    }

    private void HandlePostAnswerFlow()
    {
        if (autoAdvance)
        {
            if (autoNextCoroutine != null) StopCoroutine(autoNextCoroutine);
            autoNextCoroutine = StartCoroutine(AutoNextAfterDelay());
        }
        else
        {
            if (nextButton) nextButton.SetActive(true);
        }
    }

    private System.Collections.IEnumerator AutoNextAfterDelay()
    {
        yield return new WaitForSeconds(autoAdvanceDelay);
        autoNextCoroutine = null;
        OnNextPressed();
    }

    public void OnNextPressed()
    {
        currentIndex++;
        if (currentIndex < activeQuestions.Length) DisplayQuestion();
        else ShowResults();
    }

    private void ShowResults()
    {
        if (autoNextCoroutine != null)
        {
            StopCoroutine(autoNextCoroutine);
            autoNextCoroutine = null;
        }

        if (questionDisplay) questionDisplay.gameObject.SetActive(false);
        foreach (var btn in answerButtons)
            if (btn) btn.gameObject.SetActive(false);

        if (nextButton) nextButton.SetActive(false);

        int maxScore = activeQuestions.Length * 20;

        if (resultPanel) resultPanel.SetActive(true);
        if (resultScoreText) resultScoreText.text = $"Score: {score}/{maxScore}";

        if (resultTitleText || resultDetailsText)
        {
            float ratio = maxScore > 0 ? (float)score / maxScore : 0f;
            string title, details;

            if (ratio >= 0.99f)
            {
                title = "Legendary Explorer!";
                details = "Perfect run – you uncovered every detail of this site.";
            }
            else if (ratio >= 0.7f)
            {
                title = "Great Job!";
                details = "You know this place very well.";
            }
            else if (ratio >= 0.4f)
            {
                title = "Good Start!";
                details = "Replay to discover more facts.";
            }
            else
            {
                title = "Keep Exploring!";
                details = "Try again and see what secrets you missed.";
            }

            if (resultTitleText) resultTitleText.text = title;
            if (resultDetailsText) resultDetailsText.text = details;
        }

        // UNLOCK SILVER TROPHY
        if (score == maxScore && maxScore > 0)
        {
            string silverKey = $"SilverUnlocked_{siteId}";
            PlayerProfileManager.SetInt(silverKey, 1);
            PlayerProfileManager.Save();
            Debug.Log($"[Quiz] Silver trophy unlocked for: {siteId}");
        }
    }

    private void ShowEmptyQuizMessage()
    {
        if (questionDisplay)
            questionDisplay.text = $"No questions configured for siteId '{siteId}'.";
        foreach (var btn in answerButtons) if (btn) btn.gameObject.SetActive(false);
        if (nextButton) nextButton.SetActive(false);
    }

    public void OnBackToMenu()
    {
        string target = PlayerPrefs.GetString("lastMenuScene", "MainMenu");
        SceneManager.LoadScene(target);
    }
}