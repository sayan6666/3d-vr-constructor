using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Create : MonoBehaviour
{
    public bool cube = false;
    public bool sphere = false;
    public bool cylinder = false;
    public bool cone = false;
    public bool torus = false;
    public bool prism = false;

    public InputActionProperty left;
    public InputActionProperty right;

    private Vector3 createPlacement = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string obj="";
        if (cube)
            obj = "Cube";
        else if (sphere)
            obj = "Sphere";
        else if (cylinder)
            obj = "Cylinder";
        else if (cone)
            obj = "Cone";
        else if (torus)
            obj = "Torus";
        else if (prism)
            obj = "Prism";
        else
            obj = "";

        //создание с левой руки
        if (left.action.IsPressed() && this.GetComponent<Actions>().create && obj!="")
        {
            createPlacement = GameObject.Find("Createpositionleft").transform.position;
            GameObject newObj = Instantiate(GameObject.Find(obj), createPlacement, GameObject.Find("left").transform.rotation);
            //cube.GetComponent<Transform>().SetParent(GameObject.Find("cubefamily").GetComponent<Transform>());
            newObj.GetComponent<Rigidbody>().useGravity = true;
            this.GetComponent<Actions>().create = false;
        }
        //создание с правой руки
        if (right.action.IsPressed() && this.GetComponent<Actions>().create && obj!="")
        {
            createPlacement = GameObject.Find("Createpositionrigth").transform.position;
            GameObject newObj = Instantiate(GameObject.Find(obj), createPlacement, GameObject.Find("right").transform.rotation);
            //cube.GetComponent<Transform>().SetParent(GameObject.Find("cubefamily").GetComponent<Transform>());
            newObj.GetComponent<Rigidbody>().useGravity = true;
            this.GetComponent<Actions>().create = false;
        }
    }
}
