using UnityEngine;
using UnityEngine.UI;

//This script is attached to the enemy prefab in GM2

public class GM2EnemyController : MonoBehaviour
{


    void Start ()
    {
		
	}	

	void Update ()
    {
        //If this enemy makes it past a certain point on the x axis
        if (gameObject.transform.position.x < -40)
        {
            //Add 1 to the score
            GM2Manager.score++;

            //Deactivate it
            gameObject.SetActive(false);

            //Set a variable so we know that there isn't an active enemy
            GM2Manager.activeEnemyPresent = false;
        }

    }


    private void OnEnable()
    {
        //Add velocity
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(-GM2Manager.enemySpeed, 0, 0);
    }
}
