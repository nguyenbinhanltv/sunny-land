using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    //Start() variable
    private Rigidbody2D rb;
    private Animator ani;
    private Collider2D coll;

    //FSM
    private enum State { idle, running, jumping, falling, hurt, climb }
    private State st = State.idle;

    //ladder varible
    [HideInInspector] public bool canClimb = false;
    [HideInInspector] public bool bottomLadder = false;
    [HideInInspector] public bool topLadder = false;
    public Ladder ld;
    private float naturalGravity;
    [SerializeField] float climbSpeed = 3f;

    //variable inspector
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] public int cherries = 0;
    [SerializeField] private TextMeshProUGUI cherriesText;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource cherry;
    [SerializeField] private int health;
    [SerializeField] private Text healthAmount;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        footstep = GetComponent<AudioSource>();
        healthAmount.text = health.ToString();
        naturalGravity = rb.gravityScale;
    }

    private void Update()
    {
        if(st == State.climb)
        {
            Climb();
        }

        else if(st != State.hurt)
        {
            moveControl();
        }
        animationState();
        ani.SetInteger("State", (int)st);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            cherries += 1;
            cherry.Play();
            cherriesText.text = cherries.ToString();
        }
        if(collision.tag == "PowerUp")
        {
            Destroy(collision.gameObject);
            cherry.Play();
            jumpForce += 2f;
            health += 2;
            //GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (st == State.falling)
            {
                enemy.JumpedOn();
                Jump();
            }
            else
            {
                st = State.hurt;
                Health();// máu <= 0 quay lại đầu
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy bên phải đụng zô bị thốn rồi né qua trái :v
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //Enemy bên trái đụng zô bị thốn rồi né qua phải :v
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }

    private void Health()
    {
        health -= 1;
        healthAmount.text = health.ToString();
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void moveControl()
    {
        //lấy Horizontal trong Axis của unity
        float hDirection = Input.GetAxis("Horizontal");

        //leo cầu thang :))
        if(canClimb && Mathf.Abs(Input.GetAxis("Vertical")) > .1f)
        {
            st = State.climb;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            transform.position = new Vector3(ld.transform.position.x, rb.position.y);
            rb.gravityScale = 0f;
        }

        //moving left
        if(hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }

        //moving right
        else if(hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }


        //jumping
        else if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        st = State.jumping;
    }

    private void animationState()
    {
        //animation climbing
        if (st == State.climb)
        {

        }

        //animation jumping
        else if(st == State.jumping)
        {
            if(rb.velocity.y < .1f)
            {
                st = State.falling;
            }
        }

        //animation falling
        else if(st == State.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                st = State.idle;
            }
        }

        //animation running
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            st = State.running;
        }

        //animation hurting
        else if(st == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f)
            {
                st = State.idle;
            }
        }

        //animation idle
        else
        {
            //Đứng yên
            st = State.idle;
        }
    }

    private void Footstep()
    {
        footstep.Play();
    }

    private void Climb()
    {
        float vDirection = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            canClimb = false;
            rb.gravityScale = naturalGravity;
            ani.speed = 1f;
            Jump();
            return;
        }

        //Climbing up
        else if(vDirection > .1f && !topLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
            ani.speed = 1f;
        }

        //Climbing down
        else if(vDirection < -.1f && !bottomLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
            ani.speed = 1f;
        }

        //Still
        else
        {
            ani.speed = 0f;
            rb.velocity = Vector2.zero;
        }
    }
}
