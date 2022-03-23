using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using DG.Tweening;

public enum SceneType
{
    LOBBY,
    MAIN
}

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private SaveData saveData;
    public SaveData savedData { get { return saveData; } }
    private string savedJson, filePath;
    private readonly string saveFileName_1 = "SaveFile01";

    public int screenWidth = 2960, screenHeight = 1440;
    public bool isReady = false;

    public List<StageBtn> stageBtns = new List<StageBtn>();
    //public Dictionary<short, StageCastle> idToCastle = new Dictionary<short, StageCastle>();
    [SerializeField] private short maxViewStage=4; //���� ������ �������� '����'�ؼ� �� ������������ �� �ܰ�(��)���� ��������
    //�� ���� ���߿� �����
    //public static string castleInfo;  
    //public static string mainInfo;

    public SceneType scType;
    public GameObject touchEffectPrefab, soundPrefab;
    public GameObject[] soldierPrefabs;
    public GameObject[] chiefPrefabs;

    [SerializeField] private float nxSpeed = 1f;

    public Dictionary<short, GameObject> idToSoldier = new Dictionary<short, GameObject>();
    public Dictionary<short, GameObject> idToChief = new Dictionary<short, GameObject>();

    private List<Vector3> startPos = new List<Vector3>();
    private List<Vector3> startRot = new List<Vector3>();

    public Image gameSpeedImg;
    public Sprite gameSpeedx1Sprite;
    private Sprite gameSpeedx2Sprite;

    [SerializeField]
    private Text[] statTexts;
    [SerializeField]
    private Text[] nextLevels;
    [SerializeField]
    private Text[] nextStats;
    [SerializeField]
    private Text[] nextPrices;
    [SerializeField]
    private Text[] lobbyStatTexts;

    [SerializeField]
    private int commandStat = 21;
    [SerializeField]
    private int defanseStat = 20;
    [SerializeField]
    private int goldStat = 10000;
    [SerializeField]
    private int silverStat = 90000000;
    [SerializeField]
    private int rouletteStat = 1;

    [SerializeField]
    private int command_price = 100;
    [SerializeField]
    private int defanse_price = 100;
    [SerializeField]
    private int roulette_price = 500;

    private int command_level = 1;
    private int defanse_level = 1;
    private int roulette_level = 1;

    private List<int> statList = new List<int>();

    public string GetFilePath(string fileName) => string.Concat(Application.persistentDataPath, "/", fileName);

    private void Awake()
    {
        filePath = GetFilePath(saveFileName_1);
        saveData = new SaveData();
        Load();
        InitData();
        CreatePool();
        isReady = true;
    }

    private void InitData()
    {
        Screen.SetResolution(screenWidth, screenHeight, true);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (scType == SceneType.MAIN)
        {
            short i;

            GameObject o = Instantiate(soldierPrefabs[0]);
            List <Transform> trList = new List<Transform>(o.GetComponentsInChildren<Transform>());
            trList.RemoveAt(0);
            for(i=0; i<trList.Count; i++)
            {
                startPos.Add(trList[i].localPosition);
                startRot.Add(trList[i].localRotation.eulerAngles);
            }
            Destroy(o);

            for(i=0; i<soldierPrefabs.Length; i++)
                idToSoldier.Add(i, soldierPrefabs[i]);
            for (i = 0; i < chiefPrefabs.Length; i++)
                idToChief.Add(i, chiefPrefabs[i]);

            gameSpeedx2Sprite = gameSpeedImg.sprite;
        }
        else if(scType == SceneType.LOBBY)
        {
            Time.timeScale = 1;
        }
    }

    private void CreatePool() //Ǯ ����
    {
        PoolManager.CreatePool<TouchEffect>(touchEffectPrefab, transform, 40);
        PoolManager.CreatePool<SoundPrefab>(soundPrefab, transform, 12);
    }

    private void Start()
    {
        
    }

    public void SaveData()
    {
        //saveData.userInfo.stageCastles.ForEach(x => x.quitDate = DateTime.Now.ToString());
        saveData.userInfo.quitDate = DateTime.Now.ToString();
    }

    public void Save()
    {
        SaveData();

        savedJson = JsonUtility.ToJson(saveData);
        byte[] bytes = Encoding.UTF8.GetBytes(savedJson);
        string code = Convert.ToBase64String(bytes);
        File.WriteAllText(filePath, code);
    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            string code = File.ReadAllText(filePath);
            byte[] bytes = Convert.FromBase64String(code);
            savedJson = Encoding.UTF8.GetString(bytes);
            saveData = JsonUtility.FromJson<SaveData>(savedJson);
        }

        SetData();
    }

    public void SetData()
    {
        if (scType == SceneType.LOBBY)
        {
            /*for (int i = 0; i < stageBtns.Count; i++)
            {
                //stageBtns[i].stageCastle = saveData.userInfo.GetStage(stageBtns[i].stageCastle.id) ?? saveData.userInfo.CreateCastleInfo(stageBtns[i].stageCastle);
                //idToCastle.Add(stageBtns[i].stageCastle.id, stageBtns[i].stageCastle);

                if ((stageBtns[i].stageCastle.isClear && stageBtns[i].stageCastle.id != saveData.userInfo.clearId)
                    || stageBtns[i].stageCastle.id > saveData.userInfo.clearId + maxViewStage)
                {
                    stageBtns[i].gameObject.SetActive(false);
                }
            }*/

            short ci = saveData.userInfo.clearId;
            for(int i=0; i<stageBtns.Count; i++)
            {
                if(stageBtns[i].cInfo.stageID < ci || stageBtns[i].cInfo.stageID > ci + 4)
                {
                    stageBtns[i].gameObject.SetActive(false);
                }
            }

            TimeSpan ts = new TimeSpan();

            ts = DateTime.Now - Convert.ToDateTime(saveData.userInfo.quitDate);
            saveData.userInfo.currentSilver += (long)ts.TotalMinutes * saveData.userInfo.cropSilver;
        }
    }

    public void Upgrade_Command()
    {
        /*if (goldStat >= command_price) {
            goldStat -= command_price;
            commandStat++;
            command_level++;
            command_price += 100;
            saveData.userInfo.leadership++;
        }*/
        if (saveData.userInfo.gold >= 300)
        {
            saveData.userInfo.gold -= 300;
            saveData.userInfo.leadership++;
        }
    }

    public void Upgrade_Defanse()
    {
        /*if (goldStat >= defanse_price) {
            goldStat -= defanse_price;
            defanseStat++;
            defanse_level++;
            defanse_price += 100;
            
        }*/
        if(saveData.userInfo.gold>=200)
        {
            saveData.userInfo.gold -= 200;
            saveData.userInfo.maxHp++;
        }
    }

    public void Upgrade_Roulette()
    {
        if (goldStat >= roulette_price) {
            goldStat -= roulette_price;
            rouletteStat++;
            roulette_level++;
            roulette_price += 1500;
        }
    }

    public void Recovery()
    {
        if(saveData.userInfo.silver>=100)
        {
            saveData.userInfo.silver -= 100;
            saveData.userInfo.hp = saveData.userInfo.maxHp;
        }
    }

    private void StatBarText()
    {
        statTexts[0].text = saveData.userInfo.leadership.ToString();
        statTexts[1].text = saveData.userInfo.maxHp.ToString();
        statTexts[2].text = saveData.userInfo.gold.ToString();
        statTexts[3].text = saveData.userInfo.silver.ToString();

        lobbyStatTexts[0].text = saveData.userInfo.leadership.ToString();
        lobbyStatTexts[1].text = saveData.userInfo.maxHp.ToString();
        lobbyStatTexts[2].text = saveData.userInfo.gold.ToString();
        lobbyStatTexts[3].text = saveData.userInfo.silver.ToString();
    }

    private void StatLevelText()
    {
        nextLevels[0].text = "Lv" + " " + command_level + " " + "->" + " " + (command_level + 1);
        nextLevels[1].text = "Lv" + " " + defanse_level + " " + "->" + " " + (defanse_level + 1);
        nextLevels[2].text = "Lv" + " " + roulette_level + " " + "->" + " " + (roulette_level + 1);
    }

    private void NextStatText()
    {
        nextStats[0].text = "통솔력: " + commandStat + " " + "->" + " " + (commandStat + 1);
        nextStats[1].text = "내구력: " + defanseStat + " " + "->" + " " + (defanseStat + 1);
        nextStats[2].text = "룰렛 배치 코인: " + rouletteStat + " " + "->" + " " + (rouletteStat + 1);
    }

    private void NextPriceText()
    {
        nextPrices[0].text = "비용(금화): " + command_price + " " + "->" + " " + (command_price + 100);
        nextPrices[1].text = "비용(금화): " + defanse_price + " " + "->" + " " + (defanse_price + 100);
        nextPrices[2].text = "비용(금화): " + roulette_price + " " + "->" + " " + (roulette_price + 1500);
    }


    //임시용
    float time1 = 0f;
    private void Update()
    {
        if (time1 < Time.time && scType==SceneType.LOBBY)
        {
            StatBarText();
            //StatLevelText();
            //NextStatText();
            //NextPriceText();
            time1 = Time.time + 1f;
        }
    }

    public void ChangeScene(string sceneName)
    {
        UIManager.Instance.FadeInOut(false);  
        
        Save();

        PoolManager.ClearAll();

        StartCoroutine(ChangeDelay(sceneName));
    }

    IEnumerator ChangeDelay(string _name)
    {
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(_name);
    }

    /*public void GameSpeedUP(float speed=-1f) //ī�� ���� ���
    {
        if (speed<0)
        {
            Time.timeScale = 1;
            return;
        }

        Time.timeScale = speed;
    }*/
    public void GameSpeed()
    {
        if(Time.timeScale<=1f)
        {
            Time.timeScale = 2f;
            gameSpeedImg.sprite = gameSpeedx1Sprite;
        }
        else
        {
            Time.timeScale = 1f;
            gameSpeedImg.sprite = gameSpeedx2Sprite;
        }
    }

    public void ResetSoldier(Transform[] trList) //�� �� �����ϸ� ����� Transform ���� �̻������� ���� ������ �ʱ�ȭ�� ���������.
    {
        for(int i=1; i<trList.Length; i++)
        {
            trList[i].localPosition = startPos[i - 1];
            trList[i].localRotation = Quaternion.Euler(startRot[i - 1]);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //���������� �Ѿ�� ���� ���� �� �Լ��� ȣ���ؼ� �� ���� �� �� ������ �����ְ� 'Save'�Լ��� ȣ���ؾ��Ѵ�. (�� �Ѿ�� ���� Ǯ ������ �ؾ���)
    public void CInfoToJson(CastleInfo ci) => saveData.battleInfo.enemyCastle = ci;  
    public void MInfoToJson(long cost) => saveData.battleInfo.myCastle = new MainInfo(cost);
    //�� ���� �����
    //public void CInfoToJson(CastleInfo ci) => castleInfo = JsonUtility.ToJson(ci); 
    //public void MInfoToJson(long s, Sprite cSpr, short sold, short ch) => mainInfo = JsonUtility.ToJson(new MainInfo(s,cSpr,sold,ch)); //�� ���� ���� ����
    #region OnApplication
    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            Save();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            Save();
        }
    }
    #endregion
}
