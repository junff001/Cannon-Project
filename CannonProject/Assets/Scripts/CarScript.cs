using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    public float moveSpeed = 2f;
    Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Vector2 position = rigid.position;
        //if (Input.GetKey(KeyCode.D))
        //{
        //    position.x += moveSpeed * Time.deltaTime;
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    position.x -= moveSpeed * Time.deltaTime;
        //}
        //transform.position = position;

        float moveX = 0;
        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveX = -1;
        }
       
        rigid.velocity = new Vector2(moveX * moveSpeed, rigid.velocity.y);
        
    }

    Quaternion beforeQut;

    private void FixedUpdate()
    {

        float z = transform.rotation.eulerAngles.z;
        if( z > 50f || z < -50f)
        {
            transform.rotation = beforeQut;
        }
        else
        {
            beforeQut = transform.rotation;
        }

    }
}
