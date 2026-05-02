using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelectTarget : MonoBehaviour
{
    [Header("Scene To Load")]
    [SerializeField] string sceneName;

    bool alreadyTriggered;

    private void OnCollisionEnter(Collision collision)
    {
        if (alreadyTriggered)
            return;

        // Accept anything that inherits from Ball
        if (collision.gameObject.GetComponent<Ball>() == null)
            return;

        alreadyTriggered = true;

        SceneManager.LoadScene(sceneName);
    }
}