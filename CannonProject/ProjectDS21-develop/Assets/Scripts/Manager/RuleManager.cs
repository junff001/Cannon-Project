using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum JQK
{
    NONE,
    J,
    Q,
    K
}

public enum Shape
{
    CLOVER,
    HEART,
    DIAMOND,
    SPADE
}

public class RuleManager : MonoSingleton<RuleManager>
{
    public bool isReady = false;

    public List<CardScript> allCardList;
    public PlayerScript player;
    public PlayerScript enemy;
    [SerializeField] private List<CardScript> trashCardList = new List<CardScript>();
    [SerializeField] private List<CardScript> deckCardList = new List<CardScript>();
    [SerializeField] private CastleInfo enemyCastle = new CastleInfo();
    [SerializeField] private MainInfo myCastle = new MainInfo();
    public CastleInfo EnemyCastle { get { return enemyCastle; } }
    public MainInfo MyCastle { get { return myCastle; } }
    public CameraMove camMove;
    public SoldierSpawner spawner;

    private RaycastHit2D hit;

    public GameObject clickPrevObj;
    public CanvasGroup jqkDecidePanel, continuePanel, viewPanel, resultCvsg, noPrevClick;
    [SerializeField] private Image[] jqkImgs;
    [SerializeField] private Text[] jqkTexts;

    public CardRuleData ruleData;
    private float zPos = 0f;
    private bool isGameStart;  //reset�Ŀ� true
    private bool isMovable;  //���� �ൿ ����?
    private bool isThrowing = false;  //�� ī�� ������ ����?
    private bool isMyTurn;  //�� ������
    private bool isCardTouch;  //ī�� Ȯ�� ����?
    private bool isDrawBattle=false, isMyTurnInDraw;

    public Transform[] trashTrs;
    private Vector3 rot1 = new Vector3(0, -90, 0);

    private WaitForSeconds ws1 = new WaitForSeconds(0.8f);
    private WaitForSeconds ws2 = new WaitForSeconds(0.3f);
    private WaitForSeconds ws3 = new WaitForSeconds(0.1f);
    private WaitForSeconds ws4 = new WaitForSeconds(0.03f);

    public PRS orgCardPRS;

    [SerializeField] private Text PTotalTxt, ETotalTxt;
    [SerializeField] private Button stopBtn, stopBtn2;
    [SerializeField] private Text moneyTxt, continueTxt, resultTxt, rewardTxt, turnTxt;
    [SerializeField] private Image cardImg;
    [SerializeField] private Text[] leftUpJQKTexts;
    [SerializeField] Text[] hpTxt;
    [SerializeField] Image[] hpFill;

    public Transform newTrashTr;

    int turn = 0;
    int enemyMaxHp;

    private void Awake()
    {
        Transform t = transform.GetChild(0);
        orgCardPRS = new PRS(t.localPosition, t.localRotation, t.localScale);
        allCardList = new List<CardScript>(transform.GetComponentsInChildren<CardScript>());

        //for (int i = 0; i < jqkImgs.Length; i++) jqkImgs[i].sprite = ruleData.backSprite;
        for (int i = 0; i < jqkImgs.Length; i++) jqkImgs[i].sprite = ruleData.jqkSpr[i];
        continueTxt.text = string.Format("����ϱ�({0}��ȭ �ʿ�)", ruleData.resapwnSilver);
       
    }

    private IEnumerator Start()
    {
        enemyCastle = GameManager.Instance.savedData.battleInfo.enemyCastle;
        myCastle = GameManager.Instance.savedData.battleInfo.myCastle;
        enemyMaxHp = enemyCastle.hp;
        GameManager.Instance.savedData.userInfo.silver -= myCastle.silver;
        moneyTxt.text = myCastle.silver.ToString();

        SetHpUI();
        allCardList.ForEach(x=>
        {
            x.SetSprite();
            x.transform.localPosition = new Vector3(Random.Range(ruleData.mixX[0],ruleData.mixX[1]),ruleData.mixY, 0);
        });  //��� ī���� ��������Ʈ�� �� ��������Ʈ�� �ϰ� ��ġ�� ���� �÷��� ���� �ִϸ��̼��� �غ��Ѵ�

        isReady = true;

        yield return DecideJQK();

        ResetGame();
    }

