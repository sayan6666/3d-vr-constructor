using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class newf : MonoBehaviour
{
    public InputActionProperty selectLeft;
    public InputActionProperty selectRight;
    public InputActionProperty creationTrue;

    private bool create=false;
    private Vector3 createPlacement = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (creationTrue.action.IsPressed() && !resize && !create)
            create= true;

        if (selectLeft.action.IsPressed() && create)
        {
            createPlacement = GameObject.Find("Createpositionleft").transform.position;
            GameObject cube = Instantiate(GameObject.Find("Cube"),createPlacement, GameObject.Find("left").transform.rotation);
            //cube.GetComponent<Transform>().SetParent(GameObject.Find("cubefamily").GetComponent<Transform>());
            cube.GetComponent<Rigidbody>().useGravity = true;
            create = false;
        }
        if (selectRight.action.IsPressed() && create)
        {
            createPlacement = GameObject.Find("Createpositionrigth").transform.position;
            GameObject sphere = Instantiate(GameObject.Find("Sphere"), createPlacement, GameObject.Find("right").transform.rotation);
            //cube.GetComponent<Transform>().SetParent(GameObject.Find("cubefamily").GetComponent<Transform>());
            sphere.GetComponent<Rigidbody>().useGravity = true;
            create = false;
        }
    }
}
