using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UIElements;
public class ButtonFunctions : MonoBehaviour
{

    [SerializeField] KeyPocket playerKeysPocket;
    [SerializeField] KeyPocket MasterKeyPocket;
    [SerializeField] PowerCrystalManifest powerCrystalManifest;
    [SerializeField] ScoreKeeper scoreKeeper;
    [SerializeField] SpellList CurrentSpells;
    public void resume()
    {
        gameManager.instance.stateUnPaused();
    }
    public void restart()
    {
      
        gameManager.instance.stopSpawning=true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnPaused();
        gameManager.instance.scoreKeeper.IncreaseDeathCount();
    }

    public void respawn(){

        gameManager.instance.Respawn(true);
        gameManager.instance.PlayerController.InitializeFractureBars();
        gameManager.instance.stateUnPaused();

    }

    public void Save()
    {
        /// this does not wok and causes game breaking errors, should have been made in json-WC
        //TODO: Add the Crystal manifest to the saves
       SaveSystem.Save();
    }

    public void Load()
    {
        playerKeysPocket.ClearAllKeys();
        powerCrystalManifest.ResetManifest();
        /// this does not wok and causes game breaking errors, should have been made in json-WC
        PlayerData data = SaveSystem.Load();

        if (data == null)
        {
            Debug.LogError("No saves can be loaded.");
            return;
        }
        powerCrystalManifest.DestroyList=data.GetCrystalManifest();
         KeySystem newKey=new KeySystem();
        foreach (DamageEngine.ElementType elementType in powerCrystalManifest.DestroyList)
        {
            switch (elementType)
            {
                case DamageEngine.ElementType.fire:
                    newKey = MasterKeyPocket.AccessKeys[0];
                    break;
                    case DamageEngine.ElementType.Water:
                    newKey = MasterKeyPocket.AccessKeys[1];
                    break;
                    case DamageEngine.ElementType.Earth:
                    newKey = MasterKeyPocket.AccessKeys[3];
                    break;
                default:
                    break;
            }
            if (!playerKeysPocket.AccessKeys.Contains(newKey))
            {
                playerKeysPocket.AccessKeys.Add(newKey);
            }
        }


        //playerKeysPocket.AccessKeys = data.GetPlayerKeys();
        //scoreKeeper=data.GetScoreKeeper();

        gameManager.instance.stopSpawning = true;
        SceneManager.LoadScene(data.GetSceneIndex());
        gameManager.instance.stateUnPaused();
    }
    public void mainMenu(int sceneIndex)
    {
        LoadAsync(sceneIndex);
        gameManager.instance.menuActive.SetActive(gameManager.instance.isPaused);
        gameManager.instance.menuActive = null;

    }
    public void quit()
    {

#if UNITY_EDITOR
        playerKeysPocket.ClearAllKeys();
        powerCrystalManifest.ResetManifest();
        scoreKeeper.ResetScoreKeeper();
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
   
    public void PlayGame(int sceneIndex)
    {
        LoadAsync(sceneIndex);
    }
    public void NewGame(int sceneIndex)
    {
        //CurrentSpells.ClearSpells();
        playerKeysPocket.ClearAllKeys();
        powerCrystalManifest.ResetManifest();
        scoreKeeper.ResetScoreKeeper();
        LoadAsync(sceneIndex);
        //gameManager.instance.PlayerController.PlayerKeys.Clear();

    }
    

public void LoadAsync(int sceneIndex)
    {


        //gameManager.instance.LoadingScreen.SetActive(true);
        
        SceneManager.LoadScene(5);  //index 5 is the loading screen        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        Time.timeScale = 1f;
        float progress = Mathf.Clamp01(operation.progress / 0.9f);
       //gameManager.instance.slider.value = progress;
    }
}
