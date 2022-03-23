using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EffectManager : MonoSingleton<EffectManager>
{
    private Camera mainCam;
    private float lastTime = 0f;
    [SerializeField] private int count=3;
    [SerializeField] private float delay = .1f;
    [SerializeField] private float zPos = -8f;

    public bool onTouchEffect = true;
    public Vector2 minPos, maxPos;
    public Color targetColor;

    [SerializeField] Sprite[] cardSprites;

    private void Start()
    {
        mainCam = Camera.main;
        cardSprites = Resources.LoadAll<Sprite>("Card/");
    }

    private void LateUpdate()
    {
        if(Input.GetMouseButton(0) && onTouchEffect) //≈Õƒ° ¿Ã∆Â∆Æ
        {
            if(lastTime<Time.time)
            {
                for(int i=0; i<count; ++i)
                {
                    Vector3 mPos = mainCam.ScreenToWorldPoint(Input.mousePosition) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

                    TouchEffect te = PoolManager.GetItem<TouchEffect>();
                    
                    te.spr.sprite = cardSprites[Random.Range(0, cardSprites.Length)];
                    te.transform.position = mPos;
                    te.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-30f, 30f));

                    te.transform.DOMove(new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), zPos), 15f);
                    te.transform.DORotate(new Vector3(0, 0, Random.Range(-100f, 100f)), 1.5f);
                    te.transform.DOScale(Vector3.zero, 1.75f);
                    te.spr.DOColor(targetColor, 1.6f);
                }
                lastTime = Time.time + delay;
            }
        }
    }
}
