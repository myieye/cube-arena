using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Logging.DAL;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DatabaseManager : NetworkBehaviour {

	[SerializeField]
	private Dropdown dbVersionList;

	public static DatabaseManager Instance { get; private set; }

	void Awake () {
#if !UNITY_EDITOR
		DataService.Instance.SetDbVersion (DatabaseVersion.Mock);
		Destroy (dbVersionList.gameObject);
		Destroy (this);
#else
		Instance = this;
#endif
	}

	void Start () {
		InitDatabaseVersionList ();
		SetDbVersion (Settings.Instance.DefaultDatabaseVersion);
	}

	void OnDestroy () {
		DataService.Instance.OnDestroy ();
	}

	public void SetDbVersion (DatabaseVersion dbVersion) {
		dbVersionList.value = (int) dbVersion;
		RefreshDbVersion ();
	}

	private void InitDatabaseVersionList () {
		var databaseVersions = Enum.GetValues (typeof (DatabaseVersion)).Cast<DatabaseVersion> ().ToList ();
		dbVersionList.options = (
			from dbVersion in databaseVersions select new Dropdown.OptionData (dbVersion.ToString ())).ToList ();
	}

	public void OnSelectedDbVersionChanged () {
		RefreshDbVersion ();
	}

	[Server]
	private void RefreshDbVersion () {
#if UNITY_EDITOR
		var dbVersion = GetSelectedDbVersion ();
		dbVersionList.GetComponent<Image> ().color = dbVersion.GetColor ();
		DataService.Instance.SetDbVersion (dbVersion);
#else
		DataService.Instance.SetDbVersion (DatabaseVersion.Mock);
#endif
	}

	private DatabaseVersion GetSelectedDbVersion () {
		var dbIndex = dbVersionList.value;
		return (DatabaseVersion) dbIndex;
	}
}