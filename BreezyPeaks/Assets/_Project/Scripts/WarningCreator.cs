using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class WarningCreator : MonoBehaviour{
    public static WarningCreator INSTANCE;
    [AssetsOnly][SerializeField] GameObject warningPrefab;
    // [SerializeField] float timerLength = 2f;
    [SerializeField] float y = 465f;
    [SerializeField] Vector2 minMaxScaleClamp = new Vector2(0f, 1f);
    [SerializeField] bool minMaxScaleClampToBoulderSpawnerSizes;
    Image img;
    RectTransform canvasRect;
    void Awake(){INSTANCE=this;}
    void Start(){
        canvasRect = transform.root.GetComponent<RectTransform>();
    }
    void Update(){
        if(minMaxScaleClampToBoulderSpawnerSizes){
            minMaxScaleClamp = BoulderSpawner.INSTANCE.GetRandomScaleRange();
        }
    }
    public void SetWarning(Transform t){
        GameObject _warning = Instantiate(warningPrefab,this.transform);
        _warning.GetComponent<WarningObject>().SetFollowTransform(t);
        _warning.GetComponent<WarningObject>().SetY(y);
        
        float _scale = Mathf.Clamp(t.localScale.x, minMaxScaleClamp.x, minMaxScaleClamp.y);
        _warning.GetComponent<WarningObject>().SetScale(_scale);
        // _warning.GetComponent<WarningObject>().SetTimer(timerLength);
    }
}
