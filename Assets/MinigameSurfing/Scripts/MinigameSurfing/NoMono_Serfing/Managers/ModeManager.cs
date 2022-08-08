using System.Collections;
using System.Collections.Generic;
using NoMono;
using UnityEngine;

namespace NoMono
{
	public enum GameMode
	{
		World,
		AVG,
		Idle
	}

	public partial class ModeManager : BaseGlobalManager
	{
		private static ModeManager _instance = new ModeManager();
		public static ModeManager I => _instance;


		private GameMode _currentMode;
		public GameMode GameMode => _currentMode;

		public override void Init()
		{
			TransferTo(GameMode.Idle);
		}

		public void TransferTo(GameMode newMode)
		{
			_currentMode = newMode;

			switch (newMode)
			{
				case GameMode.World:
					SetCursor(CursorLockMode.Locked, false);
					break;
				case GameMode.AVG:
					SetCursor(CursorLockMode.Confined, true);
					break;
				case GameMode.Idle:
					SetCursor(CursorLockMode.Confined, true);
					break;
			}
		}

		public void SetCursor(CursorLockMode lockMode, bool isVisible)
		{
			Cursor.lockState = lockMode;
			Cursor.visible = isVisible;
		}

	}
}