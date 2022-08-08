using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoMono
{
	public partial class PlayerDataManager : BaseGlobalManager
	{
		public PlayerDataManager Instance;

		public override void Init()
		{
			Instance = this;
		}
	}
}