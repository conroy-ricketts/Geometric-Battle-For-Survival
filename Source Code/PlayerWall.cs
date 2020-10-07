using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//This script is attached to the player wall object

public class PlayerWall : MonoBehaviour {

    //Health variables
    private float health = 100;
    [SerializeField] private Text playerHealthText;

    //Game over variables
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private Button goToMainMenuButton;
    [SerializeField] private Text scoreOnGameOver;

    //Explosions
    [SerializeField] private GameObject explosionPrefab;


    void Start ()
    {
        //Display health
        playerHealthText.text = "Health: " + health;

        //Load the main menu when the "go to main menu button" is clicked in the game over canvas
        goToMainMenuButton.GetComponent<Button>().onClick.AddListener(  delegate{ SceneManager.LoadScene("Main Menu"); }  );
    }
    
	void Update ()
    {
        //If health = zero
        if(health <= 0)
        {
            //Activate game over canvas
            gameOverCanvas.SetActive(true);

            //Freeze time to prevent things from happening in the background
            Time.timeScale = 0;

            //Display the player's score
            scoreOnGameOver.GetComponent<Text>().text = "SCORE: " + EnemyWall.score;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        //If an enemy basic unit collides with this wall, then lose 5 health, display it
        if(other.name == "BasicUnit(enemy)(Clone)")
        {
            health -= 5;
            playerHealthText.text = "Health: " + health;
        }

        //If an enemy strong unit collides with this wall, then lose 25 health, display it
        if (other.name == "StrongUnit(enemy)(Clone)")
        {
            health -= 25;
            playerHealthText.text = "Health: " + health;
        }

        //If an enemy fast unit collides with this wall, then lose 50 health, display it
        if (other.name == "FastUnit(enemy)(Clone)")
        {
            health -= 50;
            playerHealthText.text = "Health: " + health;
        }

        //If an enemy ultimate unit collides with this wall, then lose 80 health, display it
        if (other.name == "UltimateUnit(enemy)(Clone)")
        {
            health -= 80;
            playerHealthText.text = "Health: " + health;
        }

        //Make an explosion when a unit collides with this wall
        if (other.gameObject.tag == "enemyUnit")
        {
            Instantiate(explosionPrefab, other.gameObject.transform.position, other.gameObject.transform.rotation);
        }

        //Deactivate anything that touches this wall
        other.gameObject.SetActive(false);
    }
}
