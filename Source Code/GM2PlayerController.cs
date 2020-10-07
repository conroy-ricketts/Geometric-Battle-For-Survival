using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//This script is attached to the player object in GM2

public class GM2PlayerController : MonoBehaviour
{
    //Game over
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private Text scoreOnGameOver;


    void Start ()
    {

	}	

	void Update ()
    {
        //Get touch input
        if (Input.GetMouseButtonDown(0))
        {
            //Check if the the user did not touch a UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //Get the y position of the player and reverse it 
                float newPositionY = gameObject.transform.position.y * -1;

                //Set the new player position 
                gameObject.transform.position = new Vector3(-25, newPositionY, 0);
            }
        }
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If player collides with enemy
        if(other.tag == "GM2Enemy")
        {
            //Activate game over canvas
            gameOverCanvas.SetActive(true);

            //Freeze time to prevent things from happening in the background
            Time.timeScale = 0;

            //Display the player's score
            scoreOnGameOver.GetComponent<Text>().text = "SCORE: " + GM2Manager.score;
        }
    }    
}
