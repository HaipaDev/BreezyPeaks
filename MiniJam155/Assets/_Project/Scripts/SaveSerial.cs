using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;

public class SaveSerial : MonoBehaviour{
	public static SaveSerial INSTANCE;
	void Awake(){if(INSTANCE!=null){Destroy(gameObject);}else{INSTANCE=this;DontDestroyOnLoad(gameObject);gameObject.name=gameObject.name.Split('(')[0];}}
	IEnumerator Start(){
		yield return new WaitForSecondsRealtime(0.02f);
		RecreatePlayerData();
	}
	// [SerializeField] string filenameLogin = "hyperGamerLogin";
	[SerializeField] string filename = "playerData";
	[SerializeField] string filenameSettings = "gameSettings";
	// public static int maxRegisteredHyperGamers=3;

/*#region//HyperGamerLogin
	public HyperGamerLoginData hyperGamerLoginData=new HyperGamerLoginData();
	[System.Serializable]public class HyperGamerLoginData{
		public int registeredCount;
		public bool loggedIn;
		public string username;
		public string password;
		public DateTime lastLoggedIn;
	}
	public string _loginDataPath(){return Application.persistentDataPath+"/"+filenameLogin+".hyper";}
	public void SetLogin(string username, string password){
		hyperGamerLoginData.loggedIn=true;
		hyperGamerLoginData.username=username;
		hyperGamerLoginData.password=password;
		hyperGamerLoginData.lastLoggedIn=DateTime.Now;
		Debug.Log("Login data set");
	}
	public void LogOut(){
		hyperGamerLoginData.loggedIn=false;
		hyperGamerLoginData.username="";
		hyperGamerLoginData.password="";
		Debug.Log("Logged out");
	}
	public void SaveLogin(){
		var settings=new ES3Settings(_loginDataPath(),ES3.EncryptionType.AES,gitignoreScript.savefilesEncryptionKey);
		ES3.Save("hyperGamerLoginData",hyperGamerLoginData,settings);
		Debug.Log("Login saved");
	}
	public void LoadLogin(){
		if(ES3.FileExists(_loginDataPath())){
			var settings=new ES3Settings(_loginDataPath(),ES3.EncryptionType.AES,gitignoreScript.savefilesEncryptionKey);
			if(ES3.KeyExists("hyperGamerLoginData",settings)){ES3.LoadInto<HyperGamerLoginData>("hyperGamerLoginData",hyperGamerLoginData,settings);}
			else{Debug.LogWarning("Key for hyperGamerLoginData not found in: "+_loginDataPath());}
		}else Debug.LogWarning("Login Data file not found in "+_loginDataPath());
	}
	void AutoLogin(){
		//TimeSpan tsSession=DateTime.Now.Subtract(hyperGamerLoginData.lastLoggedIn);
		//if(tsSession.TotalDays>=14){LogOut();}
		//else{
			TryLogin(hyperGamerLoginData.username,hyperGamerLoginData.password);
		//}
	}
	public void TryLogin(string username, string password){
		//try{
			if(DBAccess.INSTANCE!=null){if(hyperGamerLoginData.username!="")DBAccess.INSTANCE.LoginHyperGamer(hyperGamerLoginData.username,hyperGamerLoginData.password);}
			else{Debug.Log("No DBAccess, cant try to login");}
		//}catch{}
	}
#endregion*/
#region//Player Data
	public PlayerData playerData=new PlayerData();
	public float buildFirstLoaded;
	public float buildLastLoaded;
	[System.Serializable]public class PlayerData{
		public Highscore highscore=new Highscore();
	}

	public string _playerDataPath(){return Application.persistentDataPath+"/"+filename+".hyper";}
	public void Save(){
        var settings=new ES3Settings(_playerDataPath(),ES3.EncryptionType.AES,gitignoreScript.savefilesEncryptionKey);
		if(!ES3.KeyExists("buildFirstLoaded",settings)){buildFirstLoaded=GameManager.INSTANCE.buildVersion;ES3.Save("buildFirstLoaded",buildFirstLoaded,settings);}
		buildLastLoaded=GameManager.INSTANCE.buildVersion;ES3.Save("buildLastLoaded",buildLastLoaded,settings);
		ES3.Save("playerData",playerData,settings);
		Debug.Log("Game Data saved");
	}
	public void Load(){
		if(ES3.FileExists(_playerDataPath())){
			var settings=new ES3Settings(_playerDataPath(),ES3.EncryptionType.AES,gitignoreScript.savefilesEncryptionKey);

			if(ES3.KeyExists("buildFirstLoaded",settings)){buildFirstLoaded=ES3.Load<float>("buildFirstLoaded",8,settings);}
			else{Debug.LogWarning("Key for buildFirstLoaded not found in: "+_playerDataPath());}

			if(ES3.KeyExists("buildLastLoaded",settings)){buildLastLoaded=ES3.Load<float>("buildLastLoaded",8,settings);}
			else{Debug.LogWarning("Key for buildLastLoaded not found in: "+_playerDataPath());}

			if(ES3.KeyExists("playerData",settings)){ES3.LoadInto<PlayerData>("playerData",playerData,settings);}
			else{Debug.LogWarning("Key for playerData not found in: "+_playerDataPath());}
			//var hi=-1;foreach(int h in playerData.highscore){hi++;if(h!=0)playerData.highscore[hi]=h;}
			Debug.Log("Game Data loaded");
		}else Debug.LogWarning("Game Data file not found in: "+_playerDataPath());
	}
	public void Delete(){
		playerData=new PlayerData();
		RecreatePlayerData();
		Debug.Log("Game Data reset");
		GC.Collect();
		if(ES3.FileExists(_playerDataPath())){
			ES3.DeleteFile(_playerDataPath());
			Debug.Log("Game Data deleted!");
		}
	}
	void RecreatePlayerData(){
		playerData.highscore=new Highscore();
		// for(int i=0;i<playerData.highscore.Length;i++){playerData.highscore[i]=new Highscore();}
		//playerData.achievsCompleted=new AchievData[StatsAchievsManager._AchievsListCount()];
	}
#endregion

#region//Settings Data
	public SettingsData settingsData=new SettingsData();
	[System.Serializable]public class SettingsData{
		public float masterVolume=0.95f;
		public float masterOOFVolume=0.25f;
		public float soundVolume=0.95f;
		public float musicVolume=0.66f;

		public bool particles=true;
	}
	
	public string _settingsDataPath(){return Application.persistentDataPath+"/"+filenameSettings+".json";}
	public void SaveSettings(){
		var settings=new ES3Settings(_settingsDataPath(),ES3.EncryptionType.None);
		ES3.Save("settingsData",settingsData,settings);
		Debug.Log("Settings saved");
	}
	public void LoadSettings(){
		if(ES3.FileExists(_settingsDataPath())){
		var settings=new ES3Settings(_settingsDataPath(),ES3.EncryptionType.None);
			if(ES3.KeyExists("settingsData",settings)){ES3.LoadInto<SettingsData>("settingsData",settingsData,settings);}
			else{Debug.LogWarning("Key for settingsData not found in: "+_settingsDataPath());}
		}else Debug.LogWarning("Settings file not found in: "+_settingsDataPath());
	}
	public void DeleteSettings(){
		settingsData=new SettingsData();
		GC.Collect();
		if(ES3.FileExists(_settingsDataPath())){
			ES3.DeleteFile(_settingsDataPath());
			Debug.Log("Settings deleted");
		}
	}
#endregion
}

[System.Serializable]
public class Highscore{
	public int score;
	public int playtime;
	public string version;
	public float build;
	public DateTime date;
}