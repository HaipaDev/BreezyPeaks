using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GSceneManager : MonoBehaviour{ public static GSceneManager INSTANCE;
    /*ParticleSystem transition;
    Animator transitioner;
    float transitionTime=0.35f;*/
    //float prevGameSpeed;
    void OnDisable(){Debug.LogWarning("GSceneManager disabled?");}
    void Awake(){if(GSceneManager.INSTANCE!=null){Destroy(gameObject);}else{INSTANCE=this;DontDestroyOnLoad(gameObject);gameObject.name=gameObject.name.Split('(')[0];}}
    void Start(){
        //transition=FindObjectOfType<Tag_Transition>().GetComponent<ParticleSystem>();
        //prevGameSpeed = GameManager.INSTANCE.gameSpeed;
    }
    void Update(){
        CheckESC();
        //transition=FindObjectOfType<Tag_Transition>().GetComponent<ParticleSystem>();
        //transitioner=FindObjectOfType<Tag_Transition>().GetComponent<Animator>();
    }
    public void LoadStartMenuLoader(){SceneManager.LoadScene("Menu");Instantiate(CoreSetup.INSTANCE.GetJukeboxPrefab());}
    public void LoadStartMenu(){
        SaveSerial.INSTANCE.Save();
        SceneManager.LoadScene("Menu");
        GameManager.INSTANCE.ResetMusicPitch();
        Resources.UnloadUnusedAssets();
    }
    public void LoadStartMenuGame(){GSceneManager.INSTANCE.StartCoroutine(LoadStartMenuGameI());}
    IEnumerator LoadStartMenuGameI(){
        if(GSceneManager.CheckScene("Game")){
            GameManager.INSTANCE.SaveHighscore();
            yield return new WaitForSecondsRealtime(0.01f);
            GameManager.INSTANCE.ResetScore();
        }
        yield return new WaitForSecondsRealtime(0.05f);
        SaveSerial.INSTANCE.Save();
        AudioManager.INSTANCE.ClearPausedSounds();
        GameManager.INSTANCE.ResetMusicPitch();
        yield return new WaitForSecondsRealtime(0.01f);
        Resources.UnloadUnusedAssets();
        /*GameManager.INSTANCE.SetGamemodeSelected(0);*/
    }
    public void RestartGame(){GSceneManager.INSTANCE.StartCoroutine(GSceneManager.INSTANCE.RestartGameI());}
    IEnumerator RestartGameI(){
        GameManager.INSTANCE.SaveHighscore();
        //if(GameManager.INSTANCE.CheckGamemodeSelected("Adventure"))GameManager.INSTANCE.SaveAdventure();//not sure if Restart should save or not
        yield return new WaitForSecondsRealtime(0.01f);
        //spawnReqsMono.RestartAllValues();
        //spawnReqsMono.ResetSpawnReqsList();
        GameManager.INSTANCE.ResetScore();
        GameManager.INSTANCE.ResetMusicPitch();
        yield return new WaitForSecondsRealtime(0.05f);
        AudioManager.INSTANCE.ClearPausedSounds();
    }
    public void LoadGameScene(){
        SceneManager.LoadScene("Game");GameManager.INSTANCE.ResetScore();
        AudioManager.INSTANCE.ClearPausedSounds();
    }
    // public void LoadGameModeInfoSceneSet(int i){SceneManager.LoadScene("InfoGameMode");GameManager.INSTANCE.SetGamemodeSelected(i);}
    // public void LoadGameModeInfoSceneSetStr(string str){SceneManager.LoadScene("InfoGameMode");GameManager.INSTANCE.SetGamemodeSelectedStr(str);}
    public void LoadOptionsScene(){SceneManager.LoadScene("Options");}
    public void LoadCreditsScene(){SceneManager.LoadScene("Credits");}
    public void LoadWebsite(string url){Application.OpenURL(url);}
    // public void SubmitScore(){if(SaveSerial.INSTANCE.hyperGamerLoginData.loggedIn){LoadScoreSubmitScene();}else{LoadLoginScene();}}
    // public void ReloadScene(){
    //     string _scene=SceneManager.GetActiveScene().name;
    //     SceneManager.LoadScene(_scene);
    //     GameManager.INSTANCE.speedChanged=false;
    //     GameManager.INSTANCE.gameSpeed=1f;
    // }
    // public void ReloadSceneUnload(){
    //     string _scene=SceneManager.GetActiveScene().name;
    //     SceneManager.UnloadSceneAsync(_scene);
    //     SceneManager.LoadScene(_scene);
    //     GameManager.INSTANCE.speedChanged=false;
    //     GameManager.INSTANCE.gameSpeed=1f;
    // }
    // public void ReloadSceneSwitch(){GSceneManager.INSTANCE.StartCoroutine(GSceneManager.INSTANCE.ReloadSceneSwitchI());}
    // IEnumerator ReloadSceneSwitchI(){
    //     string _scene=SceneManager.GetActiveScene().name;
    //     int _gm=GameManager.INSTANCE.gamemodeSelected;
    //     Debug.Log(GameManager.INSTANCE.gamemodeSelected);
    //     SceneManager.UnloadSceneAsync(_scene);
    //     //SceneManager.LoadScene("ChooseGameMode");
    //     LoadStartMenuGame();
    //     Debug.Log(GameManager.INSTANCE.gamemodeSelected);
    //     yield return new WaitForSeconds(0.1f);
    //     GameManager.INSTANCE.gamemodeSelected=_gm;
    //     Debug.Log(GameManager.INSTANCE.gamemodeSelected);
    //     if(_scene=="Game"){GameManager.INSTANCE.ReAddSpawnReqsMono();}
    //     SceneManager.LoadScene(_scene);
    //     Debug.Log(GameManager.INSTANCE.gamemodeSelected);
    //     GameManager.INSTANCE.gamemodeSelected=_gm;
    //     Debug.Log(GameManager.INSTANCE.gamemodeSelected);
    //     if(_scene=="Game"){GameManager.INSTANCE.ReAddSpawnReqsMono();}
    //     GameManager.INSTANCE.speedChanged=false;
    //     GameManager.INSTANCE.gameSpeed=1f;
    //     Debug.Log(GameManager.INSTANCE.gamemodeSelected);
    // }
    public void QuitGame(){Application.Quit();}
    /*void OnApplicationQuit(){
        GameManager.
    }*/
    public void RestartApp(){
        if(Jukebox.INSTANCE!=null)Destroy(Jukebox.INSTANCE.gameObject);
        SceneManager.LoadScene("Loading");
    }
    public static bool EscPressed(){return Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Joystick1Button1);}
    void CheckESC(){    if(EscPressed()){
            var scene=SceneManager.GetActiveScene().name;
            if(scene=="Credits"){LoadStartMenu();}
    }}
    public static bool CheckScene(string name){return SceneManager.GetActiveScene().name==name;}
}
