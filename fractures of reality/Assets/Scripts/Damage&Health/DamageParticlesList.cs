using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DamageParticlesList : ScriptableObject
{
    public GameObject damageParticle;
    public GameObject healParticle;
    public GameObject blockParticle;

    [Header("StatusEffects")]
    public GameObject burnParticle;
    public GameObject wetParticle;
}
