using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoMono
{
	public partial class PlayManager : BaseLocalManager
	{
		public PlayManager Instance;

		public override void Init()
		{
			Instance = this;

			SetNeedTick(true);
		}

		public override void Setup()
		{
			
		}

		public override void Tick2(float deltaTime)
		{
			base.Tick2(deltaTime);

			PlayerDataManager.Instance.ShieldLiushiPerSecond(deltaTime);
		}



	}
}