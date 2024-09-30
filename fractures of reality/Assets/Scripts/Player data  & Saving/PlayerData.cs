using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //List<KeySystem> PlayerKeys;
    List<DamageEngine.ElementType> CrystalManifest;
    int SceneIndex;
    //ScoreKeeper ScoreKeeper;
    public PlayerData (List<KeySystem> playerKeys, PowerCrystalManifest crystals, int sceneIndex)
    {
        //PlayerKeys = playerKeys;
        CrystalManifest = crystals.DestroyList;
        SceneIndex=sceneIndex;
    }
    public int GetSceneIndex() { return SceneIndex; }
    //public List<KeySystem> GetPlayerKeys() { return PlayerKeys; }
    public List<DamageEngine.ElementType> GetCrystalManifest() { return CrystalManifest; }
    //public ScoreKeeper GetScoreKeeper() { return ScoreKeeper; }
}
