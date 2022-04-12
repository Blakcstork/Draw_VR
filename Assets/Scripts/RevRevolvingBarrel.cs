using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevRevolvingBarrel : MonoBehaviour
{


    /// ----------------------------
    /// Public Variables
    /// -----------------------------
    /// 
    [HideInInspector]
    public int numberOfChambers = 6;

    [HideInInspector]
    public float rotationSpeed = 0.1f;
    [HideInInspector]
    public AudioClip revolverOpenClip;
    [HideInInspector]
    public AudioClip revolverClickClip;
    [HideInInspector]
    public AudioClip revolverCloseClip;
    [HideInInspector]
    public Vector3 openPosition;
    [HideInInspector]
    public Vector3 closedPosition;
    [HideInInspector]
    public float openSpeed = 1000f;

    [HideInInspector]
    public GameObject bulletPrefab;

    [HideInInspector]
    public float minRotationSpeedForReloading = 200f;
    [HideInInspector]
    public float openDurationReloadNeeded = 3f;
    [HideInInspector]
    public float openDurationReloadNotNeeded = 0.65f;
    [HideInInspector]
    public float minStayOpenDuration = 0.65f;

    [HideInInspector]
    public float shellEjectionForce = 1f;



    // Keep Here
    public Collider[] gunColliders;
    public float bulletPlacementRadius = 0.0102f;
    public float bulletForwardAxisOffset = 0.0092f;

    /* These are set by a script */
    [HideInInspector]
    public RevRevolver revolverParent;

    [HideInInspector]
    public bool isOpen = false;




    /// ----------------------------
    /// Private Variables
    /// -----------------------------
    /// 

    // 총알 상태
    private GameObject[] bullets;
    private bool[] bulletSpent;

    // 열려있을때 회전
    private float revolverClickAngle = 0f;
    private int currentRevolverIndex = 0;
    private int shellEjectionIndex = -1;

    // 쏘는 중의 회전
    private Quaternion rotationTarget;
    private bool isAnimating = false;

    // 탄창 자동닫기
    private float openStartTime;
    private float stayOpenDuration;

    // 사운드
    private Coroutine positionCoroutine;
    private AudioSource clickAS;
    private AudioSource openCloseAS;

    //탄창 열려있을때 상태
    private bool shouldSpin = false;
    private bool canEjectShells = false;

    // Start is called before the first frame update
    void Start()
    {
        this.closedPosition = this.transform.localPosition;
        bullets = new GameObject[numberOfChambers];
        bulletSpent = new bool[numberOfChambers];
        ReloadBarrel();
        
    }

    void ReloadBarrel()
    {
        foreach(GameObject bullet in bullets)
        {
            if(bullet != null)
            {
                Destroy(bullet);
            }
        }


        for(int i = 0; i<numberOfChambers; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, this.transform);
            float degrees = AngleForIndex(i);
            float x = bulletPlacementRadius * Mathf.Cos(degrees * Mathf.Deg2Rad);
            float y = bulletPlacementRadius * Mathf.Sin(degrees * Mathf.Deg2Rad);

            bullet.transform.localPosition = new Vector3(bulletForwardAxisOffset, x, y);
            Collider col = bullet.GetComponent<Collider>();
            foreach(Collider col2 in gunColliders)
            {
                Physics.IgnoreCollision(col, col2);
            }
            col.enabled = false;
            bullet.GetComponent<Collider>().enabled = false;

            bulletSpent[i] = false;
            bullets[i] = bullet;

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (this.shouldSpin) // 돌아야하면
        {
            transform.localRotation = transform.localRotation * Quaternion.AngleAxis(Time.deltaTime * -Mathf.Max(openSpeed, minRotationSpeedForReloading), Vector3.right);
            int index = (int)Mathf.Floor((Time.time - this.openStartTime) / 0.135f);
            index = index % this.numberOfChambers; // index를 6으로 나눈 나머지
            if(shellEjectionIndex != index && canEjectShells)
            {
                EjectShell(index);
            }

            if (canEjectShells)
            {
                float angle = BarrelAngle();
                if(Mathf.Abs(angle - this.revolverClickAngle) > 25)
                {
                    this.revolverClickAngle = angle;
                    clickAS.Play();
                }

                openSpeed *= 0.995f; // 마찰


            }
            else if (this.isAnimating)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, this.rotationTarget, rotationSpeed);
                float curAngle = Quaternion.Angle(this.transform.localRotation, this.rotationTarget);
                if(curAngle < 1)
                {
                    this.isAnimating = false;
                }
            }

            if(this.isOpen && Time.time > (this.openStartTime + this.minStayOpenDuration))
            {
                this.CloseFromReload();
            }
        
        
        } // 회전
    }

    public void Revolve()
    {
        bulletSpent[currentRevolverIndex] = true;
        this.rotationTarget = RotationForIndex(++currentRevolverIndex);
        this.isAnimating = true;

        if(currentRevolverIndex > numberOfChambers - 1)
        {
            currentRevolverIndex = 0;
        }
    }

    public void OpenForReload(bool isRight)
    {
        if (isOpen)
            return;

        this.openStartTime = Time.time;
        this.stayOpenDuration = HasShotsToReload() ? openDurationReloadNeeded : openDurationReloadNotNeeded;
        if(this.positionCoroutine != null)
        {
            StopCoroutine(this.positionCoroutine);
        }
        this.positionCoroutine = StartCoroutine(AnimateBarrelTo(isRight ? this.openPosition : -this.openPosition));

        openSpeed = 800f;
        shouldSpin = true;
        isOpen = true;

        openCloseAS.clip = this.revolverOpenClip;
        openCloseAS.Play();
    }


    public bool CloseFromReload()
    {
        if(!isOpen || Time.time - this.openStartTime < this.minStayOpenDuration)
        {
            return false;
        }

        // 재장전
        revolverParent.Reload();

        // 배럴 닫기
        if(this.positionCoroutine != null)
        {
            StopCoroutine(this.positionCoroutine);
        }
        this.positionCoroutine = StartCoroutine(AnimateBarrelTo(this.closedPosition));
        isOpen = false;
        canEjectShells = false;
        ReloadBarrel();

        openCloseAS.clip = this.revolverCloseClip;
        openCloseAS.Play();

        return true;
    }

    private void EjectShell(int shellIndex)
    {
        if(bullets[shellIndex] != null && bulletSpent[shellIndex] == true)
        {
            GameObject bullet = bullets[shellIndex];
            bullet.transform.parent = null;
            bullet.GetComponent<Collider>().enabled = true;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(shellEjectionForce * bullet.transform.right, ForceMode.Force);

            bullet.GetComponent<RevDestroyAfterTime>().StartTimer();

            bullets[shellIndex] = null;
        }
    }

    //animation
    private IEnumerator AnimateBarrelTo(Vector3 position)
    {
        while (Mathf.Abs((this.transform.localPosition - position).magnitude) > 0.001f)
        {
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, position, 0.175f);
            yield return null;
        }

        this.transform.localPosition = position;
        if (isOpen)
        {
            canEjectShells = true;
        }
        else
        {
            shouldSpin = false;
            this.isAnimating = true;
        }

    }

    bool HasShotsToReload()
    {
        foreach(bool spent in this.bulletSpent)
        {
            if(spent == true)
            {
                return true;
            }
        }
        return false;
    }

    //Math helpers


    Quaternion RotationForIndex(int curIndex)
    {
        float angle = AngleForIndex(curIndex);
        return Quaternion.AngleAxis(angle, Vector3.left);
    }

    float AngleForIndex(int curIndex)
    {
        return 360.0f * ((float)curIndex / (float)numberOfChambers);
    }

    int IndexForAngle(float angle)
    {
        return (int)Mathf.Floor((angle / 360.0f) * (float)numberOfChambers);
    }


    float BarrelAngle()
    {
        Vector3 forwardVector = transform.localRotation * Vector3.forward;
        float angle = Mathf.Atan2(forwardVector.y, forwardVector.z) * Mathf.Rad2Deg;
        if(angle < 0)
        {
            angle += 360.0f;
        }
        return angle;
    }


}
