using UnityEngine;

public class VRReferences : MonoBehaviour
{
    public static VRReferences Instance;

    public Transform LeftHand;
    public Transform RightHand;

    public GameObject LeftHandVisual;
    public GameObject RightHandVisual;

    public ControllerData ControllerData;

    private void Awake()
    {
        Instance = this;
    }

    public void SetVisualVisibleForHand(Transform hand, bool visible)
    {
        if (hand == LeftHand && LeftHandVisual != null)
            LeftHandVisual.SetActive(visible);

        if (hand == RightHand && RightHandVisual != null)
            RightHandVisual.SetActive(visible);
    }
}