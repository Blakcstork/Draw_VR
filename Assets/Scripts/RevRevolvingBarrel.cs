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


    /// ----------------------------
    /// Private Variables
    /// -----------------------------
    /// 

    // ÃÑ¾Ë »óÅÂ
    private GameObject[] bullets;
    private bool[] bulletSpent;


    // ÅºÃ¢ ÀÚµ¿´Ý±â
    private float openStartTime;



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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
