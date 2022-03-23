using UnityEngine;
using DG.Tweening;

public class Soldier : MonoBehaviour
{
    public Animator ani;
    public float rotSpeed = 0f;

    protected bool isMoving = false, isLocal;
    protected Vector3 target;
    protected float moveSpeed;

    protected Vector3 dir;

    public void InitSet(int scaleX, Vector2 localPos, Vector2 target, bool targetToMove=true) 
    {
        transform.localScale = new Vector2(scaleX, 1);
        transform.localPosition = localPos;

        if(targetToMove)
           transform.DOLocalMove(target, 0.6f);
    }

    protected void Update()
    {
        transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);

        if(isMoving) //어쩔 수 없다.. 편하게 간다..
        {
            if (isLocal)
            {
                dir = target - transform.localPosition;
                transform.localPosition += dir.normalized * moveSpeed * Time.deltaTime;

                if ((transform.localPosition - target).sqrMagnitude <= 0.64f)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                dir = target - transform.position;
                transform.position += dir.normalized * moveSpeed * Time.deltaTime;

                if ((transform.position - target).sqrMagnitude <= 0.64f)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void Fall(Vector3 target, float speed, bool local=true) 
    {
        this.target = target;
        moveSpeed = speed;
        isMoving = true;
        isLocal = local;
    }

    protected void OnDisable()
    {
        rotSpeed = 0;
        isMoving = false;
        transform.localRotation = Quaternion.identity;
        GameManager.Instance.ResetSoldier(GetComponentsInChildren<Transform>());
    }
}
