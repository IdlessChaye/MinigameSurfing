using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdlessChaye.VRStory;

namespace IdlessChaye.IdleToolkit.AVGEngine
{
	public class OtherMarkAddNode : FunNode
	{
		public override void Interpret(ScriptSentenceContext context)
		{
			context.SkipToken("MarkAdd");
			InterpretPart(context);
		}



		protected override void OnUpdateStageContext()
		{
			if (paraList.Count != 1)
				throw new System.Exception("OtherMarkAddNode");

			Const.markList.Add(paraList[0]);
			Debug.Log(paraList[0]);
		}


	}
}