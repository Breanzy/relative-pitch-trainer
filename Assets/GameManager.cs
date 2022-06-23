using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;


/*TODO: 
 * 
 * add more shit in finished panel (show final scores)
 * add record run history
 * add option to manually press next after an attempt to answer, OR to modify the time it takes to automatically move on to another text
 * make replay mode limited
 * add multiple other instruments
 * make default settings customizable
 * add a profile system account bullshit (its for training too you know)
 * 
 * KNOWN BUGS:
 * 
 */

public class GameManager : MonoBehaviour
{

    //Sounds n shit
    public SoundManager Sounds;
    public Button play, replayNote, restart;

    //RNG variables
    public List<int> note, number;

    //Game Settings
    public Toggle middle, low, high;
    public bool isPlaying;
    public int gameModeSetting;
    public int buttonsPerSession;
    public int playAttempts;
    public int progressCounter, tempCounter;
    public int timerSeconds, timerMinutes, timerMilliSeconds;

    //UI shits
    public TextMeshProUGUI scoreCount, errorCount, totalScore, timerText;
    public GameObject finishedPanel, customSliderObject;
    public Slider customSlider;
    public TMPro.TMP_Dropdown gameModeDropdown, notesPerSessionDropdown;
    public List<int> coloredButtonsList;
    public int _scoreCount, _errorCount, _totalScore;

    private void Start()
    {
        //Initializing basic default settings

        //Initializing game mode settings to Classic
        gameModeSetting = 0;

        //Initializing Buttons-per-session settings to 1 note per session
        notesPerSessionDropdown.value = 1;

        //Initializing Note variation with Middle Key activated
        high.isOn = true;
        middle.isOn = true;
        low.isOn = true;

        ResetGame();
    }

    //Reset Button
    public void ResetGame()
    {

        //Initalizing UI
        _scoreCount = 0;
        _errorCount = 0;

        scoreCount.text = _scoreCount.ToString();
        errorCount.text = _errorCount.ToString();

        scoreCount.faceColor = Color.gray;
        errorCount.faceColor = Color.gray;
        totalScore.faceColor = Color.gray;

        timerMinutes = 0;
        timerSeconds = 0;
        timerText.text = "0:00";
        timerText.faceColor = Color.gray;
        StopCoroutine("Timer");

        //enables and disables buttons in appropriate conditions (char)
        play.interactable = true;
        restart.interactable = false;
        replayNote.interactable = false;

        middle.interactable = true;
        low.interactable = true;
        high.interactable = true;
        CheckButtonsEnabled();

        gameModeDropdown.interactable = true;
        notesPerSessionDropdown.interactable = true;
        if (gameModeSetting == 2) //for Custom Mode condition
            customSlider.interactable = true;

        //Initializing game mode settings to default setting
        changeGameMode(gameModeSetting);

        //Initializing notes-per-session
        changeButtonPerSession(buttonsPerSession - 1);

        //Initializing general game settings in case of junk values from previous games
        isPlaying = false;
        progressCounter = 0;
        playAttempts = 0;
        tempCounter = 0;
        number.Clear();
        note.Clear();
        returnNoteColor();

    }

    //Play Button
    public void playButton()
    {

        //recolors texts to black
        scoreCount.faceColor = Color.black;
        errorCount.faceColor = Color.black;
        totalScore.faceColor = Color.black;

        //enables and disables buttons in appropriate conditions (char)
        replayNote.interactable = true;
        play.interactable = false;
        restart.interactable = true;

        middle.interactable = false;
        low.interactable = false;
        high.interactable = false;

        gameModeDropdown.interactable = false;
        notesPerSessionDropdown.interactable = false;
        if (gameModeSetting == 2)
            customSlider.interactable = false;

        //statements to add the notes in play, depending on the specifications of the player
        if (low.isOn)
            for (int a = 24; a <= 35; a++)
                note.Add(a);

        if (middle.isOn)
            for (int a = 0; a <= 11; a++)
                note.Add(a);

        if (high.isOn)
            for (int a = 12; a <= 23; a++)
                note.Add(a);

        //creates random generated elements and starts the loop of operations.
        StartRNG();
    }

