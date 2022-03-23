using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SaveData 
{
    public UserInfo userInfo = new UserInfo();
    public Option option = new Option();
    public BattleInfo battleInfo = new BattleInfo();
}

[Serializable]
public class UserInfo
{
    public bool isFirstStart = true;  //�� ������ ó�� �����ߴ���

    public short leadership = 21; //��ַ� (�� ���� ������ ������ų �� �ִ� �ִ� ���� ��)
    public short clearId = 0; //Ŭ������ �ܰ��� ���� ���� �ܰ�

    public int hp = 20;  //���� �ڽ� ���� ü�� 
    public int maxHp=20;  //�ִ� ü�� 

    public long gold = 0;  //��ȭ 
   
    public long cropSilver = 100;  //'��'�� '���̴�' ��ȭ 
    public long currentSilver = 0;  //���� '�׿��ִ�' ��ȭ�� �� 
    public long maxSilver = 1000;  //�ִ�� '���� �� �ִ�' ��ȭ �� 
    public long silver = 1000; //���� '������' �ִ� ��ȭ�� ��

    public string quitDate;  //���� ���� ��¥

    //public short soldier;  // �Ϲ� ����   �ϴ� ������ �Ⱦ�
    //public short chief;  //1��1 �ϱ��� ����      �ϴ� ������ �Ⱦ�

    //public List<StageCastle> stageCastles = new List<StageCastle>(); //ó�� ������ �� ���� ���� (��ȹ��ζ�� ���� �̰� ��� �� ���� ������ ���� �߰����� �𸣴� �ϴ� ��)


    /*public StageCastle GetStage(short id) => stageCastles.Find(x => x.id == id);

    public StageCastle CreateCastleInfo(StageCastle sc)
    {
        stageCastles.Add(new StageCastle(sc));
        return sc;
    }*/
}

[Serializable]
public class Option
{
    //0f~1f
    public float bgmSize = 0.5f;
    public float soundEffectSize = 0.7f;
}

[Serializable]
public class BattleInfo
{
    public CastleInfo enemyCastle = new CastleInfo();
    public MainInfo myCastle = new MainInfo();
}

#region �ּ�1
/*[Serializable]
public class StageCastle  //���� ���� (��������)  �̰͵� �翬�� ��ȹ��ζ�� ��� �� �� ������ ���� �߰����� �𸣴� �ϴ� ��
{
    public short id;  //�ܰ�

    public bool isOpen;
    public bool isClear = false;

    public StageCastle() { }
    public StageCastle(StageCastle sc)
    {
        id = sc.id;
        isOpen = sc.isOpen;
        isClear = sc.isClear;
    }

    #region �ּ�
    *//*public short level = 1;  //��ȭ ��ġ
    public short maxLevel;

    //needTimeForCrop�ʿ� �� ���� crop��ŭ gold����. maxCrop�� �����ϸ� ��Ȯ�ϱ� ������ �� �� ����
    public float needTimeForCrop;  //����: �� 
    public long crop; //�� �� ������ ������ ���̴� ��
    public long currentCrop;
    public long maxCrop;

    public string quitDate;

    public StageCastle(short id, bool open, short maxLv, float needTime, long crop, long maxCrop)
    {
        this.id = id;
        this.isOpen = open;
        this.maxLevel = maxLv;
        this.needTimeForCrop = needTime;
        this.crop = crop;
        this.maxCrop = maxCrop;
    }

    public StageCastle(StageCastle sc)
    {
        this.id = sc.id;
        this.isOpen = sc.isOpen;
        this.maxLevel = sc.maxLevel;
        this.needTimeForCrop = sc.needTimeForCrop;
        this.crop = sc.crop;
        this.maxCrop = sc.maxCrop; 
    }*//*
    #endregion
}*/
#endregion

[Serializable]
public class CastleInfo  //�� ���� �ɷ�ġ ����
{
    public short stageID; // n��������
    public int hp = 20; 
    public short leaderShip = 21;  //���� ��ַ� (�� ���� '�ʰ�'�ϸ� ����)
    public short minLeaderShip = 16;  //���� �ּ� ī�� ������ �� ���̴�
    public Sprite castleSprite;  //�� ��������Ʈ      �ϴ� ������ �Ⱦ�
    public short soldier;  // �Ϲ� ����   �ϴ� ������ �Ⱦ�
    public short chief;  //1��1 �ϱ��� ����      �ϴ� ������ �Ⱦ�

    public long rewardSilver;  //��ȭ ����
    public long rewardGold;  //��ȭ ����
}

[Serializable]
public class MainInfo  //�� ���� ����
{
    public long silver; //�������
    public Sprite castleSprite;  //�� ��������Ʈ
    public short soldier;  // �Ϲ� ����
    public short chief;  //1��1 �ϱ��� ���� 

    public MainInfo() { }
    public MainInfo(long silver)
    {
        this.silver = silver;
    }
    public MainInfo(long s, Sprite cSpr, short sold, short ch)
    {
        silver = s;
        castleSprite = cSpr;
        soldier = sold;
        chief = ch;
    }
}

[Serializable]
public class TrashCardUI
{
    public GameObject panel;

    public int trashCnt;  //�ش� ���� ���� ����
    public Text trashCountTxt; //�ش� ���� ���� ������ ��Ÿ���� �ؽ�Ʈ UI

    //public bool[] isTrashShape; // L: 4   //�ش� ����� ī�尡 ����������
    public Image[] shapeImageList = new Image[4];  //  //Shape���������� �����Ѵ�� �Ȱ��� �������缭 �̹����� ����ִ´�.

    public void UpdateUI(int shape = -1)
    {
        if (shape == -1) //reset
        {
            trashCnt = 0;
            trashCountTxt.text = "0";
            //for (int i = 0; i < isTrashShape.Length; i++) isTrashShape[i] = false;
            for (int i = 0; i < shapeImageList.Length; i++) shapeImageList[i].gameObject.SetActive(false);

            return;
        }

        if (!panel.activeSelf) panel.SetActive(true);
        shapeImageList[shape].gameObject.SetActive(true);
        trashCountTxt.text = trashCnt.ToString();
    }
}