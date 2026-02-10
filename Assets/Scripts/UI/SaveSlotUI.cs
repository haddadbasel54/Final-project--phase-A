using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual + buttons for a single save slot line (1, 2, 3).
/// </summary>
public class SaveSlotUI : MonoBehaviour
{
    [Header("Config")]
    public int slotIndex = 1; // 1..3

    [Header("UI")]
    public Button selectButton;
    public Button deleteButton;
    public TMP_Text labelText;

    [Tooltip("Optional icon that shows when this slot has 100% completion (gold).")]
    public Image goldTrophyIcon;

    [Header("Owner")]
    public MainMainMenuController controller;

    private void Awake()
    {
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectClicked);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDeleteClicked);
        }
    }

    public void Refresh()
    {
        bool exists = PlayerProfileManager.SlotExists(slotIndex);

        if (exists)
        {
            string name = PlayerProfileManager.GetPlayerName(slotIndex);
            float percent = PlayerProfileManager.GetCompletionPercent(slotIndex);
            int percentInt = Mathf.RoundToInt(percent * 100f);

            if (labelText != null)
                labelText.text = $"{slotIndex}. {name} – {percentInt}%";

            if (deleteButton != null) deleteButton.gameObject.SetActive(true);

            if (goldTrophyIcon != null)
                goldTrophyIcon.gameObject.SetActive(PlayerProfileManager.IsGoldUnlockedForSlot(slotIndex));
        }
        else
        {
            if (labelText != null)
                labelText.text = $"{slotIndex}. [Empty]";

            if (deleteButton != null) deleteButton.gameObject.SetActive(false);

            if (goldTrophyIcon != null)
                goldTrophyIcon.gameObject.SetActive(false);
        }
    }

    private void OnSelectClicked()
    {
        controller?.HandleSlotSelected(this);
    }

    private void OnDeleteClicked()
    {
        controller?.HandleSlotDelete(this);
    }
}
