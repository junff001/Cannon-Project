using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StageBtn : MonoBehaviour
{
    //public StageCastle stageCastle;

    private Button btn;
    public CastleInfo cInfo;

    private void Awake()
    {
        btn = GetComponent<Button>();

        btn.onClick.AddListener(() =>
        {
            GameManager.Instance.CInfoToJson(cInfo);
        });
    }
}



#region 주석
/*if (!stageCastle.isClear)
            {
                if (stageCastle.isOpen)
                {
                    StageInfo.Instance.StartAction = () =>
                    {
                        GameManager.Instance.CInfoToJson(cInfo);
                    };
                    //해당 스테이지의 정보(체력, 통솔력, 병사와 성 이미지 등)
                }
                else
                {
                    //'아직 개방되지 않은 스테이지입니다' 창 띄우거나 버튼 클릭 안되게 해준다
                }
            }
            else
            {
                //'이미 클리어된 스테이지입니다' 창 띄우거나 버튼색 어둡게 해주고 클릭 안되게 해준다
            }*/
#endregion