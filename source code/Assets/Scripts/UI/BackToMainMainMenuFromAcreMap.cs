using UnityEngine;
using UnityEngine.SceneManagement; // Required for switching scenes

public class SceneLoader : MonoBehaviour
{
    public void GoToMainMainMenu()
    {
        // Go Back to MainMainMenuScene
        SceneManager.LoadScene("MainMainMenu");
    }
}