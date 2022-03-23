using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CannonBallScript : MonoBehaviour
{
    public LayerMask WhatIsTarget; //���ٴ�, ���� 1100 0000
    public float expRadius = 4f; //���� �ݰ�
    private Rigidbody2D rigid;
    private int crateLayer;
    public float expPower;

    public CinemachineVirtualCamera cannonCam;

    public GameObject effect;



    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        crateLayer = LayerMask.NameToLayer("CRATE"); //���̾� ����ũ�� �̸��� ��ȣ�� �ٲ���
    }

    public void Shoot(Vector2 direction, float power, CinemachineVirtualCamera cam)
    {
        cannonCam = cam;
        rigid.AddForce(direction * power);
    }

    private void OnCollisionEnter2D(Collision2D collision) //collision�� �Ű�����
    {
       int layer =  collision.gameObject.layer;
        if (((1 << layer) & WhatIsTarget) > 0) //��ģ�ٸ�
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(        //���� ���� ����
                transform.position, expRadius, 1 << crateLayer); //���� �߽�, �ݰ�, ���̾��ũ
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
