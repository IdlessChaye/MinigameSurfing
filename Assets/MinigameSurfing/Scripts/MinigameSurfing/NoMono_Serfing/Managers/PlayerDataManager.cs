using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoMono
{
	public partial class PlayerDataManager : BaseGlobalManager
	{
		public static PlayerDataManager Instance;

		public override void Init()
		{
			Instance = this;

			Const.markList.Clear();
		}

		public float shield = 100f;
		public float shield_max = 100f;

		public float hp = 100f;
		public float hp_max = 100f;

		public float boss_hp = 100f;
		public float boss_hp_max = 100f;

		public int foodCount = 0;
		public int woodCount = 0;
	}
}