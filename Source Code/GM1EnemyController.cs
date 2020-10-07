using UnityEngine;

//This script is attached to the enemy object in game mode 1
//GM1 stands for Game Mode 1, which means that this script is specific to game mode 1


public class GM1EnemyController : MonoBehaviour
{
    [SerializeField] private float minEnemySpeed;
    [SerializeField] private float maxEnemySpeed;


    void Start ()
    {
        //Add velocity 
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3( Random.Range(minEnemySpeed, maxEnemySpeed), 0, 0 );
    }

	void Update ()
    {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //If the enemy hits the 2nd enemy point
        if(other.name == "Enemy Point 2")
        {
            //Add velocity so that the enemy moves towards the left
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3( Random.Range(minEnemySpeed, maxEnemySpeed) * -1, 0, 0 );

            //Reverse direction
            gameObject.transform.Rotate( new Vector3(0, 0, 180) );
        }

        //If the enemy hits the 1st enemy Point
        if (other.name == "Enemy Point 1")
        {
            //Add velocity so that the enemy moves towards the right
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3( Random.Range(minEnemySpeed, maxEnemySpeed), 0, 0 );

            //Reverse direction
            gameObject.transform.Rotate( new Vector3(0, 0, 180) );
        }
    }
}
