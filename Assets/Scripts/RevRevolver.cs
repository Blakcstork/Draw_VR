using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevRevolver : MonoBehaviour
{


    /// -------------------------
    /// Variables
    /// -------------------------
    /// 

    [Space(15)]


    [Space(15)]
    [Header("Firing settings")]
    public GameObject bulletPrefab;
    public float muzzleVelocity = 200f;
    public LayerMask shothitLayers = -1;
    public int maxBullets = 6;
    public float kickForce = 30;
    public int numberOfChambers = 6;
    public float barrelRotationSpeed = 0.1f;


    [Space(15)]
    [Header("Reload Settings")]

    public GameObject shellPrefab;
    public float barrelOpenSpeed = 1000f;
    public float minRotationSpeedForReloading = 200f;
    public float openDurationReloadNeeded = 3f;
    public float minStayOpenDuration = 0.65f;
    public float shellEjectionForce = 1f;

    public Vector3 barrelOpenPosition;
    public Vector3 barrelClosedPosition;

    [Space(15)]
    [Header("Grab Settings")]
    public float grabDistance = 1;
    public float grabFlyTime = .1f;
    public bool shouldFly = true;

    [Tooltip("The thickness of the outline effect")]
    public float outlineThickness = 1f;
    public Color outlineColor;

    [Space(15)]
    [Header("Sounds and FX")]
    public AudioClip revolverOpenClip;
    public AudioClip revolverClickClip;
    public AudioClip revoloverCloseClip;
    public GameObject muzzleFlashPrefab;
    public GameObject dryFirePrefab;


    [Space(15)]
    [Header("Gun Components")]
    public RevRotator trigger;
    public RevRotator hammer;
    public RevRevolvingBarrel revolvingBarrel;


    /// <summary>
    /// Private Variables
    /// </summary>

    private RevGrabbable grabComponent;
    private RevFireBullet fireBulletComponent;
    private RevControllerInput input;
    private int curBullets = 6;

    private RevLinearAcceleration linearAccelerationTracker;



    // Start is called before the first frame update
    void Start()
    {
        this.grabComponent = GetComponent<RevGrabbable>();
        this.fireBulletComponent = GetComponent<RevFireBullet>();
        this.input = this.gameObject.GetComponent<RevControllerInput>();

        linearAccelerationTracker = new RevLinearAcceleration();
        revolvingBarrel.revolverParent = this;

        this.SetupComponents();

    }

    // Update is called once per frame
    void Update()
    {
        if(input.activeController != RevControllerType.RevController_None)
        {
            if (input.GetTriggerButtonPressed(input.activeController))
            {
                Fire();
            }

            if(!this.revolvingBarrel.isOpen && input.GetOpenBarrelPressed(input.activeController))
            {
                ToggleBarrel();
            }
            else if (this.revolvingBarrel.isOpen && input.GetCloseBarrelPressed(input.activeController))
            {
                ToggleBarrel();
            }

        }
    }

    void FixedUpdate()
    {
        if(input.activeController != RevControllerType.RevController_None && input.openWithPhysics && grabComponent.inHand)
        {
            Vector3 gunAcceleration;
            Vector3 gunVelocity = linearAccelerationTracker.LinearAccerlation(out gunAcceleration, this.transform.localPosition, 0);
            if (gunVelocity == Vector3.zero)
                return;
            gunAcceleration = this.transform.InverseTransformDirection(gunAcceleration);
            gunVelocity = this.transform.InverseTransformDirection(gunVelocity);

        }
    }
}
