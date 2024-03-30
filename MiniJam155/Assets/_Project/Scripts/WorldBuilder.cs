using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WorldBuilder : MonoBehaviour{
    [SerializeField] List<GameObject> prefabs;
    [SerializeField] GameObject prefab;
    [SerializeField] int amntToBuild = 10;
    [SerializeField] float spacing = 14f;
    GameObject lastSpawnedElement;
    void Start(){
        BuildNew();
    }
    
    void BuildNew(){
        for(int i = 0; i < amntToBuild; i++){
            float lastElementLength = (lastSpawnedElement != null) ? lastSpawnedElement.GetComponentInChildren<Renderer>().bounds.size.x : 0f;
            if(lastSpawnedElement!=null)Debug.Log(lastSpawnedElement.GetComponentInChildren<Renderer>().bounds.size.x);
            // float actualSpacing = (lastSpawnedElement != null) ? spacing : 0f;
            float actualSpacing = (lastSpawnedElement != null) ? lastElementLength : 0f;
            Vector2 origin = (lastSpawnedElement != null) ? lastSpawnedElement.transform.position : (Vector2)transform.position;
            Vector2 nextSpawnPosition = lastSpawnedElement!=null ? (origin + (Vector2)lastSpawnedElement.transform.right * actualSpacing) : transform.position;
            // if(lastSpawnedElement!=null)Debug.Log(lastSpawnedElement.transform.localEulerAngles.z);
            float nextRotation = lastSpawnedElement!=null ? ((Random.Range(0,2)==1) ? lastSpawnedElement.transform.localEulerAngles.z+90f : lastSpawnedElement.transform.localEulerAngles.z) : 0f;
            // Debug.Log(nextRotation);

            GameObject element = Instantiate(prefab, transform);
            lastSpawnedElement = element;
            element.transform.position = nextSpawnPosition;
            element.transform.localEulerAngles = new Vector3(0,0,nextRotation);
        }
    }
    void Build(){
        for(int i = 0; i < amntToBuild; i++) {
            // float lastElementLength = (lastSpawnedElement != null) ? lastSpawnedElement.GetComponent<Renderer>().bounds.size.x : 0f;
            float actualSpacing = (lastSpawnedElement != null) ? spacing : 0f;
            Vector2 origin = (lastSpawnedElement != null) ? lastSpawnedElement.transform.position : (Vector2)transform.position;
            Vector2 nextSpawnPosition = origin + Vector2.right * actualSpacing;
            GameObject element = Instantiate(prefabs[Random.Range(0,prefabs.Count)],transform);
            lastSpawnedElement = element;
            element.transform.position = nextSpawnPosition;
        }
    }
    [Button("Rebuild")]
    public void Rebuild(){
        lastSpawnedElement = null;
        for(var i = transform.childCount-1; i>0; i--){
            Destroy(transform.GetChild(i).gameObject);
        }
        // Build();
        BuildNew();
    }
}

[System.Serializable]
public class WorldElement{
    public GameObject prefab;
    public float width;
}