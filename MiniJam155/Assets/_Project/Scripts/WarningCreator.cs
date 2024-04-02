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
    Image img;
    RectTransform canvasRect;
    void Awake(){INSTANCE=this;}
    void Start(){
        canvasRect = transform.root.GetComponent<RectTransform>();
    }
    public void SetWarning(Transform t){
        GameObject _warning = Instantiate(warningPrefab,this.transform);
        _warning.GetComponent<WarningObject>().SetFollowTransform(t);
        _warning.GetComponent<WarningObject>().SetY(y);
        // _warning.GetComponent<WarningObject>().SetTimer(timerLength);
    }
}
