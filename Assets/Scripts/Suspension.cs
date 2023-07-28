using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Suspension : MonoBehaviour {

    [Header("Wheel specs")]
    public bool front;
    public bool left;
    public bool canSteer;
    public bool steerInversion;

    public float steeringAngle { get; set; }


    private void Awake() {
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        if(canSteer) {
            joint.angularYMotion = ConfigurableJointMotion.Free;
        }
    }


}
