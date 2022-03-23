using UnityEngine;
using DG.Tweening;

public class CameraMove : MonoBehaviour
{
    private Camera cam;
    public Transform target;
    private bool isMoving = false;
    [SerializeField] private float speed = 10f;

    private Vector3 orgPos;

    public Vector2 posMin, posMax;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        orgPos = transform.position;
        
    }

    private void Update()
    {
        PositionList();    
    }

    private void LateUpdate()
    {
        if(isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10), Time.deltaTime * speed);
        }
        else
        {
            if(transform.position!=orgPos)
            {
                transform.position = Vector3.Lerp(transform.position, orgPos, Time.deltaTime * speed);
            }
        }
    }

    public void SetMoveState(bool move) => isMoving = move;

    public void ShakeCamera(float duration, float strength) 
    {
        cam.DOShakePosition(duration, strength);
    }

    private void PositionList()
    {
        float X = Mathf.Clamp(transform.position.x, posMin.x, posMax.x);
        float Y = Mathf.Clamp(transform.position.y, posMin.y, posMax.y);
        transform.position = new Vector3(X, Y, -10);
    }
}
