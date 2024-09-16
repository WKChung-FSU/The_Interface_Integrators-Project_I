using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class KeyPocket : ScriptableObject
{
    [SerializeField] List<KeySystem> PlayerKeys;


    public List<KeySystem> AccessKeys
    {

        get { return PlayerKeys; }
        set { PlayerKeys = value; }
    }
    public void ClearAllKeys()
    {
    PlayerKeys.Clear(); 
    }

}
