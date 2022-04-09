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


#if USES_STEAM_VR
[HideInInspector]
public SteamVR_RenderModel activeRenderModel;
[HideInInspector]
public SteamVR_RenderModel activeRenderModel;
private SteamVR_ControllerManager controllerManager;
#elif USES_OPEN_VR
    private OVRManager controllerManager;
    private Dictionary<int, bool> buttonStateLeft;
    private Dictionary<int, bool> buttonStateRight;
    OVRHapticsClip clipHard;
#endif


    /// ---------------
    /// Setup
    /// ---------------

    // Start is called before the first frame update
    void Start()
    {
#if USES_STEAM_VR
controller Manager = Object.FindObjectOfType<SteamVR_ControllerManager>();
Assert.IsNotNull(controllerManager, "SVControllerInput(with SteamVR) Needs a SteamVR_ControllerManager in the scene to function correctly.");
#elif USES_OPEN_VR
        controllerManager = Object.FindObjectOfType<OVRManager>();
        Assert.IsNotNull(controllerManager, "RevControllerInput (with Open VR) Needs a OVRManager in the scene to function correctly.");

        buttonStateLeft = new Dictionary<int, bool>();
        buttonStateRight = new Dictionary<int, bool>();

        int cnt = 10;
        clipHard = new OVRHapticsClip(cnt);

        for (int i = 0; i < cnt; i++)
        {
            clipHard.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)180;
        }
        clipHard = new OVRHapticsClip(clipHard.Samples, clipHard.Samples.Length);
#else
Debug.LogError("Revolver Kit requires you to choose either Steam VR or Oculus SDK");
#endif


    }

    /// <------------->
    /// Getters
    /// <------------->


#if USES_STEAM_VR
	private GameObject SteamController(SVControllerType type) {
		return (type == SVControllerType.SVController_Left ? controllerManager.left : controllerManager.right); //TODO : Cache this component for performance
	}

	private SteamVR_Controller.Device Controller(SVControllerType type) {
		GameObject steamController = (type == SVControllerType.SVController_Left ? controllerManager.left : controllerManager.right); //TODO : Cache this component for performance
		return SteamVR_Controller.Input((int)steamController.GetComponent<SteamVR_TrackedObject>().index);
	}
#endif


    public bool LeftControllerIsConnected // LeftController Connected 되었는지?
    {
        get
        {
#if USES_STEAM_VR
return (controllerManager.left != null && controllerManager.left.activeHierarchy);

#elif USES_OPEN_VR
            return ((OVRInput.GetConnectedControllers() & OVRInput.Controller.LTouch) == OVRInput.Controller.LTouch);
#else
return false;
#endif
        }
    }

    public bool RightControllerIsConnected // LeftController Connected 되었는지?
    {
        get
        {
#if USES_STEAM_VR
return (controllerManager.right != null && controllerManager.right.activeHierarchy);

#elif USES_OPEN_VR
            return ((OVRInput.GetConnectedControllers() & OVRInput.Controller.RTouch) == OVRInput.Controller.RTouch);
#else
return false;
#endif
        }
    }

    public Vector3 LeftControllerPosition // 왼쪽 Controller 위치
    {
        get
        {
#if USES_STEAM_VR
if(this.LeftControllerIsConnected){
return controllerManager.left.transform.position;
    }
    return Vector3.zero;
#elif USES_OPEN_VR
            return OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);

#else
return Vector3.zero;
#endif
        }
    }
}