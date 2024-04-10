using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour{
    [SerializeField] bool turnRedSelf=true;
    void OnCollisionEnter2D(Collision2D other){
        if(other.relativeVelocity.magnitude > Player.INSTANCE.deathCollisionThreshold){
            if(Player.INSTANCE.GetDebugCollisions()){Debug.LogWarning(gameObject.name+" | "+System.Math.Round(other.relativeVelocity.magnitude,2)+" / "+Player.INSTANCE.deathCollisionThreshold);}
            Player.INSTANCE.DeathCollision(other.relativeVelocity.magnitude);
            if(turnRedSelf){
                if(GetComponent<SpriteRenderer>()!=null){
                    GetComponent<SpriteRenderer>().color=Color.red;
                }
            }
            Player.INSTANCE.AddBodypartToHitList(gameObject);
        }else{
            if(Player.INSTANCE.GetDebugCollisions()){Debug.Log(gameObject.name+" | "+System.Math.Round(other.relativeVelocity.magnitude,2)+" / "+Player.INSTANCE.deathCollisionThreshold);}
            if(other.relativeVelocity.magnitude > Player.INSTANCE.GetCollisionSoundThreshold()){
                Player.INSTANCE.RegularCollisionSound(other.relativeVelocity.magnitude);
            }
        }
    }
}
