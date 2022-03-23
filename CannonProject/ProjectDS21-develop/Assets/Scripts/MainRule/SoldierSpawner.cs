using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SoldierSpawner : MonoBehaviour  //������ ����Ϸ��ٰ� �� �Ŵ����� �ʹ� ������� ��Ʋ�� ����ϴ� ��ũ��Ʈ
{
    [SerializeField] private CameraMove camMove;

    public Transform[] castleTr;
    public Transform[] spawnTr; //���� ������ ���� �θ� ����. ���߿� ����� ������ �� �� �̰��� �����̸� ��
    public Transform playersTarget; //����� ��ü�� ������ ���� Ÿ�� ��ġ
    public GameObject dust;
    public Transform[] removeTrs; //10�� ����
    private Vector3 dustScale;

    public float[] spawnLocalYArr;  //������� ��ȯ�� ���� ��ȯ�� ��ǥ�� y����
    public float fallY = -8.73f;  //��ַ� �ʰ��� ������ ������ ���� ȭ�� �� Ÿ����ǥ�� y��
    public float xInterval;  //����� ������ X��ǥ�� �������� �� ����
    public float[] firstX, spawnX;  //���� ���簡 ��ó���� ���� ��ġ X���� ���簡 ȭ��ۿ��� ��ȯ �� ���� ��ġ�� x��

    [SerializeField] private List<Soldier> soldierList = new List<Soldier>(), enemySoldierList = new List<Soldier>();
    private int index = 0;
    private Vector2[] playersStartPos = new Vector2[2];
    private bool bFighting=false;
    private bool isWin;
    private bool isDraw;
    private int runBool;
    private int atkTrigger;

    public float soldierRotSpeed = 115f;
    public float fallSpeed = 10f;  //�������� �ӵ�

    [SerializeField] float[] drawX;  //���º� ���� �ϱ��� �� �� ��ȯ�� X��ǥ
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

    public void SpawnMySoldiers(int count) //���縦 count��ŭ ��ȯ
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

    public void SpawnChief()  //��θӸ� ��ȯ�ϰ� �Ϲ� ���� �� ���̰�
    {
        Camera.main.cullingMask = ~(1 << soldierLayer);
        for(int i=0; i<2; i++)
        {
            Chief chief = PoolManager.GetItem<Chief>();
            chief.transform.parent = spawnTr[0];  //�������� �ϴ� �� ��θӸ��� �÷��̾� �׷쿡 �־�����
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
        if (!isAnimation)  //������ ������ ������ ������ �� ����
        {
            soldierList.ForEach(x => x.gameObject.SetActive(false));
            enemySoldierList.ForEach(x => x.gameObject.SetActive(false));
            enemySoldierList.Clear();
        }
        else  //��ַ��� �Ѿ ����� �������� �ִϸ��̼� �ְ� ������� ��
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

    public void BattleStart(int enemyCount)  //���� ���� ����
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
        else if(isDraw && enemyCount==0)  //�Ѵ� �뵥������ ������ ���º�
        {
            StartCoroutine(BattleCo());
        }
        else
        {
            Debug.Log("�������� ���� ����ó��");
        }
    }

    private IEnumerator BattleCo()  //���� �����ϰ� ��Ʋ��
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

            //�̱� ���� ���󰡼� �̱����� ���� �����ϰ� �������� �ش�
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

    private IEnumerator DrawCo()  //���º� ���� ����
    {
        SpawnChief();

        yield return RuleManager.Instance.DrawBattle();
        yield return new WaitForSeconds(0.3f);

        //�̱� ��θӸ��� �� ������ ���� �����ϰ� ������ ������������ �ణ �Ŀ� �̱� �� ������� ������ ��� ����� ������� �̱� ���� ������ ���� ������ �ش�. 
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

    //���������� ����� ���󰡰� �ں����� �� �� Ÿ�� ��ġ ����
    private Transform GetRemoveTrm(bool player) => removeTrs[Random.Range(player?0:5, player?5:removeTrs.Length)];
}
