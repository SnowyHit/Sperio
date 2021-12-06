using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityObserver : MonoBehaviour
{
    public Material baseMaterial;
    public Material fadeMaterial;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            Debug.Log(other.name);
            baseMaterial = other.GetComponent<MeshRenderer>().material;
            other.GetComponent<MeshRenderer>().material = fadeMaterial;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            Debug.Log(other.name);
            other.GetComponent<MeshRenderer>().material = baseMaterial;
        }
    }
}
