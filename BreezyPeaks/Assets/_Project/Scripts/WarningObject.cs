using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningObject : MonoBehaviour{
    [SerializeField] Transform followTransform;
    // [DisableInEditorMode][SerializeField] float timer;
    RectTransform rt;
    RectTransform canvasRect;
    void Awake(){
        // timer = 2f;
        rt = GetComponent<RectTransform>();
        canvasRect = transform.root.GetComponent<RectTransform>();
    }
    void Update(){
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(followTransform.position);

        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out canvasPosition);

        rt.localPosition = new Vector2(canvasPosition.x, rt.localPosition.y);

        if(followTransform.position.y < Player.INSTANCE.GetPosition().y+15f || followTransform==null){Destroy(gameObject);}
        // if(timer>0){timer-=Time.deltaTime;}
        // else{Destroy(gameObject);}
        
    }
    public void SetFollowTransform(Transform t){
        followTransform = t;
    }
    public void SetY(float y){
        rt = GetComponent<RectTransform>();
        rt.localPosition = new Vector2(rt.localPosition.x, y);
    }
    public void SetScale(float scale){
        rt = GetComponent<RectTransform>();
        rt.localScale = new Vector2(scale, scale);
    }
}
