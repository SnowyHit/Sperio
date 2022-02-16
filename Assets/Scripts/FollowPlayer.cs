using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowPlayer : MonoBehaviour
{
	public Transform target;

	public float smoothSpeed = 0.125f;
	public Vector3 offset;

	void FixedUpdate()
	{
		Vector3 desiredPosition = target.position + offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.DOMove(smoothedPosition , 0.1f);
		transform.LookAt(target);
	}

}
