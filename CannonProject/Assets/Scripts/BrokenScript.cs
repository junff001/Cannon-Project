using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenScript : MonoBehaviour
{
    private Rigidbody2D[] childRigid;

    void Awake()
    {
        childRigid = GetComponentsInChildren<Rigidbody2D>();
    }
    //자식에 있는 모든 리지드바디에 힘을 부여한다.
    public void AddExplosion(Vector3 dir, float power)
    {
        foreach (Rigidbody2D r in childRigid)
        {
            r.AddForce(dir * power, ForceMode2D.Impulse);
        }
    }
}