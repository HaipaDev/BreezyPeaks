using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Boulder : MonoBehaviour{
    [SerializeField] float collisionSoundStartThreshold=0.5f;
    [SerializeField] float hardCollisionThreshold=30f;
    [SerializeField] bool scaleDependentVolume=true;
    [SerializeField] bool scaleDependentPitch=false;
    [DisableInEditorMode][SerializeField] int collisionCounter;
    [DisableInEditorMode][SerializeField] bool collidedWithFloor;
    void Update(){
        if(transform.position.y < Player.INSTANCE.GetPosition().y - 50f){
            // Debug.Log(transform.position.y+" | "+(Player.INSTANCE.GetPosition().y - 50f));
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.CompareTag(gameObject.tag) && other.relativeVelocity.magnitude > collisionSoundStartThreshold){
            float _volume=-1;
            if(scaleDependentVolume){
                _volume = 0.1f/transform.localScale.x;
                _volume = Mathf.Clamp(_volume, 0.2f, 1f);
            }
            float _pitch=-1;
            if(scaleDependentPitch){
                _pitch = 0.1f/transform.localScale.x;
                _pitch = AssetsManager.Normalize(_pitch, 0.9f,1.1f);
            }

            if(other.relativeVelocity.magnitude > hardCollisionThreshold){
                AudioManager.INSTANCE.PlayCustom("rockHitHard",_volume,-1, _pitch);
            }else{
                if(other.gameObject.GetComponent<Boulder>()==null ||
                    (other.gameObject.GetComponent<Boulder>()!=null && this.transform.localScale.x < other.transform.localScale.x) // Make sound if smaller
                ){
                    if(other.gameObject.name.ToLower() == "floor" && !collidedWithFloor){
                        AudioManager.INSTANCE.PlayCustom("rockHit2",_volume,-1, _pitch);
                        collidedWithFloor = true;
                    }else{
                        if(collisionCounter<5){ // Up to 5 normal sounds
                            if(other.gameObject.GetComponent<Boulder>()!=null){AudioManager.INSTANCE.PlayCustom("rockHit",-1,-1, _pitch);}
                            else{AudioManager.INSTANCE.PlayCustom("rockHit4",_volume,-1, _pitch);}  // Hitting the walls basically
                            collisionCounter++;
                        }
                    }
                }
            }
        }
    }
}
