using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public enum ShootingStyle { Straight, Triangle, Stream2, Stream3, Wave }
public enum Trail { FireFloor , ColdFloor, SpikeFireTrail, ElectricBlue, ElectricPurple, ElectricYellow , Void}
public enum Soul { Crimson , Green , Orange , Purple}
public class PlayerManager : MonoBehaviour
{
    // TODO : Make A persistance Datamanager , log in screen and sace player data ( owned skills and owned skins ) there.

    //get a Skill class here

    //get a player prefab here for skins
    public CinemachineVirtualCamera vCam;

    [SerializeField] float cameraTurnSpeed;
    public CinemachineOrbitalTransposer orbitalCam;
    public GameObject PlayerSkin;
    public GameObject PlayerSkinPrefab;
    public GameObject PlayerSoulGameObject;
    public GameObject PlayerSoulExplosionGameObject;
    public Skill PlayerSkill;
    public Rigidbody PlayerRigidbody;
    public Collider PlayerCollider;
    public Animator anim;
    public VariableJoystick varJoystick;
    public float PlayerSpeed;
    public float BaseSpeed;
    public float ProjectileRange;
    GameObject cameraGO;
    public GameObject projectileRoot;
    public GameObject ShootRight;
    public GameObject ShootMiddle;
    public GameObject ShootLeft;
    bool canShoot = true;
    bool shotRight = true;
    bool isDead = false;
    public bool ShootLock = false;
    public bool MovementLock = false;
    public ShootingStyle PlayerShootingStyle;
    public Trail PlayerTrail;
    public Soul PlayerSoul;
    private int waveShootNumber;
    public List<GameObject> Trails = new List<GameObject>();
    public List<GameObject> Souls = new List<GameObject>();
    public List<GameObject> SoulExplosions = new List<GameObject>();
    public bool touchingGround;
    private void Start()
    {
        // Set Skill Prefab And Skill Reload Time here 

        // Set Player Skin here
        orbitalCam =  vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        //Instantiate Player Skin
        PlayerSkill.SkillReloadTime = PlayerSkill.BaseSkillReloadTime;
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

            if(!ShootLock)
            {
                Shoot();
            }

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
    public void Shoot()
    {
        if (canShoot)
        {
            if(PlayerShootingStyle == ShootingStyle.Wave)
                StartCoroutine(ShootWave());
            else if(PlayerShootingStyle == ShootingStyle.Triangle)
                StartCoroutine(ShootTriangle());
            else if(PlayerShootingStyle == ShootingStyle.Stream2)
                StartCoroutine(ShootStream2());
            else if(PlayerShootingStyle == ShootingStyle.Stream3)
                StartCoroutine(ShootStream3());
            else 
                StartCoroutine(ShootStraight());
        }
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
    public void BuffATKSpeedNow(float multiplier, float time)
    {
        StartCoroutine(BuffATKSpeed(multiplier, time));
    }
    public IEnumerator BuffATKSpeed(float multiplier, float time)
    {
        PlayerSkill.SkillReloadTime = PlayerSkill.SkillReloadTime / multiplier;
        yield return new WaitForSeconds(time);
        PlayerSkill.SkillReloadTime = PlayerSkill.BaseSkillReloadTime;
    }
    IEnumerator ShootStraight()
    {
        canShoot = false;
        GameObject projectile = Instantiate(projectileRoot , ShootMiddle.transform.position , Quaternion.Euler(0,0,0));
        projectile.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile.transform.DOScale(PlayerSkill.SkillRootScale , 0.01f);
        GameObject projectileSkin = Instantiate(PlayerSkill.SkillPrefab , projectile.transform.position , Quaternion.Euler(0,0,0) , projectile.transform);
        projectile.transform.DOMove(transform.position + transform.forward * ProjectileRange , 1f);
        Destroy(projectile, 1.2f);
        yield return new WaitForSeconds(PlayerSkill.SkillReloadTime);
        canShoot = true;
        yield return new WaitForSeconds(0.8f);
        if (projectile != null)
        {
            GameObject projectileSplash = Instantiate(PlayerSkill.SplashEffect, projectile.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash.transform.DOScale(PlayerSkill.SkillRootScale , 0.01f);
            Destroy(projectileSplash, 0.4f);
            
        }
    }
    IEnumerator ShootWave()
    {
        int tempWaveNumber = waveShootNumber;
        GameObject projectile = null;
        canShoot = false;
        if(waveShootNumber == 0)
        {
            projectile = Instantiate(projectileRoot, ShootRight.transform.position + (Vector3.up * 0.1f) + Vector3.right * 0.2f, Quaternion.Euler(0, 0, 0));
            shotRight = true;
            waveShootNumber += 1;
        }
        else if (waveShootNumber == 1)
        {
            projectile = Instantiate(projectileRoot, ShootMiddle.transform.position + (Vector3.up * 0.1f) + Vector3.right * 0.2f, Quaternion.Euler(0, 0, 0));
            if(shotRight)
            {
                waveShootNumber += 1;
            }
            else
            {
                waveShootNumber -= 1;
            }
        }
        else
        {
            projectile = Instantiate(projectileRoot, ShootLeft.transform.position + (Vector3.up * 0.1f) + Vector3.right * 0.2f, Quaternion.Euler(0, 0, 0));
            waveShootNumber -= 1;
            shotRight = false;
        }

        projectile.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin = Instantiate(PlayerSkill.SkillPrefab, projectile.transform.position, Quaternion.Euler(0, 0, 0), projectile.transform);

        if (tempWaveNumber == 0)
            projectile.transform.DOMove(ShootRight.transform.position + ShootRight.transform.forward * ProjectileRange, 1f);
        else if (tempWaveNumber == 1)
            projectile.transform.DOMove(ShootMiddle.transform.position + ShootMiddle.transform.forward * ProjectileRange, 1f);
        else
            projectile.transform.DOMove(ShootLeft.transform.position + ShootLeft.transform.forward * ProjectileRange, 1f);

        Destroy(projectile, 1.2f);
        yield return new WaitForSeconds(PlayerSkill.SkillReloadTime);
        canShoot = true;
        yield return new WaitForSeconds(0.8f);
        if (projectile != null)
        {
            GameObject projectileSplash = Instantiate(PlayerSkill.SplashEffect, projectile.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
            Destroy(projectileSplash, 0.4f);

        }
    }
    IEnumerator ShootTriangle()
    {
        canShoot = false;
        GameObject projectile = Instantiate(projectileRoot, ShootRight.transform.position, Quaternion.Euler(0, 0, 0));
        GameObject projectile2 = Instantiate(projectileRoot, ShootMiddle.transform.position, Quaternion.Euler(0, 0, 0));
        GameObject projectile3 = Instantiate(projectileRoot, ShootLeft.transform.position, Quaternion.Euler(0, 0, 0));
        projectile.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin = Instantiate(PlayerSkill.SkillPrefab, projectile.transform.position, Quaternion.Euler(0, 0, 0), projectile.transform);
        projectile.transform.DOMove(ShootRight.transform.position + ShootRight.transform.forward * ProjectileRange + ShootRight.transform.right * 3, 1f);
        Destroy(projectile, 1.2f);
        projectile2.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile2.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile2.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin2 = Instantiate(PlayerSkill.SkillPrefab, projectile2.transform.position, Quaternion.Euler(0, 0, 0), projectile2.transform);
        projectile2.transform.DOMove(ShootMiddle.transform.position + ShootMiddle.transform.forward * ProjectileRange, 1f );
        Destroy(projectile2, 1.2f);
        projectile3.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile3.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile3.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin3 = Instantiate(PlayerSkill.SkillPrefab, projectile3.transform.position, Quaternion.Euler(0, 0, 0), projectile3.transform);
        projectile3.transform.DOMove(ShootLeft.transform.position + ShootLeft.transform.forward * ProjectileRange + ShootLeft.transform.right * -3, 1f);
        Destroy(projectile3, 1.2f);
        yield return new WaitForSeconds(PlayerSkill.SkillReloadTime);
        canShoot = true;
        yield return new WaitForSeconds(0.8f);
        if (projectile != null)
        {
            GameObject projectileSplash = Instantiate(PlayerSkill.SplashEffect, projectile.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
            Destroy(projectileSplash, 0.4f);
        }
        if (projectile2 != null)
        {
            GameObject projectileSplash2 = Instantiate(PlayerSkill.SplashEffect, projectile2.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash2.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
            Destroy(projectileSplash2, 0.4f);
        }
        if (projectile3 != null)
        {
            GameObject projectileSplash3 = Instantiate(PlayerSkill.SplashEffect, projectile3.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash3.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
            Destroy(projectileSplash3, 0.4f);
        }
    }
    IEnumerator ShootStream2()
    {
        canShoot = false;
        GameObject projectile = Instantiate(projectileRoot, ShootRight.transform.position, Quaternion.Euler(0, 0, 0));
        GameObject projectile3 = Instantiate(projectileRoot, ShootLeft.transform.position, Quaternion.Euler(0, 0, 0));
        projectile.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin = Instantiate(PlayerSkill.SkillPrefab, projectile.transform.position, Quaternion.Euler(0, 0, 0), projectile.transform);
        projectile.transform.DOMove(ShootRight.transform.position + ShootRight.transform.forward * ProjectileRange, 1f);
        Destroy(projectile, 1.2f);
        projectile3.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile3.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile3.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin3 = Instantiate(PlayerSkill.SkillPrefab, projectile3.transform.position, Quaternion.Euler(0, 0, 0), projectile3.transform);
        projectile3.transform.DOMove(ShootLeft.transform.position + ShootLeft.transform.forward * ProjectileRange, 1f);
        Destroy(projectile3, 1.2f);
        yield return new WaitForSeconds(PlayerSkill.SkillReloadTime);
        canShoot = true;
        yield return new WaitForSeconds(0.8f);
        if (projectile != null)
        {
            GameObject projectileSplash = Instantiate(PlayerSkill.SplashEffect, projectile.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
            Destroy(projectileSplash, 0.4f);
        }
        if (projectile3 != null)
        {
            GameObject projectileSplash3 = Instantiate(PlayerSkill.SplashEffect, projectile3.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash3.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
            Destroy(projectileSplash3, 0.4f);
        }
    }
    IEnumerator ShootStream3()
    {
        canShoot = false;
        GameObject projectile = Instantiate(projectileRoot, ShootRight.transform.position, Quaternion.Euler(0, 0, 0));
        GameObject projectile2 = Instantiate(projectileRoot, ShootMiddle.transform.position, Quaternion.Euler(0, 0, 0));
        GameObject projectile3 = Instantiate(projectileRoot, ShootLeft.transform.position, Quaternion.Euler(0, 0, 0));
        projectile.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin = Instantiate(PlayerSkill.SkillPrefab, projectile.transform.position, Quaternion.Euler(0, 0, 0), projectile.transform);
        projectile.transform.DOMove(ShootRight.transform.position + ShootRight.transform.forward * ProjectileRange, 1f);
        Destroy(projectile, 1.2f);
        projectile2.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile2.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile2.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin2 = Instantiate(PlayerSkill.SkillPrefab, projectile2.transform.position, Quaternion.Euler(0, 0, 0), projectile2.transform);
        projectile2.transform.DOMove(ShootMiddle.transform.position + ShootMiddle.transform.forward * ProjectileRange, 1f);
        Destroy(projectile2, 1.2f);
        projectile3.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile3.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile3.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin3 = Instantiate(PlayerSkill.SkillPrefab, projectile3.transform.position, Quaternion.Euler(0, 0, 0), projectile3.transform);
        projectile3.transform.DOMove(ShootLeft.transform.position + ShootLeft.transform.forward * ProjectileRange, 1f);
        Destroy(projectile3, 1.2f);
        yield return new WaitForSeconds(PlayerSkill.SkillReloadTime);
        canShoot = true;
        yield return new WaitForSeconds(0.8f);
        if (projectile != null)
        {
            GameObject projectileSplash = Instantiate(PlayerSkill.SplashEffect, projectile.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
            Destroy(projectileSplash, 0.4f);
        }
        if (projectile2 != null)
        {
            GameObject projectileSplash2 = Instantiate(PlayerSkill.SplashEffect, projectile2.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash2.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
            Destroy(projectileSplash2, 0.4f);
        }
        if (projectile3 != null)
        {
            GameObject projectileSplash3 = Instantiate(PlayerSkill.SplashEffect, projectile3.transform.position, Quaternion.Euler(0, 0, 0));
            projectileSplash3.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
            Destroy(projectileSplash3, 0.4f);
        }
    }
}
