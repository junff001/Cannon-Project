using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SoldierSpawner : MonoBehaviour  //스폰만 담당하려다가 룰 매니저가 너무 길어져서 배틀도 담당하는 스크립트
{
    [SerializeField] private CameraMove camMove;

    public Transform[] castleTr;
    public Transform[] spawnTr; //병사 옵젝들 담을 부모 옵젝. 나중에 병사들 움직일 때 걍 이것을 움직이면 됨
    public Transform playersTarget; //병사들 전체를 움직일 때의 타겟 위치
    public GameObject dust;
    public Transform[] removeTrs; //10개 정도
    private Vector3 dustScale;

    public float[] spawnLocalYArr;  //병사들이 소환될 때의 소환될 좌표의 y값들
    public float fallY = -8.73f;  //통솔력 초과로 밑으로 떨어질 때의 화면 밖 타겟좌표의 y값
    public float xInterval;  //병사들 사이의 X좌표를 기준으로 한 간격
    public float[] firstX, spawnX;  //각각 병사가 맨처음에 있을 위치 X값과 병사가 화면밖에서 소환 될 때의 위치의 x값

    [SerializeField] private List<Soldier> soldierList = new List<Soldier>(), enemySoldierList = new List<Soldier>();
    private int index = 0;
    private Vector2[] playersStartPos = new Vector2[2];
    private bool bFighting=false;
    private bool isWin;
    private bool isDraw;
    private int runBool;
    private int atkTrigger;

    public float soldierRotSpeed = 115f;
    public float fallSpeed = 10f;  //떨어지는 속도

    [SerializeField] float[] drawX;  //무승부 떠서 일기토 할 때 소환될 X좌표
    private int soldierLayer;
    private Chief[] _chiefs = new Chief[2];

    private void Start()
    {
        GameObject soldier = GameManager.Instance.idToSoldier[RuleManager.Instance.MyCastle.soldier];
        PoolManager.CreatePool<Soldier>(soldier, transform, 35);
        playersStartPos[0] = spawnTr[0].position;
        playersStartPos[1] = spawnTr[1].position;
        runBool = Animator.StringToHash("run");
        atkTrigger = Animator.StringToHash("attack");
        dustScale = dust.transform.localScale;
        dust.transform.localScale = Vector3.zero;

        GameObject chief = GameManager.Instance.idToChief[RuleManager.Instance.MyCastle.chief];
        PoolManager.CreatePool<Chief>(chief, transform, 3);
        soldierLayer = LayerMask.NameToLayer("Soldier");
    }

    public void SpawnMySoldiers(int count) //병사를 count만큼 소환
    {
        for(int i=0; i<count; i++)
        {
            Soldier soldier = PoolManager.GetItem<Soldier>();
            soldierList.Add(soldier);
            soldier.transform.parent = spawnTr[0];
            float x = ( (soldierList.Count-1) / spawnLocalYArr.Length) * xInterval;
            soldier.InitSet(-1, new Vector2(spawnX[0], spawnLocalYArr[index]), new Vector2(firstX[0]-x,spawnLocalYArr[index]) );
            index = ++index % spawnLocalYArr.Length;
        }
    }

    public void SpawnChief()  //우두머리 소환하고 일반 병사 안 보이게
    {
        Camera.main.cullingMask = ~(1 << soldierLayer);
        for(int i=0; i<2; i++)
        {
            Chief chief = PoolManager.GetItem<Chief>();
            chief.transform.parent = spawnTr[0];  //귀찮으니 일단 적 우두머리도 플레이어 그룹에 넣어주자
            chief.InitSet(i == 0 ? -1 : 1, new Vector2(drawX[i], fallY), new Vector2(drawX[i], spawnLocalYArr[3]));
            _chiefs[i] = chief;
        }
    }

    public void ResetData(bool isAnimation) 
    {
        index = 0;
        spawnTr[0].position = playersStartPos[0];
        spawnTr[1].position = playersStartPos[1];
        bFighting = false;
        if (!isAnimation)  //전투가 끝나고 다음판 시작할 때 해줌
        {
            soldierList.ForEach(x => x.gameObject.SetActive(false));
            enemySoldierList.ForEach(x => x.gameObject.SetActive(false));
            enemySoldierList.Clear();
        }
        else  //통솔력을 넘어서 병사들 떨어지는 애니메이션 주고 사라지게 함
        {
            soldierList.ForEach(x =>
            {
                x.rotSpeed = soldierRotSpeed;
                x.Fall(new Vector3(x.transform.localPosition.x, fallY), fallSpeed);
            });
        }
        isDraw = false;
        isWin = false;
        soldierList.Clear();
    }

    public void BattleStart(int enemyCount)  //전투 시작 세팅
    {
        bFighting = true;
        soldierList.ForEach(x => x.ani.SetBool(runBool, true));

        if (soldierList.Count == enemyCount) isDraw = true;
        else isWin = soldierList.Count > enemyCount ? true : false;

        if(!isDraw)
        {
            RuleManager.Instance.camMove.SetMoveState(true);
            RuleManager.Instance.camMove.target = spawnTr[0];
        }

        index = 0;
        for (int i = 0; i < enemyCount; i++)
        {
            Soldier soldier = PoolManager.GetItem<Soldier>();
            enemySoldierList.Add(soldier);
            soldier.transform.parent = spawnTr[1];
            float x = ((enemySoldierList.Count - 1) / spawnLocalYArr.Length) * xInterval;
            soldier.InitSet(1, new Vector2(firstX[1] + x, spawnLocalYArr[index]), Vector2.zero, false);
            soldier.ani.SetBool(runBool, true);
            index = ++index % spawnLocalYArr.Length;
        }

        Sequence seq = DOTween.Sequence();
        if (enemyCount > 0 && soldierList.Count > 0 && !isDraw)
        {
            seq.Append(spawnTr[0].DOMove(playersTarget.position, 2.7f));
            spawnTr[1].DOMove(playersTarget.position, 1.4f);
            seq.InsertCallback(2.8f, () => { dust.SetActive(true); dust.transform.DOScale(dustScale, 0.3f); });
            seq.AppendCallback(() => StartCoroutine(BattleCo())).Play();
        }
        else if( (enemyCount==0 && soldierList.Count > 0) || (enemyCount > 0 && soldierList.Count == 0) )
        {
            StartCoroutine(BattleCo());
        }
        else if(isDraw && enemyCount>0)
        {
            StartCoroutine(DrawCo());
        }
        else if(isDraw && enemyCount==0)  //둘다 노데미지로 끝나는 무승부
        {
            StartCoroutine(BattleCo());
        }
        else
        {
            Debug.Log("예상하지 못한 예외처리");
        }
    }

    private IEnumerator BattleCo()  //전투 시작하고 배틀중
    {
        while(enemySoldierList.Count>0 && soldierList.Count>0)
        {
            yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));

            enemySoldierList[0].Fall(GetRemoveTrm(false).position, fallSpeed, false);
            enemySoldierList[0].rotSpeed = -soldierRotSpeed;
            soldierList[0].Fall(GetRemoveTrm(true).position, fallSpeed, false);
            soldierList[0].rotSpeed = soldierRotSpeed;

            enemySoldierList.RemoveAt(0);
            soldierList.RemoveAt(0);
        }

        if (!isDraw)
        {
            dust.transform.DOScale(Vector3.zero, 0.25f);
            yield return new WaitForSeconds(0.5f);
            dust.SetActive(false);

            //이긴 쪽을 따라가서 이긴쪽이 성을 공격하고 데미지를 준다
            Transform winTr = isWin ? spawnTr[0] : spawnTr[1];
            RuleManager.Instance.camMove.target = winTr;
            winTr.DOMove(castleTr[isWin ? 1 : 0].position, 2.5f);
            yield return new WaitForSeconds(2.6f);

            List<Soldier> sList = isWin ? soldierList : enemySoldierList;
            sList.ForEach(x => x.ani.SetTrigger(atkTrigger));
            camMove.ShakeCamera(0.3f, 2f);
            yield return new WaitForSeconds(1f);
            RuleManager.Instance.Damaged(isWin, sList.Count);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            RuleManager.Instance.Damaged(false, -1);
        }
        yield return new WaitForSeconds(1.5f);
        ResetData(false);
    }

    private IEnumerator DrawCo()  //무승부 시의 전투
    {
        SpawnChief();

        yield return RuleManager.Instance.DrawBattle();
        yield return new WaitForSeconds(0.3f);

        //이긴 우두머리가 진 놈한테 가서 공격하고 진놈은 나가떨어지고 약간 후에 이긴 놈 사라지고 진놈의 모든 병사는 사라지고 이긴 놈이 성으로 가서 뎀지를 준다. 
        Chief winner = isWin ? _chiefs[0] : _chiefs[1];
        Chief loser = isWin ? _chiefs[1] : _chiefs[0];

        Transform t = loser.transform;
        float x = isWin ? -xInterval : xInterval;
        winner.ani.SetBool(runBool, true);
        winner.transform.DOLocalMove(new Vector2(t.localPosition.x + x,t.localPosition.y), 0.5f);
        yield return new WaitForSeconds(0.5f);

        winner.ani.SetTrigger(atkTrigger);
        yield return new WaitForSeconds(0.2f);

        loser.Fall(new Vector2(loser.transform.localPosition.x, fallY), fallSpeed);
        yield return new WaitForSeconds(0.6f);

        winner.gameObject.SetActive(false);
        Camera.main.cullingMask = -1;

        RuleManager.Instance.FadeObj(true,1,1);
        yield return new WaitForSeconds(0.9f);

        RuleManager.Instance.camMove.SetMoveState(true);
        if (isWin)
        {
            enemySoldierList.ForEach(x => x.gameObject.SetActive(false));
            enemySoldierList.Clear();
        }
        else
        {
            soldierList.ForEach(x => x.gameObject.SetActive(false));
            soldierList.Clear();
        }
        isDraw = false;

        StartCoroutine(BattleCo());
    }

    public void DecideWinner(int p, int e)
    {
        isWin = p > e;
    }

    /*private int GetMinCount()
    {
        if(enemySoldierList.Count>soldierList.Count)
        {
            return enemySoldierList.Count;
        }
        else if(enemySoldierList.Count<=soldierList.Count)
        {
            return soldierList.Count;
        }
        return 0;
    }*/

    //전투상태의 병사들 날라가게 자빠지게 할 때 타겟 위치 정함
    private Transform GetRemoveTrm(bool player) => removeTrs[Random.Range(player?0:5, player?5:removeTrs.Length)];
}
