using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoMono
{
	public enum KeyType
	{
		Down,
		Hold,
		Up
	}

	public partial class InputManager : BaseGlobalManager
	{
		public static InputManager instance;

		public InputManager() : base()
		{
			instance = this;
		}

		public override void Init()
		{
			
		}

		public override void Tick2(float deltaTime)
		{
			if (Input.GetKey(KeyCode.A))
			{
				//Messenger.Broadcast<KeyType>(GlobalVars.EA, KeyType.Hold);
				//Debug.Log("A");
			}
		}
	}
}