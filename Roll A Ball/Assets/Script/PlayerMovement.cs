using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public TMP_Text countText;
    public TMP_Text winText;

    //Component
    Renderer pickup2Rend;
    Collider pickup1Collider;
    Collider pickup2Collider;

    //Game objects
    public GameObject[] pickup1Obj;
    public GameObject[] pickup2Obj;

    //Textures
    public Texture2D pickup2inactive;
    public Texture2D pickup2active;

    //Timer
    private float timeRemaining = 10;
    private float prevTime = 10;
    private bool timerIsRunning = false;

    private Rigidbody rb;
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";

        pickup1Obj = GameObject.FindGameObjectsWithTag("Pickup");
        foreach (GameObject pickup in pickup1Obj)
        {
            pickup1Collider = pickup.GetComponent<Collider>();
        }
        pickup2Obj = GameObject.FindGameObjectsWithTag("Pickup2");
        foreach (GameObject cube in pickup2Obj)
        {
            pickup2Collider = cube.GetComponent<Collider>();
            pickup2Collider.isTrigger = false;
        }
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                //DisplayTime(timeRemaining);
            }
            else
            {
                foreach (GameObject cube in pickup2Obj)
                {
                    pickup2Collider = cube.GetComponent<Collider>();
                    pickup2Rend = cube.GetComponent<Renderer>();
                    pickup2Collider.isTrigger = false;
                    pickup2Rend.material.SetTexture("_MainTex", pickup2inactive);
                }
                timerIsRunning = false;
                if (prevTime > 1)
                    --prevTime;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();

            foreach (GameObject cube in pickup2Obj)
            {
                pickup2Collider = cube.GetComponent<Collider>();
                pickup2Rend = cube.GetComponent<Renderer>();
                pickup2Collider.isTrigger = true;
                pickup2Rend.material.SetTexture("_MainTex", pickup2active);
            }
            timeRemaining = prevTime;
            timerIsRunning = true; //Start timer to make type 2 pickups active

        }
        if (other.gameObject.CompareTag("Pickup2"))
        {
                other.gameObject.SetActive(false);
                count += 1;
                SetCountText();
        }
    }

    void OnCollisionEnter (Collision other)
    {
        if (other.gameObject.CompareTag("Pickup2"))
        {
            Collider pickupCollider = other.collider;
            bool pickupReadyToMove = false;
            Vector3 pickupPosFinal = new Vector3(0, 0, 0);
            while (pickupReadyToMove == false) //keep looping until available spot is found for pickup cube to move
            {
                bool freeSpace = true;
                Vector3 pickupPos = new Vector3(Random.Range(-8.5f, 8.5f), 1, Random.Range(-8.5f, 8.5f));
                foreach (GameObject cube in pickup1Obj) //check all type 1 cubes
                {
                    if ((pickupPos - cube.transform.position).magnitude < 1.5)
                    {
                        freeSpace = false;
                    }
                }
                foreach (GameObject cube in pickup2Obj) //check all type 2 cubes
                {
                    if ((pickupPos - cube.transform.position).magnitude < 1.5)
                    {
                        freeSpace = false;
                    }
                }
                if ((pickupPos - transform.position).magnitude < 1) //check player position
                    freeSpace = false;
                if (freeSpace == true) //if position is free then set vector and exit loop
                {
                    pickupReadyToMove = true;
                    pickupPosFinal = pickupPos;
                }
            }
            other.transform.position = pickupPosFinal;
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 15)
        {
            winText.text = "You win!";
        }
    }
}
