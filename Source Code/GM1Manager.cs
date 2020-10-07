using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//GM1 stands for Game Mode 1, which means that this script is specific to game mode 1


public class GM1Manager : MonoBehaviour {

    //Music
    [SerializeField] private Button musicOnButton;
    [SerializeField] private Button musicOffButton;
    [SerializeField] private AudioSource music;

    //Sound effects
    [SerializeField] private Button soundEffectsOnButton;
    [SerializeField] private Button soundEffectsOffButton;

    //Pause menu 
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button closePauseMenuButton;
    [SerializeField] private Button goToMainMenuFromPauseMenuButton;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private Text scoreOnPauseMenu;
    [SerializeField] private Text highScoreOnPauseMenu;


    void Start ()
    {
        HandleAds();

        HandleSound();

        HandleUIButtons();

        //Make sure the screen doesn't turn off while the player is playing the game
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

	void Update ()
    {

    }


    private void HandleAds()
    {
        //Show an Ad after the player has loaded the game scene a few times
        if (PlayerPrefs.HasKey("ShowAd") == false)
        {
            PlayerPrefs.SetInt("ShowAd", 1);
        }
        else if (PlayerPrefs.GetInt("ShowAd") == 1)
        {
            PlayerPrefs.SetInt("ShowAd", 2);
        }
        else if (PlayerPrefs.GetInt("ShowAd") == 2)
        {
            PlayerPrefs.SetInt("ShowAd", 3);
        }
        else if (PlayerPrefs.GetInt("ShowAd") == 3)
        {
            //Show the ad
            if (Advertisement.IsReady())
            {
                Advertisement.Show();
            }

            PlayerPrefs.SetInt("ShowAd", 1);
        }

    }

    private void HandleSound()
    {
        //If the music was unmuted in the main menu, then keep it unmuted in the main scene 
        if (PlayerPrefs.GetString("Music") == "unmuted")
        {
            music.GetComponent<AudioSource>().mute = false;
        }

        //If the music was muted in the main menu, then keep it muted in the main scene 
        if (PlayerPrefs.GetString("Music") == "muted")
        {
            music.GetComponent<AudioSource>().mute = true;
        }

        //Unmute the music if the "music on" button is clicked; and set a playerprefs variable so that we know the music is on
        musicOnButton.GetComponent<Button>().onClick.AddListener(delegate { music.GetComponent<AudioSource>().mute = false; PlayerPrefs.SetString("Music", "unmuted"); });

        //Mute the music if the "music off" button is clicked; and set a playerprefs variable so that we know the music is off
        musicOffButton.GetComponent<Button>().onClick.AddListener(delegate { music.GetComponent<AudioSource>().mute = true; PlayerPrefs.SetString("Music", "muted"); });



        //If the sound effects were muted in the main menu then keep them muted in the main scene
        if (PlayerPrefs.GetString("SoundEffects") == "muted")
        {
            MuteSoundEffects();
        }

        //If the sound effects were unmuted in the main menu then keep them unmuted in the main scene
        if (PlayerPrefs.GetString("SoundEffects") == "unmuted")
        {
            UnmuteSoundEffects();
        }

        //Mute all sound effects if the "sound effects off" button is clicked
        soundEffectsOffButton.GetComponent<Button>().onClick.AddListener(MuteSoundEffects);

        //Unmute all sound effects if the "sound effects on" button is clicked
        soundEffectsOnButton.GetComponent<Button>().onClick.AddListener(UnmuteSoundEffects);
    }

    private void HandleUIButtons()
    {
        //Activate the pause menu when the pause button is clicked
        pauseButton.GetComponent<Button>().onClick.AddListener(PauseMenu);

        //Load the main menu if the go to main menu from pause menu button is clicked
        goToMainMenuFromPauseMenuButton.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("Main Menu"); });

        //Deactivate the pause menu and unfreeze time when the close pause menu button is clicked
        closePauseMenuButton.GetComponent<Button>().onClick.AddListener(delegate { pauseMenuCanvas.SetActive(false); Time.timeScale = 1; });
    }

    private void PauseMenu()
    {
        //Activate the pause menu
        pauseMenuCanvas.SetActive(true);

        //Freeze time so stuff doesn't happen in the background
        pauseMenuCanvas.SetActive(true); Time.timeScale = 0;

        //Display the player's score
        scoreOnPauseMenu.GetComponent<Text>().text = "SCORE: " + GM1PlayerController.score;

        //Display the palyer's current high score
        highScoreOnPauseMenu.GetComponent<Text>().text = "HIGH SCORE: " + PlayerPrefs.GetInt("GM1HighScore");
    }

    private void MuteSoundEffects()
    {
        //Make a player prefs variable
        PlayerPrefs.SetString("SoundEffects", "muted");
    }

    private void UnmuteSoundEffects()
    {
        //Make a player prefs variable
        PlayerPrefs.SetString("SoundEffects", "unmuted");
    }
}
