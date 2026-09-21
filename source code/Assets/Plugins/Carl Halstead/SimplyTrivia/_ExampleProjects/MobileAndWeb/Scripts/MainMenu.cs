using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject mainGameCanvas;

    public void OpenPage()
    {
        Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/102628?aid=1101l3ozs");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
