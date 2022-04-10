using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevControllerManager : MonoBehaviour
{
    public static bool leftControllerActive;
    public static bool rightControllerActive;

    public static RevGrabbable nearestGrabbableToRightController = null;
    public static RevGrabbable nearestGrabbableToLeftController = null;

    public static float distanceToRightController = 10000f;
    public static float distanceToLeftController = 10000f;
}
