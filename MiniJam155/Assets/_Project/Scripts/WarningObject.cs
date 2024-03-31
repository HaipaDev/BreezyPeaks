using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class WarningObject : MonoBehaviour{
    public static WarningObject INSTANCE;
    [SerializeField] float timerLength = 2f;
    [DisableInEditorMode][SerializeField] float timer;
    Image img;
    RectTransform rt;
    RectTransform canvasRect;
    void Awake(){INSTANCE=this;}
    void Start(){
        img = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
        canvasRect = transform.root.GetComponent<RectTransform>();
    }
    public void SetWarning(Vector2 position){
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);

        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out canvasPosition);

        rt.localPosition = new Vector2(canvasPosition.x, rt.localPosition.y);

        timer=timerLength;

        img.color = Color.white;
    }
    void Update(){
        if(timer>0){timer-=Time.deltaTime;}
        else{img.color = Color.clear;}
    }
}
