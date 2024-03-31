using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class ValueDisplay : MonoBehaviour{
    [SerializeField] public string value="score";
    [DisableInPlayMode][SerializeField] bool onlyOnEnable=false;
    [HideInPlayMode][SerializeField] bool onValidate=false;
    TextMeshProUGUI txt;
    TMP_InputField tmpInput;
    void Start(){
        if(GetComponent<TextMeshProUGUI>()!=null)txt=GetComponent<TextMeshProUGUI>();
        if(GetComponent<TMP_InputField>()!=null)tmpInput=GetComponent<TMP_InputField>();
        if(onlyOnEnable){ChangeText();}
    }
    void OnEnable(){if(onlyOnEnable){ChangeText();}}
    void OnValidate(){if(onValidate){ChangeText();}}
    void Update(){if(!onlyOnEnable){ChangeText();}}


    void ChangeText(){      string _txt="";
    #region//GameManager
        if(GameManager.INSTANCE!=null){     var gs=GameManager.INSTANCE;
            if(value=="score") _txt=("Score: "+gs.score.ToString());
        }
    #endregion
        
        if(txt!=null)txt.text=_txt;
        // else{if(tmpInput!=null){if(UIInputSystem.INSTANCE!=null)if(UIInputSystem.INSTANCE.currentSelected!=tmpInput.gameObject){tmpInput.text=_txt;}
        else{if(tmpInput!=null){tmpInput.text=_txt;}
        foreach(TextMeshProUGUI t in GetComponentsInChildren<TextMeshProUGUI>()){t.text=_txt;}}
    }
}
