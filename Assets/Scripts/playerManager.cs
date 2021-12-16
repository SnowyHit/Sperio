using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class playerManager : MonoBehaviour
{
    // TODO : Make A persistance Datamanager , log in screen and sace player data ( owned skills and owned skins ) there.

    //get a Skill class here

    //get a player prefab here for skin

    Rigidbody rb;
    Animator anim;
    VariableJoystick varJoystick;
    public float playerSpeed;
    GameObject cameraGO;
    public GameObject Projectile;
    bool canShoot = true;
    public float SkillReloadTime;
    public float SkillPrefab;
    private void Start()
    {
        // Set Skill Prefab And Skill Reload Time here 

        // Set Player Skin here

        cameraGO = Camera.main.gameObject;
        varJoystick = GameObject.FindObjectOfType<VariableJoystick>();
        rb = GetComponent<Rigidbody>();
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
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

    public void Shoot()
    {
        if (canShoot)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Shooting());
            }
        }
    }

    IEnumerator Shooting()
    {
        canShoot = false;
        Debug.Log("Shooting");
        yield return new WaitForSeconds(SkillReloadTime);
        canShoot = true;
    }


    
    
}
