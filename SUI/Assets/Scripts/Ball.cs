
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Oculus.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public abstract class Ball : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] Rigidbody rb;

    [SerializeField] protected GameObject effect;
    [SerializeField] protected GameObject onImpact;
    [SerializeField] protected float destroyDelay = 2f;
    [SerializeField] private XRGrabInteractable interactable;

    [SerializeField] protected HapticClip hoverClip;
    [SerializeField] protected HapticClip grabClip;

    [SerializeField] private HapticPlayer hapticPlayer;


    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
       

        if (onImpact != null)
        {
            onImpact.SetActive(false);
        }
        
    }

    // Update is called once per frame
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
    void OnHoverExited(HoverExitEventArgs args)
    {
        hapticPlayer = null;
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("Hover triggered");

        if (hapticPlayer == null)
        {
            Debug.LogError("HapticPlayer is NULL!");
            hapticPlayer = HapticPlayer.Instance;
        }

        if (hoverClip == null)
            Debug.LogError("HoverClip is NULL!");
        
        hapticPlayer.PlayHaptics(hoverClip, args.interactorObject, true);
    }

    

    void OnGrabbed(SelectEnterEventArgs args)
    {
        hapticPlayer = null;
        hapticPlayer = HapticPlayer.Instance;
        hapticPlayer.PlayHaptics(grabClip, args.interactorObject, false);
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
            Destroy(gameObject, destroyDelay);
        }
    }
}
