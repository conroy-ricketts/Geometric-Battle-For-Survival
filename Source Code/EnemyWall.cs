using UnityEngine;
using UnityEngine.UI;

//This script is attached to the enemy wall object

public class EnemyWall : MonoBehaviour
{
    //Score
    [SerializeField] private Text scoreText;
    public static int score;

    //Explosions
    [SerializeField] private GameObject explosionPrefab;

    void Start ()
    {
        //Set the score to 0 as a default
        score = 0;

        //Add 1 to the score every second
        InvokeRepeating("Add1ToScore", 0f, 1f);
	}

	void Update ()
    {
        //Display the score
        scoreText.GetComponent<Text>().text = "Score: " + score;

        //If the score is higher than the high score, then set the high score to that score
        if( score > PlayerPrefs.GetInt("HighScore") )
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

        //Unlock player achievements based on the palyer's score
        UnlockAchievementsBasedOnScore();        
    }
     
    
    private void Add1ToScore()
    {
        score++;
    }

    private void UnlockAchievementsBasedOnScore()
    {
        //I got the IDs for each achievement from the google play developer console

        if(score >= 500)
        {
            Social.ReportProgress("CgkIq9Ks--gMEAIQAg", 100, success => { });
        }

        if (score >= 1000)
        {
            Social.ReportProgress("CgkIq9Ks--gMEAIQBA", 100, success => { });
        }

        if (score >= 1500)
        {
            Social.ReportProgress("CgkIq9Ks--gMEAIQBQ", 100, success => { });
        }

        if (score >= 2000)
        {
            Social.ReportProgress("CgkIq9Ks--gMEAIQBg", 100, success => { });
        }

        if (score >= 3000)
        {
            Social.ReportProgress("CgkIq9Ks--gMEAIQBw", 100, success => { });
        }

        if (score >= 4000)
        {
            Social.ReportProgress("CgkIq9Ks--gMEAIQCA", 100, success => { });
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //If a player basic unit collides with this wall
        if(other.name == "BasicUnit(player)(Clone)")
        {
            //Add to the score
            score += 10;
        }

        //If a player strong unit collides with this wall
        if (other.name == "StrongUnit(player)(Clone)")
        {
            //Add to the score
            score += 50;
        }

        //If a player fast unit collides with this wall
        if (other.name == "FastUnit(player)(Clone)")
        {
            //Add to the score
            score += 1000;
        }

        //If a player ultimate unit collides with this wall
        if (other.name == "UltimateUnit(player)(Clone)")
        {
            //Add to the score
            score += 100;
        }

        //Make an explosion when a unit collides with this wall
        if(other.gameObject.tag == "playerUnit")
        {
            Instantiate(explosionPrefab, other.gameObject.transform.position, other.gameObject.transform.rotation);
        }

        //Deactivate anything that touches this wall
        other.gameObject.SetActive(false);
    }    
}
