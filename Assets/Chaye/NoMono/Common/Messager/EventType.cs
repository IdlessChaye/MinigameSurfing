using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoMono
{
	public enum EventType
	{
		GameStart,

		Player_ShieldChanged,
		Player_HPChanged,
		Boss_StartBattle,
		Boss_HPChanged,
		Boss_EndBattle,

		
		Player_WuXingUpgradeChanged,
		//Player_WuXingUpgradeCDChanged,
		Player_ElementSlotChanged,
		Player_ElementDotChanged,

		//Player_SwordSkillCDChanged, // 剑谱的CD变化
		Player_SwordUpgradeChanged, // 剑谱升级
		Player_SwordSkill, // 使用剑谱结算

		Player_PickJinLeaf,
		Player_PickYinLeaf,

		Select_UpgradeSlot, // 選擇了升級槽

		None
	}
}