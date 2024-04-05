using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxBG : MonoBehaviour{
    [SerializeField] float parallaxEffect;
    float length;
    float startpos;
    void Start(){
        startpos = transform.position.x;
        length = GetComponent<Renderer>().bounds.size.x;
    }

    void FixedUpdate(){
        float dist = (Camera.main.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
    }
}