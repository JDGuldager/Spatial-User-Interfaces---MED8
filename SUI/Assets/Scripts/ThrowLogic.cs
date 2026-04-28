using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public enum State { Idle, Throw, Return }

public class ThrowLogic : MonoBehaviour
{
    [SerializeField] float throwSpeed = 20f;
    [SerializeField] float catchDistance = 0.25f;

    public Transform PlayersHand;

    Rigidbody rb;
    XRGrabInteractable grab;
    State state;

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
            grab.selectEntered.AddListener(OnGrab);
        }
    }

    private void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("Object was grabbed by: " + args.interactorObject.transform.name);

        // This is usually the controller/interactor that grabbed the object
        PlayersHand = args.interactorObject.transform;

        Debug.Log("PlayersHand set to: " + PlayersHand.name);

        state = State.Idle;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;

            case State.Throw:
                transform.SetParent(null);
                rb.isKinematic = false;

                if (rb.linearVelocity != Vector3.zero)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * throwSpeed;
                }

                break;

            case State.Return:
                ReturnToHand();
                break;
        }
    }

    private void ReturnToHand()
    {
        if (PlayersHand == null)
        {
            Debug.LogWarning("PlayersHand is missing. Grab the object first, or assign the hand manually.");
            return;
        }

        Vector3 toHand = PlayersHand.position - transform.position;
        float distance = toHand.magnitude;

        if (distance > catchDistance)
        {
            Vector3 dir = toHand.normalized;

            rb.isKinematic = false;
            rb.linearVelocity = dir * throwSpeed;
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

        transform.SetParent(PlayersHand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void IdleObject()
    {
        state = State.Idle;
    }

    public void ThrowObject()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        state = State.Throw;
    }

    public void ReturnObject()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        state = State.Return;
    }
}