using UnityEngine;
using DG.Tweening;

public class CardScript : MonoBehaviour
{
    [SerializeField] private int value;
    public int Value { get { return value; } set { this.value = value; } }
    public JQK jqk = JQK.NONE;
    public Shape cardShape;

    public SpriteRenderer spriteRenderer;
    private Sprite firstSpr;  //ī���� �ո� ��������Ʈ

    private Vector3 rot1, rot2;
    //private SoundPrefab soundObj;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        firstSpr = spriteRenderer.sprite;

        rot1 = new Vector3(0, -90, 0);
        rot2 = new Vector3(0, -180, 0);
    }

    public void SetSprite(bool back=true) 
    {
        if (!back) spriteRenderer.sprite = firstSpr;
        else spriteRenderer.sprite = RuleManager.Instance.ruleData.backSprite;
    }

    public void RotateCard(bool front=true) //ī�� ȸ�� ȿ��
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalRotate(rot1, 0.12f));
        seq.AppendCallback(() =>
        {
            SetSprite(!front);
            transform.DOLocalRotate(front? Vector3.zero: rot2, 0.12f);
        }).Play();

        //soundObj = PoolManager.GetItem<SoundPrefab>();
        //soundObj.PlaySound(SoundEffectType.CARD_OVERTURN);
        SoundManager.Instance.PlaySound(SoundEffectType.CARD_OVERTURN);
    }
}
