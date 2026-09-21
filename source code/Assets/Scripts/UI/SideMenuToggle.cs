using UnityEngine;
using UnityEngine.UI;

public class SideMenuToggle : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The whole side panel GameObject (parent of the buttons).")]
    public GameObject sideMenuPanel;

    [Tooltip("Button that opens the side menu (hamburger).")]
    public Button hamburgerButton;

    [Tooltip("Button that closes the side menu (X / close / background).")]
    public Button closeButton;

    [Header("Start State")]
    public bool startHidden = true;

    private void Awake()
    {
        if (sideMenuPanel == null)
            Debug.LogError("[SideMenuToggle] sideMenuPanel is not assigned!");

        if (hamburgerButton == null)
            Debug.LogError("[SideMenuToggle] hamburgerButton is not assigned!");

        if (startHidden && sideMenuPanel != null)
            sideMenuPanel.SetActive(false);

        if (hamburgerButton != null)
        {
            hamburgerButton.onClick.RemoveAllListeners();
            hamburgerButton.onClick.AddListener(OpenMenu);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseMenu);
        }
    }

    public void OpenMenu()
    {
        if (sideMenuPanel != null)
            sideMenuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        if (sideMenuPanel != null)
            sideMenuPanel.SetActive(false);
    }

    // Optional: toggle from animation events or other UI
    public void ToggleMenu()
    {
        if (sideMenuPanel == null) return;
        sideMenuPanel.SetActive(!sideMenuPanel.activeSelf);
    }
}
