using UnityEngine;
using UnityEngine.UI;

public class Press : MonoBehaviour
{
    public int arrayNumber;
    public SoundManager sounds;
    public GameManager GameManager;
    public Button pianoButton;

    public void ButtonPress()
    {
        sounds.Sound[arrayNumber].Source.Play();

        if(GameManager.isPlaying)
        {
            if (GameManager.number.Contains(arrayNumber))
                GameManager.CheckAnswer(pianoButton, true, arrayNumber);

            else 
                GameManager.CheckAnswer(pianoButton, false, arrayNumber);
        }


    }

}
