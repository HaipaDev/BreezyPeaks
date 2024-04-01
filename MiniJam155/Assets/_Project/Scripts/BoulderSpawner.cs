using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class BoulderSpawner : MonoBehaviour{
    [SerializeField] List<GameObject> prefabs;
    [SerializeField] Vector2 spawnRange = new Vector2(-10f,10f);
    [SerializeField] Vector2 spawnYAbovePlayer = new Vector2(45f,55f);
    // [SerializeField] float spawnTimeRange = new Vector2(3f,6f);
    [SerializeField] float[] spawnTimeRange = new float[]{1f,3f,6f};
    [DisableInEditorMode][SerializeField] float spawnTimer;
    [SerializeField] Vector2 randomXVelRange = new Vector2(-1f,1f);
    [SerializeField] Vector2 randomScaleRange = new Vector2(0.2f,1.1f);
    void Start(){
        spawnTimer = spawnTimeRange[spawnTimeRange.Length-1];
    }

    void Update(){
        if(spawnTimer<=0){
            SpawnBoulder();
            // spawnTimer=spawnTime;
            // spawnTimer=Random.Range(spawnTimeRange.x, spawnTimeRange.y);
            spawnTimer=spawnTimeRange[Random.Range(0,spawnTimeRange.Length-1)];
        }else{
            spawnTimer-=Time.deltaTime;
        }
    }
    [Button("SpawnBoulder")]
    public void SpawnBoulder(){
        float xx = Random.Range(spawnRange.x, spawnRange.y);
        float yy = Random.Range(spawnYAbovePlayer.x, spawnYAbovePlayer.y);
        Vector2 spawnPos = new Vector2(xx,Player.INSTANCE.GetPosition().y+yy);
        GameObject obj = Instantiate(prefabs[Random.Range(0, prefabs.Count)],spawnPos,Quaternion.identity);
        // Debug.Log("Spawned at: "+spawnPos);

        Vector2 randomVel = new Vector2(Random.Range(randomXVelRange.x, randomXVelRange.y),0);
        obj.GetComponent<Rigidbody2D>().velocity = randomVel;
        float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
        obj.transform.localScale = new Vector2(randomScale, randomScale);

        WarningObject.INSTANCE.SetWarning(spawnPos);
    }
}
