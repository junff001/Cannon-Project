using UnityEngine;
using DG.Tweening;

public class TouchEffect : MonoBehaviour
{
    public Vector3 scl;
    public SpriteRenderer spr;
    public Color col;

    private void LateUpdate()
    {
        if(transform.localScale==Vector3.zero)
        {
            transform.DOKill();
            transform.localScale = scl;
            spr.color = col;
            gameObject.SetActive(false);
        }
    }
}
