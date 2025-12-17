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
    public InputActionProperty resizeTrue;
    public InputActionProperty resizeFalse;

    private bool create=false;
    private bool resize = false;
    private bool combine = false;
    private Vector3 createPlacement = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(GameObject.Find("Cubeleftarm").transform.position, GameObject.Find("Cubeleftarm").transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        Ray ray1 = new Ray(GameObject.Find("Cuberightarm").transform.position, GameObject.Find("Cuberightarm").transform.forward);
        Debug.DrawRay(ray1.origin, ray1.direction * 10);
        RaycastHit hit1;
        Physics.Raycast(ray1, out hit1);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag=="PhysObj" && selectLeft.action.IsPressed() && combine)
            {
                hit.collider.gameObject.GetComponent<combine>().SetCombined();
                Debug.Log(hit.collider.gameObject.GetComponent<combine>().GetCombined());
            }
            if (hit.collider.tag == "PhysObj" && selectLeft.action.IsPressed() && resize)
            {
                //hit.collider.attachedRigidbody.useGravity = false;
                BoxCollider[] sides = hit.collider.gameObject.GetComponents<BoxCollider>();
                if (hit.collider == sides[0] || hit.collider == sides[1])
                {
                    hit.collider.gameObject.transform.localScale = new Vector3(
                        hit.collider.gameObject.transform.localScale.x, hit.collider.gameObject.transform.localScale.y, hit.collider.gameObject.transform.localScale.z + 0.01f);
                }
                if (hit.collider == sides[2] || hit.collider == sides[3])
                {
                    hit.collider.gameObject.transform.localScale = new Vector3(
                        hit.collider.gameObject.transform.localScale.x + 0.01f, hit.collider.gameObject.transform.localScale.y, hit.collider.gameObject.transform.localScale.z);
                }
                if (hit.collider == sides[4] || hit.collider == sides[5])
                {
                    hit.collider.gameObject.transform.localScale = new Vector3(
                        hit.collider.gameObject.transform.localScale.x, hit.collider.gameObject.transform.localScale.y + 0.01f, hit.collider.gameObject.transform.localScale.z);
                }
                //Debug.Log(hit.collider==GameObject.Find("Cube (2)").GetComponents<BoxCollider>()[1]);
                //hit.collider.gameObject.GetComponent<MeshCollider>().bounds.max;
            }
        }
        if (Physics.Raycast(ray1, out hit1))
        { 
            if (hit1.collider.tag == "PhysObj" && selectRight.action.IsPressed() && !create && resize)
            {
                //hit.collider.attachedRigidbody.useGravity = false;
                BoxCollider[] sides = hit1.collider.gameObject.GetComponents<BoxCollider>();
                if ((hit1.collider == sides[0] || hit1.collider == sides[1]) && !(hit1.collider.gameObject.transform.localScale.z < 0.01))
                {
                    hit1.collider.gameObject.transform.localScale = new Vector3(
                        hit1.collider.gameObject.transform.localScale.x, hit1.collider.gameObject.transform.localScale.y, hit1.collider.gameObject.transform.localScale.z - 0.01f);
                }
                if ((hit1.collider == sides[2] || hit1.collider == sides[3]) && !(hit1.collider.gameObject.transform.localScale.x < 0.01))
                {
                    hit1.collider.gameObject.transform.localScale = new Vector3(
                        hit1.collider.gameObject.transform.localScale.x - 0.01f, hit1.collider.gameObject.transform.localScale.y, hit1.collider.gameObject.transform.localScale.z);
                }
                if ((hit.collider == sides[4] || hit.collider == sides[5]) && !(hit1.collider.gameObject.transform.localScale.y < 0.01))
                {
                    hit1.collider.gameObject.transform.localScale = new Vector3(
                        hit1.collider.gameObject.transform.localScale.x, hit1.collider.gameObject.transform.localScale.y - 0.01f, hit1.collider.gameObject.transform.localScale.z);
                }
                //Debug.Log(hit.collider==GameObject.Find("Cube (2)").GetComponents<BoxCollider>()[1]);
                //hit.collider.gameObject.GetComponent<MeshCollider>().bounds.max;
            }
        }

        if (resizeTrue.action.IsPressed() && !create && !resize)
            resize = true;
            combine = true;
        if (creationTrue.action.IsPressed() && !resize && !create)
            create= true;
        if (resizeFalse.action.IsPressed() && resize)
            resize = false;
            combine = false;

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
