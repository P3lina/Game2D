using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{

    /*
     * TODOS
     * 
     * Überarbeitung der onGround-Methode?, da auch teilweise, wenn sich der Charakter schon in der Luft befindet trotzdem ein normaler Jump ausgeführt wird
     * Charakter sollte schneller sein
    */


    [SerializeField] private LayerMask platformLayerMask;
    public Animator animator;
    public GameObject bone1;
    public GameObject rocketlauncher;
    public GameObject rocketlauncherCollection;
    public GameObject emptyrocketlauncher;
    public GameObject rocketlauncherrocket;
    public GameObject rocketlauncherrocket_fire;
    public GameObject rocketlauncherfire;
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
            rocketlauncher.transform.rotation = Quaternion.Euler(0, 180, 0);
            rocketlauncherCollection.transform.rotation = Quaternion.Euler(0, 180, 0);
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
            rocketlauncher.transform.rotation = Quaternion.Euler(0, 0, 0);
            rocketlauncherCollection.transform.rotation = Quaternion.Euler(0, 0, 0);
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
        #region alle Rocketlauncherinzanzen zum Mauszeiger drehen
        Vector3 mousePos = UtilsClass.GetMouseWorldPosition();
        Vector3 direction = (mousePos - rocketlauncherCollection.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if(rocketlauncher.active)
            rocketlauncher.transform.eulerAngles = new Vector3(0, 0, angle - 180);
        if(rocketlauncherCollection.active)
            rocketlauncherCollection.transform.eulerAngles = new Vector3(0, 0, angle - 180);
            
        #endregion
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            rocketlauncherrocket_fire.transform.eulerAngles = new Vector3(0, 0, angle - 180);
            rocketlauncher.SetActive(false);
            rocketlauncherCollection.SetActive(true);
            rocketlauncherrocket.SetActive(false);
            rocketlauncherrocket_fire.SetActive(true);
            rocketlauncherrocket_fire.transform.position = rocketlauncherrocket.transform.position;
            rocketlauncherrocket_fire.transform.rotation = rocketlauncherrocket.transform.rotation;
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            rocketlauncherrocket_fire.GetComponent<Rigidbody2D>().velocity = (mousePosition - rocketlauncherrocket.transform.position).normalized;
            StartCoroutine(accelerateShot());
        }
    }


    IEnumerator accelerateShot()
    {
        if (rocketlauncherrocket_fire.active && rocketlauncherrocket_fire.GetComponent<Rigidbody2D>().velocity.magnitude<80)
        {
            rocketlauncherrocket_fire.GetComponent<Rigidbody2D>().velocity = rocketlauncherrocket_fire.GetComponent<Rigidbody2D>().velocity * 2f;
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(accelerateShot());
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
