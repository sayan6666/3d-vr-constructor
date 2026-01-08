using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class newf : MonoBehaviour
{
    public InputActionProperty selectLeft;
    public InputActionProperty selectRight;
    public InputActionProperty creationTrue;
    public InputActionProperty resizeTrue;
    public InputActionProperty resizeFalse;

    private bool create=false;
    private bool resize = false;
    private bool combine = false;
    private bool decombine = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject.Find("Combiner").transform.GetComponents<BoxCollider>()[0].size = GameObject.Find("Cube").transform.GetComponent<Renderer>().bounds.size;//new Vector3(1,1, GameObject.Find("Combiner").transform.GetComponents<BoxCollider>()[0].size.z+0.05f);
        

        //переключение соединения
        /*if (hit.collider.tag == "PhysObj" && left.action.IsPressed() && combine)
        {
            hit.collider.gameObject.GetComponent<combine>().SetCombined();
            Debug.Log(hit.collider.gameObject.GetComponent<combine>().GetCombined());
        }

        if (resizeTrue.action.IsPressed() && !create && !resize)
        {
            resize = true;
        }
        if (creationTrue.action.IsPressed() && !resize && !create)
            decombine=true;
        if (resizeFalse.action.IsPressed() && resize)
            combine = false;
        */
        
    }
}
