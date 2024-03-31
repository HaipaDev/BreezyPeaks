using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WallMover : MonoBehaviour{
    [ChildGameObjectsOnly][SerializeField] GameObject wallR1;
    [ChildGameObjectsOnly][SerializeField] GameObject wallR2;
    [ChildGameObjectsOnly][SerializeField] GameObject wallL1;
    [ChildGameObjectsOnly][SerializeField] GameObject wallL2;
    void Update(){
        if(Player.INSTANCE.transform.position.y > wallL1.transform.position.y+wallL1.GetComponent<Renderer>().bounds.size.y){
            // wallL1.transform.position = new Vector2(wallL1.transform.position.x, wallL2.transform.position.y+wallL2.GetComponent<Renderer>().bounds.extents.y);
            wallL1.transform.position = new Vector2(wallL1.transform.position.x, wallL2.transform.position.y+wallL2.GetComponent<Renderer>().bounds.size.y-1f);
            wallR1.transform.position = new Vector2(wallR1.transform.position.x, wallR2.transform.position.y+wallR2.GetComponent<Renderer>().bounds.size.y-1f);
        }
        if(Player.INSTANCE.transform.position.y > wallL2.transform.position.y+wallL2.GetComponent<Renderer>().bounds.size.y){
            wallL2.transform.position = new Vector2(wallL2.transform.position.x, wallL1.transform.position.y+wallL1.GetComponent<Renderer>().bounds.size.y-1f);
            wallR2.transform.position = new Vector2(wallR2.transform.position.x, wallR1.transform.position.y+wallL1.GetComponent<Renderer>().bounds.size.y-1f);
        }
        
        if(Player.INSTANCE.transform.position.y > wallL1.GetComponent<Renderer>().bounds.size.y){  // Above starting point
            if(Player.INSTANCE.transform.position.y < wallL1.transform.position.y-wallL1.GetComponent<Renderer>().bounds.size.y){
                wallL1.transform.position = new Vector2(wallL1.transform.position.x, wallL2.transform.position.y-wallL2.GetComponent<Renderer>().bounds.size.y+1f);
                wallR1.transform.position = new Vector2(wallR1.transform.position.x, wallR2.transform.position.y-wallR2.GetComponent<Renderer>().bounds.size.y+1f);
            }
            if(Player.INSTANCE.transform.position.y < wallL2.transform.position.y-wallL2.GetComponent<Renderer>().bounds.size.y){
                wallL2.transform.position = new Vector2(wallL2.transform.position.x, wallL1.transform.position.y-wallL1.GetComponent<Renderer>().bounds.size.y+1f);
                wallR2.transform.position = new Vector2(wallR2.transform.position.x, wallR1.transform.position.y-wallL1.GetComponent<Renderer>().bounds.size.y+1f);
            }
        }
    }
}
