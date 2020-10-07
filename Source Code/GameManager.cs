using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    /*
     *  BEFORE YOU PUBLISH THE GAME MAKE SURE YOU DO THESE THINGS:
     * 
     *  -Reset the high score 
     *  -Deactivate the reset high score button
     *  -Make sure the music is on by default
     *  -Make sure sound effects are on by default
     *  -Make sure ad services are not in test mode
     *  
     */


    //Explosions
    [SerializeField] private GameObject explosionPrefab;

    //Bullets/ lasers (for sound effects and game object pooling)
    [SerializeField] private GameObject playerBulletPrefab;
    [SerializeField] private GameObject enemyBulletPrefab;

    //Music
    [SerializeField] private Button musicOnButton;
    [SerializeField] private Button musicOffButton;
    [SerializeField] private AudioSource music;

    //Sound effects
    [SerializeField] private Button soundEffectsOnButton;
    [SerializeField] private Button soundEffectsOffButton;

    //Pause menu variabels
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button closePauseMenuButton;
    [SerializeField] private Button goToMainMenuFromPauseMenuButton;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private Text scoreOnPauseMenu;
    [SerializeField] private Text highScoreOnPauseMenu;

    //Unit Spawns
    private List<Transform> playerSpawns = new List<Transform>();
    private List<Transform> enemySpawns = new List<Transform>();

    //Money variables
    [SerializeField] private Text moneyText;
    [SerializeField] private float money;

    //Basic unit variables
    [SerializeField] private Button basicUnitFactoryButton;
    [SerializeField] private Text basicUnitButtonTextCost;
    [SerializeField] private Text basicUnitButtonTextAmount;
    [SerializeField] private float basicUnitFactoryRate;
    [SerializeField] private float basicUnitFactoryCost;
    [SerializeField] private GameObject playerBasicUnitPrefab;
    [SerializeField] private GameObject enemyBasicUnitPrefab;
    private int numberOfBasicFactories = 0;

    //Strong unit variables
    [SerializeField] private Button strongUnitFactoryButton;
    [SerializeField] private Text strongUnitButtonTextCost;
    [SerializeField] private Text strongUnitButtonTextAmount;
    [SerializeField] private float strongUnitFactoryRate;
    [SerializeField] private float strongUnitFactoryCost;
    [SerializeField] private GameObject playerStrongUnitPrefab;
    [SerializeField] private GameObject enemyStrongUnitPrefab;
    private int numberOfStrongFactories = 0;

    //Fast unit variables
    [SerializeField] private Button fastUnitFactoryButton;
    [SerializeField] private Text fastUnitButtonTextCost;
    [SerializeField] private Text fastUnitButtonTextAmount;
    [SerializeField] private float fastUnitFactoryRate;
    [SerializeField] private float fastUnitFactoryCost;
    [SerializeField] private GameObject playerFastUnitPrefab;
    [SerializeField] private GameObject enemyFastUnitPrefab;
    private int numberOfFastFactories = 0;

    //Ultimate unit variables
    [SerializeField] private Button ultimateUnitFactoryButton;
    [SerializeField] private Text ultimateUnitButtonTextCost;
    [SerializeField] private Text ultimateUnitButtonTextAmount;
    [SerializeField] private float ultimateUnitFactoryRate;
    [SerializeField] private float ultimateUnitFactoryCost;
    [SerializeField] private GameObject playerUltimateUnitPrefab;
    [SerializeField] private GameObject enemyUltimateUnitPrefab;
    private int numberOfUltimateFactories = 0;

    //Special abilities variables
    [SerializeField] private Button destroyAllEnemyUnitsButton;
    [SerializeField] private float destroyAllEnemyUnitsCost;
    [SerializeField] private Text destroyAllEnemyUnitsTextCost;

    //Game object pool variables
    private int gameObjectPoolAmount = 500;
    public static List<GameObject> pooledPlayerBullets = new List<GameObject>();
    public static List<GameObject> pooledEnemyBullets = new List<GameObject>();
    public static List<GameObject> pooledPlayerBasicUnits = new List<GameObject>();
    public static List<GameObject> pooledPlayerStrongUnits = new List<GameObject>();
    public static List<GameObject> pooledPlayerFastUnits = new List<GameObject>();
    public static List<GameObject> pooledPlayerUltimateUnits = new List<GameObject>();
    public static List<GameObject> pooledEnemyBasicUnits = new List<GameObject>();
    public static List<GameObject> pooledEnemyStrongUnits = new List<GameObject>();
    public static List<GameObject> pooledEnemyFastUnits = new List<GameObject>();
    public static List<GameObject> pooledEnemyUltimateUnits = new List<GameObject>();

    //Tutorial variables
    public static bool playTutorial;
    [SerializeField] private GameObject tutorialCanvas1;
    [SerializeField] private Button tutorialCanvas1Button;
    [SerializeField] private GameObject tutorialCanvas2;
    [SerializeField] private Button tutorialCanvas2Button;
    [SerializeField] private GameObject tutorialCanvas3;
    [SerializeField] private Button tutorialCanvas3Button;
    [SerializeField] private GameObject tutorialCanvas4;
    [SerializeField] private Button tutorialCanvas4Button;
    [SerializeField] private GameObject tutorialCanvas5;
    [SerializeField] private Button tutorialCanvas5Button;
    [SerializeField] private GameObject tutorialCanvas6;
    [SerializeField] private Button tutorialCanvas6Button;
    [SerializeField] private GameObject tutorialCanvas7;
    [SerializeField] private Button tutorialCanvas7ContinueButton;
    [SerializeField] private Button tutorialCanvas7MainMenuButton;


    void Start ()
    {
        HandleGameObjectPools();

        HandleAds();

        HandleSound();

        HandleUIButtons();

        HandleUnitSpawns();

        //Make sure the screen doesn't turn off while the player is playing the game
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //Add $10 to the player's money every second
        InvokeRepeating("UpdateMoney", 1f, 1f);

        //Every 120 seconds, invoke reapet all of the spawn enemy unit functions
        InvokeRepeating("SpawnEnemyUnits", 0f, 120f);

        //Start the player with 1 basic unit factory
        PlayerBasicUnitFactory();

        //Play the tutorial
        if(playTutorial == true)
        {
            Tutorial();
            playTutorial = false;
        }
    }

    void Update ()
    {
        //Display money
        moneyText.GetComponent<Text>().text = "$" + money;
    }


    private void HandleGameObjectPools()
    {
        //For the amount of game objects in the game objects pool amount
        for (int i = 0; i < gameObjectPoolAmount; i++)
        {
            //Instantiate a player bullet, immediately deactivate it, and add it to the list of pooled player bullets
            GameObject playerBullet = Instantiate(playerBulletPrefab);
            playerBullet.SetActive(false);
            pooledPlayerBullets.Add(playerBullet);

            //Instantiate an enemy bullet, immediately deactivate it, and add it to the list of pooled enemy bullets
            GameObject enemyBullet = Instantiate(enemyBulletPrefab);
            enemyBullet.SetActive(false);
            pooledEnemyBullets.Add(enemyBullet);

            ///////////////////////////////////////////

            //Instantiate a player basic unit, immediately deactivate it, and add it to the list of pooled player basic units
            GameObject PBU = Instantiate(playerBasicUnitPrefab);
            PBU.SetActive(false);
            PBU.tag = "playerUnit";
            pooledPlayerBasicUnits.Add(PBU);

            //Instantiate a player strong unit, immediately deactivate it, and add it to the list of pooled player strong units
            GameObject PSU = Instantiate(playerStrongUnitPrefab);
            PSU.SetActive(false);
            PSU.tag = "playerUnit";
            pooledPlayerStrongUnits.Add(PSU);

            //Instantiate a player fast unit, immediately deactivate it, and add it to the list of pooled player fast units
            GameObject PFU = Instantiate(playerFastUnitPrefab);
            PFU.SetActive(false);
            PFU.tag = "playerUnit";
            pooledPlayerFastUnits.Add(PFU);

            //Instantiate a player ultimate unit, immediately deactivate it, and add it to the list of pooled player ultimate units
            GameObject PUU = Instantiate(playerUltimateUnitPrefab);
            PUU.SetActive(false);
            PUU.tag = "playerUnit";
            pooledPlayerUltimateUnits.Add(PUU);

            //////////////////////////////////////////

            //Instantiate an enemy basic unit, immediately deactivate it, and add it to the list of pooled enemy basic units
            GameObject EBU = Instantiate(enemyBasicUnitPrefab);
            EBU.SetActive(false);
            EBU.tag = "enemyUnit";
            pooledEnemyBasicUnits.Add(EBU);

            //Instantiate an enemy strong unit, immediately deactivate it, and add it to the list of pooled enemy strong units
            GameObject ESU = Instantiate(enemyStrongUnitPrefab);
            ESU.SetActive(false);
            ESU.tag = "enemyUnit";
            pooledEnemyStrongUnits.Add(ESU);

            //Instantiate an enemy fast unit, immediately deactivate it, and add it to the list of pooled enemy fast units
            GameObject EFU = Instantiate(enemyFastUnitPrefab);
            EFU.SetActive(false);
            EFU.tag = "enemyUnit";
            pooledEnemyFastUnits.Add(EFU);

            //Instantiate an enemy ultimate unit, immediately deactivate it, and add it to the list of pooled enemy ultimate units
            GameObject EUU = Instantiate(enemyUltimateUnitPrefab);
            EUU.SetActive(false);
            EUU.tag = "enemyUnit";
            pooledEnemyUltimateUnits.Add(EUU);
        }
    }

    private void HandleAds()
    {
        //Show an Ad after the player has loaded the main game scene a few times
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

        //Buy basic unit factory
        basicUnitFactoryButton.GetComponent<Button>().onClick.AddListener(PlayerBasicUnitFactory);

        //Buy strong unit factory
        strongUnitFactoryButton.GetComponent<Button>().onClick.AddListener(PlayerStrongUnitFactory);

        //Buy fast unit factory
        fastUnitFactoryButton.GetComponent<Button>().onClick.AddListener(PlayerFastUnitFactory);

        //Buy ultimate unit factory
        ultimateUnitFactoryButton.GetComponent<Button>().onClick.AddListener(PlayerUltimateUnitFactory);

        //Buy special ability to destroy all enemy units
        destroyAllEnemyUnitsButton.GetComponent<Button>().onClick.AddListener(DestroyAllEnemyUnits);

        //Display the factory costs on the factory buttons
        basicUnitButtonTextCost.GetComponent<Text>().text = "$" + basicUnitFactoryCost;
        strongUnitButtonTextCost.GetComponent<Text>().text = "$" + strongUnitFactoryCost;
        fastUnitButtonTextCost.GetComponent<Text>().text = "$" + fastUnitFactoryCost;
        ultimateUnitButtonTextCost.GetComponent<Text>().text = "$" + ultimateUnitFactoryCost;

        //Display the default number of factories on the factory buttons
        basicUnitButtonTextAmount.GetComponent<Text>().text = numberOfBasicFactories.ToString();
        strongUnitButtonTextAmount.GetComponent<Text>().text = numberOfStrongFactories.ToString();
        fastUnitButtonTextAmount.GetComponent<Text>().text = numberOfFastFactories.ToString();
        ultimateUnitButtonTextAmount.GetComponent<Text>().text = numberOfUltimateFactories.ToString();

        //Display the special ability costs on the special ability buttons
        destroyAllEnemyUnitsTextCost.GetComponent<Text>().text = "$" + destroyAllEnemyUnitsCost;
    }

    private void HandleUnitSpawns()
    {
        //Add each player spawn to the player spawns list
        for (int i = 1; i < GameObject.Find("Player Spawns").transform.childCount + 1; i++)
        {
            playerSpawns.Add(GameObject.Find("Player Spawn " + i).GetComponent<Transform>());
        }

        //Add each enemy spawn to the enemy spawns list
        for (int i = 1; i < GameObject.Find("Enemy Spawns").transform.childCount + 1; i++)
        {
            enemySpawns.Add(GameObject.Find("Enemy Spawn " + i).GetComponent<Transform>());
        }
    }

    private void PauseMenu()
    {
        //Activate the pause menu
        pauseMenuCanvas.SetActive(true);

        //Freeze time so stuff doesn't happen in the background
        Time.timeScale = 0;

        //Display the player's score
        scoreOnPauseMenu.GetComponent<Text>().text = "SCORE: " + EnemyWall.score;

        //Display the palyer's current high score
        highScoreOnPauseMenu.GetComponent<Text>().text = "HIGH SCORE: " + PlayerPrefs.GetInt("HighScore");
    }

    private void UpdateMoney()
    {
        money += 10;
    }

    private void PlayerBasicUnitFactory()
    {
        //Check if the player has enough money
        if(money >= basicUnitFactoryCost)
        {
            //Spawn a unit at the factory rate
            InvokeRepeating("SpawnPlayerBasicUnit", 0f, basicUnitFactoryRate);

            //Take away the money that was just spent
            money -= basicUnitFactoryCost;

            //Add 1 to the number of factories
            numberOfBasicFactories++;

            //Display the number of factories
            basicUnitButtonTextAmount.GetComponent<Text>().text = numberOfBasicFactories.ToString();
        }
    }

    private void PlayerStrongUnitFactory()
    {
        //Check if the player has enough money
        if (money >= strongUnitFactoryCost)
        {
            //Spawn a unit at the factory rate
            InvokeRepeating("SpawnPlayerStrongUnit", 0f, strongUnitFactoryRate);

            //Take away the money that was just spent
            money -= strongUnitFactoryCost;

            //Add 1 to the number of factories
            numberOfStrongFactories++;

            //Display the number of factories
            strongUnitButtonTextAmount.GetComponent<Text>().text = numberOfStrongFactories.ToString();
        }
    }

    private void PlayerFastUnitFactory()
    {
        //Check if the player has enough money
        if (money >= fastUnitFactoryCost)
        {
            //Spawn a unit at the factory rate
            InvokeRepeating("SpawnPlayerFastUnit", 0f, fastUnitFactoryRate);

            //Take away the money that was just spent
            money -= fastUnitFactoryCost;

            //Add 1 to the number of factories
            numberOfFastFactories++;

            //Display the number of factories
            fastUnitButtonTextAmount.GetComponent<Text>().text = numberOfFastFactories.ToString();
        }
    }

    private void PlayerUltimateUnitFactory()
    {
        //Check if the player has enough money
        if (money >= ultimateUnitFactoryCost)
        {
            //Spawn a unit at the factory rate
            InvokeRepeating("SpawnPlayerUltimateUnit", 0f, ultimateUnitFactoryRate);

            //Take away the money that was just spent
            money -= ultimateUnitFactoryCost;

            //Add 1 to the number of factories
            numberOfUltimateFactories++;

            //Display the number of factories
            ultimateUnitButtonTextAmount.GetComponent<Text>().text = numberOfUltimateFactories.ToString();
        }
    }

    private void SpawnPlayerBasicUnit()
    {
        //for every player basic unit in the pooled player basic units list
        for (int i = 0; i < pooledPlayerBasicUnits.Count; i++)
        {
            //Check if the player basic unit is not null
            if (pooledPlayerBasicUnits[i] != null)
            {
                //Check if the player basic unit is inactive 
                if (!pooledPlayerBasicUnits[i].activeInHierarchy)
                {
                    //Pick random spawn point
                    Transform unitSpawn = playerSpawns[ Random.Range(0, playerSpawns.Count) ];

                    //Set the unit's position and rotation to the unit spawn's position and rotation
                    pooledPlayerBasicUnits[i].transform.position = unitSpawn.transform.position;
                    pooledPlayerBasicUnits[i].transform.rotation = unitSpawn.transform.rotation;

                    //Enable the unit
                    pooledPlayerBasicUnits[i].SetActive(true);

                    //Break out of this for loop so we don't end up activating every single unit that is inactive in the player basic units pool
                    break;
                }
            }
        }
    }

    private void SpawnPlayerStrongUnit()
    {
        //for every player strong unit in the pooled player strong units list
        for (int i = 0; i < pooledPlayerStrongUnits.Count; i++)
        {
            //Check if the player strong unit is not null
            if (pooledPlayerStrongUnits[i] != null)
            {
                //Check if the player strong unit is inactive 
                if (!pooledPlayerStrongUnits[i].activeInHierarchy)
                {
                    //Pick random spawn point
                    Transform unitSpawn = playerSpawns[Random.Range(0, playerSpawns.Count)];

                    //Set the unit's position and rotation to the unit spawn's position and rotation
                    pooledPlayerStrongUnits[i].transform.position = unitSpawn.transform.position;
                    pooledPlayerStrongUnits[i].transform.rotation = unitSpawn.transform.rotation;

                    //Enable the unit
                    pooledPlayerStrongUnits[i].SetActive(true);

                    //Break out of this for loop so we don't end up activating every single unit that is inactive in the player strong units pool
                    break;
                }
            }
        }
    }

    private void SpawnPlayerFastUnit()
    {
        //for every player fast unit in the pooled player fast units list
        for (int i = 0; i < pooledPlayerFastUnits.Count; i++)
        {
            //Check if the player fast unit is not null
            if (pooledPlayerFastUnits[i] != null)
            {
                //Check if the player fast unit is inactive 
                if (!pooledPlayerFastUnits[i].activeInHierarchy)
                {
                    //Pick random spawn point
                    Transform unitSpawn = playerSpawns[Random.Range(0, playerSpawns.Count)];

                    //Set the unit's position and rotation to the unit spawn's position and rotation
                    pooledPlayerFastUnits[i].transform.position = unitSpawn.transform.position;
                    pooledPlayerFastUnits[i].transform.rotation = unitSpawn.transform.rotation;

                    //Enable the unit
                    pooledPlayerFastUnits[i].SetActive(true);

                    //Break out of this for loop so we don't end up activating every single unit that is inactive in the player fast units pool
                    break;
                }
            }
        }
    }

    private void SpawnPlayerUltimateUnit()
    {
        //for every player ultimate unit in the pooled player ultimate units list
        for (int i = 0; i < pooledPlayerUltimateUnits.Count; i++)
        {
            //Check if the player ultimate unit is not null
            if (pooledPlayerUltimateUnits[i] != null)
            {
                //Check if the player ultimate unit is inactive 
                if (!pooledPlayerUltimateUnits[i].activeInHierarchy)
                {
                    //Pick random spawn point
                    Transform unitSpawn = playerSpawns[Random.Range(0, playerSpawns.Count)];

                    //Set the unit's position and rotation to the unit spawn's position and rotation
                    pooledPlayerUltimateUnits[i].transform.position = unitSpawn.transform.position;
                    pooledPlayerUltimateUnits[i].transform.rotation = unitSpawn.transform.rotation;

                    //Enable the unit
                    pooledPlayerUltimateUnits[i].SetActive(true);

                    //Break out of this for loop so we don't end up activating every single unit that is inactive in the player ultimate units pool
                    break;
                }
            }
        }
    }

    private void SpawnEnemyBasicUnit()
    {
        //for every enemy basic unit in the pooled enemy basic units list
        for (int i = 0; i < pooledEnemyBasicUnits.Count; i++)
        {
            //Check if the enemy basic unit is not null
            if (pooledEnemyBasicUnits[i] != null)
            {
                //Check if the enemy basic unit is inactive 
                if (!pooledEnemyBasicUnits[i].activeInHierarchy)
                {
                    //Pick random spawn point
                    Transform unitSpawn = enemySpawns[Random.Range(0, enemySpawns.Count)];

                    //Set the unit's position and rotation to the unit spawn's position and rotation
                    pooledEnemyBasicUnits[i].transform.position = unitSpawn.transform.position;
                    pooledEnemyBasicUnits[i].transform.rotation = unitSpawn.transform.rotation;

                    //Enable the unit
                    pooledEnemyBasicUnits[i].SetActive(true);

                    //Break out of this for loop so we don't end up activating every single unit that is inactive in the enemy basic units pool
                    break;
                }
            }
        }
    }

    private void SpawnEnemyStrongUnit()
    {
        //for every enemy strong unit in the pooled enemy strong units list
        for (int i = 0; i < pooledEnemyStrongUnits.Count; i++)
        {
            //Check if the enemy strong unit is not null
            if (pooledEnemyStrongUnits[i] != null)
            {
                //Check if the enemy strong unit is inactive 
                if (!pooledEnemyStrongUnits[i].activeInHierarchy)
                {
                    //Pick random spawn point
                    Transform unitSpawn = enemySpawns[Random.Range(0, enemySpawns.Count)];

                    //Set the unit's position and rotation to the unit spawn's position and rotation
                    pooledEnemyStrongUnits[i].transform.position = unitSpawn.transform.position;
                    pooledEnemyStrongUnits[i].transform.rotation = unitSpawn.transform.rotation;

                    //Enable the unit
                    pooledEnemyStrongUnits[i].SetActive(true);

                    //Break out of this for loop so we don't end up activating every single unit that is inactive in the enemy strong units pool
                    break;
                }
            }
        }
    }

    private void SpawnEnemyFastUnit()
    {
        //for every enemy fast unit in the pooled enemy fast units list
        for (int i = 0; i < pooledEnemyFastUnits.Count; i++)
        {
            //Check if the enemy fast unit is not null
            if (pooledEnemyFastUnits[i] != null)
            {
                //Check if the enemy fast unit is inactive 
                if (!pooledEnemyFastUnits[i].activeInHierarchy)
                {
                    //Pick random spawn point
                    Transform unitSpawn = enemySpawns[Random.Range(0, enemySpawns.Count)];

                    //Set the unit's position and rotation to the unit spawn's position and rotation
                    pooledEnemyFastUnits[i].transform.position = unitSpawn.transform.position;
                    pooledEnemyFastUnits[i].transform.rotation = unitSpawn.transform.rotation;

                    //Enable the unit
                    pooledEnemyFastUnits[i].SetActive(true);

                    //Break out of this for loop so we don't end up activating every single unit that is inactive in the enemy fast units pool
                    break;
                }
            }
        }
    }

    private void SpawnEnemyUltimateUnit()
    {
        //for every enemy ultimate unit in the pooled enemy ultimate units list
        for (int i = 0; i < pooledEnemyUltimateUnits.Count; i++)
        {
            //Check if the enemy ultimate unit is not null
            if (pooledEnemyUltimateUnits[i] != null)
            {
                //Check if the enemy ultimate unit is inactive 
                if (!pooledEnemyUltimateUnits[i].activeInHierarchy)
                {
                    //Pick random spawn point
                    Transform unitSpawn = enemySpawns[Random.Range(0, enemySpawns.Count)];

                    //Set the unit's position and rotation to the unit spawn's position and rotation
                    pooledEnemyUltimateUnits[i].transform.position = unitSpawn.transform.position;
                    pooledEnemyUltimateUnits[i].transform.rotation = unitSpawn.transform.rotation;

                    //Enable the unit
                    pooledEnemyUltimateUnits[i].SetActive(true);

                    //Break out of this for loop so we don't end up activating every single unit that is inactive in the enemy ultimate units pool
                    break;
                }
            }
        }
    }

    private void SpawnEnemyUnits()
    {
        //Spawn an enemy basic unit after 3 seconds at the factory rate
        InvokeRepeating("SpawnEnemyBasicUnit", 0f, basicUnitFactoryRate);

        //Spawn an enemy strong unit after 30 seconds at the factory rate
        InvokeRepeating("SpawnEnemyStrongUnit", 30f, strongUnitFactoryRate);

        //Spawn an enemy fast unit after 60 seconds at the factory rate
        InvokeRepeating("SpawnEnemyFastUnit", 60f, fastUnitFactoryRate);

        //Spawn an enemy ultimate after 90 seconds at the factory rate
        InvokeRepeating("SpawnEnemyUltimateUnit", 90f, ultimateUnitFactoryRate);
    }

    private void DestroyAllEnemyUnits()
    {
        //Check if player has enough money
        if(money >= destroyAllEnemyUnitsCost)
        {
            //Take away the money that was just spent
            money -= destroyAllEnemyUnitsCost;

            //Store all enemy unit in an array
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemyUnit");

            //For each enemy in the enemies array
            foreach (GameObject enemy in enemies)
            {
                //Make an explosion
                Instantiate(explosionPrefab, enemy.transform.position, enemy.transform.rotation);

                //Deactivate the enemy
                enemy.SetActive(false);
            }
        }
    }

    private void MuteSoundEffects()
    {
        //Mute explosions
        explosionPrefab.GetComponent<AudioSource>().mute = true;

        //Mute each player bullet
        for(int i = 0; i < pooledPlayerBullets.Count; i++)
        {
            if(pooledPlayerBullets[i] != null)
            {
                pooledPlayerBullets[i].GetComponent<AudioSource>().mute = true;
            }
        }

        //Mute each enemy bullet
        for(int i = 0; i < pooledEnemyBullets.Count; i++)
        {
            if(pooledEnemyBullets[i] != null)
            {
                pooledEnemyBullets[i].GetComponent<AudioSource>().mute = true;
            }
        }

        //Make a player prefs variable
        PlayerPrefs.SetString("SoundEffects", "muted");
    }

    private void UnmuteSoundEffects()
    {
        //Unmute explosions
        explosionPrefab.GetComponent<AudioSource>().mute = false;

        //Unmute each player bullet
        for (int i = 0; i < pooledPlayerBullets.Count; i++)
        {
            if (pooledPlayerBullets[i] != null)
            {
                pooledPlayerBullets[i].GetComponent<AudioSource>().mute = false;
            }
        }

        //Unmute each enemy bullet
        for (int i = 0; i < pooledEnemyBullets.Count; i++)
        {
            if (pooledEnemyBullets[i] != null)
            {
                pooledEnemyBullets[i].GetComponent<AudioSource>().mute = false;
            }
        }

        //Make a player prefs variable
        PlayerPrefs.SetString("SoundEffects", "unmuted");
    }

    private void Tutorial()
    {
        //Run the first segment of the tutorial
        tutCanvas1Function();
    }

    private void tutCanvas1Function()
    {
        //Freeze time
        Time.timeScale = 0;

        //Deactivate the UI buttons 
        DeactivateUIButtons();

        //Activate the 1st tutorial window
        tutorialCanvas1.SetActive(true);

        //When the user clicks the button to continue, deactivate this tutorial window, reactivate the UI buttons, invoke the next tutorial segment after a certain amount of time, and unfreeze time
        tutorialCanvas1Button.GetComponent<Button>().onClick.AddListener(delegate { tutorialCanvas1.SetActive(false); ActivateUIButtons(); Invoke("tutCanvas2Function", 20f); Time.timeScale = 1; });
    }

    private void tutCanvas2Function()
    {
        //Freeze time
        Time.timeScale = 0;

        //Deactivate the UI buttons 
        DeactivateUIButtons();

        //Activate the 2nd tutorial window
        tutorialCanvas2.SetActive(true);

        //When the user clicks the button to continue, deactivate this tutorial window, reactivate the UI buttons, invoke the next tutorial segment after a certain amount of time, and unfreeze time
        tutorialCanvas2Button.GetComponent<Button>().onClick.AddListener(delegate { tutorialCanvas2.SetActive(false); ActivateUIButtons(); Invoke("tutCanvas3Function", 10f); Time.timeScale = 1; });
    }

    private void tutCanvas3Function()
    {
        //Freeze time
        Time.timeScale = 0;

        //Deactivate the UI buttons 
        DeactivateUIButtons();

        //Activate the 3rd tutorial window
        tutorialCanvas3.SetActive(true);

        //When the user clicks the button to continue, deactivate this tutorial window, reactivate the UI buttons, invoke the next tutorial segment after a certain amount of time, and unfreeze time
        tutorialCanvas3Button.GetComponent<Button>().onClick.AddListener(delegate { tutorialCanvas3.SetActive(false); ActivateUIButtons(); Invoke("tutCanvas4Function", 10f); Time.timeScale = 1; });
    }

    private void tutCanvas4Function()
    {
        //Freeze time
        Time.timeScale = 0;

        //Deactivate the UI buttons 
        DeactivateUIButtons();

        //Activate the 4th tutorial window
        tutorialCanvas4.SetActive(true);

        //When the user clicks the button to continue, deactivate this tutorial window, reactivate the UI buttons, invoke the next tutorial segment after a certain amount of time, and unfreeze time
        tutorialCanvas4Button.GetComponent<Button>().onClick.AddListener(delegate { tutorialCanvas4.SetActive(false); ActivateUIButtons(); Invoke("tutCanvas5Function", 10f); Time.timeScale = 1; });
    }

    private void tutCanvas5Function()
    {
        //Freeze time
        Time.timeScale = 0;

        //Deactivate the UI buttons 
        DeactivateUIButtons();

        //Activate the 5th tutorial window
        tutorialCanvas5.SetActive(true);

        //When the user clicks the button to continue, deactivate this tutorial window, reactivate the UI buttons, invoke the next tutorial segment after a certain amount of time, and unfreeze time
        tutorialCanvas5Button.GetComponent<Button>().onClick.AddListener(delegate { tutorialCanvas5.SetActive(false); ActivateUIButtons(); Invoke("tutCanvas6Function", 10f); Time.timeScale = 1; });
    }

    private void tutCanvas6Function()
    {
        //Freeze time
        Time.timeScale = 0;

        //Deactivate the UI buttons 
        DeactivateUIButtons();

        //Activate the 6th tutorial window
        tutorialCanvas6.SetActive(true);

        //When the user clicks the button to continue, deactivate this tutorial window, reactivate the UI buttons, invoke the next tutorial segment after a certain amount of time, and unfreeze time
        tutorialCanvas6Button.GetComponent<Button>().onClick.AddListener(delegate { tutorialCanvas6.SetActive(false); ActivateUIButtons(); Invoke("tutCanvas7Function", 10f); Time.timeScale = 1; });
    }

    private void tutCanvas7Function()
    {
        //Freeze time
        Time.timeScale = 0;

        //Deactivate the UI buttons 
        DeactivateUIButtons();

        //Activate the 7th tutorial window
        tutorialCanvas7.SetActive(true);

        //If the user clicks the button to continue, deactivate this tutorial window, reactivate the UI buttons, and unfreeze time
        tutorialCanvas7ContinueButton.GetComponent<Button>().onClick.AddListener(delegate { tutorialCanvas7.SetActive(false); ActivateUIButtons(); Time.timeScale = 1; });

        //If the user clicks the button to go to the main menu then reactivate the UI buttons and load the main menu
        tutorialCanvas7MainMenuButton.GetComponent<Button>().onClick.AddListener(delegate { ActivateUIButtons(); SceneManager.LoadScene("Main Menu"); });
    }

    private void DeactivateUIButtons()
    {
        //Make the UI buttons unusable
        pauseButton.interactable = false;
        basicUnitFactoryButton.interactable = false;
        strongUnitFactoryButton.interactable = false;
        fastUnitFactoryButton.interactable = false;
        ultimateUnitFactoryButton.interactable = false;
        destroyAllEnemyUnitsButton.interactable = false;
    }

    private void ActivateUIButtons()
    {
        //Make the UI buttons usable
        pauseButton.interactable = true;
        basicUnitFactoryButton.interactable = true;
        strongUnitFactoryButton.interactable = true;
        fastUnitFactoryButton.interactable = true;
        ultimateUnitFactoryButton.interactable = true;
        destroyAllEnemyUnitsButton.interactable = true;
    }
}
