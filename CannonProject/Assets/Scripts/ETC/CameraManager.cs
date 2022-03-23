using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public void SelfDisable()
    {
        gameObject.SetActive(false);
    }

    public void SetDisable(float time)
    {
        Invoke("SelfDisable", time);
    }
}
