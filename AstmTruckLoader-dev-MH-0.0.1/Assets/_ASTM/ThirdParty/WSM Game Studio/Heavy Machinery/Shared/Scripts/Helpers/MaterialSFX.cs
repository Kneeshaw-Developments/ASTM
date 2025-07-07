using System;
using UnityEngine;

namespace WSMGameStudio.Audio
{
    [Serializable]
    public struct MaterialSFX
    {
        [SerializeField] public PhysicsMaterial physicMaterial;
        [SerializeField] public AudioSource audioSurce;
    }
}
