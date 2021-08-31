using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;
    public Animator animator;
    private float maxSpeed = 10;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private bool doubleJumped = false;
    private void setSpeed(float value)
    {
        animator.SetFloat("Speed", value);
    }

    private float getSpeed()
    {
        return animator.GetFloat("Speed");
    }

    private void setWalkingLeft(bool value)
    {
        animator.SetBool("walking_left", value);
    }

    private bool getWalkingLeft()
    {
        return animator.GetBool("walking_left");
    }

    private bool getIsJumping()
    {
        return animator.GetBool("isJumping");
    }

    private void setIsJumping(bool value)
    {
        animator.SetBool("isJumping", value);
    }

    private bool getIsFalling()
    {
        return animator.GetBool("isFalling");
    }

    private void setIsFalling(bool value)
    {
        animator.SetBool("isFalling", value);
    }

    private bool getDoubleJumping()
    {
        return animator.GetBool("doubleJumping");
    }

    private void setDoubleJumping(bool value)
    {
        animator.SetBool("doubleJumping", value);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.D))
        {
            if (getSpeed() < maxSpeed)
            {
                setWalkingLeft(false);
                setSpeed(getSpeed() + 0.5f);
                rb.velocity = new Vector2(getSpeed(), rb.velocity.y);
                animator.speed = getSpeed()/5;
            }
            
        }
        if (getSpeed() > 0 && getWalkingLeft()==false)
        {
            setSpeed(getSpeed() - 0.3f);
            rb.velocity = new Vector2(getSpeed(), rb.velocity.y);
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (getSpeed() < maxSpeed)
            {
                setWalkingLeft(true);
                setSpeed(getSpeed() + 0.5f);
                rb.velocity = new Vector2(-getSpeed(), rb.velocity.y);
                animator.speed = getSpeed()/5;
            }

        }
        if (getSpeed() > 0 && getWalkingLeft()==true)
        {
            setSpeed(getSpeed() - 0.3f);
            rb.velocity = new Vector2(-getSpeed(), rb.velocity.y);
        }
        if (Mathf.Abs(getSpeed()) < 0.3f && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            setWalkingLeft(false);
            setSpeed(0);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        if (getIsJumping() == true) setIsJumping(false);
        if (getIsFalling() && isOnGround())
        {
            animator.speed = 1;
            setIsFalling(false);
        }
        if (!isOnGround())
        {
            animator.speed = 1;
            setIsFalling(true);
            setDoubleJumping(false);
        }
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround() == true || Input.GetKeyDown(KeyCode.Space) && doubleJumped == false)
        {
            animator.speed = 1;
            if (isOnGround() == true) { 
                setIsJumping(true);
                setIsFalling(true);
                StartCoroutine(jumpDelayed());
            }
            else if (doubleJumped == false)
            {
                setDoubleJumping(true);
                doubleJumped = true;
                setIsFalling(true);
                StartCoroutine(jumpDelayed());
            }
        }
    }
    IEnumerator jumpDelayed()
    {
        yield return new WaitForSeconds(0.14f);
        rb.velocity = Vector2.up * 10;
    }

    private bool isOnGround()
    {
        float yOffset = 0.02f;
        RaycastHit2D rh = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, yOffset, platformLayerMask);
        if(rh.collider!=null)
            doubleJumped = false;
        return rh.collider!=null;
    }
}
