using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Resize : MonoBehaviour
{
    public InputActionProperty left;
    public InputActionProperty right;

    public GameObject lineleft;
    public GameObject lineright;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.GetComponent<Actions>().resize != 0)
        {
            lineleft.SetActive(true);
            lineright.SetActive(true);
        }
        else
        {
            lineleft.SetActive(false);
            lineright.SetActive(false);
        }
        //левый луч
        Ray ray = new Ray(GameObject.Find("Cubeleftarm").transform.position, GameObject.Find("Cubeleftarm").transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        //правый луч
        Ray ray1 = new Ray(GameObject.Find("Cuberightarm").transform.position, GameObject.Find("Cuberightarm").transform.forward);
        Debug.DrawRay(ray1.origin, ray1.direction * 10);
        RaycastHit hit1;
        Physics.Raycast(ray1, out hit1);

        int resize = this.gameObject.GetComponent<Actions>().resize;

        //столкновение левого луча
        if (Physics.Raycast(ray, out hit))
        {
            //масштабирование - увеличение размера
            if (hit.collider.tag == "PhysObj" && left.action.IsPressed() && resize!=0)
            {
                //hit.collider.attachedRigidbody.useGravity = false;
                BoxCollider[] sides = hit.collider.gameObject.GetComponents<BoxCollider>();
                if ((hit.collider == sides[0] || hit.collider == sides[1]) && !(hit1.collider.gameObject.transform.localScale.z < 0.01))
                {
                    /* Vector3 newPos = hit.collider.transform.position + new Vector3( 0f, 0f, (1f - 0.01f) * (hit.collider.transform.position.z-hit.collider.transform.localScale.z/2f));
                     hit.collider.gameObject.transform.SetPositionAndRotation(newPos//new Vector3(hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y, hit.collider.gameObject.transform.position.z+0.005f)
                         ,hit.collider.gameObject.transform.rotation);*/
                    hit.collider.gameObject.transform.localScale = new Vector3(
                        hit.collider.gameObject.transform.localScale.x, hit.collider.gameObject.transform.localScale.y, hit.collider.gameObject.transform.localScale.z + (0.01f*resize));
                }
                if ((hit.collider == sides[2] || hit.collider == sides[3]) && !(hit1.collider.gameObject.transform.localScale.x < 0.01))
                {
                    hit.collider.gameObject.transform.localScale = new Vector3(
                        hit.collider.gameObject.transform.localScale.x + (0.01f*resize), hit.collider.gameObject.transform.localScale.y, hit.collider.gameObject.transform.localScale.z);
                }
                if ((hit.collider == sides[4] || hit.collider == sides[5]) && !(hit1.collider.gameObject.transform.localScale.y < 0.01))
                {
                    hit.collider.gameObject.transform.localScale = new Vector3(
                        hit.collider.gameObject.transform.localScale.x, hit.collider.gameObject.transform.localScale.y + (0.01f*resize), hit.collider.gameObject.transform.localScale.z);
                }
                //Debug.Log(hit.collider==GameObject.Find("Cube (2)").GetComponents<BoxCollider>()[1]);
                //hit.collider.gameObject.GetComponent<MeshCollider>().bounds.max;
            }
        }
        //столкновение правого луча
        if (Physics.Raycast(ray1, out hit1))
        {
            //машстабирование - уменьшение размера
            if (hit1.collider.tag == "PhysObj" && right.action.IsPressed() && resize != 0)
            {
                //hit.collider.attachedRigidbody.useGravity = false;
                BoxCollider[] sides = hit1.collider.gameObject.GetComponents<BoxCollider>();
                if ((hit1.collider == sides[0] || hit1.collider == sides[1]) && !(hit1.collider.gameObject.transform.localScale.z < 0.01))
                {
                    hit1.collider.gameObject.transform.localScale = new Vector3(
                        hit1.collider.gameObject.transform.localScale.x, hit1.collider.gameObject.transform.localScale.y, hit1.collider.gameObject.transform.localScale.z + (0.01f * resize));
                }
                if ((hit1.collider == sides[2] || hit1.collider == sides[3]) && !(hit1.collider.gameObject.transform.localScale.x < 0.01))
                {
                    hit1.collider.gameObject.transform.localScale = new Vector3(
                        hit1.collider.gameObject.transform.localScale.x + (0.01f * resize), hit1.collider.gameObject.transform.localScale.y, hit1.collider.gameObject.transform.localScale.z);
                }
                if ((hit.collider == sides[4] || hit.collider == sides[5]) && !(hit1.collider.gameObject.transform.localScale.y < 0.01))
                {
                    hit1.collider.gameObject.transform.localScale = new Vector3(
                        hit1.collider.gameObject.transform.localScale.x, hit1.collider.gameObject.transform.localScale.y + (0.01f * resize), hit1.collider.gameObject.transform.localScale.z);
                }
                //Debug.Log(hit.collider==GameObject.Find("Cube (2)").GetComponents<BoxCollider>()[1]);
                //hit.collider.gameObject.GetComponent<MeshCollider>().bounds.max;
            }
        }
    }
}
