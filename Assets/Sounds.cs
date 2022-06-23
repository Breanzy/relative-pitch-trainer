using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Sounds
{
    public string name;
    public AudioClip Clip;
    public Button pianoButton;

    [HideInInspector]
    public AudioSource Source;
    [HideInInspector]
    public ColorBlock buttonColor;
}
