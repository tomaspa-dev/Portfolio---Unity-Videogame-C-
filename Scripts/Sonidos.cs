using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sonidos
{    
    public string nombreDelAudio;
    public AudioClip pistaDeSonido;
    [HideInInspector]
    public AudioSource source;    
}
