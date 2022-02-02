using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ShootingStyle { Straight, Triangle, Stream2, Stream3, Wave }
public class playerManager : MonoBehaviour
{
    // TODO : Make A persistance Datamanager , log in screen and sace player data ( owned skills and owned skins ) there.

    //get a Skill class here

    //get a player prefab here for skin
    public Skill PlayerSkill;
    Rigidbody rb;
    Animator anim;
    VariableJoystick varJoystick;
    public float playerSpeed;
    GameObject cameraGO;
    public GameObject projectileRoot;
    public GameObject ShootRight;
    public GameObject ShootMiddle;
    public GameObject ShootLeft;
    bool canShoot = true;
    bool shotRight = true;
    bool isDead = false;
    public ShootingStyle PlayerShootingStyle;
    private int waveShootNumber; 

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
        if(!isDead)
        {
            Shoot();

            if (varJoystick.Horizontal != 0 || varJoystick.Vertical != 0)
            {
                Vector3 direction = new Vector3(varJoystick.Horizontal * playerSpeed, 0f, varJoystick.Vertical * playerSpeed);
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                transform.DORotate(new Vector3(0f, targetAngle, 0f), 0.1f);
                float forward = Mathf.Clamp01(direction.magnitude);
                anim.SetFloat("Forward", forward);
                rb.velocity = new Vector3(direction.x, rb.velocity.y, direction.z);
            }
            else if (varJoystick.Horizontal == 0 && varJoystick.Vertical == 0)
            {
                anim.SetFloat("Forward", Mathf.Clamp01(rb.velocity.magnitude));
            }
        }
    }

    public void Death()
    {
        isDead = true;
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

    IEnumerator ShootStraight()
    {
        canShoot = false;
        Debug.Log(ShootMiddle.transform.position);
        GameObject projectile = Instantiate(projectileRoot , ShootMiddle.transform.position , Quaternion.Euler(0,0,0));
        projectile.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile.transform.DOScale(PlayerSkill.SkillRootScale , 0.01f);
        GameObject projectileSkin = Instantiate(PlayerSkill.SkillPrefab , projectile.transform.position , Quaternion.Euler(0,0,0) , projectile.transform);
        projectile.transform.DOMove(transform.position + transform.forward * 10 , 1f);
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
            projectile.transform.DOMove(ShootRight.transform.position + ShootRight.transform.forward * 10, 1f);
        else if (tempWaveNumber == 1)
            projectile.transform.DOMove(ShootMiddle.transform.position + ShootMiddle.transform.forward * 10, 1f);
        else
            projectile.transform.DOMove(ShootLeft.transform.position + ShootLeft.transform.forward * 10, 1f);

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
        projectile.transform.DOMove(ShootRight.transform.position + ShootRight.transform.forward * 10 + ShootRight.transform.right * 3, 1f);
        Destroy(projectile, 1.2f);
        projectile2.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile2.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile2.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin2 = Instantiate(PlayerSkill.SkillPrefab, projectile2.transform.position, Quaternion.Euler(0, 0, 0), projectile2.transform);
        projectile2.transform.DOMove(ShootMiddle.transform.position + ShootMiddle.transform.forward * 10, 1f );
        Destroy(projectile2, 1.2f);
        projectile3.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile3.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile3.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin3 = Instantiate(PlayerSkill.SkillPrefab, projectile3.transform.position, Quaternion.Euler(0, 0, 0), projectile3.transform);
        projectile3.transform.DOMove(ShootLeft.transform.position + ShootLeft.transform.forward * 10 + ShootLeft.transform.right * -3, 1f);
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
        projectile.transform.DOMove(ShootRight.transform.position + ShootRight.transform.forward * 10, 1f);
        Destroy(projectile, 1.2f);
        projectile3.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile3.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile3.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin3 = Instantiate(PlayerSkill.SkillPrefab, projectile3.transform.position, Quaternion.Euler(0, 0, 0), projectile3.transform);
        projectile3.transform.DOMove(ShootLeft.transform.position + ShootLeft.transform.forward * 10, 1f);
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
        projectile.transform.DOMove(ShootRight.transform.position + ShootRight.transform.forward * 10, 1f);
        Destroy(projectile, 1.2f);
        projectile2.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile2.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile2.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin2 = Instantiate(PlayerSkill.SkillPrefab, projectile2.transform.position, Quaternion.Euler(0, 0, 0), projectile2.transform);
        projectile2.transform.DOMove(ShootMiddle.transform.position + ShootMiddle.transform.forward * 10, 1f);
        Destroy(projectile2, 1.2f);
        projectile3.GetComponent<ProjectileObserver>().SplashEffect = PlayerSkill.SplashEffect;
        projectile3.GetComponent<ProjectileObserver>().SplashScale = PlayerSkill.SkillRootScale;
        projectile3.transform.DOScale(PlayerSkill.SkillRootScale, 0.01f);
        GameObject projectileSkin3 = Instantiate(PlayerSkill.SkillPrefab, projectile3.transform.position, Quaternion.Euler(0, 0, 0), projectile3.transform);
        projectile3.transform.DOMove(ShootLeft.transform.position + ShootLeft.transform.forward * 10, 1f);
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
