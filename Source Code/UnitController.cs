using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//This script is attached to the unit prefabs

public class UnitController : MonoBehaviour{
    
    [SerializeField] private float unitSpeed;
    [SerializeField] private float health;    
    [SerializeField] private float fireRate;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private GameObject explosionPrefab;

    private GameObject healthBar;
    private GameObject closestTarget = null;
    private float thisUnitsHealth;
    private float basicBulletDamage = 10;
    private float strongBulletDamage = 100;
    private float fastBulletDamage = 5;
    private float ultimateBulletDamage = 1000;


    void Start ()
    {
    }	

	void Update ()
    {
        //If this game object is active
        if (gameObject.activeSelf)
        {
            //Rotate towards the closest target
            RotateTowardsClosestTarget();

            //If this unit's health is 0 or lower
            if (thisUnitsHealth <= 0)
            {
                //Make an explosion
                Instantiate(explosionPrefab, gameObject.transform.position, gameObject.transform.rotation);

                //Deactivate this unit
                gameObject.SetActive(false);
            }
            
            //Display health on health bar (fill amount takes a value between 0 and 1, so divide the units health by its original health)
            healthBar.GetComponent<Image>().fillAmount = thisUnitsHealth / health;
        }
    }


    //This function is called when the gameobject is enabled
    private void OnEnable()
    {
        //Set this units health
        thisUnitsHealth = health;

        //Every half of a second, update the closest target
        InvokeRepeating("UpdateClosestTarget", 0f, 0.5f);

        //Shoot the target at this unit's fire rate
        InvokeRepeating("ShootClosestTarget", fireRate, fireRate);

        //Add velocity 
        if (gameObject.tag == "playerUnit")
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(unitSpeed, 0, 0);
        }
        if (gameObject.tag == "enemyUnit")
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(-unitSpeed, 0, 0);
        }

        //Find the health bar attached to this unit
        healthBar = gameObject.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
    }

    //This function is called when the gameobject is disabled
    private void OnDisable()
    {
        //Cancel all invoke methods
        CancelInvoke();
    }

    private void UpdateClosestTarget()
    {
        //Store all of the possible in an array
        GameObject[] possibleTargets = null;
        if(gameObject.tag == "playerUnit")
        {
            possibleTargets = GameObject.FindGameObjectsWithTag("enemyUnit");
        }
        if (gameObject.tag == "enemyUnit")
        {
            possibleTargets = GameObject.FindGameObjectsWithTag("playerUnit");
        }

        //Set the default shortest distance to infinity 
        float shortestDistance = Mathf.Infinity;

        //Set the deafault nearest target to null
        GameObject nearestTarget = null;

        //Use a for each loop to access each target in the array of possible targets
        foreach (GameObject target in possibleTargets)
        {
            //Find the distance between the this unit (the one that this script is attached to) and the target
            float distanceToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);

            /*if the distance between this unit and the target is less than the current shortest distance, 
            then set the current shortest distance to that distance and set the current nearest target to
            that target. (Keep in mind that this is looping for each target in the possible targets array!!!)*/
            if (distanceToTarget< shortestDistance)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = target;
            }
        }

        //if the nearest target is not set to null, then set the closest target to the nearest target
        if (nearestTarget != null)
        {
            closestTarget = nearestTarget;
        }
    }

    private void RotateTowardsClosestTarget()
    {
        if(closestTarget != null && closestTarget.activeSelf)
        {
            Quaternion newRotation = Quaternion.LookRotation(gameObject.transform.position - closestTarget.transform.position, Vector3.forward);
            newRotation.x = 0;
            newRotation.y = 0;
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, newRotation, Time.deltaTime * 100000);
        }        
    }

    private void ShootClosestTarget()
    {
        //Only shoot if there is an enemy
        if(closestTarget != null && closestTarget.activeSelf)
        {
            //Check if this unit is a player unit
            if(gameObject.tag == "playerUnit")
            {
                //for every bullet in the pooled player bullets list
                for (int i = 0; i < GameManager.pooledPlayerBullets.Count; i++)
                {
                    //Check if the bullet is not null
                    if(GameManager.pooledPlayerBullets[i] != null)
                    {
                        //Check if the bullet is inactive 
                        if (!GameManager.pooledPlayerBullets[i].activeInHierarchy)
                        {
                            //Set the bullet position and rotation to the bullet spawn transform's position and rotation
                            GameManager.pooledPlayerBullets[i].transform.position = bulletSpawn.transform.position;
                            GameManager.pooledPlayerBullets[i].transform.rotation = bulletSpawn.transform.rotation;

                            //Set the bullet's target to the closest target
                            GameManager.pooledPlayerBullets[i].GetComponent<BulletController>().target = closestTarget;

                            //Enable the bullet
                            GameManager.pooledPlayerBullets[i].SetActive(true);

                            //Tag the bullet
                            if (gameObject.name == "BasicUnit(player)(Clone)")
                            {
                                GameManager.pooledPlayerBullets[i].tag = "playerBasicBullet";
                            }
                            if (gameObject.name == "StrongUnit(player)(Clone)")
                            {
                                GameManager.pooledPlayerBullets[i].tag = "playerStrongBullet";
                            }
                            if (gameObject.name == "FastUnit(player)(Clone)")
                            {
                                GameManager.pooledPlayerBullets[i].tag = "playerFastBullet";
                            }
                            if (gameObject.name == "UltimateUnit(player)(Clone)")
                            {
                                GameManager.pooledPlayerBullets[i].tag = "playerUltimateBullet";
                            }

                            //Break out of this for loop so we don't end up activating every single bullet that is inactive in the bullet pool
                            break;
                        }    
                    }                    
                }
            }


            //Check if this unit is an enemy unit
            if (gameObject.tag == "enemyUnit")
            {
                //for every bullet in the pooled enemy bullets list
                for (int i = 0; i < GameManager.pooledEnemyBullets.Count; i++)
                {
                    //Check if the bullet is null
                    if(GameManager.pooledEnemyBullets[i] != null)
                    {
                        //Check if the bullet is inactive
                        if (!GameManager.pooledEnemyBullets[i].activeInHierarchy)
                        {
                            //Set the bullet position and rotation to the bullet spawn transform's position and rotation
                            GameManager.pooledEnemyBullets[i].transform.position = bulletSpawn.transform.position;
                            GameManager.pooledEnemyBullets[i].transform.rotation = bulletSpawn.transform.rotation;

                            //Set the bullet's target to the closest target
                            GameManager.pooledEnemyBullets[i].GetComponent<BulletController>().target = closestTarget;

                            //Enable the bullet
                            GameManager.pooledEnemyBullets[i].SetActive(true);

                            //Tag the bullet
                            if (gameObject.name == "BasicUnit(enemy)(Clone)")
                            {
                                GameManager.pooledEnemyBullets[i].tag = "enemyBasicBullet";
                            }
                            if (gameObject.name == "StrongUnit(enemy)(Clone)")
                            {
                                GameManager.pooledEnemyBullets[i].tag = "enemyStrongBullet";
                            }
                            if (gameObject.name == "FastUnit(enemy)(Clone)")
                            {
                                GameManager.pooledEnemyBullets[i].tag = "enemyFastBullet";
                            }
                            if (gameObject.name == "UltimateUnit(enemy)(Clone)")
                            {
                                GameManager.pooledEnemyBullets[i].tag = "enemyUltimateBullet";
                            }

                            //Break out of this for loop so we don't end up activating every single bullet that is inactive in the bullet pool
                            break;
                        }
                    }                    
                }
            }
        }        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Check if this script is attached to a player unit
        if(gameObject.tag == "playerUnit")
        {
            //Lose health if hit by an enemy basic bullet and deactivate the bullet
            if (other.tag == "enemyBasicBullet")
            {
                thisUnitsHealth -= basicBulletDamage;
                other.gameObject.SetActive(false);
            }

            //Lose health if hit by an enemy strong bullet and deactivate the bullet
            if (other.tag == "enemyStrongBullet")
            {
                thisUnitsHealth -= strongBulletDamage;
                other.gameObject.SetActive(false);
            }

            //Lose health if hit by an enemy fast bullet and deactivate the bullet
            if (other.tag == "enemyFastBullet")
            {
                thisUnitsHealth -= fastBulletDamage;
                other.gameObject.SetActive(false);
            }

            //Lose health if hit by an enemy ultimate bullet and deactivate the bullet
            if (other.tag == "enemyUltimateBullet")
            {
                thisUnitsHealth -= ultimateBulletDamage;
                other.gameObject.SetActive(false);
            }
        }

        //Check if this script is attached to an enemy unit
        if (gameObject.tag == "enemyUnit")
        {
            //Lose health if hit by a player basic bullet and deactivate the bullet
            if (other.tag == "playerBasicBullet")
            {
                thisUnitsHealth -= basicBulletDamage;
                other.gameObject.SetActive(false);
            }

            //Lose health if hit by a player strong bullet and deactivate the bullet
            if (other.tag == "playerStrongBullet")
            {
                thisUnitsHealth -= strongBulletDamage;
                other.gameObject.SetActive(false);
            }

            //Lose health if hit by a player fast bullet and deactivate the bullet
            if (other.tag == "playerFastBullet")
            {
                thisUnitsHealth -= fastBulletDamage;
                other.gameObject.SetActive(false);
            }

            //Lose health if hit by a player ultimate bullet and deactivate the bullet
            if (other.tag == "playerUltimateBullet")
            {
                thisUnitsHealth -= ultimateBulletDamage;
                other.gameObject.SetActive(false);
            }
        }
    }
}
