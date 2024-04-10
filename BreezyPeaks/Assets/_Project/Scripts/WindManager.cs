using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WindManager : MonoBehaviour{
    [SerializeField] Vector2 velocityRange = new Vector2(10f,40f);
    [SerializeField] float maxVolume = 1f;
    [SerializeField] float volumeMultiplierPre = 1f;
    [SerializeField] float volumeMultiplierPost = 1f;
    [DisableInEditorMode]public float volume;
    [DisableInEditorMode]public float actualVolume;
    AudioSource audioSource;
    void Start(){
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    void Update(){
        if(!GameManager.GlobalTimeIsPaused){
            float playerVelocity = Player.INSTANCE.GetVelocity().magnitude;
            volume = (playerVelocity - velocityRange.x) / (velocityRange.y - velocityRange.x);
        }else{
            volume = Mathf.Lerp(volume, 0, 0.01f);
        }
        actualVolume = Mathf.Clamp(volume*volumeMultiplierPre,0,maxVolume)*volumeMultiplierPost;
        // Debug.Log(playerVelocity+" | "+volume+" | "+actualVolume);
        audioSource.volume = actualVolume;
    }
}
