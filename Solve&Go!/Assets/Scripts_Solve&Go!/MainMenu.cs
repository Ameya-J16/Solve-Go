using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    // Defined in Assets/Plugins/WebGL/QuitScreen.jslib
    [DllImport("__Internal")]
    static extern void SolveGo_ShowQuitScreen();
#endif

    void Awake()
    {
        // Neon title, tagline, button hover effects, last-run stats.
        if (GetComponent<MainMenuStyle>() == null) { gameObject.AddComponent<MainMenuStyle>(); }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        // A web page can't close its own tab, so show a goodbye screen
        // (with a "Play again" button) and stop the game.
        SolveGo_ShowQuitScreen();
        AudioListener.pause = true;
        Time.timeScale = 0f;
#else
        Application.Quit();
#endif
    }
}
