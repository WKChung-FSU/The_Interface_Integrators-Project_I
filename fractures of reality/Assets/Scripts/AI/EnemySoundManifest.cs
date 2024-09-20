using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemySoundManifest : ScriptableObject
{
    [SerializeField] List<SoundEffects> idleSounds;
    [SerializeField] List<SoundEffects> hurtSounds;
    [SerializeField] List<SoundEffects> deathSounds;

    public SoundEffects GetRandomIdleSound()
    {
        return idleSounds[Random.Range(0, idleSounds.Count - 1)];
    }

    public SoundEffects GetRandomHurtSound()
    {
        return hurtSounds[Random.Range(0, hurtSounds.Count - 1)];
    }
    public SoundEffects GetRandomDeathSound()
    {
        return deathSounds[Random.Range(0, deathSounds.Count - 1)];
    }
}
