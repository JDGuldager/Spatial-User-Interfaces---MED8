using UnityEngine;

public class TargetHit : MonoBehaviour
{
    [SerializeField] int points = 1;

    MiniGameManager miniGameManager;
    bool alreadyHit;

    public void SetMiniGameManager(MiniGameManager manager)
    {
        miniGameManager = manager;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (alreadyHit)
            return;

        if (collision.gameObject.GetComponent<ThrowLogic>() == null)
            return;

        alreadyHit = true;

        if (miniGameManager != null)
        {
            miniGameManager.AddPoint(points);
            miniGameManager.SpawnTarget();
        }

        Destroy(gameObject);
    }
}