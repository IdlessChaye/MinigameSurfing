using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUniStormManager : MonoBehaviour
{
	void Start()
	{
		UniStormSystem.Instance.OnHourChangeEvent.AddListener(
			() => UniStormManager.Instance.RandomWeather()
			);
	}
}
