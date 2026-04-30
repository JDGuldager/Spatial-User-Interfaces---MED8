using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerData : MonoBehaviour
{
    InputDevice LeftControllerDevice;
    InputDevice RightControllerDevice;
    Vector3 LeftControllerVelocity;
    Vector3 RightControllerVelocity;

    public bool leftVeloDetected = false;
    public bool rightVeloDetected = false;

    [SerializeField] private float velocityThreshold;

    void Start()
    {
        LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        RightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        if (!LeftControllerDevice.isValid)
            LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (!RightControllerDevice.isValid)
            RightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        GetInput();

        if (LeftControllerVelocity.magnitude > velocityThreshold)
        {
            leftVeloDetected = true;
        }
        else
        {
            leftVeloDetected = false;
        }

        if (RightControllerVelocity.magnitude > velocityThreshold)
        {
            rightVeloDetected = true;
        }
        else
        {
            rightVeloDetected = false;
        }
    }

    void GetInput()
    {
        if (LeftControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out LeftControllerVelocity))
        {
            Debug.Log("Left Velocity: " + LeftControllerVelocity);
        }
        else
        {
            Debug.Log("Left velocity NOT available");
        }

        if (RightControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out RightControllerVelocity))
        {
            Debug.Log("Right Velocity: " + RightControllerVelocity);
        }
        else
        {
            Debug.Log("Right velocity NOT available");
        }
    }
}
