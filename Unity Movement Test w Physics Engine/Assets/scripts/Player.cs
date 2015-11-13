using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    
    public float maxspeed = 4f;             //maximum character speed: 4 Float (unless decimal)
    
    private Rigidbody2D rb2D;               //access to rigidbody: lets character be affected by unity physics engine
                                            //Linear Drag: Resistance of forces
                                            //Angular Drag: How much character can turn
    private Animator animator;              //access to animator

    // Use this for initialization
    protected virtual void Start()
    {
        //gets the rigidbody2d of the character
        rb2D = GetComponent<Rigidbody2D>();
        //gets the animation control of the character
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontal = 0;
        float vertical = 0;

        //checks what key got pressed down and moves character accordingly
        //directions: 1 right, 2 up, 3 left, 4 down
        //isStair is a control, checking if the character is touching the stairs to be able to go up or down.
        //gravity is no longer turned on and off at the stairs due to many problems
        //Vertical: Move Up + /Down -
        //Horizontal: Move Left - /Right +
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (animator.GetBool("isStair") == true)
            {
                //the force here is lesser because gravity will do most of the work
                animator.SetInteger("Direction", 4);
                vertical = -0.5f;
            }
        }
        
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            if (animator.GetBool("isStair") == true)
            {
                //the force here is greater to surpass gravity
                animator.SetInteger("Direction", 2);
                vertical = 5.5f;
            }
        }
        
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            animator.SetInteger("Direction", 3);
            horizontal = -2.5f;
        }
        
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            animator.SetInteger("Direction", 1);
            horizontal = 2.5f;
        }


        //checks if keys are being pressed to run the animation
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            animator.SetBool("isMoving", true);
        else
            animator.SetBool("isMoving", false);

        //movement cycle is initiated, there is no need to check for collisions because we are using the Unity Physics Engine
        //so long as that it used, it checks for colissions by itself.
        Move(horizontal, vertical);
    }

    protected void Move(float xDir, float yDir)
    {
        //creates a vector out of the directions inputed by the player in Update() function
        Vector2 movementForce = new Vector2(xDir, yDir);
        
        //adds the force to the rigidbody so it moves, multiplying it so it gets past the linear drag and actually creates movement
        //movementForce chooses the xy direction and force. 
        //Linear Drag example: player stops pressing keys and a force stops them from moving.
        //addForce example: push player to move with forces.
        rb2D.AddForce(movementForce * 2.3f);

        //speed control, messing with values to know which speed is best
        //used to set a maximum velocity (speed of player) for the character, to avoid the force making them go too fast.
        if (rb2D.velocity.x > maxspeed)
            rb2D.velocity = new Vector2(maxspeed, rb2D.velocity.y);
        if (rb2D.velocity.y > maxspeed)
            rb2D.velocity = new Vector2(rb2D.velocity.x, maxspeed);

        return;
    }


    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    //Trigger: is attached to Box Collider 2D (on vine prefab has a tag: Stairs)
    private void OnTriggerEnter2D(Collider2D otherObjectTouched)
    {
        //Check if the (inspector: tag) of the trigger collided with is stairs (vine image).
        if (otherObjectTouched.tag == "Stairs")
        {
            animator.SetBool("isStair", true);
        }
        
        //checks if the tag of the trigger collided with is the barrier
        //this barrier exists to tell the game that the player is now off the stairs and can no longer go up or down
        if (otherObjectTouched.tag == "notStairs")
        {
            animator.SetBool("isStair", false);
        }
    }
}
