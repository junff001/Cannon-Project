using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{

    public GameObject brokenPrefab;

    private Rigidbody2D rigid;
    public CannonHandler ch;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void AddExplosion(Vector3 pos, float power)
    {
        Vector3 dir = transform.position - pos; //��ź�� ��ġ�� �� ��ġ���� ����.
        power *= 1 / dir.sqrMagnitude;
        //rigid.AddForce(dir.normalized * power, ForceMode2D.Impulse);
        GameObject broken = Instantiate(brokenPrefab, transform.position, transform.rotation);
        BrokenScript bs = broken.GetComponent<BrokenScript>();
        bs.AddExplosion(dir.normalized, power);

        //ch.DecBox();

        Destroy(gameObject);
        Destroy(broken, 2f);
    }
} 
