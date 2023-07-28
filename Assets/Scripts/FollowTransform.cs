using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 angle;
    [SerializeField] private float followSpeed;

    
    void Update() {
        Vector3 followPosition = target.position + target.right * offset.x + target.up * offset.y +  target.forward * offset.z;
        transform.position = Vector3.Slerp(transform.position, followPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + angle);
    }

}
