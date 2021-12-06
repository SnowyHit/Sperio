using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class playerManager : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    VariableJoystick varJoystick;
    public float playerSpeed;
    GameObject cameraGO;
    private void Start()
    {
        cameraGO = Camera.main.gameObject;
        varJoystick = GameObject.FindObjectOfType<VariableJoystick>();
        rb = GetComponent<Rigidbody>();
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        float distToCamera = Vector3.Distance(cameraGO.transform.position, transform.position);
        Vector3 dirToCamera = cameraGO.transform.position - transform.position;

        RaycastHit[] hits;
        hits = Physics.RaycastAll(cameraGO.transform.position, dirToCamera, distToCamera);
        foreach (var item in hits)
        {
            item.transform.GetComponent<MeshRenderer>().material.DOFade(50 , 0.1f);
            Debug.Log(item.transform.gameObject.name);
        }
        if (varJoystick.Horizontal != 0 || varJoystick.Vertical != 0)
        {
            Vector3 direction = new Vector3(varJoystick.Horizontal * playerSpeed , 0f, varJoystick.Vertical * playerSpeed);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.DORotate(new Vector3(0f , targetAngle , 0f), 0.1f);
            float forward = Mathf.Clamp01(direction.magnitude);
            anim.SetFloat("Forward", forward);
            rb.velocity = new Vector3(direction.x, rb.velocity.y, direction.z);
        }
        else if(varJoystick.Horizontal == 0 && varJoystick.Vertical == 0)
        {
            anim.SetFloat("Forward", Mathf.Clamp01(rb.velocity.magnitude));
        }
    }

    
}
