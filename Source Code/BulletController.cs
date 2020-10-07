using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is attached to the enemy and player bullet prefabs


public class BulletController : MonoBehaviour
{
    private float bulletSpeed = 100;
    public GameObject target;

    void Start ()
    {

    }

    void Update ()
    {
        //Move towards target
        if (target != null)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target.transform.position, bulletSpeed * Time.deltaTime);
        }

        //Deactivate this bullet if it's target is deactivated (or if it's target was killed before the bullet could get to it)
        if (!target.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }


    //This function is called when the gameobject is enabled
    private void OnEnable()
    {
        //Deactivate after 5 seconds of this bullet being eneabled
        Invoke("DeactivateBullet", 5f);
    }

    //This function is called when the gameobject is disabled
    private void OnDisable()
    {
        //Set target to null
        target = null;

        //Cancel all invoke methods
        CancelInvoke();
    }

    private void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }
}
