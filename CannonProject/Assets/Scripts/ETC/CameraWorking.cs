using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWorking : MonoBehaviour
{
    public float minX = 0;
    public float maxX = 15f;
    public float minY = 0;
    public float maxY = 3;
    public float moveSpeed = 4f;
    
    void Update()
    {
        //Raw = (-1, 0, 1) 만 받는다
        float x = Input.GetAxisRaw("Horizontal"); // Horizontal을 x에 담음
        float y = Input.GetAxisRaw("Vertical");
        Vector3 nextPos = transform.position + new Vector3(x * moveSpeed * Time.deltaTime, 
            y * moveSpeed * Time.deltaTime, 0);
        //더미 카메라의 위치 * Horizontal * moveSpeed * Time.deltaTime === x축
        //더미 카메라의 위치 * Vertical * moveSpeed * Time.deltaTime === y축

        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);
        transform.position = nextPos;
        
        

        
    }
}
