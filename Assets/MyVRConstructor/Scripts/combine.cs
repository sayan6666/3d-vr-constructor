using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;

public class combine : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private bool combined = false;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject control = GameObject.Find("t");
        if (combined && collision.gameObject.TryGetComponent<combine>(out combine comp))
        {
            Debug.Log("+");
            if (comp.GetCombined() )//&& !control.GetComponent<newf>().selectLeft.action.IsPressed() && !control.GetComponent<newf>().selectRight.action.IsPressed())
            {
                Debug.Log("++");

                bool dontcombine = false;
                //объединение через fixedjoint
                if (this.gameObject.GetComponent<FixedJoint>())
                    foreach (FixedJoint joint in this.gameObject.GetComponents<FixedJoint>())
                    {
                        if (joint.connectedBody == collision.gameObject.GetComponent<Rigidbody>())
                            dontcombine = true;
                    }
                if (!dontcombine)
                    this.gameObject.AddComponent<FixedJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();

                //объединение через перенос rigidbody
                /* Destroy(this.gameObject.GetComponent<XRGrabInteractable>());
                 Destroy(this.gameObject.GetComponent<Rigidbody>());
                 Destroy(collision.gameObject.GetComponent<XRGrabInteractable>());
                 Destroy(collision.gameObject.GetComponent<Rigidbody>());

                 MeshCollider col1 = this.gameObject.GetComponent<MeshCollider>();
                 MeshCollider col2 = collision.gameObject.GetComponent<MeshCollider>();
                 SetCombined();
                 collision.gameObject.GetComponent<combine>().SetCombined();
                 GameObject comb = GameObject.Instantiate(GameObject.Find("Combiner"));
                 comb.transform.SetPositionAndRotation((this.transform.position+collision.transform.position)/2f,new Quaternion(0,0,0,0));
                 this.gameObject.transform.SetParent(comb.transform, false);
                 collision.gameObject.transform.SetParent(comb.transform, false);
                 comb.GetComponent<Rigidbody>().useGravity=true;
                 comb.GetComponent<XRGrabInteractable>().colliders.Add(col1);
                 comb.GetComponent<XRGrabInteractable>().colliders.Add(col2);*/
            }
        }
    }

    public void SetCombined() { combined = !combined; }
    public bool GetCombined() { return combined; }
}
