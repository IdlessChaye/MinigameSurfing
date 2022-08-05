using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActiveRagdoll;

public class MinigameManager : MonoBehaviour
{

	private PersonBoatMananger _personBoatMananger;
	private void Awake()
	{
		AwakeSettings();
		_personBoatMananger = GetComponent<PersonBoatMananger>();
	}

	private void AwakeSettings()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

}
