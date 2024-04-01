using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour{
    public float targetRotation;
    public float force;
    Rigidbody2D rb;

    void Start(){
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void Update(){
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetRotation, force * Time.deltaTime));
    }
}
