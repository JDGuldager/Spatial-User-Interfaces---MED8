using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Oculus.Haptics;

public abstract class Ball : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] Rigidbody rb;
    [SerializeField] protected XRGrabInteractable interactable;

    private bool hasBeenGrabbed = false;


    [Header("Effects")]
    [SerializeField] protected GameObject effect;
    [SerializeField] protected GameObject onImpact;
    [SerializeField] protected float destroyDelay = 2f;

    [Header("Haptics")]
    [SerializeField] protected HapticClip hoverClip;
    [SerializeField] protected HapticClip grabClip;

    [Header("Fly To Hand")]
    [SerializeField] private Transform playerHandRight;
    [SerializeField] private Transform playerHandLeft;
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve grabCurve;

    private HapticClipPlayer rightHoverPlayer;
    private HapticClipPlayer leftHoverPlayer;
    private HapticClipPlayer rightGrabPlayer;
    private HapticClipPlayer leftGrabPlayer;

    private bool isHoveringLeft;
    private bool isHoveringRight;
    private bool canFly;
    private bool isFlyingToHand;

    private ControllerData controllerDataScript;

    protected virtual void Awake()
    {
        // Auto-fill references if they were not assigned on the prefab.
        // This is important because each prefab has its own serialized fields.
        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (sphereCollider == null)
            sphereCollider = GetComponent<SphereCollider>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (interactable == null)
            interactable = GetComponent<XRGrabInteractable>();

        if (interactable == null)
        {
            Debug.LogError($"{name} has no XRGrabInteractable assigned or found.");
            enabled = false;
            return;
        }

        if (rb == null)
            Debug.LogWarning($"{name} has no Rigidbody assigned or found.");

        if (sphereCollider == null)
            Debug.LogWarning($"{name} has no SphereCollider assigned or found.");

        if (meshRenderer == null)
            Debug.LogWarning($"{name} has no MeshRenderer assigned or found.");

        // Create haptic players only if clips exist.
        if (hoverClip != null)
        {
            rightHoverPlayer = new HapticClipPlayer(hoverClip);
            leftHoverPlayer = new HapticClipPlayer(hoverClip);

            rightHoverPlayer.isLooping = true;
            leftHoverPlayer.isLooping = true;
        }

        if (grabClip != null)
        {
            rightGrabPlayer = new HapticClipPlayer(grabClip);
            leftGrabPlayer = new HapticClipPlayer(grabClip);
        }

        if (onImpact != null)
            onImpact.SetActive(false);

        controllerDataScript = FindAnyObjectByType<ControllerData>();

        if (controllerDataScript == null)
            Debug.LogWarning("ControllerData was not found in the scene.");
    }

    protected virtual void Start()
    {
        if (VRReferences.Instance != null)
        {
            playerHandLeft = VRReferences.Instance.LeftHand;
            playerHandRight = VRReferences.Instance.RightHand;
        }
        else
        {
            Debug.LogError("VRReferences not found in scene.");
        }
    }

    protected virtual void Update()
    {
        if (hasBeenGrabbed)
            return;

        if (canFly)
        {
            BallToHand();
        }
    }

    private void OnEnable()
    {
        if (interactable == null)
            return;

        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        if (interactable == null)
            return;

        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
        interactable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Controller hand = GetController(args.interactorObject);

        Debug.Log($"{name} hover entered by {hand}");

        if (hand == Controller.Left)
        {
            if (isHoveringLeft)
                return;

            isHoveringLeft = true;
        }
        else
        {
            if (isHoveringRight)
                return;

            isHoveringRight = true;
        }

        PlayHoverClip(hand);

        canFly = true;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        Controller hand = GetController(args.interactorObject);

        Debug.Log($"{name} hover exited by {hand}");

        if (hand == Controller.Left)
        {
            if (!isHoveringLeft)
                return;

            isHoveringLeft = false;
        }
        else
        {
            if (!isHoveringRight)
                return;

            isHoveringRight = false;
        }

        StopHoverClip(hand);

        if (!isHoveringLeft && !isHoveringRight)
            canFly = false;
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        hasBeenGrabbed = true;
        canFly = false;

        StopHoverClip(GetController(args.interactorObject));
        PlayGrabClip(GetController(args.interactorObject));
    }

    private void PlayHoverClip(Controller hand)
    {
        if (hoverClip == null)
            return;

        switch (hand)
        {
            case Controller.Right:
                rightHoverPlayer?.Play(Controller.Right);
                break;

            case Controller.Left:
                leftHoverPlayer?.Play(Controller.Left);
                break;

            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
    }

    public void StopHoverClip(Controller hand)
    {
        if (hoverClip == null)
            return;

        switch (hand)
        {
            case Controller.Right:
                rightHoverPlayer?.Stop();
                break;

            case Controller.Left:
                leftHoverPlayer?.Stop();
                break;

            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
    }

    public void PlayGrabClip(Controller hand)
    {
        if (grabClip == null)
            return;

        switch (hand)
        {
            case Controller.Right:
                rightGrabPlayer?.Play(Controller.Right);
                break;

            case Controller.Left:
                leftGrabPlayer?.Play(Controller.Left);
                break;

            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
    }

    private Controller GetController(IXRInteractor interactor)
    {
        if (interactor is XRBaseInputInteractor controllerInteractor)
        {
            Controller oculusController =
                controllerInteractor.handedness == InteractorHandedness.Left
                    ? Controller.Left
                    : Controller.Right;

            return oculusController;
        }

        Debug.LogWarning("Interactor is not XRBaseInputInteractor. Defaulting to Right controller.");
        return Controller.Right;
    }

    public void BallToHand()
    {
        if (isFlyingToHand)
            return;

        if (controllerDataScript == null)
            return;

        Transform hand = null;

        if (controllerDataScript.leftVeloDetected && isHoveringLeft)
        {
            hand = playerHandLeft;
        }
        else if (controllerDataScript.rightVeloDetected && isHoveringRight)
        {
            hand = playerHandRight;
        }

        if (hand == null)
            return;

        canFly = false;
        isFlyingToHand = true;

        StartCoroutine(FlyToHandCoroutine(transform.position, hand.position, duration));
    }

    private IEnumerator FlyToHandCoroutine(Vector3 start, Vector3 target, float flyDuration)
    {
        float time = 0f;

        while (time < flyDuration)
        {
            time += Time.deltaTime;

            float t = time / flyDuration;
            float curvedT = grabCurve != null ? grabCurve.Evaluate(t) : t;

            transform.position = Vector3.Lerp(start, target, curvedT);

            yield return null;
        }

        transform.position = target;

        isFlyingToHand = false;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Floor"))
            return;

        if (meshRenderer != null)
            meshRenderer.enabled = false;

        if (sphereCollider != null)
            sphereCollider.enabled = false;

        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeAll;

        if (effect != null)
            effect.SetActive(false);

        if (onImpact != null)
            onImpact.SetActive(true);

        if (interactable != null)
            interactable.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}