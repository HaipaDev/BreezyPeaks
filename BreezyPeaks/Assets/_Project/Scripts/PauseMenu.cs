using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class PauseMenu : MonoBehaviour{
    public static PauseMenu INSTANCE;
    public static bool GameIsPaused = false;
    [ChildGameObjectsOnly]public GameObject pauseMenuUI;
    [ChildGameObjectsOnly]public GameObject restartHighscoreUI;
    // [ChildGameObjectsOnly]public GameObject optionsUI;
    [ChildGameObjectsOnly]public GameObject blurChild;
    [ChildGameObjectsOnly]public Button restartHighscoreButton;
    [ChildGameObjectsOnly]public Toggle holdToFlyToggle;
    [ChildGameObjectsOnly]public Toggle particlesToggle;
    [ChildGameObjectsOnly]public Slider soundVolumeSlider;
    [ChildGameObjectsOnly]public Slider windVolumeSlider;
    [ChildGameObjectsOnly]public Slider musicVolumeSlider;
    float unpausedTimer;
    float unpausedTimeReq=0.3f;
    bool awaitingResetHighscoreConfirm;

    void Start(){
        INSTANCE=this;
        Resume();
        unpausedTimeReq=0;

        UpdateRestartHighscoreButtonVisibility();
        holdToFlyToggle.isOn=SaveSerial.INSTANCE.settingsData.holdToFly;
        UpdateToggleHoldToFly();
        UpdateToggleParticles();
        soundVolumeSlider.value=SaveSerial.INSTANCE.settingsData.soundVolume;
        windVolumeSlider.value=SaveSerial.INSTANCE.settingsData.windVolume;
        musicVolumeSlider.value=SaveSerial.INSTANCE.settingsData.musicVolume;
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
                    else if(awaitingResetHighscoreConfirm){ResetHighscoreConfirmClose();return;}
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
            if(pauseMenuUI.activeSelf){
                if(Input.GetKeyDown(KeyCode.R)){Restart();}
            }
        }
    }
    public void Resume(){
        blurChild.SetActive(false);
        pauseMenuUI.SetActive(false);
        restartHighscoreUI.SetActive(false);
        // if(optionsUI.transform.childCount>0)if(optionsUI.transform.GetChild(0).gameObject.activeSelf){if(SettingsMenu.INSTANCE!=null)SettingsMenu.INSTANCE.Back();}
        Time.timeScale=1;
        GameIsPaused=false;
        
        SaveSerial.INSTANCE.SaveSettings();
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
        restartHighscoreUI.SetActive(false);
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


    
    public void SetSoundVolume(float val){
        SaveSerial.INSTANCE.settingsData.soundVolume=val;
        // SaveSerial.INSTANCE.SaveSettings();
    }
    public void SetWindVolume(float val){
        SaveSerial.INSTANCE.settingsData.windVolume=val;
        // SaveSerial.INSTANCE.SaveSettings();
    }
    public void SetMusicVolume(float val){
        SaveSerial.INSTANCE.settingsData.musicVolume=val;
        // SaveSerial.INSTANCE.SaveSettings();
    }
    public void ResetHighscoreConfirmOpen(){
        awaitingResetHighscoreConfirm = true;
        pauseMenuUI.SetActive(false);
        restartHighscoreUI.SetActive(true);
    }
    public void ResetHighscoreConfirmClose(){
        awaitingResetHighscoreConfirm = false;
        pauseMenuUI.SetActive(true);
        restartHighscoreUI.SetActive(false);
    }
    public void ResetHighscore(){
        ResetHighscoreConfirmClose();
        GameManager.INSTANCE.ResetHighscore();
    }
    public void ToggleHoldToFly(bool isOn){
        SaveSerial.INSTANCE.settingsData.holdToFly=isOn;
        UpdateToggleHoldToFly();
        // SaveSerial.INSTANCE.SaveSettings();
    }
    public void ToggleParticles(bool isOn){
        SaveSerial.INSTANCE.settingsData.particles=isOn;
        UpdateToggleParticles();
        // SaveSerial.INSTANCE.SaveSettings();
    }
    void UpdateToggleHoldToFly(){
        holdToFlyToggle.isOn = SaveSerial.INSTANCE.settingsData.holdToFly;
        holdToFlyToggle.GetComponent<TextMeshProUGUI>().text = SaveSerial.INSTANCE.settingsData.holdToFly ? "Hold to\nFly On" : "Hold to\nFly Off";
        holdToFlyToggle.GetComponent<TextMeshProUGUI>().color = SaveSerial.INSTANCE.settingsData.holdToFly ? new Color(0,128,0) : Color.white;
    }
    void UpdateToggleParticles(){
        particlesToggle.isOn = SaveSerial.INSTANCE.settingsData.particles;
        particlesToggle.GetComponent<TextMeshProUGUI>().text = SaveSerial.INSTANCE.settingsData.particles ? "Particles\nOn" : "Particles\nOff";
        particlesToggle.GetComponent<TextMeshProUGUI>().color = SaveSerial.INSTANCE.settingsData.particles ? new Color(0,128,0) : Color.white;
    }
    public void UpdateRestartHighscoreButtonVisibility(){
        restartHighscoreButton.gameObject.SetActive(SaveSerial.INSTANCE.playerData.highscore!=null && SaveSerial.INSTANCE.playerData.highscore.score>0 ? true : false);
    }
}