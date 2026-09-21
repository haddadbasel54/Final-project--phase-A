using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Name of the map scene to load after pressing Start with an active profile.")]
    public string mapSceneName = "AcreMap";

    [Header("Main Buttons")]
    public Button startButton;
    public Button loadButton;
    public Button exitButton;

    [Header("Exit Dialog")]
    public GameObject exitDialog;      // ExitDialogBackground
    public Button confirmExitButton;   // Btn-ConfirmExit
    public Button cancelExitButton;    // Btn-CancelExit

    [Header("Greeting")]
    public TMP_Text greetingText;      // top-left text ("Welcome adventurer" / player name)

    [Header("Active Profile Gold Trophy")]
    [Tooltip("Icon shown on the main menu when the ACTIVE profile has 100% completion.")]
    public Image activeProfileGoldTrophyIcon;

    [Header("Slots Panel")]
    public GameObject slotsPanel;      // panel with all 3 slots (start hidden)
    public SaveSlotUI[] slotUIs;       // size = 3, drag each row here

    [Header("New Profile Panel")]
    public GameObject newProfilePanel; // panel with input + OK/Cancel (start hidden)
    public TMP_InputField nameInputField;
    public Button confirmNewProfileButton;
    public Button cancelNewProfileButton;

    [Header("Delete Profile Panel")]
    [Tooltip("Confirm dialog shown when deleting a profile slot.")]
    public GameObject deleteProfilePanel;
    public TMP_Text deleteProfileMessage;
    public Button confirmDeleteProfileButton;
    public Button cancelDeleteProfileButton;

    private int pendingSlotIndex = 0;         // used when creating a new profile
    private int pendingDeleteSlotIndex = 0;   // used when confirming delete

    private void Awake()
    {
        if (startButton != null) { startButton.onClick.RemoveAllListeners(); startButton.onClick.AddListener(OnStartPressed); }
        if (loadButton != null) { loadButton.onClick.RemoveAllListeners(); loadButton.onClick.AddListener(OnLoadPressed); }
        if (exitButton != null) { exitButton.onClick.RemoveAllListeners(); exitButton.onClick.AddListener(OnExitPressed); }

        if (confirmExitButton != null)
        {
            confirmExitButton.onClick.RemoveAllListeners();
            confirmExitButton.onClick.AddListener(OnConfirmExit);
        }

        if (cancelExitButton != null)
        {
            cancelExitButton.onClick.RemoveAllListeners();
            cancelExitButton.onClick.AddListener(OnCancelExit);
        }

        if (slotsPanel != null) slotsPanel.SetActive(false);
        if (newProfilePanel != null) newProfilePanel.SetActive(false);
        if (deleteProfilePanel != null) deleteProfilePanel.SetActive(false);

        if (confirmNewProfileButton != null)
        {
            confirmNewProfileButton.onClick.RemoveAllListeners();
            confirmNewProfileButton.onClick.AddListener(OnConfirmNewProfile);
        }

        if (cancelNewProfileButton != null)
        {
            cancelNewProfileButton.onClick.RemoveAllListeners();
            cancelNewProfileButton.onClick.AddListener(OnCancelNewProfile);
        }

        if (confirmDeleteProfileButton != null)
        {
            confirmDeleteProfileButton.onClick.RemoveAllListeners();
            confirmDeleteProfileButton.onClick.AddListener(OnConfirmDeleteProfile);
        }

        if (cancelDeleteProfileButton != null)
        {
            cancelDeleteProfileButton.onClick.RemoveAllListeners();
            cancelDeleteProfileButton.onClick.AddListener(OnCancelDeleteProfile);
        }
    }

    private void Start()
    {
        RefreshSlotsUI();
        RefreshGreeting();
        RefreshActiveGoldTrophy();
    }

    // -------- GREETING & GOLD --------

    private void RefreshGreeting()
    {
        if (greetingText == null) return;

        if (PlayerProfileManager.HasActiveSlot &&
            PlayerProfileManager.SlotExists(PlayerProfileManager.ActiveSlotIndex))
        {
            string name = PlayerProfileManager.GetActivePlayerName();
            greetingText.text = string.IsNullOrEmpty(name)
                ? "Welcome, adventurer"
                : $"Welcome, {name}";
        }
        else if (PlayerProfileManager.AnySlotExists())
        {
            greetingText.text = "Choose a profile to continue";
        }
        else
        {
            greetingText.text = "Welcome, adventurer";
        }
    }

    private void RefreshActiveGoldTrophy()
    {
        if (activeProfileGoldTrophyIcon == null) return;

        bool hasGold = PlayerProfileManager.IsGoldUnlockedForActive();
        activeProfileGoldTrophyIcon.gameObject.SetActive(hasGold);
    }

    private void RefreshSlotsUI()
    {
        if (slotUIs == null) return;
        foreach (var s in slotUIs)
        {
            if (s != null) s.Refresh();
        }
    }

    // -------- MAIN BUTTONS --------

    public void OnStartPressed()
    {
        // NEW BEHAVIOUR:
        // - If no active profile, just open SlotsPanel (player must pick or create)
        // - If active profile exists, THEN go to map scene.

        if (!PlayerProfileManager.HasActiveSlot)
        {
            ShowSlotsPanel();
            return;
        }

        SceneManager.LoadScene(mapSceneName);
    }

    public void OnLoadPressed()
    {
        // Always open slots panel to choose / switch profile
        ShowSlotsPanel();
    }

    public void OnExitPressed()
    {
        if (exitDialog != null) exitDialog.SetActive(true);
        else Application.Quit();
    }

    private void OnConfirmExit()
    {
        Application.Quit();
    }

    private void OnCancelExit()
    {
        if (exitDialog != null) exitDialog.SetActive(false);
    }

    // -------- SLOTS PANEL --------

    private void ShowSlotsPanel()
    {
        if (slotsPanel != null) slotsPanel.SetActive(true);
        RefreshSlotsUI();
    }

    private void HideSlotsPanel()
    {
        if (slotsPanel != null) slotsPanel.SetActive(false);
    }

    // Called by SaveSlotUI when a slot is clicked
    public void HandleSlotSelected(SaveSlotUI slotUI)
    {
        if (slotUI == null) return;
        int index = slotUI.slotIndex;
        pendingSlotIndex = index;

        bool exists = PlayerProfileManager.SlotExists(index);

        if (exists)
        {
            // NEW BEHAVIOUR:
            // Just activate and stay in MainMainMenu. Player presses Start to go to map.
            PlayerProfileManager.ActiveSlotIndex = index;

            if (newProfilePanel != null) newProfilePanel.SetActive(false);

            HideSlotsPanel();
            RefreshSlotsUI();
            RefreshGreeting();
            RefreshActiveGoldTrophy();
        }
        else
        {
            // Need to create new profile in this slot
            if (newProfilePanel != null) newProfilePanel.SetActive(true);
            if (nameInputField != null)
            {
                nameInputField.text = "";
                nameInputField.ActivateInputField();
            }
        }
    }

    // Called by SaveSlotUI when delete is pressed
    public void HandleSlotDelete(SaveSlotUI slotUI)
    {
        if (slotUI == null) return;
        int index = slotUI.slotIndex;

        pendingDeleteSlotIndex = index;

        if (deleteProfilePanel != null)
        {
            deleteProfilePanel.SetActive(true);
            if (deleteProfileMessage != null)
            {
                string existingName = PlayerProfileManager.GetPlayerName(index);
                if (string.IsNullOrEmpty(existingName))
                    deleteProfileMessage.text = $"Delete slot {index}?";
                else
                    deleteProfileMessage.text = $"Delete profile '{existingName}' in slot {index}?";
            }
        }
        else
        {
            // Fallback: delete immediately if no dialog assigned
            DoDeleteSlotImmediate(index);
        }
    }

    private void DoDeleteSlotImmediate(int slotIndex)
    {
        PlayerProfileManager.ClearSlot(slotIndex);
        pendingDeleteSlotIndex = 0;

        RefreshSlotsUI();
        RefreshGreeting();
        RefreshActiveGoldTrophy();
    }

    // -------- NEW PROFILE PANEL --------

    private void OnConfirmNewProfile()
    {
        if (pendingSlotIndex <= 0 || pendingSlotIndex > PlayerProfileManager.MaxSlots)
        {
            if (newProfilePanel != null) newProfilePanel.SetActive(false);
            return;
        }

        string name = nameInputField != null ? nameInputField.text.Trim() : "";
        if (string.IsNullOrEmpty(name))
            name = $"Explorer {pendingSlotIndex}";

        // Make sure old progress in this slot is wiped
        PlayerProfileManager.ClearSlot(pendingSlotIndex);
        PlayerProfileManager.SetPlayerName(pendingSlotIndex, name);
        PlayerProfileManager.ActiveSlotIndex = pendingSlotIndex;

        if (newProfilePanel != null) newProfilePanel.SetActive(false);

        RefreshSlotsUI();
        RefreshGreeting();
        RefreshActiveGoldTrophy();
        HideSlotsPanel();

        // IMPORTANT: DO NOT load map here → player stays on MainMainMenu
        // and has to press Start to actually go to AcreMap.
    }

    private void OnCancelNewProfile()
    {
        pendingSlotIndex = 0;
        if (newProfilePanel != null) newProfilePanel.SetActive(false);
    }

    // -------- DELETE PROFILE PANEL --------

    private void OnConfirmDeleteProfile()
    {
        if (pendingDeleteSlotIndex > 0)
        {
            DoDeleteSlotImmediate(pendingDeleteSlotIndex);
        }

        if (deleteProfilePanel != null)
            deleteProfilePanel.SetActive(false);
    }

    private void OnCancelDeleteProfile()
    {
        pendingDeleteSlotIndex = 0;

        if (deleteProfilePanel != null)
            deleteProfilePanel.SetActive(false);
    }
}
