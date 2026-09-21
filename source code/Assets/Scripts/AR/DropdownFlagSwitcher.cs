using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownFlagSwitcher : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Image flagImage;

    public Sprite englishFlag;
    public Sprite arabicFlag;
    public Sprite hebrewFlag;

    // We keep this enum so your other scripts (Tower/Khan) still compile
    public enum Language { English, Arabic, Hebrew }

    void Start()
    {
        // 1. Load the saved language index from the phone's memory
        int savedIndex = PlayerPrefs.GetInt("SelectedLanguage", 0);

        // 2. Set the dropdown value WITHOUT triggering the event (prevents loops)
        if (dropdown != null)
        {
            dropdown.SetValueWithoutNotify(savedIndex);

            // 3. Update the Flag image to match the saved state
            UpdateLanguage(savedIndex);

            // 4. Start listening for manual user changes
            dropdown.onValueChanged.AddListener(UpdateLanguage);
        }
    }

    public void UpdateLanguage(int index)
    {
        // Save the choice to the phone immediately
        PlayerPrefs.SetInt("SelectedLanguage", index);
        PlayerPrefs.Save();

        // Update the flag sprite
        if (flagImage != null)
        {
            switch (index)
            {
                case 0:
                    flagImage.sprite = englishFlag;
                    break;
                case 1:
                    flagImage.sprite = arabicFlag;
                    break;
                case 2:
                    flagImage.sprite = hebrewFlag;
                    break;
            }
        }
    }
}