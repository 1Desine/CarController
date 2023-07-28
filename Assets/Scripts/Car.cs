using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Car : MonoBehaviour {

    [Header("Steering")]
    [SerializeField] private float turnRadius;
    [SerializeField] private float steerSpeed;

    [Header("Tires")]
    [SerializeField] private float accelerationTorque;
    [SerializeField] private float tireGrip;

    [Header("Suspention")]
    [SerializeField] private float restLengh;
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float minLength;
    [SerializeField] private float maxLength;


    [Header("Suspension and wheels")]
    [SerializeField] private List<Wheel> wheelsList;
    [SerializeField] private List<Suspension> suspensionList;

    [Header("Auto find wheelBase and rearTrack")]
    [SerializeField] private bool autoFind_wheelBase_AND_rearTrack;
    [SerializeField] private Wheel frontLeftWheel;
    [SerializeField] private Wheel backRightWheel;
    [SerializeField] private float wheelBase;
    [SerializeField] private float rearTrack;


    private float steerInput;
    private float ackermannAngleLeft;
    private float ackermannAngleRight;


    private void Awake() {
        if(autoFind_wheelBase_AND_rearTrack) {
            AutoSetWheelBase_AND_RearTrack();
        }
    }

    private void Start() {
        foreach(Wheel wheel in wheelsList) {
            wheel.SetTireGrip(tireGrip);
        }
    }

    private void Update() {
        Steering();

        HandleHandBreak();
    }

    private void FixedUpdate() {
        Acceleration();
    }


    private void Steering() {
        steerInput = Input.GetAxis("Horizontal");

        if(steerInput == 0) {
            ackermannAngleLeft = 0;
            ackermannAngleRight = 0;
        } else {
            if(steerInput > 0) {
                ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
                ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
            }
            if(steerInput < 0) {
                ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
                ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
            }
        }

        foreach(Suspension suspension in suspensionList) {
            if(suspension.canSteer == false) continue;

            float steerAngleToApply = 0;
            if(suspension.left) {
                steerAngleToApply = ackermannAngleLeft;
            } else {
                steerAngleToApply = ackermannAngleRight;
            }

            suspension.steeringAngle = Mathf.Lerp(suspension.steeringAngle, steerAngleToApply, steerSpeed * Time.deltaTime);
            suspension.transform.localRotation = Quaternion.Euler(Vector3.up * suspension.steeringAngle);

            if(suspension.steerInversion) {
                suspension.transform.localRotation = Quaternion.Euler(Vector3.up * -suspension.steeringAngle);
            }
        }
    }

    private void Acceleration() {
        float accelerationInput = Input.GetAxisRaw("Vertical");

        foreach(Wheel wheel in wheelsList) {
            wheel.Accelirate(accelerationTorque * accelerationInput);
        }
    }

    private void HandleHandBreak() {
        bool breaking = Input.GetKey(KeyCode.Space);
        foreach(Wheel wheel in wheelsList) {
            wheel.HandBreak(breaking);
        }
    }

    private void AutoSetWheelBase_AND_RearTrack() {
        if(frontLeftWheel == null || backRightWheel == null) {
            Debug.Log("Auto find wheelBase and rearTrack REQUIRES wheels set: frontLeftWheel and backRightWheel");
            return;
        }

        wheelBase = Mathf.Abs(frontLeftWheel.transform.position.z) + Mathf.Abs(backRightWheel.transform.position.z);
        rearTrack = Mathf.Abs(frontLeftWheel.transform.position.x) + Mathf.Abs(backRightWheel.transform.position.x);

        Debug.Log("Auto set wheelBase = \"" + wheelBase + "\" and rearTrack = \"" + rearTrack + "\"");
    }

}
