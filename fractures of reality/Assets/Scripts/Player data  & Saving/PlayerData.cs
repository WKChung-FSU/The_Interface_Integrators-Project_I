using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    KeyPocket PlayerKeys;
    PowerCrystalManifest CrystalManifest;
    int SceneIndex;
    ScoreKeeper ScoreKeeper;
    public PlayerData ( KeyPocket playerKeys, PowerCrystalManifest crystals,ScoreKeeper score, int sceneIndex)
    {
        PlayerKeys = playerKeys;
        CrystalManifest = crystals;
        SceneIndex=sceneIndex;
        ScoreKeeper = score;
    }
    public int GetSceneIndex() { return SceneIndex; }
    public KeyPocket GetPlayerKeys() { return PlayerKeys; }
    public PowerCrystalManifest GetCrystalManifest() { return CrystalManifest; }
    public ScoreKeeper GetScoreKeeper() { return ScoreKeeper; }
}
