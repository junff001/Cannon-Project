using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CannonBallScript : MonoBehaviour
{
    public LayerMask WhatIsTarget; //땅바닥, 상자 1100 0000
    public float expRadius = 4f; //폭발 반경
    private Rigidbody2D rigid;
    private int crateLayer;
    public float expPower;

    public CinemachineVirtualCamera cannonCam;

    public GameObject effect;



    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        crateLayer = LayerMask.NameToLayer("CRATE"); //레이어 마스크의 이름을 번호로 바꿔줌
    }

    public void Shoot(Vector2 direction, float power, CinemachineVirtualCamera cam)
    {
        cannonCam = cam;
        rigid.AddForce(direction * power);
    }

    private void OnCollisionEnter2D(Collision2D collision) //collision은 매개변수
    {
       int layer =  collision.gameObject.layer;
        if (((1 << layer) & WhatIsTarget) > 0) //겹친다면
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(        //원의 영역 감지
                transform.position, expRadius, 1 << crateLayer); //원의 중심, 반경, 레이어마스크
            if (cols.Length >= 1)
            {
                foreach(Collider2D c in cols)
                {
                    CrateScript cs = c.gameObject.GetComponent<CrateScript>();
                    if(cs != null)
                    {
                        cs.AddExplosion(transform.position, expPower);
                    }
                }
                
            }
            cannonCam.gameObject.GetComponent<CameraManager>().SetDisable(1.5f);
            GameObject eff = Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if(transform.position.y <= -10)
        {
            cannonCam.gameObject.GetComponent<CameraManager>().SetDisable(1.5f);
            Destroy(this.gameObject);
        }
    }

    


}
