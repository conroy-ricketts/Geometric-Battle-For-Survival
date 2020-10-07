using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GM2Manager : MonoBehaviour
{
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

    //Game over
    [SerializeField] private Button goToMainMenuFromGameOverButton;

    //Enemy
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemySpawn1;
    [SerializeField] private Transform enemySpawn2;
    public static float enemySpeed;
    public static bool activeEnemyPresent;
    
    //Score
    [SerializeField] private Text scoreText;
    public static int score;

    //Game object pool variables
    private int gameObjectPoolAmount = 50;
    public static List<GameObject> pooledEnemies = new List<GameObject>();


    void Start ()
    {
        HandleGameObjectPools();

        HandleAds();

        HandleSound();

        HandleUIButtons();

        //Make sure the screen doesn't turn off while the player is playing the game
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //As soon as the scene starts, there is no acitve enemy present, the enemy's speed is 20, and the score is 0
        activeEnemyPresent = false;
        enemySpeed = 20;
        score = 0;
    }	

	void Update ()
    {
        //If ther is no active enemy
        if(activeEnemyPresent == false)
        {
            //Increase the enemy speed
            enemySpeed += 5;

            //Spawn an enemy
            SpawnEnemy();

            //Set the variable so we know that an enemy is active
            activeEnemyPresent = true;
        }

        //Display the score
        scoreText.GetComponent<Text>().text = "Score: " + score;

        //If the score is higher than the high score, then set the high score to that score
        if (score > PlayerPrefs.GetInt("GM2HighScore"))
        {
            PlayerPrefs.SetInt("GM2HighScore", score);
        }
    }



    private void HandleGameObjectPools()
    {
        //For the amount of game objects in the game objects pool amount
        for (int i = 0; i < gameObjectPoolAmount; i++)
        {
            //Instantiate an enemy, immediately deactivate it, and add it to the list of pooled enemies
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            pooledEnemies.Add(enemy);
        }
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

        //Load the main menu when the "go to main menu button" is clicked in the game over canvas
        goToMainMenuFromGameOverButton.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("Main Menu"); });

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
        scoreOnPauseMenu.GetComponent<Text>().text = "SCORE: " + score;

        //Display the palyer's current high score
        highScoreOnPauseMenu.GetComponent<Text>().text = "HIGH SCORE: " + PlayerPrefs.GetInt("GM2HighScore");
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

    private void SpawnEnemy()
    {
        //for every enemy in the pooled enemies list
        for (int i = 0; i < pooledEnemies.Count; i++)
        {
            //Check if the enemy is not null
            if (pooledEnemies[i] != null)
            {
                //Check if the enemy is inactive 
                if (!pooledEnemies[i].activeInHierarchy)
                {
                    //Make a variable to store a spawn point
                    Transform spawn = null;

                    //Pick a random number, 0 or 1
                    float random = Random.Range(0, 2);

                    //if the random number is 0
                    if (random == 0)
                    {
                        //Set spawn point to 1st enemy spawn
                        spawn = enemySpawn1;
                    }

                    //if the random number is 1
                    if (random == 1)
                    {
                        //Set spawn point to 2nd enemy spawn
                        spawn = enemySpawn2;
                    }

                    //Set the enemy's position and to the spawn's position
                    pooledEnemies[i].transform.position = spawn.position;

                    //Enable the unit
                    pooledEnemies[i].SetActive(true);

                    //Break out of this for loop so we don't end up activating every single unit that is inactive in the enemy strong units pool
                    break;
                }
            }
        }
    }
}
