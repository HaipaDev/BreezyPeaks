using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour{
    [SerializeField] int collisionCounter;
    void Update(){
        if(transform.position.y < Player.INSTANCE.GetPosition().y - 50f){
            // Debug.Log(transform.position.y+" | "+(Player.INSTANCE.GetPosition().y - 50f));
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.CompareTag(gameObject.tag)){
            if(collisionCounter<5){ // Up to 5 sounds from 1 boulder
                AudioManager.INSTANCE.Play("rockHit");
                collisionCounter++;
            }
        }
    }
}
