using UnityEngine;
using UnityEngine.XR;

public class ControllerData : MonoBehaviour
{
    InputDevice leftControllerDevice;
    InputDevice rightControllerDevice;

    public Vector3 LeftControllerVelocity { get; private set; }
    public Vector3 RightControllerVelocity { get; private set; }

    public bool leftVeloDetected = false;
    public bool rightVeloDetected = false;

    [SerializeField] private float velocityThreshold = 0.5f;

    void Start()
    {
        leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        if (!leftControllerDevice.isValid)
            leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (!rightControllerDevice.isValid)
            rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        GetInput();

        leftVeloDetected = LeftControllerVelocity.magnitude > velocityThreshold;
        rightVeloDetected = RightControllerVelocity.magnitude > velocityThreshold;
    }

    void GetInput()
    {
        leftControllerDevice.TryGetFeatureValue(
            CommonUsages.deviceVelocity,
            out Vector3 leftVelocity
        );

        rightControllerDevice.TryGetFeatureValue(
            CommonUsages.deviceVelocity,
            out Vector3 rightVelocity
        );

        LeftControllerVelocity = leftVelocity;
        RightControllerVelocity = rightVelocity;
    }
}