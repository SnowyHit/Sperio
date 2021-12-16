using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityObserver : MonoBehaviour
{
    public List<Material> baseMaterial;
    public Material fadeMaterial;
    int objectNumber = -1 ;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            objectNumber += 1;
            Debug.Log(other.name);
            baseMaterial.Add(other.GetComponent<MeshRenderer>().material);
            other.GetComponent<MeshRenderer>().material = fadeMaterial;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            Debug.Log(other.name);
            other.GetComponent<MeshRenderer>().material = baseMaterial[objectNumber];
            baseMaterial.Remove(baseMaterial[objectNumber]);
            objectNumber -= 1;
        }
    }

}
