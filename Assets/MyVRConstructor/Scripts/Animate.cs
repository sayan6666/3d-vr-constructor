using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction;

public class Animate : MonoBehaviour
{
    public InputActionProperty select;
    public InputActionProperty activate;

    public Animator handAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        handAnimator.SetFloat("Grip",(select.action.ReadValue<float>()));
    }
}