    //Changes how many notes played per session
    public void changeButtonPerSession(int value)
    {
        buttonsPerSession = value + 1;
    }

    //Game Mode Toggle Button
    public void changeGameMode(int value)
    {
        gameModeSetting = value;

        //reset total score back to zero for a fresh start
        _totalScore = 0;

        //This is just to disable Custom Mode slider when inactive
        customSliderObject.SetActive(false);

        //Classic Mode
        if (gameModeSetting == 0)
        {
            if (low.isOn) _totalScore += 12;
            if (middle.isOn) _totalScore += 12;
            if (high.isOn) _totalScore += 12;
            totalScore.text = _totalScore.ToString();
        }

        //Unlimited Mode
        else if (gameModeSetting == 1)
        {
            //The Total Score is -1 to essentially breaking the total score, making it useless, as it is in this game mode
            _totalScore = -1;
            totalScore.text = "0";
        }

        //Custom Size Mode
        else if (gameModeSetting == 2)
        {
            //enables slider
            customSliderObject.SetActive(true);

            //updates min value of custom slider guaranteeing that a game will have enough buttons to finish at least one session
            customSlider.minValue = buttonsPerSession;

            //updates total score
            _totalScore = (int)customSlider.value;
            totalScore.text = _totalScore.ToString();
        }

    }

    public void customSliderNumber(float value)
    {
        _totalScore = (int)value;
        totalScore.text = _totalScore.ToString();
    }

    //Toggle Difficulty buttons
    public void EnableDifficultyButtons(bool addNumbers)
    {
        CheckButtonsEnabled();

        //Updates total score (Exclusive for Classic Game Mode)
        if(gameModeSetting == 0)
        {
            if (addNumbers) _totalScore += 12;
            else _totalScore -= 12;

            totalScore.text = _totalScore.ToString();
        }
    }

    //Checks if disabling a toggle is allowed, and disables its ability to be disabled if its not. Otherwise, you can disable all toggles, then there's nothing left lmao
    void CheckButtonsEnabled()
    {

        if (middle.isOn && !low.isOn && !high.isOn)
            middle.interactable = false;
        else if (!middle.isOn && low.isOn && !high.isOn)
            low.interactable = false;
        else if (!middle.isOn && !low.isOn && high.isOn)
            high.interactable = false;

        else
        {
            high.interactable = true;
            middle.interactable = true;
            low.interactable = true;
        }

    }

    //Back Button in Finished Panel
    public void finishedGoBackButton()
    {
        finishedPanel.SetActive(false);
        ResetGame();
    }

    //RNG for selecting the notes
    public void StartRNG()
    {
        //Boolean to assure that we are, indeed, playing.
        isPlaying = true;

        //starts or resumes back timer and turns its color to black, indicating that it is active
        StartCoroutine("Timer");
        timerText.faceColor = Color.black;

        //RNG for what notes to play and depending how many the player specified.
        while (number.Count != buttonsPerSession && progressCounter != _totalScore)
        {
            //The RNG part
            int tempNumber = Random.Range(0, note.Count - 1);
            number.Add(note[tempNumber]);

            //Temporarily removes an element so it won't repeat in the next RNG
            note.RemoveAt(tempNumber);

            //progressCounter a multipurpose variable for checking if the score matches the total score, and ends the game if true,
            //and it also restricts "number" from adding any more integers when "note" is already empty
            progressCounter++;

            //tempCounter to count how many times this loop iterated so player can only attempt sessions as far as this counter goes
            tempCounter++;

        }

        //adds everything that's inside "number" to "coloredButtonsList" for future reference of the indexes of buttons that are colored green
        foreach (int numberIndex in number)
            coloredButtonsList.Add(numberIndex);

        //plays notes
        PlaySound();
    }

    //plays notes (also functions as a Replay Button)
    public void PlaySound()
    {
        foreach (int noteNumber in number)
            Sounds.Sound[noteNumber].Source.Play();
    }

