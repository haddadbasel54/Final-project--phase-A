using UnityEngine;

/// <summary>
/// Central place to manage player profiles & per-slot PlayerPrefs.
/// Slot keys look like: "Slot1_PlayerName", "Slot2_BronzeUnlocked_tower_of_flies", etc.
/// </summary>
public static class PlayerProfileManager
{
    public const int MaxSlots = 3;

    private const string ActiveSlotKey = "ActiveSlotIndex";
    private const string PlayerNameKey = "PlayerName";

    // All site IDs we currently support (for clearing / progress calculation)
    private static readonly string[] SiteIds = { "tower_of_flies", "khan_el_hamir" };

    // -------- ACTIVE SLOT --------

    public static int ActiveSlotIndex
    {
        get => PlayerPrefs.GetInt(ActiveSlotKey, 0); // 0 = none
        set
        {
            PlayerPrefs.SetInt(ActiveSlotKey, Mathf.Clamp(value, 0, MaxSlots));
            PlayerPrefs.Save();
        }
    }

    public static bool HasActiveSlot => ActiveSlotIndex >= 1 && ActiveSlotIndex <= MaxSlots;

    public static void Save() => PlayerPrefs.Save();

    private static string SlotKey(int slotIndex, string rawKey)
    {
        return $"Slot{slotIndex}_{rawKey}";
    }

    private static string ActiveSlotKeyWrapped(string rawKey)
    {
        int slot = ActiveSlotIndex;
        if (slot <= 0)
        {
            Debug.LogWarning($"[Profile] No active slot, ignoring key '{rawKey}'.");
            return null;
        }
        return SlotKey(slot, rawKey);
    }

    // -------- GENERIC GET/SET FOR ACTIVE SLOT --------

    public static int GetInt(string rawKey, int defaultValue = 0)
    {
        string key = ActiveSlotKeyWrapped(rawKey);
        if (string.IsNullOrEmpty(key)) return defaultValue;
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetInt(string rawKey, int value)
    {
        string key = ActiveSlotKeyWrapped(rawKey);
        if (string.IsNullOrEmpty(key)) return;
        PlayerPrefs.SetInt(key, value);
    }

    public static string GetString(string rawKey, string defaultValue = "")
    {
        string key = ActiveSlotKeyWrapped(rawKey);
        if (string.IsNullOrEmpty(key)) return defaultValue;
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static void SetString(string rawKey, string value)
    {
        string key = ActiveSlotKeyWrapped(rawKey);
        if (string.IsNullOrEmpty(key)) return;
        PlayerPrefs.SetString(key, value);
    }

    public static bool HasKey(string rawKey)
    {
        string key = ActiveSlotKeyWrapped(rawKey);
        if (string.IsNullOrEmpty(key)) return false;
        return PlayerPrefs.HasKey(key);
    }

    // -------- PER-SLOT ACCESS (used by UI / delete / preview) --------

    public static bool SlotExists(int slotIndex)
    {
        return !string.IsNullOrEmpty(GetPlayerName(slotIndex));
    }

    public static string GetPlayerName(int slotIndex)
    {
        return PlayerPrefs.GetString(SlotKey(slotIndex, PlayerNameKey), "");
    }

    public static void SetPlayerName(int slotIndex, string name)
    {
        PlayerPrefs.SetString(SlotKey(slotIndex, PlayerNameKey), name);
        PlayerPrefs.Save();
    }

    public static int GetIntForSlot(int slotIndex, string rawKey, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(SlotKey(slotIndex, rawKey), defaultValue);
    }

    public static void SetIntForSlot(int slotIndex, string rawKey, int value)
    {
        PlayerPrefs.SetInt(SlotKey(slotIndex, rawKey), value);
    }

    /// <summary>
    /// Clears ALL known data for a given slot (name + progression).
    /// </summary>
    public static void ClearSlot(int slotIndex)
    {
        // Clear name
        PlayerPrefs.DeleteKey(SlotKey(slotIndex, PlayerNameKey));

        // Clear per-site flags
        foreach (var siteId in SiteIds)
        {
            PlayerPrefs.DeleteKey(SlotKey(slotIndex, $"HasPlayed_{siteId}"));
            PlayerPrefs.DeleteKey(SlotKey(slotIndex, $"BronzeUnlocked_{siteId}"));
            PlayerPrefs.DeleteKey(SlotKey(slotIndex, $"SilverUnlocked_{siteId}"));
        }

        // If this was the active slot, clear it
        if (ActiveSlotIndex == slotIndex)
        {
            PlayerPrefs.SetInt(ActiveSlotKey, 0);
        }

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Returns 0..1 representing 0%-100% completion for a given slot.
    /// Each trophy (bronze/silver per site) counts as 25%.
    /// </summary>
    public static float GetCompletionPercent(int slotIndex)
    {
        float totalTrophies = SiteIds.Length * 2; // bronze + silver per site
        float owned = 0;

        foreach (var siteId in SiteIds)
        {
            if (GetIntForSlot(slotIndex, $"BronzeUnlocked_{siteId}", 0) == 1) owned++;
            if (GetIntForSlot(slotIndex, $"SilverUnlocked_{siteId}", 0) == 1) owned++;
        }

        if (totalTrophies <= 0) return 0;
        return owned / totalTrophies;
    }

    /// <summary>
    /// True if this slot has 100% completion (gold condition).
    /// </summary>
    public static bool IsGoldUnlockedForSlot(int slotIndex)
    {
        return GetCompletionPercent(slotIndex) >= 0.999f;
    }

    /// <summary>
    /// True if the ACTIVE slot has 100% completion.
    /// </summary>
    public static bool IsGoldUnlockedForActive()
    {
        if (!HasActiveSlot) return false;
        return IsGoldUnlockedForSlot(ActiveSlotIndex);
    }

    /// <summary>
    /// Returns true if ANY slot has a registered player.
    /// </summary>
    public static bool AnySlotExists()
    {
        for (int i = 1; i <= MaxSlots; i++)
            if (SlotExists(i)) return true;
        return false;
    }

    /// <summary>
    /// Returns the first existing slot index, or 0 if none.
    /// </summary>
    public static int GetFirstExistingSlot()
    {
        for (int i = 1; i <= MaxSlots; i++)
            if (SlotExists(i)) return i;
        return 0;
    }

    public static string GetActivePlayerName()
    {
        if (!HasActiveSlot) return "";
        return GetPlayerName(ActiveSlotIndex);
    }
}
