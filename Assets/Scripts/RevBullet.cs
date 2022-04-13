using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevBullet : MonoBehaviour
{

    /// <summary>
    /// Variables
    /// </summary>
    /// 

    public float bulletVelocity;
    public float bulletDamage = 1;
    public float bulletLifetime = 3;
    public float bulletMass = 0.015f;


    public LayerMask hitLayers = -1;
    private float startTime;
    private bool hasHit = false;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }


    void FixedUpdate()
    {
        if (this.hasHit)
        {
            Destroy(gameObject);
            return;
        }

        float distanceTraveled = Time.fixedDeltaTime * bulletVelocity;

        RaycastHit hitOut;
        bool hit = Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.forward), out hitOut, distanceTraveled, hitLayers);


        if (hit)
        {
            if(hitOut.rigidbody != null)
            {
                HitWithObject(hitOut.rigidbody.gameObject, hitOut);
            }
            else if(hitOut.collider != null)
            {
                HitWithObject(hitOut.collider.gameObject, hitOut);
            }

            this.transform.position += this.transform.TransformDirection(Vector3.forward) * hitOut.distance;
            this.hasHit = true;
        }
        else
        {
            this.transform.position += this.transform.TransformDirection(Vector3.forward) * distanceTraveled;
            if(Time.time - this.startTime > bulletLifetime)
            {
                Destroy(gameObject);
            } // ½Ã°£ ´ÙµÇ¸é ÃÑ¾Ë »ç¶óÁü
        }
    }
    
    private void HitWithObject(GameObject hitObject, RaycastHit hit)
    {
        if (hitObject.GetComponent<RevShootable>())
        {
            RevShootable shootable = hitObject.GetComponent<RevShootable>();
            shootable.Hit(hit, this, this.transform.TransformDirection(Vector3.forward));
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
