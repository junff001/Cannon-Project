using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardRuleData", menuName = "Scriptable Object/CardRuleData",order =int.MaxValue)]
public class CardRuleData : ScriptableObject
{
    public Color noColor;
    public Color totalTxtColor;
    public Sprite backSprite;  //ī�� �޸�
    public float[] mixX;  //ī�� ���� �� ȭ�鿡�� �Ⱥ��̴� ������ �̵� ��ų�ǵ� �� ��ġ���� x��ǥ ����
    public float mixY;  //ī�� ���� �� ȭ�鿡�� �Ⱥ��̴� ������ �̵� ��ų�ǵ� �� ��ġ���� y
    public Vector3 cardScale, trashCardScale;  //���� ī�带 ������ ���� ī�� ũ��� ������ ���� ī�� ũ��
    public Sprite[] jqkSpr;  //JQK��������Ʈ��

    public long drawSilver = 20;  //��ο� ���
    public long resapwnSilver = 100; //��ַ��� �ʰ��ؼ� ���� ������ �ٽ� �ϱ����� ���
}
