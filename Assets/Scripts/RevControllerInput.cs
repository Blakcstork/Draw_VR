using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public enum RevControllerType
{
    RevController_None,
    RevController_Left,
    RevController_Right
};

public enum RevInputButton
{
    RevButton_None = -1,
    RevButton_A = 0,
    RevButton_B,
    RevButton_System,
    RevButton_Menu,
    RevButton_Thumbstick_Press,
    RevButton_Trigger,
    RevButton_Grip,
    RevButton_Thumbstick_Left,
    RevButton_Thumbstick_Right,
    RevButton_Thumbstick_Down,
    RevButton_Thumbstick_Up
};

public class RevControllerInput : MonoBehaviour
{

    [Space(15)]
    [Header("Grip Settings")]
    public RevInputButton gripButton = RevInputButton.RevButton_Grip;
    public RevInputButton releaseGripButton = RevInputButton.RevButton_None;
    public bool gripAutoHolds = false;


    [Space(15)]
    [Header("Firing Settings")]
    public RevInputButton triggerButton = RevInputButton.RevButton_Trigger;

    [Space(15)]
    [Header("Reload Settings")]
    public RevInputButton openBarrelButton = RevInputButton.RevButton_None;
    public RevInputButton closeBarrelButton = RevInputButton.RevButton_None;


    public bool openWithPhysics = true;
    public float openAcceration = 4f;
    public float openEmptyAccerlation = 2f;
    public float closeAcceleration = -2f;


    [HideInInspector]
    public RevControllerType activeController;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
