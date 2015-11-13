//Reference Tutorial for 'SmoothMovement': https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial

using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float moveTime = 0.1f;
    public float conespeed = 0.1f;
    public LayerMask blockingLayer;
        
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    private Animator animator;              //access to animator

    // Use this for initialization
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        animator = GetComponent<Animator>();
    }

    //Updates WASD keys
    void Update()
    {
        float horizontal = 0;
        float vertical = 0;

        //checks what key got pressed down and moves character accordingly
        //directions: 1 left, 2 up, 3 right, 4 down
        if (Input.GetKeyDown("s") == true)
        {
            if (animator.GetBool("isStair") == true)
            {
                animator.SetInteger("Direction", 4);
                vertical = -0.5f;
            }
        }
        if (Input.GetKeyDown("w") == true)
        {
            if (animator.GetBool("isStair") == true)
            {
                animator.SetInteger("Direction", 2);
                vertical = 0.5f;
            }
        }
        if (Input.GetKeyDown("a") == true)
        {
            animator.SetInteger("Direction", 3);
            horizontal = -0.5f;
        }
        if (Input.GetKeyDown("d") == true)
        {
            animator.SetInteger("Direction", 1);
            horizontal = 0.5f;
        }


        //checks if character movement is being pressed by keys, to run the animation:
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        animator.SetBool("isMoving", true);
        else
        animator.SetBool("isMoving", false);
            
        Move(horizontal, vertical); //checking if he smashes into something
    }

    //X and Y from Update()
    protected bool Move(float xDir, float yDir)
    {
        RaycastHit2D hit; //Raycast collision: (drawing a line from where you are to your new position)
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        //For skipping raycast collision when character is on the stairs:
        //TRUE: Not check for collisions (platform)
        //FALSE: Check collisions
        //This is so character can get on the top platform when on the stairs.
        if (animator.GetBool("isStair") == false)
        {
            //collision check
            if (hit.transform == null)
            {
                //Parent routine: 'move'
                //Coroutine is another movement call that runs parallel to parent one
                StartCoroutine(SmoothMovement(end));
                return true;
            }
            return false;
        }
        else
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
    }

    //Maths for movement of the character... still complicated to understand
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            kinematicCheck();
            animator.SetBool("isStair", false);
            yield return null;
        }
    }

    //Turn physics engine from Unity ON and OFF
    //Kinematic: character will NOT obey Unity's physics engine
    protected void kinematicCheck()
    {
        //Character is affected by the physics engine when falling
        //floor and vine (stairs): Kinematic ON
        // -5 is the floor on Y
        if (transform.position.y < -5 || (transform.position.x > -2.95 && transform.position.x < -1.45))
            rb2D.isKinematic = true;
        //stand and move on platform: Kinematic ON
        else if (transform.position.x > -5.1 && transform.position.x < 3.49 && transform.position.y > 3.29 && transform.position.y < 4.1)
            rb2D.isKinematic = true;
        else
            //kinematicCheck OFF: Fall using gravity
            rb2D.isKinematic = false;

            return;
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    //EX: Vine1 (prefab folder) > tag: stairs
    //Trigger: Check if the tag of the trigger collided with is stairs.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Stairs")
        {
            //(animation folder) > Conan > parameter: isStairs
            animator.SetBool("isStair", true);
        }
    }
}
