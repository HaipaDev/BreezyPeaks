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
            if(value=="score"){
                if(!Player.INSTANCE.dead){_txt=("Score: "+gs.heightScore.ToString());}
                else{_txt=("Height: "+gs.heightScore.ToString());}
            }
            if(value=="scorePostMortem"){
                if(Player.INSTANCE.dead){
                    if(GameManager.INSTANCE.setNewHighscore){
                        _txt=gs.lastHighscore>0 ? ("Previous: "+gs.lastHighscore.ToString()) : "";
                    }else{
                        _txt=("Score: "+gs.score.ToString());
                    }
                }
                else{_txt="";}
            }
            if(value=="gameVersion") _txt=("V "+gs.gameVersion.ToString());
        }
    #endregion
    #region//SaveSerial
        if(SaveSerial.INSTANCE!=null){     var s=SaveSerial.INSTANCE;
        var pd=s.playerData;
            if(value=="highscore"){
                if(pd.highscore.score>0){
                    _txt=GameManager.INSTANCE.setNewHighscore ? ("New Highscore: "+pd.highscore.score.ToString()) : ("Highscore: "+pd.highscore.score.ToString());
                }
                else{_txt="";}
            }
        }
    #endregion
        
        if(txt!=null)txt.text=_txt;
        // else{if(tmpInput!=null){if(UIInputSystem.INSTANCE!=null)if(UIInputSystem.INSTANCE.currentSelected!=tmpInput.gameObject){tmpInput.text=_txt;}
        else{if(tmpInput!=null){tmpInput.text=_txt;}
        foreach(TextMeshProUGUI t in GetComponentsInChildren<TextMeshProUGUI>()){t.text=_txt;}}
    }
}
