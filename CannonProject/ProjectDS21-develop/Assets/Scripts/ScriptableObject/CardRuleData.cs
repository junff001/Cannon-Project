using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardRuleData", menuName = "Scriptable Object/CardRuleData",order =int.MaxValue)]
public class CardRuleData : ScriptableObject
{
    public Color noColor;
    public Color totalTxtColor;
    public Sprite backSprite;  //카드 뒷면
    public float[] mixX;  //카드 섞을 때 화면에서 안보이는 쪽으로 이동 시킬건데 그 위치값의 x좌표 구간
    public float mixY;  //카드 섞을 때 화면에서 안보이는 쪽으로 이동 시킬건데 그 위치값의 y
    public Vector3 cardScale, trashCardScale;  //각각 카드를 보유할 때의 카드 크기와 버려질 때의 카드 크기
    public Sprite[] jqkSpr;  //JQK스프라이트들

    public long drawSilver = 20;  //드로우 비용
    public long resapwnSilver = 100; //통솔력을 초과해서 병사 디지면 다시 하기위한 비용
}
