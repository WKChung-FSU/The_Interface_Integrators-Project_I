using UnityEngine;
using TMPro;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class SaveSystem : MonoBehaviour
{

    public static void Save(GameObject player)
    {
        PlayerWeapon ammo = player.GetComponent<PlayerWeapon>();
        DestructibleHealthCore playercore = player.GetComponent<DestructibleHealthCore>();

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.dat";
        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(ammo, playercore);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData Load()
    {
        string path = Application.persistentDataPath + "/save.dat";
        if (File.Exists(path))
        {
            Debug.Log("Is possible work");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode .Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found at: " + path);

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter ();
                FileStream stream = new FileStream (path, FileMode .Open);

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
