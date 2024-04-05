using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class PauseMenu : MonoBehaviour{     public static PauseMenu INSTANCE;
    public static bool GameIsPaused = false;
    [ChildGameObjectsOnly]public GameObject pauseMenuUI;
    // [ChildGameObjectsOnly]public GameObject optionsUI;
    [ChildGameObjectsOnly]public GameObject blurChild;
    float unpausedTimer;
    float unpausedTimeReq=0.3f;
    //Shop shop;
    void Start(){
        INSTANCE=this;
        Resume();
        unpausedTimeReq=0;
    }
    void Update(){
        var _isEditor=false;
        #if UNITY_EDITOR
            _isEditor=true;
        #endif
        if(((GSceneManager.EscPressed())||Input.GetKeyDown(KeyCode.P)//||Input.GetKeyDown(KeyCode.Backspace)||Input.GetKeyDown(KeyCode.JoystickButton7))
        ||(!Application.isFocused&&!_isEditor&&SaveSerial.INSTANCE!=null&&SaveSerial.INSTANCE.settingsData.pauseWhenOOF))
        // &&(UIInputSystem.INSTANCE.currentSelected==null||(UIInputSystem.INSTANCE.currentSelected!=null&&UIInputSystem.INSTANCE.currentSelected.GetComponent<TMPro.TextMeshProUGUI>()!=null))
        ){
            if(GameIsPaused){
                if(Application.isFocused){
                    if(pauseMenuUI.activeSelf){Resume();return;}
                    // if(optionsUI.transform.GetChild(0).gameObject.activeSelf){SaveSerial.INSTANCE.SaveSettings();pauseMenuUI.SetActive(true);return;}
                    // if(optionsUI.transform.GetChild(1).gameObject.activeSelf){optionsUI.GetComponent<SettingsMenu>().OpenSettings();PauseEmpty();return;}
                }
            }else{
                if(_isPausable()){Pause();}
            }
        }//if(Input.GetKeyDown(KeyCode.R)){//in GameManager}
        if(!GameIsPaused){
            if(unpausedTimer==-1)unpausedTimer=0;
            unpausedTimer+=Time.unscaledDeltaTime;
        }else{
            if(Input.GetKeyDown(KeyCode.R)){Restart();}
        }
    }
    public void Resume(){
        pauseMenuUI.SetActive(false);
        blurChild.SetActive(false);
        // if(optionsUI.transform.childCount>0)if(optionsUI.transform.GetChild(0).gameObject.activeSelf){if(SettingsMenu.INSTANCE!=null)SettingsMenu.INSTANCE.Back();}
        Time.timeScale=1;
        GameIsPaused=false;
    }
    public void Restart(){
        GSceneManager.INSTANCE.RestartGame();
        GameManager.INSTANCE.ResetScore();
    }
    public void PauseEmpty(){
        GameIsPaused=true;
        Time.timeScale=0;
        unpausedTimer=-1;
    }
    public void Pause(){
        pauseMenuUI.SetActive(true);
        blurChild.SetActive(true);
        PauseEmpty();
    }
    
    // public void OpenOptions(){
    //     optionsUI.GetComponent<SettingsMenu>().OpenSettings();
    //     pauseMenuUI.SetActive(false);
    // }
    public void Quit(){
        // GSceneManager.INSTANCE.LoadStartMenuGame();
        GameManager.INSTANCE.SaveHighscore();
        Application.Quit();
    }

    public bool _isPausable(){
        var _isEditor=false;
        #if UNITY_EDITOR
            _isEditor=true;
        #endif
        bool _pauseWhenOOF=SaveSerial.INSTANCE!=null&&SaveSerial.INSTANCE.settingsData!=null&&SaveSerial.INSTANCE.settingsData.pauseWhenOOF;
        return(
        ((unpausedTimer>=unpausedTimeReq||unpausedTimer==-1))&&
        ((Application.isFocused)||(!Application.isFocused&&!_isEditor&&_pauseWhenOOF))
        );
    }
}