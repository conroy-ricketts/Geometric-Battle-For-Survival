using UnityEngine;

//This script is attached to the background object

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] private float speedX;
    [SerializeField] private float speedY;

    void Start()
    {
    }

    void Update()
    {
        //Scroll the background texture with an offset
        Vector2 offset = new Vector2(Time.time * speedX, Time.time * speedY);
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }
}
