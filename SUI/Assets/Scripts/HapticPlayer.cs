using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Oculus.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class HapticPlayer : MonoBehaviour
{

    public static HapticPlayer Instance;
    

    private HapticClipPlayer clipPlayer;

    void Awake()
    {
        Instance = this;
    }

    //Get Controller



    public void PlayHaptics(HapticClip clip, IXRInteractor interactor, bool isLooping)
    {
        if (clip == null)
        {
            Debug.LogError("HAPTIC CLIP is NULL");
        }

        if (interactor is XRBaseInputInteractor controllerInteractor)
        {
            var oculusController = controllerInteractor.handedness == InteractorHandedness.Left
                ? Controller.Left
                : Controller.Right;

            Debug.Log("Playing haptic on " + controllerInteractor.handedness);

            var player = new HapticClipPlayer(clip);

            player.isLooping = isLooping;

            player.Play(oculusController);
        }
    }

}