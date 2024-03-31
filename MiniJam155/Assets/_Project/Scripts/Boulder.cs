using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour{
    void Update(){
        if(transform.position.y < Player.INSTANCE.transform.position.y - 50f){
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.CompareTag(gameObject.tag)){
            AudioManager.INSTANCE.Play("rockHit");
        }
    }
}
