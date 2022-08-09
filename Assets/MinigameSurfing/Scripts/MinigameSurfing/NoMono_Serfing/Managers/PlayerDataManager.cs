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

		public int foodCount = -1;
		public int woodCount = -1;

		public float shield_liushi_rate = 0.5f;
		public float shield_liushi = 1f;
		public float ShieldLiushiPerSecond(float deltaTime)
		{
			shield -= shield_liushi * shield_liushi_rate * deltaTime;
			shield = Mathf.Clamp(shield, 0, shield_max);
			return shield;
		}

		public float ShieldAdd(float count)
		{
			shield += count;
			shield = Mathf.Clamp(shield, 0, shield_max);
			return shield;
		}
	}
}