using UnityEngine;
using UnityEngine.InputSystem;
public class ObjectConnection : MonoBehaviour
{
    public InputActionReference rightPrimaryButton;
    public ThrowLogic Object;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rightPrimaryButton.action.performed += ButtonPressed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ButtonPressed(InputAction.CallbackContext context)
    {
        Object.ReturnObject();
    }
}
