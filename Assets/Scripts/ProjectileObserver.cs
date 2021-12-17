using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProjectileObserver : MonoBehaviour
{
    public GameObject SplashEffect;
    public float SplashScale;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")
        {
            Destroy(gameObject);
            DOTween.Kill(gameObject);
            var collisionPoint = other.ClosestPoint(transform.position);
            GameObject temp = Instantiate(SplashEffect, collisionPoint, Quaternion.Euler(0, 0, 0));
            temp.transform.DOScale(SplashScale  , 0.01f);
            Destroy(temp , 0.4f);
        }
    }

}