    private IEnumerator DecideJQK()  //JQK�������� ���ϰ� �׿� ���� �ִϸ��̼� ȿ��
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.loadingPanel.gameObject.SetActive(false);
        jqkDecidePanel.DOFade(1, 0.4f);  //JQK�г� ����

        for(int i=0; i<jqkImgs.Length; i++)
        {
            yield return ws2;
            int ran = Random.Range(20, 41);
            int num = 1;  //�� ���� for���� ������ ���������� �����ɰ���

            for(int j=0; j<ran; j++)
            {
                yield return new WaitForSeconds(0.07f);  //���߿� ���� �ؽ�Ʈ ��ȭ�ϴ� �ӵ� �ٿ���������(�ð� �Ǹ�). �ϴ��� ����ġ��
                num = num % 10 + 1;
                jqkTexts[i].text = num.ToString();
            }

            /*Sequence seq = DOTween.Sequence();
            seq.Append( jqkImgs[i].transform.DORotate(rot1, 0.12f));  //ī�� ȸ�� ȿ��
            seq.AppendCallback(() =>
            {
                jqkImgs[i].sprite = ruleData.jqkSpr[i];
                jqkImgs[i].transform.DORotate(Vector3.zero, 0.12f);
            }).Play();*/  //90������ ��������Ʈ �����ϰ� �ٽ� 0���� ȸ��
            //PoolManager.GetItem<SoundPrefab>().PlaySound(SoundEffectType.CARD_OVERTURN);  //������ �Ҹ�
            allCardList.FindAll(x => (int)x.jqk == i + 1).ForEach(y => y.Value = num);  //ī�� ����Ʈ���� ��� J(Ȥ�� Q�� K)�� ã�� �� ���� ������ ���� ����������
            leftUpJQKTexts[i].text = num.ToString();  
            yield return ws1;
        }

