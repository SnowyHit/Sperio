using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VisibilityObserver : MonoBehaviour
{
    // Check if if this collides with any objects with the tag of Obstacle , fade's its material.
    MeshRenderer tempMeshRenderer = null;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            if(other.transform.childCount > 0)
            {
                if(other.GetComponent<MeshRenderer>() != null)
                {
                    tempMeshRenderer = other.GetComponent<MeshRenderer>();
                    foreach (var material in tempMeshRenderer.materials)
                    {
                        material.DOFade(0, 0.2f);
                    }

                }
                foreach (Transform child in other.transform)
                {
                    if (child.GetComponent<MeshRenderer>() != null)
                    {
                        tempMeshRenderer = child.GetComponent<MeshRenderer>();
                        foreach (var material in tempMeshRenderer.materials)
                        {
                            material.DOFade(0, 0.2f);
                        }

                    }
                }
            }
            else
            {
                tempMeshRenderer = other.GetComponent<MeshRenderer>();
                foreach (var material in tempMeshRenderer.materials)
                {
                    material.DOFade(0, 0.2f);
                }
            }
            
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Obstacle")
        {
            if (other.transform.childCount > 0)
            {
                if (other.GetComponent<MeshRenderer>() != null)
                {
                    tempMeshRenderer = other.GetComponent<MeshRenderer>();
                    foreach (var material in tempMeshRenderer.materials)
                    {
                        material.DOFade(1, 0.2f);
                    }

                }
                foreach (Transform child in other.transform)
                {
                    if (child.GetComponent<MeshRenderer>() != null)
                    {
                        tempMeshRenderer = child.GetComponent<MeshRenderer>();
                        foreach (var material in tempMeshRenderer.materials)
                        {
                            material.DOFade(1, 0.2f);
                        }

                    }
                }
            }
            else
            {
                tempMeshRenderer = other.GetComponent<MeshRenderer>();
                foreach (var material in tempMeshRenderer.materials)
                {
                    material.DOFade(1, 0.2f);
                }
            }

        }
    }

}
