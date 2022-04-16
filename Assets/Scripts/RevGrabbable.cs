using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RevGrabbable : MonoBehaviour
{

    public float grabDistance = 1;
    public float grabFlyTime = 2f;
    public bool shouldFly = true;

    [HideInInspector]
    public bool inHand = false;

    private RevOutline outlineComponent;

    private RevControllerInput input;
    private float grabStartTime;
    private Vector3 grabStartPosition;

    private float kickStartTime;
    private float maxKickDuration = 1f;
    private float minKickAngle = .25f;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
