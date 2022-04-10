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
    /// Getters Get 모음들
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


    public Vector3 RightControllerPosition // 오른쪽 Controller 위치
    {
        get
        {
#if USES_STEAM_VR
if(this.RightControllerIsConnected){
return controllerManager.right.transform.position;
    }
    return Vector3.zero;
#elif USES_OPEN_VR
            return OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

#else
return Vector3.zero;
#endif
        }
    }


    public Quaternion LeftControllerRotation // 좌 컨트롤러 회전
    {
        get
        {
            if (this.LeftControllerIsConnected)
            {
#if USES_STEAM_VR
return controllerManager.left.transform;
#elif USES_OPEN_VR
                return OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
#endif
            }

            return Quaternion.identity;
        }
    }

    public Quaternion RightControllerRotation // 우 컨트롤러 회전
    {
        get
        {
            if (this.RightControllerIsConnected)
            {
#if USES_STEAM_VR
return controllerManager.right.transform;
#elif USES_OPEN_VR
                return OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
#endif
            }

            return Quaternion.identity;
        }
    }



    ///
    /// Controller info 컨트롤러 정보
    ///

    public Vector3 PositionForController(RevControllerType controller)
    {
        if(controller == RevControllerType.RevController_Left)
        {
            return LeftControllerPosition;
        }
        else if (controller == RevControllerType.RevController_Right)
        {
            return RightControllerPosition;
        }

        return Vector3.zero; // 기본값
    }

    public Quaternion RotationForController(RevControllerType controller)
    {
        if (controller == RevControllerType.RevController_Left)
        {
            return LeftControllerRotation;
        }
        else if (controller == RevControllerType.RevController_Right)
        {
            return RightControllerRotation;
        }

        return Quaternion.identity; // 기본값
    }

    public bool ControllerIsConnected(RevControllerType controller)
    {
        if (controller == RevControllerType.RevController_Left)
        {
            return LeftControllerIsConnected;
        }
        else if (controller == RevControllerType.RevController_Right)
        {
            return RightControllerIsConnected;
        }

        return false; // 기본값
    }


    /// -----------------------------
    /// Input Checkers 인풋 확인
    /// -----------------------------
    
    public bool GetGripButtonDown(RevControllerType controller)
    {
        return this.GetButtonDown(controller, this.gripButton);
    }

    public bool GetGripButtonPressed(RevControllerType controller)
    {
        return this.GetButtonPressDown(controller, this.gripButton);
    }
    public bool GetReleaseGripButtonPressed(RevControllerType controller)
    {
        return this.GetButtonPressDown(controller, this.releaseGripButton);
    }

    public bool GetTriggerButtonPressed(RevControllerType controller)
    {
        return this.GetButtonPressDown(controller, this.triggerButton);
    }

    public bool GetOpenBarrelPressed(RevControllerType controller)
    {
        return this.GetButtonPressDown(controller, this.openBarrelButton);
    }

    public bool GetCloseBarrelPressed(RevControllerType controller)
    {
        return this.GetButtonPressDown(controller, this.openBarrelButton);
    }


    /// -----------------
    /// Public
    /// ------------------

    public bool GetButtonDown(RevControllerType controller, RevInputButton button)
    {
        if(button == RevInputButton.RevButton_None || !ControllerIsConnected(controller))
        {
            return false;
        }

#if USES_STEAM_VR
return Controller(controller).GetPress(GetSteamButtonMapping(button));
#elif USES_OPEN_VR
        return GetOVRButtonDown(controller, button);
#endif
    }

    public bool GetButtonPressDown(RevControllerType controller, RevInputButton button)
    {
        if (button == RevInputButton.RevButton_None || !ControllerIsConnected(controller))
        {
            return false;
        }

#if USES_STEAM_VR
return Controller(controller).GetPressDown(GetSteamButtonMapping(button));
#elif USES_OPEN_VR
        return GetOVRButtonPressDown(controller, button);
#endif
    }



    public bool SetActiveController(RevControllerType activeController) // 컨트롤러 set
    {
        if(activeController == RevControllerType.RevController_Left && RevControllerManager.leftControllerActive)
        {
            return false;
        }

        if(activeController == RevControllerType.RevController_Right && RevControllerManager.rightControllerActive)
        {
            return false;
        }

        this.activeController = activeController;

#if USES_STEAM_VR
		this.activeControllerDevice = Controller (activeController);
		this.activeRenderModel = SteamController(this.activeController).GetComponentInChildren<SteamVR_RenderModel>();
#endif

        if(this.activeController == RevControllerType.RevController_Right)
        {
            RevControllerManager.rightControllerActive = true;
        }
        else
        {
            RevControllerManager.leftControllerActive = true;
        }

        return true;
    }

    public void ClearActController() // 컨트롤러 해제
    {
#if USES_STEAM_VR
this.activeControllerDevice = null;
this.activeRenderModel = null;
#endif

        if(this.activeController == RevControllerType.RevController_Right)
        {
            RevControllerManager.rightControllerActive = false;
        }
        else
        {
            RevControllerManager.leftControllerActive = false;
        }
    }

    public void RumbleActiveController(float rumbleLength) // 컨트롤러 진동
    {
#if USES_STEAM_VR
if(activeControllerDevice != null){
StartCoroutine(LongViberation(activeControllerDevice, rumbleLength, 1.0f));
}
#elif USES_OPEN_VR
        StartCoroutine(OVRVibrateForTime(rumbleLength));
#endif
    }

    public Vector3 ActiveControllerAngularVelocity() // 컨트롤러 각속도
    {
#if USES_STEAM_VR
return this.activeControllerDevice.angularVelocity;
#elif USES_OPEN_VR
        if(activeController == RevControllerType.RevController_Left)
        {
            return OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.LTouch);
        }
        else if (activeController == RevControllerType.RevController_Right)
        {
            return OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
        }
        return Vector3.zero;
