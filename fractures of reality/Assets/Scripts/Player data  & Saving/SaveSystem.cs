using UnityEngine;
using TMPro;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{

    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string fPath = Application.persistentDataPath + "/save.dat";
        FileStream stream = new FileStream(fPath, FileMode.Create);
        PlayerData data = new PlayerData(gameManager.instance.PlayerController.Pocket.AccessKeys,gameManager.instance.PCrystalManifest,
            SceneManager.GetActiveScene().buildIndex);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData Load()
    {
        string sPath = Application.persistentDataPath + "/save.dat";
        if (File.Exists(sPath))
        {
            Debug.Log("It's ALIVE");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(sPath, FileMode.Open);
            PlayerData pData = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return pData;
        }
        else
        {
            Debug.LogError("Save file not found at: " + sPath);
            if (File.Exists(sPath))
            {
                BinaryFormatter formatter = new BinaryFormatter ();
                FileStream stream = new FileStream (sPath, FileMode.Open);

               PlayerData data = formatter.Deserialize(stream) as PlayerData;
               stream.Close();

               return data;
            }
            else
            {
                Debug.LogError("Save file not found");
                return null;
            }
        }
    }
}
