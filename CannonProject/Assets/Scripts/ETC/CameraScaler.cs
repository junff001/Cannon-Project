using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float scaleFactor = 1f;
    private Vector3 beforePos; //������ ī�޶��� ��ġ

    void Start()
    {
        beforePos = Camera.main.transform.position;
    }

    void Update()
    {
       Vector3 delta = Camera.main.transform.position - beforePos;
        transform.Translate(delta * scaleFactor);

        beforePos = Camera.main.transform.position;
    }
}