#else
return Vector3.zero;
#endif
    }
    //------------------------
    // Visibility
    //------------------------
    public void HideActiveModel()
    {
#if USES_STEAM_VR
		this.activeRenderModel.gameObject.SetActive (false);
#endif
    }

    public void ShowActiveModel()
    {
#if USES_STEAM_VR
		this.activeRenderModel.gameObject.SetActive (true);
#endif
    }

    //------------------------
    // Haptics
    //------------------------


#if USES_STEAM_VR
	//length is how long the vibration should go for
	//strength is vibration strength from 0-1
	private IEnumerator LongVibration(SteamVR_Controller.Device device, float totalLength, float strength) {
		ushort rLength = (ushort)Mathf.Lerp (0, 3999, strength);
		for (float i = 0f; i < totalLength; i += Time.deltaTime) {
			device.TriggerHapticPulse(rLength);
			yield return null;
		}
	}

	//vibrationCount is how many vibrations
	//vibrationLength is how long each vibration should go for
	//gapLength is how long to wait between vibrations
	//strength is vibration strength from 0-1
	IEnumerator LongVibration(SteamVR_Controller.Device device, int vibrationCount, float vibrationLength, float gapLength, float strength) {
		strength = Mathf.Clamp01(strength);
		for(int i = 0; i < vibrationCount; i++) {
			if(i != 0) yield return new WaitForSeconds(gapLength);
			yield return StartCoroutine(LongVibration(device, vibrationLength, strength));
		}
	}
#endif

#if USES_OPEN_VR

    public IEnumerator OVRVibrateForTime(float time)
    {
        OVRHaptics.OVRHapticsChannel channel;
        if(activeController == RevControllerType.RevController_Left)
        {
            channel = OVRHaptics.LeftChannel;
        }
        else
        {
            channel = OVRHaptics.RightChannel;
        }

        for(float t= 0; t<time ; t+= Time.deltaTime)
        {
            channel.Queue(clipHard);
        }

        yield return new WaitForSeconds(time);
        channel.Clear();
        yield return null;
    }
#endif


    //------------------------
    // Steam Mappings
    //------------------------


