using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class SceneReturnOnButton : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] string sceneSelectorName = "SceneSelector";

    [Header("Input")]
    [SerializeField] XRNode controller = XRNode.LeftHand;

    bool buttonWasPressedLastFrame;

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controller);

        if (!device.isValid)
            return;

        // Y button = secondary button on LEFT controller
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool pressed);

        if (pressed && !buttonWasPressedLastFrame)
        {
            SceneManager.LoadScene(sceneSelectorName);
        }

        buttonWasPressedLastFrame = pressed;
    }
}