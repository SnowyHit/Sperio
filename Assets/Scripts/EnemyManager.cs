using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public enum EnemyType {Spike , Shooter , Blob};
public class EnemyManager : MonoBehaviour
{
    public EnemyType Type; 
    NavMeshAgent navMeshAgent;
    playerManager playerManager;
    public int ShooterRange = 10;
    public bool MovementLock = false;
    Animator enemyAnimator;
    CapsuleCollider enemyCapsule;
    // Start is called before the first frame update
    void Start()
    {
        enemyAnimator = transform.GetComponentInChildren<Animator>();
        enemyCapsule = GetComponent<CapsuleCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerManager = GameObject.FindObjectOfType<playerManager>();
        SetType();
    }

    // Update is called once per frame
    void Update()
    {
        if(!MovementLock)
        {
            navMeshAgent.SetDestination(playerManager.transform.position);
            transform.DOLookAt(playerManager.transform.position , 0.1f , AxisConstraint.Y);
        }
    }
    public void SetType()
    {
        if(Type == EnemyType.Spike)
        {
            SetSpike();
        }
        else if (Type == EnemyType.Shooter)
        {
            SetShooter();
        }
        else if (Type == EnemyType.Blob)
        {
            SetBlob();
        }
    }
    public void SetSpike()
    {
        navMeshAgent.stoppingDistance = 1;
        navMeshAgent.speed = 1.5f;
    }
    public void SetBlob()
    {
        navMeshAgent.stoppingDistance = 1;
        navMeshAgent.speed = 7f;
    }
    public void SetShooter()
    { 
        
        if(Physics.Linecast(transform.position , playerManager.transform.position))
        {
            navMeshAgent.stoppingDistance = 1;
        }
        else
        {
            navMeshAgent.stoppingDistance = ShooterRange;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(Type == EnemyType.Blob)
            {
                Debug.Log(transform.forward);
                playerManager.PlayerRigidbody.AddForce(transform.forward + transform.up, ForceMode.Impulse);
                Stun(0.5f);
            }
            else if(Type == EnemyType.Spike)
            {
                playerManager.Death();
            }
        }
    }

    public void Stun(float time)
    {
        StartCoroutine(StunNow(time));
    }

    IEnumerator StunNow(float time)
    {
        MovementLock = true;
        yield return new WaitForSeconds(time);
        MovementLock = false;
    }

    public void Kill()
    {
        MovementLock = true;
        enemyCapsule.enabled = false;
        navMeshAgent.enabled = false;
        enemyAnimator.SetTrigger("Dead");
        Destroy(gameObject, 2f);
    }
}
