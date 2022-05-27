using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public enum Trail { FireFloor , ColdFloor, SpikeFireTrail, ElectricBlue, ElectricPurple, ElectricYellow , Void}
public enum Soul { Crimson , Green , Orange , Purple}
public class playerManager : MonoBehaviour
{
    // TODO : Make A persistance Datamanager , log in screen and sace player data ( owned skills and owned skins ) there.

    //get a Skill class here

    //get a player prefab here for skins
    public CinemachineVirtualCamera vCam;

    [SerializeField] float cameraTurnSpeed;
    [SerializeField] CinemachineOrbitalTransposer orbitalCam;
    public GameObject PlayerSkin;
    public GameObject PlayerSkinPrefab;
    public GameObject PlayerSoulGameObject;
    public GameObject PlayerSoulExplosionGameObject;
    public Rigidbody PlayerRigidbody;
    public Collider PlayerCollider;
    public Animator anim;
    public VariableJoystick varJoystick;
    public float PlayerSpeed;
    public float BaseSpeed;
    GameObject cameraGO;
    bool isDead = false;
    public bool MovementLock = false;
    public Trail PlayerTrail;
    public Soul PlayerSoul;

    public List<GameObject> Trails = new List<GameObject>();
    public List<GameObject> Souls = new List<GameObject>();
    public List<GameObject> SoulExplosions = new List<GameObject>();
    public bool touchingGround;
    private void Start()
    {
        // Set Player Skin here
        orbitalCam =  vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        //Instantiate Player Skin
        PlayerSkin = Instantiate(PlayerSkinPrefab, transform.position, Quaternion.identity, transform);
        cameraGO = Camera.main.gameObject;
        anim = gameObject.GetComponentInChildren<Animator>();
    }
    private void FixedUpdate()
    {
        if(!isDead)
        {
            CheckTrail();
            CheckSoul();


            if (!MovementLock && Physics.Raycast(PlayerCollider.bounds.center, Vector3.down, PlayerCollider.bounds.extents.y + 0.1f))
            {
                if (varJoystick.Horizontal != 0 || varJoystick.Vertical != 0)
                {
                    orbitalCam.m_XAxis.m_InputAxisValue = varJoystick.Horizontal * cameraTurnSpeed;
                    //joystick inputs to direction vector   
                    Vector3 direction = new Vector3(varJoystick.Horizontal, 0f, varJoystick.Vertical);
                    //Set Rotation
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    transform.DORotate(new Vector3(0f, targetAngle, 0f), 0.1f);
                    Vector3 moveDir = Quaternion.Euler(0f, targetAngle , 0f) * Vector3.forward;

                    
                    //Set movement
                    PlayerRigidbody.velocity = new Vector3(moveDir.x * PlayerSpeed, PlayerRigidbody.velocity.y , moveDir.z*PlayerSpeed);

                    //Set Animation for running
                    float forward = Mathf.Clamp01(direction.magnitude);
                    anim.SetFloat("Forward", forward);
                }
                else if (varJoystick.Horizontal == 0 && varJoystick.Vertical == 0)
                {
                    anim.SetFloat("Forward", Mathf.Clamp01(PlayerRigidbody.velocity.magnitude));
                    orbitalCam.m_XAxis.m_InputAxisValue = 0;
                }
            }
        }
    }

    public void Death()
    {
        isDead = true;
        anim.SetTrigger("Death");
        GameObject.FindObjectOfType<GameUI>().SetState(GameState.Dead.ToString());
    }

    public void Revive()
    {
        isDead = false;
        anim.ResetTrigger("Death");
        anim.CrossFade("Locomotion" , 0.1f);
    }
   

    public void CheckTrail()
    {
        if (!Trails[((int)PlayerTrail)].activeSelf)
        {
            Debug.Log("CheckTrailWorking..");
            foreach (GameObject trail in Trails)
            {
                if (trail == Trails[((int)PlayerTrail)])
                    trail.SetActive(true);
                else
                    trail.SetActive(false);
            }
        }
    }
    public void CheckSoul()
    {
        if (Souls[((int)PlayerSoul)] != PlayerSoulGameObject)
        {
            Debug.Log("CheckSoulWorking..");
            int i = 0;
            foreach (GameObject soul in Souls)
            {
                if (soul == Souls[((int)PlayerSoul)])
                {
                    PlayerSoulGameObject = soul;
                    PlayerSoulExplosionGameObject = SoulExplosions[i];
                }
                i += 1;
            }
        }
    }
    public void BuffSpeedNow(float multiplier, float time)
    {
        StartCoroutine(BuffSpeed(multiplier, time));
    }
    public IEnumerator BuffSpeed(float multiplier , float time)
    {
        PlayerSpeed = PlayerSpeed * multiplier;
        yield return new WaitForSeconds(time);
        PlayerSpeed = BaseSpeed;
    }
    
}
