//A Unity project I'm working on. This script is to call NPCs in the right location of their route based on time and allows NPCs to go to and back from their route at different 
//times. The NPC is animated and will stop walking when you talk to it. Talking to it will make it late, but if that is important you could set the speed based on movePercent. 
//For my purposes, this was not needed.
//This script would be placed on the NPC. There is a separate script in the scene that activates and deactivates the NPCs based on time 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class routeWalk : MonoBehaviour
{
    private float moveTime; 
    private float movePercent; 
    private float totalTimePassed; 
    public GameObject targetPos; 
    public GameObject lastPos; 
    public float timestarted; //minute after the hour that they begin on route
    private bool onceThrough;
    private bool ranThrough;
    public float howLong; //how many minutes do you want the route to take
    public int walkStart; //in hour for on the route to the location
    public int walkEnd; //in hour for on the route back from the location
    public Animator anim;
    public Rigidbody2D myRigidbody;
    public GameObject sayDialog; //the gameobject that becomes active when they talk
    private float distance;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("Moving", true);
    }

    void Update()
    {
        moveTime = howLong * 0.35f; //0.35 is the delay per second in the time system
        distance = Vector3.Distance(targetPos.transform.position, lastPos.transform.position);

        if (TimeSystem.currentTime.Hour >= walkStart && TimeSystem.currentTime.Hour <= walkEnd && sayDialog.activeInHierarchy == false) //if time is during walk there
        {

            totalTimePassed = TimeSystem.currentTime.Minute - timestarted;  

            if (onceThrough == false && totalTimePassed < howLong) //doesn't transform if the minute past the hour is passed
            {
                movePercent = (totalTimePassed / howLong);
                transform.position = (((targetPos.transform.position - lastPos.transform.position) * movePercent) + lastPos.transform.position);
                onceThrough = true;
            }


            else 
            {
                anim.SetBool("Moving", true);
                transform.position = Vector2.MoveTowards(transform.position, targetPos.transform.position, (distance / moveTime) * Time.deltaTime); 
                Vector3 temp = Vector3.MoveTowards(transform.position, targetPos.transform.position, (distance / moveTime) * Time.deltaTime);
                changeAnim(temp - transform.position);

                if (transform.position.x == targetPos.transform.position.x && transform.position.y == targetPos.transform.position.y) //sets game object inactive when they get to the location
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else if (sayDialog.activeInHierarchy == false) //for on the route back
        {
            totalTimePassed = TimeSystem.currentTime.Minute - timestarted;
            if (onceThrough == false && totalTimePassed < howLong)
            {
                movePercent = (totalTimePassed / howLong); 
                transform.position = (((lastPos.transform.position - targetPos.transform.position) * movePercent) + targetPos.transform.position);
                onceThrough = true;
            }

            else
            {
                anim.SetBool("Moving", true);
                transform.position = Vector2.MoveTowards(transform.position, lastPos.transform.position, (distance / moveTime) * Time.deltaTime); // Sends 
                Vector3 temp = Vector3.MoveTowards(transform.position, lastPos.transform.position, (distance / moveTime) * Time.deltaTime);
                changeAnim(temp - transform.position);

                if (transform.position.x == lastPos.transform.position.x && transform.position.y == lastPos.transform.position.y)
                {
                    gameObject.SetActive(false);
                }
            }

        }
        else //if the dialogue box is open, stop the walking animation because they are not moving
        {
            anim.SetBool("Moving", false);
        }

    }
    //For animation
    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("MoveX", setVector.x);
        anim.SetFloat("MoveY", setVector.y);
    }
    public void changeAnim(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                SetAnimFloat(Vector2.right);
            }
            else if (direction.x < 0)
            {
                SetAnimFloat(Vector2.left);
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
            {
                SetAnimFloat(Vector2.up);
            }
            else if (direction.y < 0)
            {
                SetAnimFloat(Vector2.down);
            }
        }
    }
}
