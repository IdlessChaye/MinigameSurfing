using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoMono;
using IdlessChaye.IdleToolkit.AVGEngine;

public class MainMenuPageContext : BasePageContext
{
	protected override string _prefabPath { get { return @"UI/Contexts/PageContexts/MainMenuPage"; } }

	private MonoMainMenuPageContext _mono;

	private static MainMenuPageContext _mainMenuPageContext;
	public static MainMenuPageContext Instance => _mainMenuPageContext;


	protected override void Init()
	{
		_mono = _monoContext as MonoMainMenuPageContext;
		needTick = true;

		_mainMenuPageContext = this;
	}

	public override void Setup()
	{

	}

	protected override void SetupCallbacks()
	{
		//BindCallback(_mono.startButton, () => UnityEngine.SceneManagement.SceneManager.LoadScene("MainLevel"));

		BindCallback(_mono.startButton, () => UnityEngine.SceneManagement.SceneManager.LoadScene(Const.Scene_Main));
		BindCallback(_mono.demoButton, QuitGame);
	}


	private void QuitGame()
	{
		Application.Quit();
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
