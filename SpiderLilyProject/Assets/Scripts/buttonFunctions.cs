using UnityEngine;
using UnityEngine.SceneManagement;
public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        if (gameManager.instance.CurrentMenu == gameManager.instance.Menus["Pause"])
            gameManager.instance.CloseCurrentMenu();
    }
    public void restart()
    {
        //bad version of restart for time's sake
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }
    public void quit()
    {
        //doesnt close down unity 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //this closes unity down, not good for testing
        Application.Quit();
#endif
    }
    public void OpenSettings()
    {
        // This will pause the game if not already paused,
        gameManager.instance.OpenMenu("Settings");
    }
    public void CloseMenu()
    {
        gameManager.instance.CloseCurrentMenu();
    }
    //Main Menu Buttons
    public void OnPlayPressed()
    {
        var sm = SceneManagerScript.instance;
        if (sm == null)
        {
            Debug.LogError("SceneManagerScript instance not found!");
            return;
        }

        sm.nextLevelIndex = 1;        // Level 1 index in Build Settings
        sm.nextLevelName = "Level 1";
        sm.isFinalLevel = false;

        SceneManager.LoadScene("TransitionScene");
        SceneManager.LoadScene("TransitionScene");
    }
    /// <summary>
    /// Called from Main Menu / Pause Menu Dev/Showcase button
    /// Loads Showcase Level directly (bypassing story/transition)
    /// </summary>
    public void OnShowcasePressed()
    {
        var sm = SceneManagerScript.instance;
        if (sm == null)
        {
            Debug.LogError("SceneManagerScript instance not found!");
            return;
        }

        SceneManager.LoadScene("ShowcaseLevel"); // Exact scene name
    }
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        gameManager.instance.unpauseTime();

    }
    public void SettingActiveTab(string tabname)
    {
        Debug.Log("Switching to tab: " + tabname);
       gameManager.instance.SetActiveSettingsTab(tabname);

    }
    public void OpenCredits()
    {
        gameManager.instance.OpenMenu("Credits");
    }

    /*    public void respawn()
        {
            gameManager.instance.playerScript.spawnPlayer(); ///check player movement 
            gameManager.instance.stateUnpause();
        }*/
}