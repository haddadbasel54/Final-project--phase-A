using System.Collections;
using TMPro;
using UnityEngine;
using RTLTMPro; // Required for proper Arabic & Hebrew shaping

public class TowerOfFliesAudioAndTextScript : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip TowerOfFliesAudio_EN;
    public AudioClip TowerOfFliesAudio_AR;
    public AudioClip TowerOfFliesAudio_HE;

    [Header("UI")]
    public RTLTextMeshPro TowerOfFliesHistoryText;
    public GameObject NarrationBox;
    public GameObject TowerOfFliesImage;

    [Header("Fonts")]
    public TMP_FontAsset EnglishFont;
    public TMP_FontAsset HebrewFont;
    public TMP_FontAsset ArabicFont;

    [Header("Timing")]
    public bool AutoSyncToClip = true;
    public float FixedSecondsPerLine = 6f;

    private AudioSource _audio;
    private Coroutine _currentNarrationCoroutine;

    // ENGLISH
    private string[] narration_EN = {
        "Rising from a rocky islet at Acre's harbor entrance.",
        "Originally built by Phoenicians as a maritime beacon.",
        "It guided ancient merchant ships and secured trade routes.",
        "Acre became a vital hub in the Phoenician network.",
        "Crusaders captured Acre in the First Crusade.",
        "They rebuilt and reinforced the tower in the 12th century.",
        "They renamed it the Tower of the Flies.",
        "A chain blocked enemy ships from entering.",
        "It played a major role in naval defense.",
        "Later it was strengthened by the Ottomans.",
        "Today its ruins rise from the sea."
    };

    // HEBREW
    private string[] narration_HE = {
        "המגדל מתנשא מתוך אי סלעי בכניסה לנמל עכו.",
        "הוא נבנה על ידי הפיניקים כמגדלור ימי.",
        "הוא הדריך ספינות מסחר עתיקות.",
        "עכו הפכה למרכז מסחרי חשוב.",
        "הצלבנים כבשו את עכו.",
        "הם חיזקו את המגדל במאה ה-12.",
        "הוא נקרא מגדל הזבובים.",
        "שרשרת חסמה ספינות אויב.",
        "הוא הגן על הנמל.",
        "העות'מאנים חיזקו אותו.",
        "היום שרידיו עומדים בים."
    };

    // ARABIC
    private string[] narration_AR = {
        "يرتفع البرج من جزيرة صخرية عند مدخل ميناء عكا.",
        "بناه الفينيقيون كمنارة بحرية.",
        "كان يرشد السفن التجارية القديمة.",
        "أصبحت عكا مركزاً مهماً.",
        "استولى الصليبيون على عكا.",
        "وأعادوا بناء البرج في القرن الثاني عشر.",
        "وسمي برج الذباب.",
        "سلسلة منعت سفن الأعداء.",
        "كان جزءاً من الدفاع البحري.",
        "عززه العثمانيون.",
        "ولا تزال أطلاله قائمة اليوم."
    };

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        if (NarrationBox != null)
            NarrationBox.SetActive(false);
    }

    public void Play()
    {
        if (_currentNarrationCoroutine != null)
            StopCoroutine(_currentNarrationCoroutine);

        if (NarrationBox != null)
            NarrationBox.SetActive(true);

        if (TowerOfFliesImage != null)
            TowerOfFliesImage.SetActive(false);

        _currentNarrationCoroutine = StartCoroutine(PlayAudioAndText());
    }

    public void Stop()
    {
        if (_audio != null)
            _audio.Stop();

        if (_currentNarrationCoroutine != null)
            StopCoroutine(_currentNarrationCoroutine);

        if (TowerOfFliesHistoryText != null)
            TowerOfFliesHistoryText.text = "";

        if (NarrationBox != null)
            NarrationBox.SetActive(false);

        if (TowerOfFliesImage != null)
            TowerOfFliesImage.SetActive(true);
    }

    private IEnumerator PlayAudioAndText()
    {
        int langIndex = PlayerPrefs.GetInt("SelectedLanguage", 0);

        AudioClip clip;
        string[] textLines;

        if (langIndex == 1)
        {
            clip = TowerOfFliesAudio_AR;
            textLines = narration_AR;
            TowerOfFliesHistoryText.font = ArabicFont;
        }
        else if (langIndex == 2)
        {
            clip = TowerOfFliesAudio_HE;
            textLines = narration_HE;
            TowerOfFliesHistoryText.font = HebrewFont;
        }
        else
        {
            clip = TowerOfFliesAudio_EN;
            textLines = narration_EN;
            TowerOfFliesHistoryText.font = EnglishFont;
        }

        if (clip == null || textLines == null || textLines.Length == 0)
            yield break;

        _audio.PlayOneShot(clip, 1f);

        float perLine = FixedSecondsPerLine;
        if (AutoSyncToClip && clip.length > 0f)
            perLine = Mathf.Max(1f, clip.length / Mathf.Max(1, textLines.Length));

        foreach (string line in textLines)
        {
            SetText(line, langIndex);
            yield return new WaitForSeconds(perLine);
        }

        TowerOfFliesHistoryText.text = "";
        _currentNarrationCoroutine = null;
    }

    private void SetText(string value, int langIndex)
    {
        if (TowerOfFliesHistoryText == null)
            return;

        // Arabic (1) & Hebrew (2) align right
        if (langIndex == 1 || langIndex == 2)
            TowerOfFliesHistoryText.alignment = TextAlignmentOptions.Right;
        else
            TowerOfFliesHistoryText.alignment = TextAlignmentOptions.Left;

        // RTLTMPro handles shaping internally
        TowerOfFliesHistoryText.text = value;
    }

    private void OnDisable()
    {
        if (_currentNarrationCoroutine != null)
            StopCoroutine(_currentNarrationCoroutine);

        if (_audio != null)
            _audio.Stop();
    }
}
