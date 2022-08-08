using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoMono;
public class MainPageContext : BasePageContext
{
	protected override string _prefabPath { get { return @"UI/Contexts/PageContexts/MainPage"; } }

	private MonoMainPageContext _mono;

	private static MainPageContext _mainPageContext;
	public static MainPageContext Instance => _mainPageContext;


	protected override void Init()
	{
		_mono = _monoContext as MonoMainPageContext;
		needTick = true;

		_mainPageContext = this;

		Debug.Log("UI Works");
	}

	public override void Setup()
	{
		Debug.Log("UI Works");
	}

	protected override void SetupCallbacks()
	{
		//BindCallback(_mono.startButton, () => UnityEngine.SceneManagement.SceneManager.LoadScene("MainLevel"));
	}


	protected override void SetupEvents()
	{
		//Messenger.AddListener((uint)EventType.Player_PickYinLeaf, OnPickYinLeaf);
	}

	protected override void ClearEvents()
	{
		//Messenger.RemoveListener((uint)EventType.Player_SwordUpgradeChanged);
	}


	protected override void Clear()
	{
		base.Clear();
	}


	public override void Tick2(float deltaTime)
	{
		base.Tick2(deltaTime);

	}
}
