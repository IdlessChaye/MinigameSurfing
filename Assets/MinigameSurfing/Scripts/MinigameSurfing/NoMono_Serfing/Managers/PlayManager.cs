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
		}

		public override void Setup()
		{
			
		}



	}
}