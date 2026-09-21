using System.Collections;
using TMPro;
using UnityEngine;
using RTLTMPro;

public class WallCannonsAudioAndTextScript : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip WallCannonsAudio_EN;
    public AudioClip WallCannonsAudio_AR;
    public AudioClip WallCannonsAudio_HE;

    [Header("UI")]
    public RTLTextMeshPro WallCannonsHistoryText;
    public GameObject NarrationBox;
    public GameObject WallCannonsImage;

    [Header("Fonts")]
    public TMP_FontAsset EnglishFont;
    public TMP_FontAsset HebrewFont;
    public TMP_FontAsset ArabicFont;

    [Header("Timing")]
    public bool AutoSyncToClip = true;
    public float FixedSecondsPerLine = 6f;

    private AudioSource _audio;
    private Coroutine _currentNarrationCoroutine;

    // ENGLISH: The Story of the 1799 Siege
    private string[] narration_EN = {
        "• Spring 1799: Napoleon Bonaparte marches from Egypt to conquer Acre.",
        "• He sought to break these very walls to open a path to the East.",
        "• But Acre stood ready under Governor Ahmad Pasha al-Jazzar.",
        "• British forces joined the defense, led by Commodore Sidney Smith.",
        "• From these ramparts, cannons pounded the French siege lines.",
        "• The brave defenders repaired every breach the attackers opened.",
        "• For 60 days the battle raged as disease weakened the French army.",
        "• In May, the assault collapsed and Napoleon was forced to retreat.",
        "• Napoleon later admitted: 'That heap of stones destroyed my destiny'.",
        "• These cannons turned back one of history's greatest conquerors.",
        "• Today, they remain a symbol of the city's resilience and survival."
    };

    // HEBREW: סיפור המצור של 1799
    private string[] narration_HE = {
        "• אביב 1799: נפוליאון בונפרטה צועד ממצרים כדי לכבוש את עכו.",
        "• הוא ביקש לפרוץ את החומות הללו כדי לפתוח נתיב למזרח.",
        "• אך עכו עמדה מוכנה תחת המושל אחמד פאשה אל-ג'זאר.",
        "• כוחות בריטיים הצטרפו להגנה בפיקודו של סידני סמית'.",
        "• מהסוללות הללו, תותחים הלומו בקווי המצור הצרפתיים.",
        "• המגינים האמיצים תיקנו כל פרצה שפתחו התוקפים.",
        "• במשך 60 יום השתולל הקרב בעוד מחלות החלישו את צבאו של נפוליאון.",
        "• במאי, המתקפה קרסה ונפוליאון נאלץ לסגת.",
        "• נפוליאון הודה מאוחר יותר: 'ערימת האבנים הזו הרסה את גורלי'.",
        "• התותחים הללו הדפו את אחד הכובשים הגדולים בהיסטוריה.",
        "• כיום, הם נותרו סמל לחוסנה ולהישרדותה של העיר."
    };

    // ARABIC: قصة حصار عام 1799
    private string[] narration_AR = {
        "• ربيع 1799: نابليون بونابرت يزحف من مصر لغزو عكا.",
        "• سعى لاختراق هذه الأسوار لفتح طريق نحو الشرق.",
        "• لكن عكا صمدت مستعدة بقيادة الوالي أحمد باشا الجزار.",
        "• انضمت القوات البريطانية للدفاع بقيادة الكومودور سيدني سميث.",
        "• من هذه الحصون، دكت المدافع خطوط الحصار الفرنسية.",
        "• قام المدافعون الشجعان بإصلاح كل ثغرة فتحها المهاجمون.",
        "• استعرت المعركة لمدة 60 يوماً بينما أضعفت الأمراض جيش نابليون.",
        "• في مايو، انهار الهجوم واضطر نابليون للانسحاب.",
        "• اعترف نابليون لاحقاً: 'تلك الكومة من الحجارة دمرت قدري'.",
        "• هذه المدافع صدت واحداً من أعظم الفاتحين في التاريخ.",
        "• واليوم، تظل رمزاً لصمود المدينة وبقائها."
    };

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        if (NarrationBox != null) NarrationBox.SetActive(false);
    }

    public void Play()
    {
        if (_currentNarrationCoroutine != null) StopCoroutine(_currentNarrationCoroutine);
        if (NarrationBox != null) NarrationBox.SetActive(true);
        if (WallCannonsImage != null) WallCannonsImage.SetActive(false);
        _currentNarrationCoroutine = StartCoroutine(PlayAudioAndText());
    }

    public void Stop()
    {
        if (_audio != null) _audio.Stop();
        if (_currentNarrationCoroutine != null) StopCoroutine(_currentNarrationCoroutine);
        if (WallCannonsHistoryText != null) WallCannonsHistoryText.text = "";
        if (NarrationBox != null) NarrationBox.SetActive(false);
        if (WallCannonsImage != null) WallCannonsImage.SetActive(true);
    }

    private IEnumerator PlayAudioAndText()
    {
        int langIndex = PlayerPrefs.GetInt("SelectedLanguage", 0);
        AudioClip clip;
        string[] textLines;

        if (langIndex == 1) { clip = WallCannonsAudio_AR; textLines = narration_AR; WallCannonsHistoryText.font = ArabicFont; }
        else if (langIndex == 2) { clip = WallCannonsAudio_HE; textLines = narration_HE; WallCannonsHistoryText.font = HebrewFont; }
        else { clip = WallCannonsAudio_EN; textLines = narration_EN; WallCannonsHistoryText.font = EnglishFont; }

        if (clip == null || textLines == null) yield break;
        _audio.PlayOneShot(clip, 1f);

        float perLine = FixedSecondsPerLine;
        if (AutoSyncToClip && clip.length > 0f)
            perLine = Mathf.Max(1f, clip.length / Mathf.Max(1, textLines.Length));

        foreach (var line in textLines)
        {
            SetText(line, langIndex);
            yield return new WaitForSeconds(perLine);
        }

        WallCannonsHistoryText.text = "";
        _currentNarrationCoroutine = null;
    }

    void SetText(string value, int langIndex)
    {
        if (WallCannonsHistoryText == null) return;

        // Increase line spacing for a "taller" look in the UI
        WallCannonsHistoryText.lineSpacing = 25f;
        WallCannonsHistoryText.alignment = (langIndex == 1 || langIndex == 2) ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
        WallCannonsHistoryText.text = value;
    }

    private void OnDisable()
    {
        if (_currentNarrationCoroutine != null) StopCoroutine(_currentNarrationCoroutine);
        if (_audio != null) _audio.Stop();
    }
}
