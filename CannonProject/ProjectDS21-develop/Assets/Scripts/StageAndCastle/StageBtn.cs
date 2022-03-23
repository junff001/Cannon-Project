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



#region �ּ�
/*if (!stageCastle.isClear)
            {
                if (stageCastle.isOpen)
                {
                    StageInfo.Instance.StartAction = () =>
                    {
                        GameManager.Instance.CInfoToJson(cInfo);
                    };
                    //�ش� ���������� ����(ü��, ��ַ�, ����� �� �̹��� ��)
                }
                else
                {
                    //'���� ������� ���� ���������Դϴ�' â ���ų� ��ư Ŭ�� �ȵǰ� ���ش�
                }
            }
            else
            {
                //'�̹� Ŭ����� ���������Դϴ�' â ���ų� ��ư�� ��Ӱ� ���ְ� Ŭ�� �ȵǰ� ���ش�
            }*/
#endregion