using UnityEngine;

public class VRReferences : MonoBehaviour
{
    public static VRReferences Instance;

    [Header("Hand Controller Transforms")]
    public Transform LeftHand;
    public Transform RightHand;

    [Header("Controller Visuals To Hide")]
    public GameObject LeftHandVisual;
    public GameObject RightHandVisual;

    private void Awake()
    {
        Instance = this;
    }

    public void SetLeftVisualVisible(bool visible)
    {
        if (LeftHandVisual != null)
            LeftHandVisual.SetActive(visible);
    }

    public void SetRightVisualVisible(bool visible)
    {
        if (RightHandVisual != null)
            RightHandVisual.SetActive(visible);
    }

    public void SetVisualVisibleForHand(Transform hand, bool visible)
    {
        if (hand == LeftHand)
            SetLeftVisualVisible(visible);
        else if (hand == RightHand)
            SetRightVisualVisible(visible);
    }
}