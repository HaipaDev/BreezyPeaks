using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour{
    [SerializeField] bool followX;
    [SerializeField] bool followY = true;
    void Update(){
        float xx = transform.position.x;
        float yy = transform.position.y;
        if(followX){xx=Player.INSTANCE.transform.position.x;}
        if(followY){yy=Player.INSTANCE.transform.position.y;}
        transform.position = new Vector2(xx, yy);
    }
}
