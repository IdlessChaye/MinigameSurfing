using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
	private void Awake()
	{
		AwakeSettings();
	}

	private void AwakeSettings()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}
}
