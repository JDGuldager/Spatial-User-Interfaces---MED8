using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public enum State { Idle, Return }

public class ThrowLogic : MonoBehaviour
{
    [Header("Return Settings")]
    [SerializeField] float returnSpeed = 20f;
    [SerializeField] float catchDistance = 0.35f;
    [SerializeField] float releaseBufferTime = 0.5f;

    [Header("Controlled Throw")]
    [SerializeField] float throwPowerMultiplier = 2.5f;
    [SerializeField] float minimumThrowSpeed = 6f;

    [Header("Crosshair")]
    [SerializeField] GameObject crosshairPrefab;
    [SerializeField] float crosshairForwardOffset = 3f;

    [Header("Input Settings")]
    [SerializeField] float triggerPressAmount = 0.7f;

    GameObject crosshairInstance;

    public Transform PlayersHand;

    Transform aimHand;
    Transform leftHandController;
    Transform rightHandController;

    Rigidbody rb;
    XRGrabInteractable grab;
    State state;

    bool triggerWasPressedLastFrame;
    float lastReleaseTime = -999f;

    static ThrowLogic lastGrabbedObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        if (grab == null)
            Debug.LogError("No XRGrabInteractable found on this object!");
    }

    private void Start()
    {
        state = State.Idle;

        if (VRReferences.Instance != null)
        {
            leftHandController = VRReferences.Instance.LeftHand;
            rightHandController = VRReferences.Instance.RightHand;
        }
        else
        {
            Debug.LogError("VRReferences not found in scene.");
        }
    }

    private void OnEnable()
    {
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
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

    private void OnDestroy()
    {
        DespawnCrosshair();
    }

    private void Update()
    {
        UpdateCrosshair();

        bool triggerPressedNow = IsReturnTriggerPressed();

        if (triggerPressedNow && !triggerWasPressedLastFrame)
            ReturnObject();

        triggerWasPressedLastFrame = triggerPressedNow;
    }

    private void FixedUpdate()
    {
        if (state == State.Return)
            ReturnToHand();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        PlayersHand = args.interactorObject.transform;
        lastGrabbedObject = this;

        SetAimHand();

        state = State.Idle;

        SpawnCrosshair();

        if (VRReferences.Instance != null && aimHand != null)
            VRReferences.Instance.SetVisualVisibleForHand(aimHand, false);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        lastReleaseTime = Time.time;

        Vector3 crosshairTarget;

        if (aimHand != null)
        {
            crosshairTarget = aimHand.position + aimHand.forward * crosshairForwardOffset;
        }
        else
        {
            crosshairTarget = transform.position + transform.forward * crosshairForwardOffset;
        }

        DespawnCrosshair();

        if (VRReferences.Instance != null && aimHand != null)
            VRReferences.Instance.SetVisualVisibleForHand(aimHand, true);

        StartCoroutine(ApplyControlledThrow(crosshairTarget));

        state = State.Idle;
    }

    private IEnumerator ApplyControlledThrow(Vector3 crosshairTarget)
    {
        yield return new WaitForFixedUpdate();

        rb.isKinematic = false;
        rb.useGravity = true;

        Vector3 controllerVelocity = GetThrowingControllerVelocity();

        Vector3 throwDirection =
            (crosshairTarget - transform.position).normalized;

        float speed =
            Mathf.Max(controllerVelocity.magnitude * throwPowerMultiplier, minimumThrowSpeed);

        rb.linearVelocity = throwDirection * speed;
    }

    private void SetAimHand()
    {
        if (leftHandController == null || rightHandController == null)
        {
            Debug.LogWarning("Missing hand references. Check VRReferences.");
            return;
        }

        float distanceToLeft =
            Vector3.Distance(PlayersHand.position, leftHandController.position);

        float distanceToRight =
            Vector3.Distance(PlayersHand.position, rightHandController.position);

        if (distanceToLeft < distanceToRight)
        {
            aimHand = rightHandController;
        }
        else
        {
            aimHand = leftHandController;
        }
    }

    private void SpawnCrosshair()
    {
        if (crosshairPrefab == null)
            return;

        if (crosshairInstance != null)
            Destroy(crosshairInstance);

        crosshairInstance = Instantiate(crosshairPrefab);
        crosshairInstance.SetActive(true);
    }

    private void DespawnCrosshair()
    {
        if (crosshairInstance != null)
        {
            Destroy(crosshairInstance);
            crosshairInstance = null;
        }
    }

    private void UpdateCrosshair()
    {
        if (crosshairInstance == null)
            return;

        if (grab != null && grab.isSelected && aimHand != null)
        {
            crosshairInstance.SetActive(true);

            crosshairInstance.transform.position =
                aimHand.position + aimHand.forward * crosshairForwardOffset;

            if (Camera.main != null)
            {
                Vector3 directionToCamera =
                    crosshairInstance.transform.position - Camera.main.transform.position;

                crosshairInstance.transform.rotation =
                    Quaternion.LookRotation(directionToCamera);
            }
        }
        else
        {
            crosshairInstance.SetActive(false);
        }
    }

    private bool IsReturnTriggerPressed()
    {
        XRNode returnController = GetReturnControllerNode();
        InputDevice device = InputDevices.GetDeviceAtXRNode(returnController);

        if (!device.isValid)
            return false;

        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButtonPressed))
        {
            if (triggerButtonPressed)
                return true;
        }

        if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            return triggerValue >= triggerPressAmount;

        return false;
    }

    private XRNode GetReturnControllerNode()
    {
        if (PlayersHand == null)
            return XRNode.RightHand;

        float distanceToLeft =
            leftHandController != null
                ? Vector3.Distance(PlayersHand.position, leftHandController.position)
                : float.MaxValue;

        float distanceToRight =
            rightHandController != null
                ? Vector3.Distance(PlayersHand.position, rightHandController.position)
                : float.MaxValue;

        return distanceToLeft < distanceToRight ? XRNode.LeftHand : XRNode.RightHand;
    }

    public void ReturnObject()
    {
        if (lastGrabbedObject != this)
            return;

        if (grab != null && grab.isSelected)
            return;

        if (Time.time < lastReleaseTime + releaseBufferTime)
            return;

        if (PlayersHand == null)
        {
            Debug.LogWarning("PlayersHand is missing. Grab the object first.");
            return;
        }

        transform.SetParent(null, true);

        rb.isKinematic = false;
        rb.useGravity = false;

        state = State.Return;
    }

    private void ReturnToHand()
    {
        Vector3 toHand = PlayersHand.position - transform.position;
        float distance = toHand.magnitude;

        if (distance > catchDistance)
        {
            Vector3 direction = toHand.normalized;
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

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.useGravity = false;

        transform.SetParent(PlayersHand, true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    private Vector3 GetThrowingControllerVelocity()
    {
        if (VRReferences.Instance == null || VRReferences.Instance.ControllerData == null)
            return Vector3.zero;

        if (PlayersHand == null || leftHandController == null || rightHandController == null)
            return Vector3.zero;

        float distanceToLeft =
            Vector3.Distance(PlayersHand.position, leftHandController.position);

        float distanceToRight =
            Vector3.Distance(PlayersHand.position, rightHandController.position);

        if (distanceToLeft < distanceToRight)
            return VRReferences.Instance.ControllerData.LeftControllerVelocity;

        return VRReferences.Instance.ControllerData.RightControllerVelocity;
    }
}