using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour{
    [SerializeField] bool turnRedSelf=true;
    void OnCollisionEnter2D(Collision2D other){
        // Debug.Log("Collision: "+other.relativeVelocity);
        // Debug.Log(gameObject.name+" | "+System.Math.Round(other.relativeVelocity.magnitude,2)+" / "+Player.INSTANCE.collisionThreshold);
        if(other.relativeVelocity.magnitude > Player.INSTANCE.collisionThreshold){
            Debug.LogWarning(gameObject.name+" | "+System.Math.Round(other.relativeVelocity.magnitude,2)+" / "+Player.INSTANCE.collisionThreshold);
            Player.INSTANCE.DeathCollision();
            if(turnRedSelf){
                if(GetComponent<SpriteRenderer>()!=null){
                    GetComponent<SpriteRenderer>().color=Color.red;
                }
            }
            Player.INSTANCE.AddBodypartToHitList(gameObject);
        }else{
            Debug.Log(gameObject.name+" | "+System.Math.Round(other.relativeVelocity.magnitude,2)+" / "+Player.INSTANCE.collisionThreshold);
            if(other.relativeVelocity.magnitude > 0.1f){
                AudioManager.INSTANCE.Play("punch");
            }
        }
    }
}
