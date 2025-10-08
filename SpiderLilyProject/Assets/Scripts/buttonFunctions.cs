using UnityEngine;
using UnityEngine.SceneManagement;
public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
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

/*    public void respawn()
    {
        gameManager.instance.playerScript.spawnPlayer(); ///check player movement 
        gameManager.instance.stateUnpause();
    }*/
}