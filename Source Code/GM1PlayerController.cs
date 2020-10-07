using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//This script is attached to the player object in game mode 1
//GM1 stands for Game Mode 1, which means that this script is specific to game mode 1

public class GM1PlayerController : MonoBehaviour {

    //Player
    [SerializeField] private float playerSpeed;
    private float slowedPlayerSpeed;

    //Game over variables
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private Button goToMainMenuFromGameOverButton;
    [SerializeField] private Text scoreOnGameOver;

    //Score
    [SerializeField] private Text scoreText;
    public static int score;


    void Start ()
    {
        //Load the main menu when the "go to main menu button" is clicked in the game over canvas
        goToMainMenuFromGameOverButton.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("Main Menu"); });

        //Set the slower player speed
        slowedPlayerSpeed = playerSpeed - 50;

        //Set the default score
        score = 0;
    }

	void Update ()
    {
        //Get mouse/touch screen input
        if (Input.GetMouseButton(0))
        {
            //Move the player in a circle around the middle of the screen at a slower speed than normal
            gameObject.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 0, 1), slowedPlayerSpeed * Time.deltaTime);

        }
        else
        {
            //Move the player in a circle around the middle of the screen at the normal speed
            gameObject.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 0, 1), playerSpeed * Time.deltaTime);
        }

        //If the score is higher than the high score, then set the high score to that score
        if (score > PlayerPrefs.GetInt("GM1HighScore"))
        {
            PlayerPrefs.SetInt("GM1HighScore", score);
        }

        //Display score
        scoreText.GetComponent<Text>().text = "Score: " + score;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //if the player collides with the player point
        if(other.tag == "playerPoint")
        {
            //Add 1 to the score
            score += 1;
        }

        //If the player collides with the enemy
        if(other.tag == "gameMode1Enemy")
        {
            //Activate game over canvas
            gameOverCanvas.SetActive(true);

            //Freeze time to prevent things from happening in the background
            Time.timeScale = 0;

            //Display the player's score
            scoreOnGameOver.GetComponent<Text>().text = "SCORE: " + score;
        }
    }

}
