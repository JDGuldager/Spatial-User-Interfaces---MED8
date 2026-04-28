
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Oculus.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Security.Cryptography;
using System.Collections;

public abstract class Ball : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] Rigidbody rb;

    [SerializeField] protected GameObject effect;
    [SerializeField] protected GameObject onImpact;
    [SerializeField] protected float destroyDelay = 2f;
    [SerializeField] protected XRGrabInteractable interactable;

    [SerializeField] protected HapticClip hoverClip;
    [SerializeField] protected HapticClip grabClip;

    private HapticClipPlayer rightHoverPlayer;
    private HapticClipPlayer leftHoverPlayer;
    private HapticClipPlayer rightGrabPlayer;
    private HapticClipPlayer leftGrabPlayer;

    private bool isHoveringLeft = false;
    private bool isHoveringRight = false;

    [SerializeField] private Transform playerHand;
    private float duration = 1f;
    [SerializeField] private AnimationCurve grabCurve;
    private float timeElapsed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rightHoverPlayer = new HapticClipPlayer(hoverClip);
        leftHoverPlayer = new HapticClipPlayer(hoverClip);

        rightGrabPlayer = new HapticClipPlayer(grabClip);
        leftGrabPlayer = new HapticClipPlayer(grabClip);


        rightHoverPlayer.isLooping = true;
        leftHoverPlayer.isLooping = true;


        if (onImpact != null)
        {
            onImpact.SetActive(false);
        }
        
    }

    protected virtual void Update()
    {
        
    }

    private void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.selectEntered.AddListener(OnGrabbed);
        interactable.hoverExited.AddListener(OnHoverExited);
    }

    private void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.selectEntered.RemoveListener(OnGrabbed);
        interactable.hoverExited.RemoveListener(OnHoverExited);
    }

    //Events
    

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        var hand = GetController(args.interactorObject);

        if (hand == Controller.Left)
        {
            if (isHoveringLeft) return; 
            isHoveringLeft = true;
        }
        else
        {
            if (isHoveringRight) return;
            isHoveringRight = true;
        }

        PlayHoverClip(hand);
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        var hand = GetController(args.interactorObject);

        if (hand == Controller.Left)
        {
            if (!isHoveringLeft) return;
            isHoveringLeft = false;
        }
        else
        {
            if (!isHoveringRight) return;
            isHoveringRight = false;
        }

        StopHoverClip(hand);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        StopHoverClip(GetController(args.interactorObject));
        PlayGrabClip(GetController(args.interactorObject));
        StartCoroutine(FlyToHandCoroutine(transform.position, playerHand.position, duration));
    }

    void PlayHoverClip(Controller hand)
    {
        switch (hand)
        {
            case Controller.Right:
                rightHoverPlayer.Play(Controller.Right);
                break;
            case Controller.Left:
                leftHoverPlayer.Play(Controller.Left);
                break;
            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
        Debug.Log("Should feel vibration from clipPlayer1 on " + hand + " controller.");
    }
    public void StopHoverClip(Controller hand)
    {
        switch (hand)
        {
            case Controller.Right:
                rightHoverPlayer.Stop();
                break;
            case Controller.Left:
                leftHoverPlayer.Stop();
                break;
            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
        Debug.Log("Vibration from clipPlayer1 should stop on hand " + hand + ".");
    }

    public void PlayGrabClip(Controller hand)
    {
        switch (hand)
        {
            case Controller.Right:
                rightGrabPlayer.Play(Controller.Right);
                break;
            case Controller.Left:
                leftGrabPlayer.Play(Controller.Left);
                break;
            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
        Debug.Log("Should feel vibration from grabClip on " + hand + " controller.");
    }    

    private Controller GetController(IXRInteractor interactor)
    {
        if (interactor is XRBaseInputInteractor controllerInteractor)
        {
            var oculusController = controllerInteractor.handedness == InteractorHandedness.Left
                ? Controller.Left
                : Controller.Right;

            Debug.Log("Playing haptic on " + controllerInteractor.handedness);

            return oculusController;
        }
        Debug.LogWarning("Interactor is not XRBaseInputInteractor! Defaulting to Right controller.");

        return Controller.Right;
    }

    

    private IEnumerator FlyToHandCoroutine(Vector3 start, Vector3 target, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float curvedT = grabCurve.Evaluate(t);

            transform.position = Vector3.Lerp(start, target, curvedT);

            yield return null;
        }
        transform.position = target;
    }


    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            meshRenderer.enabled = false; 
            sphereCollider.enabled = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            effect.SetActive(false);
            onImpact.SetActive(true);
            interactable.enabled = false;
            Destroy(gameObject, destroyDelay);
        }
    }
}
