using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerScript 
{
    public bool isMine;
    public List<CardScript> cardList = new List<CardScript>();
    public Transform[] cardTrs;  //카드 기본 위치들
    public int trIdx = 0;
    public int total = 0;

    public void AddCard(CardScript card)
    {
        cardList.Add(card);
        trIdx++;
        total += card.Value;
    }
    public void RemoveAllCard()
    {
        cardList.Clear();
        trIdx = 0;
        total = 0;
    }
}

[Serializable]
public class PRS
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion quat, Vector3 scl)
    {
        position = pos;
        rotation = quat;
        scale = scl;
    }
}
