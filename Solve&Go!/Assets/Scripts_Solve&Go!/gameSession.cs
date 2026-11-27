using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Keeps score and the death counter across levels, and owns the HUD.
//
// Dying never ends the run. The level restarts after a short pause and the
// death counter goes up, troll-platformer style. Finishing the last level
// returns to the main menu, which shows the stats of the run.
public class gameSession : MonoBehaviour
{
    public const string LastRunDeathsKey = "LastRunDeaths";
    public const string LastRunScoreKey = "LastRunScore";

    [Tooltip("Seconds between dying and the level restarting.")]
    [SerializeField] float respawnDelay = 0.8f;
    [SerializeField] int score = 0;

    [Header("HUD")]
    [SerializeField] TMP_FontAsset hudFont;
    [SerializeField] Sprite coinIcon;

    [Header("Old HUD texts (hidden when the game starts)")]
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;

    int deaths;
    bool respawning;
    bool isDuplicate;
    GameHud hud;

    void Awake()
    {
        if (FindObjectsOfType<gameSession>().Length > 1)
        {
            isDuplicate = true;
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (isDuplicate) { return; }

        TextMeshProUGUI oldText = livesText != null ? livesText : scoreText;
        if (oldText != null && oldText.canvas != null) { oldText.canvas.gameObject.SetActive(false); }

        hud = GameHud.Create(transform, hudFont != null ? hudFont : TMP_Settings.defaultFontAsset, coinIcon);
        hud.SetDeaths(deaths, false);
        hud.SetScore(score, false);
        hud.SetLevel(SceneManager.GetActiveScene().buildIndex, SceneManager.sceneCountInBuildSettings - 1);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            // Back at the main menu means the run is over.
            PlayerPrefs.SetInt(LastRunDeathsKey, deaths);
            PlayerPrefs.SetInt(LastRunScoreKey, score);
            PlayerPrefs.Save();
            Destroy(gameObject);
            return;
        }
        if (hud != null) { hud.SetLevel(scene.buildIndex, SceneManager.sceneCountInBuildSettings - 1); }
    }

    public void ProcessPlayerDeath()
    {
        if (respawning) { return; }
        respawning = true;
        deaths++;
        if (hud != null) { hud.SetDeaths(deaths, true); }
        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        respawning = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        if (hud != null) { hud.SetScore(score, true); }
    }
}
