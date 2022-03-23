using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMoney : MonoBehaviour
{
    public Animator ani;
    bool open = true;
    GameManager gma;

    public void Start()
    {
        gma = GameManager.Instance;
    }
    public void Update()
    {
    }

    public void Click()
    {
        ani.SetInteger("open", open ? 1 : 2);
        open = !open;

        gma.savedData.userInfo.silver += gma.savedData.userInfo.currentSilver;
        gma.savedData.userInfo.currentSilver = 0;
    }
}
