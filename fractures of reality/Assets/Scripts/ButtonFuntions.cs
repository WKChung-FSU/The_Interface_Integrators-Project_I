using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonFunctions : MonoBehaviour
{
  public void resume()
    {
        gameManager.instance.stateUnPaused();
    }
    public void restart()
    {
        gameManager.instance.stopSpawning=true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnPaused();
    }

    public void respawn(){

        gameManager.instance.Respawn(true);
        gameManager.instance.stateUnPaused();

    }
    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
