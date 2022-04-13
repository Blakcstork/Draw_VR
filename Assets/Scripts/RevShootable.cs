using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevShootable : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioClip bulletHitSoundEffect;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    public float volume = 1f;

    private AudioSource audioSource;


    public virtual void Hit(RaycastHit hit, RevBullet bullet, Vector3 rayDirection)
    {
        if(audioSource == null)
        {
            if (!GetComponent<AudioSource>())
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
