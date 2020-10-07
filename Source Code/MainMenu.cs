using GooglePlayGames;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    //Play
    [SerializeField] private Button playButton;

    //High scores
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text GM1HighScoreText;
    [SerializeField] private Text GM2HighScoreText;

    //Music
    [SerializeField] private Button musicOnButton;
    [SerializeField] private Button musicOffButton;
    [SerializeField] private AudioSource music;

    //Sound effects
    [SerializeField] private Button soundEffectsOnButton;
    [SerializeField] private Button soundEffectsOffButton;

    //Reset high score
    [SerializeField] private Button resetHSButton;

    //Achievements
    [SerializeField] private Button achievementsButton;

    //Leaderboard
    [SerializeField] private Button leaderboardButton;

    //Game modes
    [SerializeField] private Button gameModesButton;
    [SerializeField] private Button goToMainMenuFromGameModesButton;
    [SerializeField] private GameObject gameModesCanvas;

    //Game mode 1
    [SerializeField] private GameObject GM1HelpMenuCanvas;
    [SerializeField] private Button GM1PlayButton;
    [SerializeField] private Button GM1HelpButton;
    [SerializeField] private Button GoBackFromGM1HelpMenuButton;

    //Game mode 2
    [SerializeField] private GameObject GM2HelpMenuCanvas;
    [SerializeField] private Button GM2PlayButton;
    [SerializeField] private Button GM2HelpButton;
    [SerializeField] private Button GoBackFromGM2HelpMenuButton;

    //Settings 
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button goToMainMenuFromSettingsButton;
    [SerializeField] private GameObject settingsCanvas;

    //Help
    [SerializeField] private Button helpButton;
    [SerializeField] private Button goToMainMenuFromHelpButton;
    [SerializeField] private GameObject helpCanvas;

    //Loading Screen
    [SerializeField] private GameObject loadingScreenCanvas;
    [SerializeField] private Slider loadingSlider;

    //Tutorial
    [SerializeField] private Button tutorialButton;



    void Start ()
    {
        //If time is frozen, then unfreeze it
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        HandleSound();

        HandleGooglePlayServices();

        HandleUIButtons();

        //Make sure the screen doesn't turn off while the player is playing the game
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //Display the high scores
        highScoreText.GetComponent<Text>().text = "HIGH SCORE: " + PlayerPrefs.GetInt("HighScore");
        GM1HighScoreText.GetComponent<Text>().text = "HIGH SCORE: " + PlayerPrefs.GetInt("GM1HighScore");
        GM2HighScoreText.GetComponent<Text>().text = "HIGH SCORE: " + PlayerPrefs.GetInt("GM2HighScore");
    }


	void Update ()
    {		
        
	}

    private void HandleSound()
    {
        //If the music was unmuted in the main scene, then keep it unmuted in the main menu 
        if (PlayerPrefs.GetString("Music") == "unmuted")
        {
            music.GetComponent<AudioSource>().mute = false;
        }

        //If the music was muted in the main scene, then keep it muted in the main menu 
        if (PlayerPrefs.GetString("Music") == "muted")
        {
            music.GetComponent<AudioSource>().mute = true;
        }

        //Unmute the music if the "music on" button is clicked; and set a playerprefs variable so that we know the music is on
        musicOnButton.GetComponent<Button>().onClick.AddListener(delegate { music.GetComponent<AudioSource>().mute = false; PlayerPrefs.SetString("Music", "unmuted"); });

        //Mute the music if the "music off" button is clicked; and set a playerprefs variable so that we know the music is off
        musicOffButton.GetComponent<Button>().onClick.AddListener(delegate { music.GetComponent<AudioSource>().mute = true; PlayerPrefs.SetString("Music", "muted"); });

        //Mute all sound effects if the "sound effects off" button is clicked; we are setting a player prefs variable so we can check to see if the user muted sound effects in the main scene
        soundEffectsOffButton.GetComponent<Button>().onClick.AddListener(delegate { PlayerPrefs.SetString("SoundEffects", "muted"); });

        //Unmute all sound effects if the "sound effects on" button is clicked; we are setting a player prefs variable so we can check to see if the user unmuted sound effects in the main scene
        soundEffectsOnButton.GetComponent<Button>().onClick.AddListener(delegate { PlayerPrefs.SetString("SoundEffects", "unmuted"); });
    }

    private void HandleGooglePlayServices()
    {
        //Activate google play games services
        PlayGamesPlatform.Activate();

        //If the user's google play account is not authenticated, authenticate it
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(success => { });
        }

        //Add the player's score to the main score leaderboard (I got the leaderboard id from the google play developer console)
        if (EnemyWall.score > 0)
        {
            Social.ReportScore(EnemyWall.score, "CgkIq9Ks--gMEAIQCQ", success => { });
        }

        //Add the player's score to the circle of death score leaderboard
        if(GM1PlayerController.score > 0)
        {
            Social.ReportScore(GM1PlayerController.score, "CgkIq9Ks--gMEAIQCg", success => { });
        }

        //Add the player's score to the dodge and run score leaderboard
        if (GM2Manager.score > 0)
        {
            Social.ReportScore(GM2Manager.score, "CgkIq9Ks--gMEAIQCw", success => { });
        }
    }

    private void HandleUIButtons()
    {
        //Load the main scene if play button is clicked
        playButton.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(LoadScene("Main")); });

        //Show achievements on button click
        achievementsButton.GetComponent<Button>().onClick.AddListener(ShowAchievements);

        //Show leaderboard on button click
        leaderboardButton.GetComponent<Button>().onClick.AddListener(ShowLeaderboard);

        //Activate game modes menu on button click
        gameModesButton.GetComponent<Button>().onClick.AddListener(delegate { gameModesCanvas.SetActive(true); });

        //Deactivate game modes menu on button click
        goToMainMenuFromGameModesButton.GetComponent<Button>().onClick.AddListener(delegate { gameModesCanvas.SetActive(false); });

        //Activate settings menu on button click
        settingsButton.GetComponent<Button>().onClick.AddListener(delegate { settingsCanvas.SetActive(true); });

        //Deactivate settings menu on button click
        goToMainMenuFromSettingsButton.GetComponent<Button>().onClick.AddListener(delegate { settingsCanvas.SetActive(false); });

        //Activate help menu on button click
        helpButton.GetComponent<Button>().onClick.AddListener(delegate { helpCanvas.SetActive(true); });

        //Deactivate help menu on button click
        goToMainMenuFromHelpButton.GetComponent<Button>().onClick.AddListener(delegate { helpCanvas.SetActive(false); });

        //When the tutorial button is clicked, load the main game but set a boolean variable so that we know to play the tutorial
        tutorialButton.GetComponent<Button>().onClick.AddListener(delegate { GameManager.playTutorial = true;  StartCoroutine(LoadScene("Main")); });



        //Load Game Mode 1 on button click
        GM1PlayButton.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine( LoadScene("Game Mode 1") ); });

        //Activate Game Mode 1 help menu on button click
        GM1HelpButton.GetComponent<Button>().onClick.AddListener(delegate { GM1HelpMenuCanvas.SetActive(true); });

        //Deactivate game mode 1 help menu on button click
        GoBackFromGM1HelpMenuButton.GetComponent<Button>().onClick.AddListener(delegate { GM1HelpMenuCanvas.SetActive(false); });



        //Load Game Mode 2 on button click
        GM2PlayButton.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(LoadScene("Game Mode 2")); });

        //Activate Game Mode 2 help menu on button click
        GM2HelpButton.GetComponent<Button>().onClick.AddListener(delegate { GM2HelpMenuCanvas.SetActive(true); });

        //Deactivate game mode 2 help menu on button click
        GoBackFromGM2HelpMenuButton.GetComponent<Button>().onClick.AddListener(delegate { GM2HelpMenuCanvas.SetActive(false); });



        //ONLY FOR DEBUGGING PURPOSES - Reset the high score when button is clicked
        resetHSButton.GetComponent<Button>().onClick.AddListener(delegate { PlayerPrefs.DeleteKey("HighScore"); PlayerPrefs.DeleteKey("GM1HighScore"); PlayerPrefs.DeleteKey("GM2HighScore"); });
    }

    private void ShowAchievements()
    {
        //if the user has been authenticated
        if (Social.localUser.authenticated)
        {
            //Show achievements
            Social.ShowAchievementsUI();
        }        
    }

    private static void ShowLeaderboard()
    {
        //if the user has been authenticated
        if (Social.localUser.authenticated)
        {
            //Show  leaderboard
            Social.ShowLeaderboardUI();
        }        
    }

    IEnumerator LoadScene(string sceneToBeLoaded)
    {
        //Load the "scene to be loaded" in the background and store it in a variable
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToBeLoaded);

        //Activate the loading screen canvas
        loadingScreenCanvas.SetActive(true);

        //While the operation is not done loading
        while (!operation.isDone)
        {
            //Update the progress
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;

            //Wait until the next frame before continuing
            yield return null;
        }
    }
}
