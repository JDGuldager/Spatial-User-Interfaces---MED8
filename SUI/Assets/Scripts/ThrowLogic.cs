using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Idle   = doing nothing special
// Return = flying back to the player's hand
public enum State { Idle, Return }

public class ThrowLogic : MonoBehaviour
{
    [Header("Return Settings")]

    // How fast the object flies back to the hand
    [SerializeField] float returnSpeed = 20f;

    // How close the object needs to be before it snaps into the hand
    [SerializeField] float catchDistance = 0.25f;

    // Which controller button should recall the object
    // RightHand = right controller
    // LeftHand = left controller
    [SerializeField] XRNode returnController = XRNode.RightHand;

    // How far the physical trigger must be pressed before it counts
    [SerializeField] float triggerPressAmount = 0.7f;

    // The hand/controller the object should return to
    public Transform PlayersHand;

    Rigidbody rb;
    XRGrabInteractable grab;
    State state;

    // Used so the button only activates once per press
    bool triggerWasPressedLastFrame;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        if (grab == null)
        {
            Debug.LogError("No XRGrabInteractable found on this object!");
        }
    }

    private void Start()
    {
        state = State.Idle;
    }

    private void OnEnable()
    {
        if (grab != null)
        {
            // Called when the player grabs the object
            grab.selectEntered.AddListener(OnGrab);

            // Called when the player lets go of the object
            grab.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
        }
    }

    private void Update()
    {
        // Check if the player pressed the trigger button
        bool triggerPressedNow = IsReturnTriggerPressed();

        // This means: trigger was just pressed this frame,
        // not held down from the previous frame
        if (triggerPressedNow && !triggerWasPressedLastFrame)
        {
            ReturnObject();
        }

        triggerWasPressedLastFrame = triggerPressedNow;

        // If the object is currently returning, keep moving it toward the hand
        if (state == State.Return)
        {
            ReturnToHand();
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Remember which hand grabbed the object
        // This is where the object will return later
        PlayersHand = args.interactorObject.transform;

        Debug.Log("Object was grabbed by: " + PlayersHand.name);

        state = State.Idle;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Do NOT add custom throw force here.
        // XR Grab Interactable already handles throwing normally when released.
        state = State.Idle;
    }

    private bool IsReturnTriggerPressed()
    {
        // Get the VR controller device, for example the right hand controller
        InputDevice device = InputDevices.GetDeviceAtXRNode(returnController);

        if (!device.isValid)
        {
            return false;
        }

        // First try the simple trigger button value
        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButtonPressed))
        {
            if (triggerButtonPressed)
            {
                return true;
            }
        }

        // Also check the analog trigger amount
        // This is useful because some controllers report trigger pressure instead of just true/false
        if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            return triggerValue >= triggerPressAmount;
        }

        return false;
    }

    public void ReturnObject()
    {
        // Do not recall the object while it is currently being held
        if (grab != null && grab.isSelected)
        {
            return;
        }

        if (PlayersHand == null)
        {
            Debug.LogWarning("PlayersHand is missing. Grab the object first so it knows which hand to return to.");
            return;
        }

        // Detach from anything just in case
        transform.SetParent(null);

        // Make sure physics is active while flying back
        rb.isKinematic = false;

        // Start returning
        state = State.Return;
    }

    private void ReturnToHand()
    {
        Vector3 toHand = PlayersHand.position - transform.position;
        float distance = toHand.magnitude;

        if (distance > catchDistance)
        {
            Vector3 direction = toHand.normalized;

            // Fly directly toward the remembered hand
            rb.linearVelocity = direction * returnSpeed;
        }
        else
        {
            CatchObject();
        }
    }

    private void CatchObject()
    {
        state = State.Idle;

        // Stop all movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Stop physics from pulling it away
        rb.isKinematic = true;

        // Lock the object back to the hand
        transform.SetParent(PlayersHand);

        // Snap into the hand position
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}