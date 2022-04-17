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
    private bool isKicking = false;
    private Quaternion kickOffset;

    private Quaternion grabStartRotation;

    /// ---------------
    /// init 
    /// ---------------




    // Start is called before the first frame update
    void Start()
    {
        outlineComponent = this.gameObject.GetComponent<RevOutline>();
        this.input = this.gameObject.GetComponent<RevControllerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.input.activeController == RevControllerType.RevController_None)
        {
            this.UngrabbedUpdate();
        }
        else
        {
            this.GrabbedUpdate();
        }
    }


    private void UngrabbedUpdate()
    {
        this.inHand = false;

        float distanceToLeftHand = 1000;
        if (input.LeftControllerIsConnected)
        {
            distanceToLeftHand = (this.transform.position - input.LeftControllerPosition).magnitude;
        }

        float distanceToRightHand = 1000;
        if (input.RightControllerIsConnected)
        {
            distanceToRightHand = (this.transform.position - input.RightControllerPosition).magnitude;
        }


        if(grabDistance > distanceToLeftHand || grabDistance > distanceToRightHand)
        {
            float distance = Mathf.Min(distanceToLeftHand, distanceToRightHand);
            if (this.outlineComponent)
            {
                float distanceForHighlight = grabDistance / 4f;
                float highlight = Mathf.Max(0, Mathf.Min(1, (grabDistance - distance) / distanceForHighlight));
                outlineComponent.outlineActive = highlight;
            }


            RevControllerType firstController = RevControllerType.RevController_None;
            RevControllerType secondController = RevControllerType.RevController_None;

            if(distanceToLeftHand < distanceToRightHand)
            {
                if (RevControllerManager.nearestGrabbableToLeftController == this)
                    firstController = RevControllerType.RevController_Left;
                if (RevControllerManager.nearestGrabbableToRightController == this)
                    secondController = RevControllerType.RevController_Right;
            }
            else
            {
                if (RevControllerManager.nearestGrabbableToRightController == this)
                    firstController = RevControllerType.RevController_Right;
                if (RevControllerManager.nearestGrabbableToLeftController == this)
                    secondController = RevControllerType.RevController_Left;
            }

            TrySetActiveController(firstController);
            TrySetActiveController(secondController);



            // 항상 가까운 총을 집도록 grabbable 거리 업데이트
            if(distanceToLeftHand < RevControllerManager.distanceToLeftController || 
                RevControllerManager.nearestGrabbableToLeftController == null ||
                RevControllerManager.nearestGrabbableToLeftController == this)
            {
                RevControllerManager.nearestGrabbableToLeftController = this;
                RevControllerManager.distanceToLeftController = distanceToLeftHand;
            }

            if (distanceToLeftHand < RevControllerManager.distanceToRightController ||
                RevControllerManager.nearestGrabbableToRightController == null ||
                RevControllerManager.nearestGrabbableToRightController == this)
            {
                RevControllerManager.nearestGrabbableToRightController = this;
                RevControllerManager.distanceToRightController = distanceToRightHand;
            }
            else
            {
                outlineComponent.outlineActive = 0;
            }
        }
    }


    private void GrabbedUpdate()
    {
        if (input.gripAutoHolds)
        {
            if (input.GetReleaseGripButtonPressed(input.activeController))
            {
                this.ClearActiveController();
                return;
            }
        }
        else if (!input.GetGripButtonDown(input.activeController))
        {
            this.ClearActiveController();
            return;
        }

        float percComplete = (Time.time - this.grabStartTime) / this.grabFlyTime;
        if(percComplete < 1 && this.shouldFly)
        {
            this.inHand = false;
            transform.position = Vector3.Lerp(this.grabStartPosition, this.input.PositionForController(this.input.activeController), percComplete);
            transform.rotation = Quaternion.Lerp(this.grabStartRotation, this.input.RotationForController(this.input.activeController), percComplete);
        }
        else if (isKicking)
        {
            this.kickOffset = Quaternion.Lerp(this.kickOffset, Quaternion.identity, 0.05f);
            this.transform.SetPositionAndRotation(this.input.PositionForController(this.input.activeController), this.input.RotationForController(this.input.activeController));

            float curAngle = Quaternion.Angle(this.kickOffset, Quaternion.identity);
            if(curAngle < minKickAngle || Time.time - this.kickStartTime > maxKickDuration)
            {
                this.isKicking = false;
            }

            else
            {
                this.inHand = true;
                this.transform.SetPositionAndRotation(this.input.PositionForController(this.input.activeController), this.input.RotationForController(this.input.activeController));
            }
        }
    }


    /// ---------------------
    /// Kick
    /// ---------------------
    /// 


    public void EditGripForKick(float kickForce)
    {
        kickStartTime = Time.time;
        isKicking = true;

        Quaternion upRotation = Quaternion.AngleAxis(-90, Vector3.right);
        this.kickOffset = Quaternion.RotateTowards(Quaternion.identity, upRotation, kickForce);
    }




    /// 상태변화
    /// 

    private void TrySetActiveController(RevControllerType controller)
    {
        if (this.input.activeController != RevControllerType.RevController_None ||
            controller == RevControllerType.RevController_None)
            return;
        if (input.gripAutoHolds)
        {
            if (!input.GetGripButtonPressed(controller))
            {
                return;
            }
            else
            {
                if (!input.GetGripButtonDown(controller))
                {
                    return;
                }
            }
        }

        if (this.input.SetActiveController(controller))
        {
            this.grabStartTime = Time.time;
            this.grabStartPosition = this.gameObject.transform.position;
            this.grabStartRotation = this.gameObject.transform.rotation;
            outlineComponent.outlineActive = 0;

            Rigidbody rigidbody = this.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;

            // controller model 숨기기
            this.input.HideActiveModel();

        }
    }

    /// <summary>
    /// /////
    /// 
    /// </summary>
    private void ClearActiveController()
    {
        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        //rigidbody.velocity = this.input.ActiveControllerVelocity();
        rigidbody.angularVelocity = this.input.ActiveControllerAngularVelocity();


        //render model 
        this.input.ShowActiveModel();
        this.input.ClearActController();
    }
}