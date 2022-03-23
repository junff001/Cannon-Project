using System.Collections;
using UnityEngine;

public class CastleManager : MonoSingleton<CastleManager>
{
    private IEnumerator CastleCrop;
    private WaitForSeconds ws = new WaitForSeconds(60);
    private GameManager gameMng;

    private void Awake()
    {
        CastleCrop = CastleCo();
    }

    private void Start()
    {
        gameMng = GameManager.Instance;
        LimitSilver();
        StartCoroutine(CastleCrop);
    }

    private IEnumerator CastleCo() //일정 시간마다 골드 쌓인다
    { 
        while (true)
        {
            yield return ws;

            if (gameMng.savedData.userInfo.currentSilver < gameMng.savedData.userInfo.maxSilver)
            {
                gameMng.savedData.userInfo.currentSilver += gameMng.savedData.userInfo.cropSilver;
                LimitSilver();
            }
        }
    }

    private void LimitSilver() //골드가 최대 보유량 넘지 않게 함
    {
        if (gameMng.savedData.userInfo.currentSilver > gameMng.savedData.userInfo.maxSilver)
        {
            gameMng.savedData.userInfo.currentSilver = gameMng.savedData.userInfo.maxSilver;
        }
    }
}
