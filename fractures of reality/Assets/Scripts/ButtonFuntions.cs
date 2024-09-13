using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
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
        gameManager.instance.PlayerController.InitializeFractureBars();
        gameManager.instance.stateUnPaused();

    }

    public void Save(GameObject player)
    {
        SaveSystem.Save(player);
    }

    public void Load()
    {
        PlayerData data = SaveSystem.Load();

        if (data == null)
        {
            Debug.LogError("No saves can be loaded.");
            return;
        }
        Vector3 position;

        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;
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
