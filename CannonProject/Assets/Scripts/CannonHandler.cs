using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CannonHandler : MonoBehaviour
{
    public float angleSpeed = 70f;
    public GameObject ballPrefab;
    public Transform firePos;
    public float power;
    public float chargingPower = 200f;
    public float maxPower = 1000f;

    public Text anlgeText;
    public Text powerText;
    public Image powerGauge;

    public CinemachineVirtualCamera cannonCam;
    public CinemachineBrain mainCam;

    public ParticleSystem muzzleFlash;

    private float z;

    private void Start()
    {
        z = transform.rotation.eulerAngles.z; 

    }
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            z +=  angleSpeed * Time.deltaTime; 
        }
        else if (Input.GetKey(KeyCode.S)) 
        {
            z -= angleSpeed * Time.deltaTime;
        }

        z = Mathf.Clamp(z, 1, 88); 
                                                                        
                                                                        
        transform.rotation = Quaternion.Euler(0, 0, z); //����

        if (Input.GetMouseButtonDown(0))
        {
            power = 0;
        }
        else if (Input.GetMouseButton(0)) //�Է� ��
        {
            power += chargingPower * Time.deltaTime;
            power = Mathf.Clamp(power, 0, maxPower);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Fire();
        }

        anlgeText.text = $"{z.ToString("N0")}도";
        powerText.text = power.ToString("N0");
        powerGauge.fillAmount = power / maxPower;
        
    }

    private void Fire()
    {
        StartCoroutine(DelayFire());
    }

    IEnumerator DelayFire()
    {
        mainCam.m_DefaultBlend.m_Time = 1f;
        Vector3 next = firePos.position;
        next.z = cannonCam.transform.position.z;
        cannonCam.transform.position = next;

        cannonCam.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameObject ball = Instantiate(ballPrefab, firePos.position, Quaternion.identity);
        CannonBallScript bs = ball.GetComponent<CannonBallScript>();
        bs.Shoot(firePos.right, power, cannonCam);

        muzzleFlash.Play();

        cannonCam.Follow = ball.transform;
    }
}
