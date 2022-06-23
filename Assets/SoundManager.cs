using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public Sounds[] Sound;

    void Start()
    {
        foreach (Sounds s in Sound)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;

            s.buttonColor = s.pianoButton.colors;

        }
    }

}
