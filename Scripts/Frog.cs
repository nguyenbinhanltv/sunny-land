using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    //variable inspector
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    [SerializeField] private float jumpLength = 10f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private LayerMask ground;

    //Start() variable
    private Collider2D coll;
    private bool facingLeft = true;

    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        //form Jump to Fall
        if(ani.GetBool("Jumping"))
        {
            if(rb.velocity.y < .1)
            {
                ani.SetBool("Falling", true);
                ani.SetBool("Jumping", false);
            }
        }

        //form Fall to Idle
        if(coll.IsTouchingLayers(ground) && ani.GetBool("Falling"))
        {
            ani.SetBool("Falling", false);
        }
    }

    private void Move()
    {
        //Frog di chuyển qua bên phải
        if (facingLeft)
        {
            if (transform.position.x > leftCap)
            {
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1);
                }

                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                    ani.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = false;
            }
        }

        //Frog di chuyển qua bên trái
        else
        {
            if (transform.position.x < rightCap)
            {
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }

                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                    ani.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }

}
