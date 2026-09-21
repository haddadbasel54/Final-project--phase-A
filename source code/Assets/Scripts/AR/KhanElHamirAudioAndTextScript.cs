using System.Collections;
using TMPro;
using UnityEngine;
using RTLTMPro; // 1. Make sure you have the RTLTMPro asset installed

public class KhanElHamirAudioAndTextScript : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip KhanElHamirAudio_EN;
    public AudioClip KhanElHamirAudio_AR;
    public AudioClip KhanElHamirAudio_HE;

    [Header("UI")]
    // 2. Change TMP_Text to RTLTextMeshPro
    public RTLTextMeshPro KhanElHamirHistoryText;
    public GameObject NarrationBox;
    public GameObject KhanElHamirImage;

    [Header("Fonts")]
    public TMP_FontAsset EnglishFont;
    public TMP_FontAsset HebrewFont;
    public TMP_FontAsset ArabicFont;

    [Header("Timing")]
    public bool AutoSyncToClip = true;
    public float FixedSecondsPerLine = 4f;

    private AudioSource _audio;
    private Coroutine _currentNarrationCoroutine;

    private string[] narration_EN = {
        "Khan el-Hamir — the “Donkeys’ Khan” — stood inside Acre’s land walls by the Land Gate.",
        "It served as stables and a supply point.",
        "Cannonballs were moved by donkeys to the wall batteries.",
        "In November 1840, during the British–Austrian–Ottoman naval bombardment,",
        "ship fire hit this area and the khan was badly damaged, not erased.",
        "Parts were built into the wall — like the small arched doorway —",
        "reminding us of its logistical role,",
        "and the damage this quarter took."
    };

    private string[] narration_HE = {
        "חאן אל-חומאר, חאן החמורים, ניצב בתוך חומות היבשה של עכו בסמוך לשער היבשה.",
        "הוא שימש כאורוות וכנקודת אספקה מרכזית.",
        "חמורים העבירו דרכו פגזי תותחים אל סוללות התותחים שעל החומה.",
        "בנובמבר 1840, במהלך הפצצה ימית של הצי הבריטי, האוסטרי והעות'מאני,",
        "נפגע אזור זה והחאן ניזוק קשות אך לא נמחק לחלוטין.",
        "חלקים ממנו שולבו בהמשך בתוך החומה — כמו פתח הכניסה הקשתי הקטן —",
        "המזכיר לנו את תפקידו הלוגיסטי,",
        "ואת הנזק הכבד שספג הרובע הזה."
    };

    private string[] narration_AR = {
        "خان الحمار، المعروف بخان الحمير، كان يقع داخل أسوار عكا البرية بجانب بوابة البر.",
        "استُخدم الخان كإسطبلات ونقطة إمداد حيوية.",
        "كانت الحمير تنقل قذائف المدافع عبره إلى بطاريات المدافع على الأسوار.",
        "في نوفمبر عام 1840، خلال القصف البحري البريطاني النمساوي العثماني،",
        "أصابت النيران هذه المنطقة وتضرر الخان بشدة لكنه لم ينمحِ تماماً.",
        "بُنيت أجزاء منه داخل السور — مثل البوابة الصغيرة المقوسة —",
        "تذكرنا بدوره اللوجيستي وبالدمار الذي حل بهذا الحي."
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
        if (KhanElHamirImage != null) KhanElHamirImage.SetActive(false);
        _currentNarrationCoroutine = StartCoroutine(PlayAudioAndText());
    }

    public void Stop()
    {
        if (_audio != null) _audio.Stop();
        if (_currentNarrationCoroutine != null) StopCoroutine(_currentNarrationCoroutine);
        if (KhanElHamirHistoryText != null) KhanElHamirHistoryText.text = "";
        if (NarrationBox != null) NarrationBox.SetActive(false);
        if (KhanElHamirImage != null) KhanElHamirImage.SetActive(true);
    }

    private IEnumerator PlayAudioAndText()
    {
        int langIndex = PlayerPrefs.GetInt("SelectedLanguage", 0);
        AudioClip clip;
        string[] textLines;

        if (langIndex == 1) { clip = KhanElHamirAudio_AR; textLines = narration_AR; KhanElHamirHistoryText.font = ArabicFont; }
        else if (langIndex == 2) { clip = KhanElHamirAudio_HE; textLines = narration_HE; KhanElHamirHistoryText.font = HebrewFont; }
        else { clip = KhanElHamirAudio_EN; textLines = narration_EN; KhanElHamirHistoryText.font = EnglishFont; }

        if (clip == null) yield break;
        _audio.PlayOneShot(clip, 1f);

        float perLine = FixedSecondsPerLine;
        if (AutoSyncToClip && clip.length > 0f)
            perLine = Mathf.Max(1f, clip.length / Mathf.Max(1, textLines.Length));

        foreach (var line in textLines)
        {
            SetText(line, langIndex);
            yield return new WaitForSeconds(perLine);
        }

        KhanElHamirHistoryText.text = "";
        _currentNarrationCoroutine = null;
    }

    void SetText(string value, int langIndex)
    {
        if (KhanElHamirHistoryText == null) return;

        // Arabic (1) and Hebrew (2) use Right Alignment
        if (langIndex == 1 || langIndex == 2)
            KhanElHamirHistoryText.alignment = TextAlignmentOptions.Right;
        else
            KhanElHamirHistoryText.alignment = TextAlignmentOptions.Left;

        // RTLTMPro handles the shaping/reversing internally when you set .text
        KhanElHamirHistoryText.text = value;
    }

    private void OnDisable()
    {
        if (_currentNarrationCoroutine != null) StopCoroutine(_currentNarrationCoroutine);
        if (_audio != null) _audio.Stop();
    }
}