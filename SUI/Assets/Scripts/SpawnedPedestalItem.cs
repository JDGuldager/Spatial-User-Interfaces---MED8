using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SpawnedPedestalItem : MonoBehaviour
{
    PedestalSpawner spawner;
    Transform spawnPoint;
    int pedestalIndex;

    bool hasTriggeredRespawn;

    [SerializeField] float leaveDistance = 0.5f;

    XRGrabInteractable grab;

    public void Setup(PedestalSpawner owner, int index, Transform point)
    {
        spawner = owner;
        pedestalIndex = index;
        spawnPoint = point;
    }

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grab != null)
            grab.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        if (grab != null)
            grab.selectExited.RemoveListener(OnReleased);
    }

    private void Update()
    {
        if (hasTriggeredRespawn)
            return;

        if (spawnPoint == null)
            return;

        float distanceFromSpawn =
            Vector3.Distance(transform.position, spawnPoint.position);

        if (distanceFromSpawn > leaveDistance)
        {
            TriggerRespawn();
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        TriggerRespawn();
    }

    private void TriggerRespawn()
    {
        if (hasTriggeredRespawn)
            return;

        hasTriggeredRespawn = true;

        if (spawner != null)
            spawner.NotifyItemUsed(pedestalIndex, gameObject);
    }
}