        jqkDecidePanel.DOFade(0, 0.4f);  //JQK�г� ����
        leftUpJQKTexts[0].transform.parent.parent.gameObject.SetActive(true);
    }

    public void DrawCard(bool isPlayer)  //��ο�
    {
        if ( (isMyTurn && isMovable && deckCardList.Count>0) || (!isMyTurn && !isPlayer) || (isMovable && isDrawBattle && isMyTurnInDraw) )
        {
            isMovable = false;
            

            if (isPlayer)
            {
                if(isGameStart && myCastle.silver < ruleData.drawSilver && !isDrawBattle)
                {
                    UIManager.Instance.OnSystemMsg("���� ����� �����մϴ�.");
                    isMovable = true;
                    return;
                }

                SoundManager.Instance.PlaySound(SoundEffectType.CARD_TAKEOUT);
                SortCardList(player, deckCardList[0]);
                if (isGameStart && !isDrawBattle)  //ó���� �ִ� ī�� �� ���� �����̹Ƿ� �̷� ���ǹ� �޾��ش�
                {
                    myCastle.silver -= ruleData.drawSilver;
                    moneyTxt.text = myCastle.silver.ToString();
                }
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundEffectType.CARD_TAKEOUT);
                SortCardList(enemy, deckCardList[0]);
            }

            deckCardList.RemoveAt(0);
        }
    }

    private void ResetGame()  //���� ����
    {
        clickPrevObj.SetActive(true);
        isGameStart = false;
        isMovable = false;
        isMyTurn = true;
        isCardTouch = false;
        stopBtn.interactable = true;
        StartCoroutine(StartGame());
    }

    private void Shuffle()  //���� �Լ�  +(�ٸ� ī�� ����Ʈ �ʱ�ȭ)
    {
        player.RemoveAllCard();
        enemy.RemoveAllCard();
        trashCardList.Clear();
        deckCardList.Clear();
        int i;

        for(i=0; i<33; ++i)
        {
            int r1 = Random.Range(0, allCardList.Count);
            int r2 = Random.Range(0, allCardList.Count);

            CardScript temp = allCardList[r1];
            allCardList[r1] = allCardList[r2];
            allCardList[r2] = temp;
        }
        for(i=0; i<allCardList.Count; i++)
        {
            deckCardList.Add(allCardList[i]);
        }
    }

    private IEnumerator StartGame()  //�� ����
    {
        yield return new WaitForSeconds(1.5f);
        jqkDecidePanel.gameObject.SetActive(false);
        UIManager.Instance.SetTrashUI();
        ETotalTxt.text = "0";
        PTotalTxt.text = "0";
        ETotalTxt.color = ruleData.totalTxtColor;
        PTotalTxt.color = ruleData.totalTxtColor;

        turn++;
        turnTxt.text = string.Concat("<color=#FF6500>TURN</color> ", turn.ToString());

        Sequence seq = DOTween.Sequence();

        for(int i=0; i<allCardList.Count; i++)
        {
            seq.Append(allCardList[i].transform.DOLocalMove(orgCardPRS.position, 0.05f));  //��� ī�尡 ������
        }
        seq.Play();

        Shuffle();  

        yield return new WaitForSeconds(4);

        zPos = 0f;
        float x = newTrashTr.localPosition.x;
        float y = newTrashTr.localPosition.y;
        for (int i = 0; i < trashTrs.Length; i++)  //ī�� 6�� ������
        {
            trashCardList.Add(deckCardList[0]);
            SoundManager.Instance.PlaySound(SoundEffectType.CARD_TAKEOUT);
            Transform t = deckCardList[0].transform;
            zPos -= 0.01f;
            t.DOLocalMove(new Vector3(x, y, zPos), 0.3f);
            //t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, -0.01f);
            //t.DOLocalMove(trashTrs[i].localPosition,0.4f);
            //t.DOScale(ruleData.trashCardScale,0.4f);
            UIManager.Instance.SetTrashUI(deckCardList[0]);

            yield return ws1;
            deckCardList[0].RotateCard();
            deckCardList.RemoveAt(0);
            yield return ws2;
        }

        {  //�� ī�� �� �� ������ ���·� �����´�
            Transform t2 = deckCardList[0].transform;
            t2.localPosition = new Vector3(t2.localPosition.x, t2.localPosition.y, -0.01f);
            t2.DOLocalMove(enemy.cardTrs[0].localPosition, 0.4f);
            t2.DOScale(ruleData.cardScale, 0.4f);
            SoundManager.Instance.PlaySound(SoundEffectType.CARD_TAKEOUT);

            yield return ws1;
            enemy.AddCard(deckCardList[0]);
            deckCardList.RemoveAt(0);
            yield return ws2;
        }

        //������ ī�� �ϳ� ���� �ְ� �� �㿡 �ڽ��� ī�� �������� ����� �� ����
        isMovable = true;
        DrawCard(false);
        while (!isMovable) yield return null;
        ETotalTxt.text = (enemy.total - enemy.cardList[0].Value).ToString();

        DrawCard(true);
        while (!isMovable) yield return null;
        yield return new WaitForSeconds(1);
        clickPrevObj.SetActive(false);

        isCardTouch = true;
        isGameStart = true;
    }

    private IEnumerator UpdateTotalUI(Text txt ,int target, int j)  //ī�� ���� ������Ʈ  j�� 1�̳� -1�� �޾Ƽ� ī���� ���� �����ϰų� ������ �� �Ѵ� ó�� �����ϰ� �Ѵ�
    {
        bool cMove = false;
        int limit = GameManager.Instance.savedData.userInfo.leadership;

        int current = int.Parse(txt.text);
        for(int i=current; i!=target+j; i+=j)  
        {
            yield return ws3;
            txt.text = i.ToString();
            if(j==1 && !cMove && int.Parse(txt.text)>limit)
            {
                cMove = true;
                camMove.ShakeCamera(0.4f, 2f);
                txt.DOColor(Color.red, 0.7f);
            }
        }
        isMovable = !isThrowing;  //ī�带 ������ �߿��� false�� ��� �����ؾ��ϹǷ� �̷���
        if(deckCardList.Count==0 && !isThrowing) StartCoroutine(DeckReShuffle());  // 
        if (cMove)
        {
            yield return new WaitForSeconds(0.7f);
            txt.DOColor(ruleData.totalTxtColor, 0.5f);
        }
        else if (player.total == limit)
        {
            txt.DOColor(Color.yellow, 0.3f);
        }
    }

    void SetHpUI()
    {
        UserInfo uif = GameManager.Instance.savedData.userInfo;

        hpTxt[0].text = uif.hp.ToString();
        hpTxt[1].text = enemyCastle.hp.ToString();

        hpFill[0].fillAmount = uif.hp/(float)uif.maxHp;
        hpFill[1].fillAmount = enemyCastle.hp / (float)enemyMaxHp;
    }

    private void SortCardList(PlayerScript ps, CardScript cs)  //ī�带 �߰��ϰ� �����Ѵ�. 
    {
        Transform t = cs.transform;
        t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, -0.01f);
        Sequence seq = DOTween.Sequence();
        ps.AddCard(cs);

        if (ps.trIdx>=ps.cardTrs.Length)
        {
            zPos = 0f;
            float x1 = ps.cardTrs[0].localPosition.x;
            float x2 = ps.cardTrs[ps.cardTrs.Length - 1].localPosition.x;
            seq.Append(t.DOScale(ruleData.cardScale, 0.4f));
            float y = ps.cardTrs[0].localPosition.y;

            for (int i = 0; i < ps.cardList.Count; i++)
            {
                zPos -= 0.01f;
                float x = Mathf.Lerp(x1, x2, (float)i / (ps.cardList.Count - 1));
                ps.cardList[i].transform.DOLocalMove(new Vector3(x, y, zPos), 0.4f);
            }
        }
        else
        {
            seq.Append(t.DOScale(ruleData.cardScale, 0.4f));

            t.DOLocalMove(ps.cardTrs[ps.cardList.Count-1].localPosition, 0.4f);
        }

        seq.AppendCallback(() =>
        {
            cs.RotateCard();
            if (ps.isMine)
            {
                if (!isDrawBattle)
                {
                    StartCoroutine(UpdateTotalUI(PTotalTxt, player.total, 1));  //������ ī�� �� �ؽ�Ʈ�� ���������� ���ϰ� ���������� ���� �� ���� �ٲ�Ƿ� �̷��� ������
                    spawner.SpawnMySoldiers(cs.Value);
                }
                else
                {
                    ps.total -= cs.Value;
                    isMyTurnInDraw = false;
                    isMovable = true;
                }
            }
            else
            {
                if (isDrawBattle)
                {
                    ps.total -= cs.Value;
                }
                isMovable = true;
            }
        }).Play();

        if(isMyTurn) CheckLeadership(ps);  //�� �Ͽ����� �ڷ�ƾ���� �ش� �Լ��� �������ֹǷ� ���� ���� ���� ����
    }

    private void CheckLeadership(PlayerScript ps, bool second=false)  //ī���� ���� ��ַ��� �Ѿ����� üũ
    {
        if (ps.isMine && ps.total > GameManager.Instance.savedData.userInfo.leadership)  //���� ����
        {
            isMovable = false;
            StartCoroutine(ThrowCard(ps));
        }
        else if(!isMyTurn && ps.total > enemyCastle.leaderShip && second)  //�� AI ����
        {
            isMovable = false;
            StartCoroutine(ThrowCard(ps));
        }
        else if(deckCardList.Count==0 && !isMyTurn)  //���⿡ �ֱ� �� �׷� ���ǹ��̱������� ���⸸ŭ ���� ���� ����. �� ���� �� ���� ������� Ȯ���ϰ� ���´�
        {
            StartCoroutine(DeckReShuffle());
        }
    }

    private IEnumerator ThrowCard(PlayerScript ps)  //ps�� ��� ī�带 ������ (������ ������ �̵���)
    {
        isThrowing = true;
        stopBtn2.interactable = false;
        //float x1 = trashTrs[0].localPosition.x;
        //float x2 = trashTrs[trashTrs.Length - 1].localPosition.x;
        //float y = trashTrs[0].localPosition.y;
        zPos = trashCardList[trashCardList.Count - 1].transform.localPosition.z;
        float x = newTrashTr.localPosition.x;
        float y = newTrashTr.localPosition.y;
        int count = ps.cardList.Count;
        if (ps.isMine)
        {
            continuePanel.gameObject.SetActive(true);
            continuePanel.DOFade(1, 3);
        }
        isCardTouch = false;

        yield return new WaitForSeconds(2.5f);
        for (int i = count-1; i>=0; i--)  //ī�� ���� ������ (���ʺ��� �����°� �ڿ�������Ƿ� for���� �̷���)
        {
            trashCardList.Add(ps.cardList[i]);
            Transform t = ps.cardList[i].transform;
            UIManager.Instance.SetTrashUI(ps.cardList[i]);

            zPos -= 0.01f;
            t.DOScale(orgCardPRS.scale, 0.3f);
            t.DOLocalMove(new Vector3(x, y, zPos), 0.3f);
            //t.DOScale(ruleData.trashCardScale, 0.3f);
            /*for (int j=0; j<trashCardList.Count; j++)
            {
                zPos -= 0.01f;
                trashCardList[j].transform.DOLocalMove(new Vector3(Mathf.Lerp(x1, x2, (float)j / (trashCardList.Count - 1)), y, zPos), 0.35f);
            }*/
            SoundManager.Instance.PlaySound(SoundEffectType.CARD_TAKEOUT);
            yield return ws1;
        }
        ps.RemoveAllCard();
        isThrowing = false;

        if (ps.isMine)
        {
            stopBtn2.interactable = true;
            spawner.ResetData(true);
            StartCoroutine(UpdateTotalUI(PTotalTxt, 0, -1));
        }
        else
        {
            isMovable = true;
            if (deckCardList.Count == 0) StartCoroutine(DeckReShuffle());
        }
    }

    public void Stop() //��������
    {
        if (!isMyTurn || !isMovable) return;

        isMyTurn = false;
        stopBtn.interactable = false;
        isCardTouch = false;

        if (continuePanel.gameObject.activeSelf)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(continuePanel.DOFade(0, 1));
            seq.AppendCallback(() => continuePanel.gameObject.SetActive(false));
            
            isCardTouch = true;
        }

        StartCoroutine(EnemyAI());
    }

    public void ContinueGame()  //��ַ� �ʰ��� ���� ������ ī�� �ٽ� ������
    {
        if(myCastle.silver<ruleData.resapwnSilver)
        {
            UIManager.Instance.OnSystemMsg("��ȭ�� �����մϴ�.");
            return;
        }

        myCastle.silver -= ruleData.resapwnSilver;
        moneyTxt.text = myCastle.silver.ToString();

        Sequence seq = DOTween.Sequence();
        
        seq.Append( continuePanel.DOFade(0, 2) );
        seq.AppendCallback(() => continuePanel.gameObject.SetActive(false));
        seq.Play(); //�� ���� ���� �������� ������ �� (�ϴ��� �׳� ��������� ����)
        isCardTouch = true;
    }

    private IEnumerator DeckReShuffle()  //ī�带 �̴ٰ� ���� ��� �Ǹ� ������ ī�带 ������ �ǵ����� ���� �ٽ� 6�� ������
    {
        isMovable = false;
        yield return ws2;
        int i;

        for(i=0; i<trashCardList.Count; i++)  //���� ī����� ���� �� ����Ʈ�� �ְ� ȭ�� ������ �о�� �޸����� �����´�
        {
            deckCardList.Add(trashCardList[i]);
            trashCardList[i].transform.DOLocalMove(new Vector3(Random.Range(ruleData.mixX[0], ruleData.mixX[1]), ruleData.mixY, 0), 0.04f);
            trashCardList[i].RotateCard(false);
            yield return ws4;
        }

        trashCardList.Clear();
        yield return ws1;

        for (i = 0; i < deckCardList.Count; i++)  //ȭ�� ������ ���� ī����� ���� ���� ���� ī�� ũ��� �� ��ġ�� ����
        {
            deckCardList[i].transform.DOLocalMove(orgCardPRS.position, 0.04f);
            deckCardList[i].transform.DOScale(orgCardPRS.scale, 0.03f);
            yield return ws4;
        }

        for(i = 0; i<25; ++i)  //���� ����
        {
            int r1 = Random.Range(0, deckCardList.Count);
            int r2 = Random.Range(0, deckCardList.Count);

            CardScript temp = deckCardList[r1];
            deckCardList[r1] = deckCardList[r2];
            deckCardList[r2] = temp;
        }

        zPos = 0f;
        float x = newTrashTr.localPosition.x;
        float y = newTrashTr.localPosition.y;
        for (i = 0; i < trashTrs.Length; i++)  //ī�� 6�� ������
        {
            trashCardList.Add(deckCardList[0]);
            Transform t = deckCardList[0].transform;
            zPos -= 0.01f;
            t.DOLocalMove( new Vector3(x, y, zPos),0.3f);
            //t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, -0.01f);
            //t.DOLocalMove(trashTrs[i].localPosition, 0.4f);
            //t.DOScale(ruleData.trashCardScale, 0.4f);

            yield return ws2;
            deckCardList[0].RotateCard();
            deckCardList.RemoveAt(0);
            yield return ws2;
        }

        isMovable = true;
    }

    private IEnumerator EnemyAI()  //���� ��
    {
        while (isThrowing) yield return null;

        yield return ws1;
        enemy.cardList[0].RotateCard();  //ó���� ������ ī�带 ���� ����
        ETotalTxt.text = enemy.total.ToString();

        bool cMove = false;
        while (enemy.total<enemyCastle.minLeaderShip)
        {
            yield return new WaitForSeconds(1.5f);

            CheckLeadership(enemy);  //���� ī�� ���� 0�̸� �ٽ� ���� ���ؼ� ȣ��
            yield return ws3;
            while (!isMovable) yield return null;  //�߰��߰����� �̰��� ���༭ �ִϸ��̼� ���� �߿� ���� �ڵ带 �����ϰ� �ϴ� ���� ������

            DrawCard(false);
            yield return ws3;
            ETotalTxt.text = enemy.total.ToString();
            if(!cMove && enemy.total>enemyCastle.leaderShip)
            {
                cMove = true;
                camMove.ShakeCamera(0.4f, 2f);
                ETotalTxt.DOColor(Color.red, 0.7f);
            }
            while (!isMovable) yield return null;
        }

        //ETotalTxt.text = enemy.total.ToString();

        yield return ws2;
        CheckLeadership(enemy, true);  //���� ī�� ���� ��ַ� ������ ī�� ���� ������ ��
        yield return ws3;
        while (!isMovable) yield return null;

        ETotalTxt.text = enemy.total.ToString();
        if(enemy.total == enemyCastle.leaderShip)
        {
            ETotalTxt.color = Color.yellow;
        }

        yield return new WaitForSeconds(.5f);  //0.5�� ���
        if (cMove)
        {
            ETotalTxt.DOColor(ruleData.totalTxtColor, 0.5f);
        }

        if (cardImg.gameObject.activeSelf)
        {
            UIManager.Instance.ViewUI(0);
        }
        //��� UI�� ī�带 �����ϰ� ���ְ� ������ �������ش�
        FadeObj(true, 1.3f, 1.1f);

        yield return new WaitForSeconds(1.3f);
        spawner.BattleStart(enemy.total);
        //camMove.SetMoveState(true);
    }

    public void Damaged(bool isEnemy, int damage)
    {
        if (damage > 0)
        {
            if (isEnemy)
            {
                enemyCastle.hp -= damage;
            }
            else
            {
                GameManager.Instance.savedData.userInfo.hp -= damage;
            }
        }

        CheckDie();

        allCardList.ForEach(x =>
        {
            x.transform.localScale = orgCardPRS.scale;
            x.SetSprite();
            x.spriteRenderer.color = Color.white;
            x.transform.localPosition = new Vector3(Random.Range(ruleData.mixX[0], ruleData.mixX[1]), ruleData.mixY, 0);
        });

        viewPanel.DOFade(1, 1.2f);
        noPrevClick.DOFade(1, 1);
        noPrevClick.interactable = true;
        ResetGame();
        camMove.SetMoveState(false);
        SetHpUI();
    }

    private void CheckDie()
    {
        if (GameManager.Instance.savedData.userInfo.hp <= 0)
        {
            GameManager.Instance.savedData.userInfo.hp = 0;
            EndGame(false);
        }
        else if (enemyCastle.hp <= 0)
        {
            enemyCastle.hp = 0;
            EndGame(true);
        }
    }

    private void EndGame(bool win)
    {
        resultCvsg.gameObject.SetActive(true);
        resultCvsg.DOFade(1, 1.5f);
        resultTxt.text = win ? "�¸��Ͽ����ϴ�" : "�й��Ͽ����ϴ�";

        if(win)
        {
            GameManager.Instance.savedData.userInfo.silver += enemyCastle.rewardSilver;
            GameManager.Instance.savedData.userInfo.gold += enemyCastle.rewardGold;
            rewardTxt.text = $"ȹ�� ��ȭ: <color=white>{enemyCastle.rewardSilver}</color>  ȹ�� ��ȭ: <color=white>{enemyCastle.rewardGold}</color>";
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))  //ī�� Ȯ��
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.transform != null)
            {
                if (hit.transform.CompareTag("Card"))
                {
                    Sprite spr = hit.transform.GetComponent<SpriteRenderer>().sprite;

                    if(spr != ruleData.backSprite && isCardTouch)  //�ո��̶��
                    {
                        if (!cardImg.gameObject.activeSelf)
                        {
                            cardImg.sprite = spr;
                            UIManager.Instance.ViewUI(0);
                        }
                    }
                }
            }
        }
    }

    public IEnumerator DrawBattle()  //���º� ���� ī���
    {
        yield return new WaitForSeconds(1);
        isDrawBattle = true;

        FadeObj(false, 1, 1);
        yield return ws1;

        isMyTurnInDraw = true;
        int pValue=0, eValue=0;

        while(pValue==eValue)
        {
            isMovable = true;
            CheckLeadership(enemy); //ī�尡 ��������� üũ����(�� ��)
            while (!isMovable) yield return null;

            while (isMyTurnInDraw) yield return ws3;
            pValue = player.cardList[player.cardList.Count - 1].Value;
            yield return ws2;

            isMovable = true;
            CheckLeadership(enemy); //ī�尡 ��������� üũ����(�� ��)
            while (!isMovable) yield return null;
            yield return ws3;

            DrawCard(false);
            while (!isMovable) yield return ws3;
            eValue = enemy.cardList[enemy.cardList.Count - 1].Value;

            if (eValue == pValue) isMyTurnInDraw = true;
        }

        spawner.DecideWinner(pValue, eValue);
        isDrawBattle = false;
        isMyTurnInDraw = false;
    }

    public void FadeObj(bool off, params float[] times) //ī��� UI ���̵� ȿ��
    {
        viewPanel.DOFade(off?0:1, times[0]);
        noPrevClick.DOFade(off ? 0 : 1, times[0]);
        noPrevClick.interactable = !off;
        Color c = off ? ruleData.noColor : Color.white;
        allCardList.ForEach(x => x.spriteRenderer.DOColor(c, times[1]));
    }
}