    //Checking Answers
    public void CheckAnswer(Button button, bool answer, int noteNumber)
    {

        //disables the button
        button.interactable = false;

        //counter for how many session attempts are done. This function loops until the counter matches how many contents are inside "numbers"
        playAttempts++;

        ColorBlock color;

        //right answer
        if (answer)
        {

            //color of button turns to green
            color = button.colors;
            color.disabledColor = new Color(0.504717f, 1, 0.6801843f);
            button.colors = color;

            //removes this button index as it is already correct and doesn't need to repeat again. (Exclusive to Classic Mode)
            if(gameModeSetting == 0)
            number.Remove(noteNumber);

            //updates score +1
            _scoreCount++;
            scoreCount.text = _scoreCount.ToString();

        }

        //wrong answer
        else if(!answer)
        {

            //color of button turns to red
            color = button.colors;
            color.disabledColor = new Color(1, 0.4481132f, 0.4481132f);
            button.colors = color;

            //adds this button index to "coloredButtonsList" as future reference for buttons that are colored red
            coloredButtonsList.Add(noteNumber);

            //updates error count +1
            _errorCount++;
            errorCount.text = _errorCount.ToString();

            //subtracts progress as this score does not count as correct
            progressCounter--;

        }

        //Increase in total play count (for Unlimited Game Mode)
        if (gameModeSetting == 1)
            totalScore.text = (_scoreCount + _errorCount).ToString();

        //checks if a single play session is done and preparation codes to reset for a new session
        if (playAttempts == tempCounter)
        {

            //to officially say, that indeed, the session stopped.
            isPlaying = false;

            //pauses timer while waiting for ResetSession and turns it to gray, indicating that it is inactive
            StopCoroutine("Timer");
            timerText.faceColor = Color.gray;

            //disables replay and restart buttons to prevent game-breaking bugs
            replayNote.interactable = false;
            restart.interactable = false;

            //resets play attempts for a new session
            playAttempts = 0;

            //resets tempCounter
            tempCounter = 0;

            //disables and colors the remaining correct buttons to green, then puts all elements in "number" back in "notes" for another re-run cycle in the next session
            foreach (int numbers in number)
            {
                Sounds.Sound[numbers].pianoButton.interactable = false;

                color = Sounds.Sound[numbers].pianoButton.colors;
                color.disabledColor = new Color(0.504717f, 1, 0.6801843f);
                Sounds.Sound[numbers].pianoButton.colors = color;

                note.Add(numbers);
            }

            //clears the now-junk value in numbers
            number.Clear();

            //coroutine for automatically moving to a new session
            StartCoroutine(ResetSession());
        }

    }

    IEnumerator ResetSession()
    {

        //timer for when the next session automatically starts
        yield return new WaitForSeconds(1.5f);

        //re-enables disabled buttons
        replayNote.interactable = true;
        restart.interactable = true;

        returnNoteColor();

        //returns buttons that changed color back to their default color, then clears the list of colored buttons
        coloredButtonsList.Clear();

        //check if game is already finished
        if (progressCounter == _totalScore)
            //finished panel pops up
            finishedPanel.SetActive(true);

        else
            StartRNG();
    }

    //re-enables and returns colored piano keys back to normal color
    public void returnNoteColor()
    {
        foreach (int numberIndex in coloredButtonsList)
        {
            Sounds.Sound[numberIndex].pianoButton.interactable = true;
            Sounds.Sound[numberIndex].pianoButton.colors = Sounds.Sound[numberIndex].buttonColor;
        }
    }

    //Timer for play time
    IEnumerator Timer()
    {

        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            timerMilliSeconds += 100;

            if(timerMilliSeconds == 1000)
            {
                timerMilliSeconds = 0;
                timerSeconds++;
            }

            else if(timerSeconds == 60)
            {
                timerSeconds = 0;
                timerMinutes++;
            }

            timerText.text = timerMinutes.ToString() + ":" + (timerSeconds < 10 ? "0" : "") + timerSeconds.ToString();

        }
    }
}
