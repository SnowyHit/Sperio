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
        if (other.gameObject.tag == "Obstacle")
        {
            objectNumber += 1;
            baseMaterial.Add(other.GetComponent<MeshRenderer>().material);
            other.GetComponent<MeshRenderer>().material = fadeMaterial;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            other.GetComponent<MeshRenderer>().material = baseMaterial[objectNumber];
            baseMaterial.Remove(baseMaterial[objectNumber]);
            objectNumber -= 1;
        }
    }

}
