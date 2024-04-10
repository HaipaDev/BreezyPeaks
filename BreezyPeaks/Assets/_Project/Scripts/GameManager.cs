using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour{
    public static GameManager INSTANCE;
    public static bool GlobalTimeIsPaused;
    public static bool GlobalTimeIsPausedNotSlowed;

    public string gameVersion = "1.0";
	public float buildVersion = 1;

    [DisableInEditorMode]public int heightScore = 0;
    [DisableInEditorMode]public int score = 0;
    [DisableInEditorMode]public int lastHighscore = 0;
    [DisableInEditorMode]public bool setNewHighscore;
    [DisableInEditorMode]public float currentPlaytime = 0;

    void Awake(){
        if(GameManager.INSTANCE!=null){Destroy(gameObject);}else{INSTANCE=this;DontDestroyOnLoad(gameObject);gameObject.name=gameObject.name.Split('(')[0];}
    }
    void Start(){
        Jukebox.INSTANCE.PlayMusic("breezy");
        SaveSerial.INSTANCE.Load();
        SaveSerial.INSTANCE.LoadSettings();
    }
    void Update(){
        if(Time.timeScale<=0.0001f){
            GlobalTimeIsPaused=true;
            GlobalTimeIsPausedNotSlowed=true;
        }else{
            GlobalTimeIsPaused=false;
            GlobalTimeIsPausedNotSlowed=false;
        }
        if(GSceneManager.CheckScene("Game")&&Player.INSTANCE!=null&&!Player.INSTANCE.dead&&!GlobalTimeIsPaused){
            currentPlaytime+=Time.unscaledDeltaTime;
        }
        if(GSceneManager.CheckScene("Game")){
            if(Player.INSTANCE.dead){
                if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space)){
                    Player.INSTANCE.canSetScore = false;
                    // SaveHighscore();
                    // ResetScore();
                    GSceneManager.INSTANCE.RestartGame();
                }
            }
        }
    }
    public void ResetMusicPitch(){
        if(Jukebox.INSTANCE!=null)Jukebox.INSTANCE.GetComponent<AudioSource>().pitch=1;
    }
    public void SetCurrentScore(){
        score = heightScore;
        setNewHighscore = false;
    }
    public void SaveHighscore(){
        //if(CheckGamemodeSelected("Adventure")&&Player.instance!=null){SaveAdventure();}
        if(score>SaveSerial.INSTANCE.playerData.highscore.score){
            lastHighscore = SaveSerial.INSTANCE.playerData.highscore.score;
            setNewHighscore = true;
            SaveSerial.INSTANCE.playerData.highscore=new Highscore(){
                score=this.score,
                playtime=Mathf.RoundToInt(currentPlaytime),
                version=gameVersion,
                build=(float)System.Math.Round(buildVersion,2),
                date=DateTime.Now
            };
            Debug.Log("Highscore set for: "+score);
        }
        PauseMenu.INSTANCE.UpdateRestartHighscoreButtonVisibility();
        SaveSerial.INSTANCE.Save();
    }
    public void ResetScore(){
        score=0;
        currentPlaytime=0;
    }
    public void ResetHighscore(){
        SaveSerial.INSTANCE.playerData.highscore=new Highscore();
        SaveSerial.INSTANCE.Save();  // Not sure if I should save it
    }
}