#if USES_STEAM_VR

	private Valve.VR.EVRButtonId GetSteamButtonMapping(SVInputButton button) {
		switch (button) {
			case SVInputButton.SVButton_A:
			return Valve.VR.EVRButtonId.k_EButton_A;
			case SVInputButton.SVButton_B:
			return Valve.VR.EVRButtonId.k_EButton_A;
			case SVInputButton.SVButton_Grip:
			return Valve.VR.EVRButtonId.k_EButton_Grip;
			case SVInputButton.SVButton_Menu:
			return Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
			case SVInputButton.SVButton_System:
			return Valve.VR.EVRButtonId.k_EButton_System;
			case SVInputButton.SVButton_Trigger:
			return Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
			case SVInputButton.SVButton_Thumbstick_Down:
			return Valve.VR.EVRButtonId.k_EButton_DPad_Down;
			case SVInputButton.SVButton_Thumbstick_Left:
			return Valve.VR.EVRButtonId.k_EButton_DPad_Left;
			case SVInputButton.SVButton_Thumbstick_Right:
			return Valve.VR.EVRButtonId.k_EButton_DPad_Right;
			case SVInputButton.SVButton_Thumbstick_Up:
			return Valve.VR.EVRButtonId.k_EButton_DPad_Up;
			case SVInputButton.SVButton_Thumbstick_Press:
			return Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
		}
		return (Valve.VR.EVRButtonId)0;
	}
#endif




    //------------------------
    // OVR Mappings
    //------------------------
#if USES_OPEN_VR
    private OVRInput.Button GetOVRButtonMapping(RevInputButton button)
    {
        switch (button)
        {
            case RevInputButton.RevButton_A:
                return OVRInput.Button.One;
            case RevInputButton.RevButton_B:
                return OVRInput.Button.Two;
            case RevInputButton.RevButton_System:
                return OVRInput.Button.Start;
            case RevInputButton.RevButton_Thumbstick_Press:
                return OVRInput.Button.PrimaryThumbstick;
        }

        return (OVRInput.Button)0;
    }



    private bool GetOVRButtonPressDown(RevControllerType controller, RevInputButton button)
    {
        bool isRight = (controller == RevControllerType.RevController_Right);
        Dictionary<int, bool> buttonState = isRight ? this.buttonStateRight : this.buttonStateLeft;

        bool isDown = GetOVRButtonDown(controller, button);
        bool inputIsDown = buttonState.ContainsKey((int)button) && (bool)buttonState[(int)button];
        bool isPressDown = (!inputIsDown && isDown);
        buttonState[(int)button] = isDown;
        return isPressDown;
    }


    private bool GetOVRButtonDown(RevControllerType controller, RevInputButton button)
    {
        bool isRight = (controller == RevControllerType.RevController_Right);
        OVRInput.Controller ovrController = (isRight ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch);

        switch (button)
        {
            case RevInputButton.RevButton_A:
            case RevInputButton.RevButton_B:
            case RevInputButton.RevButton_System:
            case RevInputButton.RevButton_Thumbstick_Press:
                return OVRInput.Get(GetOVRButtonMapping(button), ovrController);

            case RevInputButton.RevButton_Thumbstick_Down:
            case RevInputButton.RevButton_Thumbstick_Up:
            case RevInputButton.RevButton_Thumbstick_Right:
            case RevInputButton.RevButton_Thumbstick_Left:
                {
                    OVRInput.Axis2D axis2D = OVRInput.Axis2D.PrimaryThumbstick;

                    Vector2 vec = OVRInput.Get(axis2D, ovrController);

                    if(button == RevInputButton.RevButton_Thumbstick_Down)
                    {
                        return vec.y < -0.75;
                    }
                    else if (button == RevInputButton.RevButton_Thumbstick_Up)
                    {
                        return vec.y > -0.75;
                    }
                    else if (button == RevInputButton.RevButton_Thumbstick_Left)
                    {
                        return vec.x < -0.75;
                    }
                    else if (button == RevInputButton.RevButton_Thumbstick_Right)
                    {
                        return vec.x > -0.75;
                    }

                    return false;
                }
            case RevInputButton.RevButton_Trigger:
            case RevInputButton.RevButton_Grip:
                {
                    OVRInput.Axis1D axis = OVRInput.Axis1D.PrimaryIndexTrigger;
                    if(button == RevInputButton.RevButton_Trigger)
                    {
                        axis = OVRInput.Axis1D.PrimaryIndexTrigger;
                    }
                    else if (button == RevInputButton.RevButton_Grip)
                    {
                        axis = OVRInput.Axis1D.PrimaryHandTrigger;
                    }
                    return (OVRInput.Get(axis, ovrController) > 0.75f);
                }
            default:
                return false;
        }
    }
#endif
}