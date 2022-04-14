using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevDestroyAfterTime : MonoBehaviour
{
    public float secondsToLive = 5.0f;
    public bool startManually = false;

    private float startTIme;
    private bool isStarted = false;
    //초기화 위해

    void Awake()
    {
        startTIme = Time.time;
    }

    public void StartTimer()
    {
        startTIme = Time.time;
        isStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(startManually && !isStarted)
        {
            return;
        }

        if(Time.time - startTIme > secondsToLive)
        {
            Destroy(gameObject);
        }
    }
}
