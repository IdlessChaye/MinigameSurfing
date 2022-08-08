using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActiveRagdoll;

public class MinigameManager : MonoBehaviour
{

	public static MinigameManager Instance;

	private PersonBoatMananger _personBoatMananger;
	private void Awake()
	{
		Instance = this;
		AwakeSettings();
		_personBoatMananger = GetComponent<PersonBoatMananger>();
	}

	private void Start()
	{
		Debug.Log("Sta");
		//StartAVGEngineOnce(Const.avgScriptTutorial);
		StartAVGEngineOnce(Const.avgScriptName);
	}

	private void AwakeSettings()
	{
		Cursor.lockState = CursorLockMode.Confined;
		Application.targetFrameRate = 60;
	}

	private void StartAVGEngine(string scriptName)
	{
		IdlessChaye.IdleToolkit.AVGEngine.PachiGrimoire.I.StartAVGEngine(scriptName);
	}

	public void StartAVGEngineOnce(string scriptName)
	{
		if (Const.HasMark(scriptName) == false)
			StartAVGEngine(scriptName);
		Const.markList.Add(scriptName);
	}

}
