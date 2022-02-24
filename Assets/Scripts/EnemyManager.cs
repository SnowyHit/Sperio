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
    PlayerManager playerManager;
    public int ShooterRange = 10;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerManager = GameObject.FindObjectOfType<PlayerManager>();
        SetType();
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.SetDestination(playerManager.transform.position);
        transform.DOLookAt(playerManager.transform.position , 0.1f);
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
}
