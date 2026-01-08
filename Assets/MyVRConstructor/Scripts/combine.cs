using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Combine : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((GameObject.Find("rightinteract").GetComponent<XRDirectInteractor>().firstInteractableSelected != null || GameObject.Find("leftinteract").GetComponent<XRDirectInteractor>().firstInteractableSelected != null) && GameObject.Find("Manager").GetComponent<Actions>().combine == -1)
        {
            GameObject.Find("Manager").GetComponent<Actions>().combine = 0;
            string interactor = "";
            if (GameObject.Find("rightinteract").GetComponent<XRDirectInteractor>().firstInteractableSelected != null)
                interactor = "rightinteract";
            if (GameObject.Find("leftinteract").GetComponent<XRDirectInteractor>().firstInteractableSelected != null)
                interactor = "leftinteract";
            if (interactor!="")
            if (GameObject.Find(interactor).GetComponent<XRDirectInteractor>().firstInteractableSelected.transform.GetComponent<FixedJoint>())
            {
                foreach (FixedJoint go in GameObject.Find(interactor).GetComponent<XRDirectInteractor>().firstInteractableSelected.transform.GetComponents<FixedJoint>())
                {
                    foreach (FixedJoint joint in go.connectedBody.gameObject.transform.GetComponents<FixedJoint>())
                    {
                        if (joint.connectedBody == go.gameObject.GetComponent<Rigidbody>())
                            Destroy(joint);
                    }
                    Destroy(go);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameObject.Find("Manager").GetComponent<Actions>().combine==1 && collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody comp))
        {
            bool dontcombine = false;
            //объединение через fixedjoint
            if (this.gameObject.GetComponent<FixedJoint>())
                foreach (FixedJoint joint in this.gameObject.GetComponents<FixedJoint>())
                {
                    //проверка соединения на существование
                    if (joint.connectedBody == collision.gameObject.GetComponent<Rigidbody>())
                        dontcombine = true;
                }
            //собственно соединение
            if (!dontcombine)
            {
                this.gameObject.AddComponent<FixedJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();
            }
        }
    }
}
