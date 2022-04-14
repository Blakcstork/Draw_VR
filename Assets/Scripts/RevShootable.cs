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
            if (!GetComponent<AudioSource>()) //오디오 소스가 없으면
            {
                audioSource = gameObject.AddComponent<AudioSource>(); //추가
            }
            else
            {
                audioSource = gameObject.GetComponent<AudioSource>();
            }
            audioSource.clip = bulletHitSoundEffect;
        }

        if (GetComponent<Rigidbody>())
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 ImpactForce = bullet.bulletMass * Mathf.Pow(bullet.bulletVelocity, 2) * rayDirection; // E= mv^2
            rb.AddForceAtPosition(ImpactForce, hit.point);
        }

        if(bulletHitSoundEffect != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.volume = volume;
            audioSource.Play();
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
