using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class Wheel : MonoBehaviour {


    [Header("Wheel abilities")]
    public bool canAccelirate;
    public bool breakOnHandBreak;

    private float grip;

    private Rigidbody body;
    private HingeJoint joint;

    public Vector3 pushingForce;


    private void Awake() {
        body = GetComponent<Rigidbody>();
        joint = GetComponent<HingeJoint>();
    }
    public void Update() {
        body.maxAngularVelocity = 10 * (5 + body.velocity.magnitude);
    }


    private void OnCollisionStay(Collision collision) {
        foreach(ContactPoint contact in collision.contacts) {
            Vector3 contactDirection = -transform.position + contact.point;

            Vector3 surficeRight = Vector3.right;
            Vector3 surficeUp = -contactDirection;
            Vector3 surficeForvard = Vector3.forward;

            Vector3.OrthoNormalize(ref contactDirection, ref surficeForvard, ref surficeRight);

            surficeRight.Normalize();
            surficeUp.Normalize();
            surficeForvard.Normalize();

            Vector3 wheelContactPointVelocity = body.GetPointVelocity(contact.point);

            Vector3 gripDirection = -new Vector3(
                Vector3.Dot(surficeRight, wheelContactPointVelocity),
                Vector3.Dot(surficeUp, wheelContactPointVelocity),
                Vector3.Dot(surficeForvard, wheelContactPointVelocity)
                );

            Vector3 gripForce = (body.mass + pushingForce.magnitude) * gripDirection * grip
                / (1 + wheelContactPointVelocity.magnitude / 2.7f) / collision.contacts.Length;

            body.AddForceAtPosition(gripForce, contact.point);


        }
    }


    public void Accelirate(float torque) {
        if(canAccelirate)
            body.AddTorque(transform.right * torque);
    }
    public void HandBreak(bool breaking) {
        if(breakOnHandBreak) {
            if(breaking) {
                joint.useMotor = true;
            } else {
                joint.useMotor = false;
            }
        }
    }

    public void SetTireGrip(float grip) {
        this.grip = grip;
    }



}
