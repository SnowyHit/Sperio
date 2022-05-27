using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType {Speed, StyleChange , AttackSpeed}
public class PowerUpObserver : MonoBehaviour
{
    public BuffType BuffType;
    public int SpeedMultiplier = 0;
    public int SpeedTime = 0;
    public GameObject BlowUp;
    public GameObject PlayerBuffEffect;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (BuffType == BuffType.Speed)
            {
                other.GetComponent<playerManager>().BuffSpeedNow(SpeedMultiplier , SpeedTime);
                Destroy(Instantiate(BlowUp, transform.position, Quaternion.identity), 0.5f);
                Destroy(Instantiate(PlayerBuffEffect, other.transform.position, Quaternion.identity), 0.5f);
                Destroy(gameObject, 0.01f);
            }
        }
        
    }
}
