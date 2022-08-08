using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace NoMono
{
	public class CombatPageContext : BasePageContext
	{
		protected override string _prefabPath { get { return @"UI/Contexts/PageContexts/CombatPage"; } }

		private MonoCombatPage _mono;

		private float _shield;
		private float _hp;
		private float _playerHP; 
		private float _playerMaxHP;
		private float _bossHP;
		private float _bossMaxHP;

		protected override void Init()
		{
			_mono = _monoContext as MonoCombatPage;
			needTick = true;
		}

		public override void Setup()
		{
			SetupPlayer();
			SetupShield();
			SetupBoss();
			SetupLeaf();
		}

		protected override void SetupCallbacks()
		{
			//BindCallback(_mono., () => Debug.Log("释放大招"));
		}

		protected override void SetupEvents()
		{
			Messenger.AddListener<float, float>((uint)EventType.Player_ShieldChanged, OnPlayerShieldChanged);
			Messenger.AddListener<float, float>((uint)EventType.Player_HPChanged, OnPlayerHPChanged);
			Messenger.AddListener<float, float>((uint)EventType.Boss_HPChanged, OnBossHPChanged);
			Messenger.AddListener((uint)EventType.Boss_StartBattle, OnBossStartBattle);
			Messenger.AddListener((uint)EventType.Boss_EndBattle, OnBossEndBattle);

			Messenger.AddListener((uint)EventType.Player_PickJinLeaf, OnPickJinLeaf);
			Messenger.AddListener((uint)EventType.Player_PickYinLeaf, OnPickYinLeaf);
		}

		protected override void ClearEvents()
		{
			Messenger.RemoveListener((uint)EventType.Boss_EndBattle);
			Messenger.RemoveListener((uint)EventType.Boss_StartBattle);
			Messenger.RemoveListener((uint)EventType.Boss_HPChanged);
			Messenger.RemoveListener((uint)EventType.Player_HPChanged);
			Messenger.RemoveListener((uint)EventType.Player_ShieldChanged);
            Messenger.RemoveListener((uint)EventType.Player_PickJinLeaf, OnPickJinLeaf);
            Messenger.RemoveListener((uint)EventType.Player_PickYinLeaf, OnPickYinLeaf);
        }


		public override void Tick2(float deltaTime)
		{
			base.Tick2(deltaTime);

			FollowPlayerShield();
			FollowBossHP();
			FollowPlayerHP();
		}

		#region Setup

		private void SetupLeaf()
        {
			OnPickJinLeaf();
			OnPickYinLeaf();
        }

		private void SetupPlayer()
		{
			_hp = PlayerDataManager.Instance.hp;
			_mono.hpSlider.minValue = 0;
			_mono.hpSlider.maxValue = PlayerDataManager.Instance.hp_max;
			_playerMaxHP = PlayerDataManager.Instance.hp_max;
			_playerHP = _hp;
			SetupPlayerHP(_hp, _playerMaxHP);
		}

		private void SetupPlayerHP(float hp, float maxHP)
		{
			var bs = _mono.hpSlider;
			Utils.TrySetActive(bs.gameObject, true);
			bs.value = hp;
			_playerHP = hp;
			_playerMaxHP = maxHP;

			_playerHPFollow = hp;
			var rectTF = _mono.hpSliderFollow?.GetComponent<RectTransform>();
			var anchorMax = rectTF.anchorMax;
			anchorMax.x = Mathf.Clamp01(_playerHPFollow / _playerMaxHP);
			rectTF.anchorMax = anchorMax;

			int playerHP = System.Convert.ToInt32(_playerHP);
			int playerMaxHP = System.Convert.ToInt32(_playerMaxHP);
			_mono.hpText.text = playerHP.ToString() + " / " + playerMaxHP.ToString();
		}

		private void SetupShield()
		{
			_shield = PlayerDataManager.Instance.shield;//_unitPropertyEntity?.Shield;
			SetupPlayerShield(_shield, PlayerDataManager.Instance.shield_max);
		}

		private void SetupPlayerShield(float shield, float maxShield)
		{
			var ss = _mono.shieldSlider;
			ss.maxValue = maxShield;
			ss.value = shield;
			int playerShieldHP = System.Convert.ToInt32(shield);
			int playerShieldMaxHP = System.Convert.ToInt32(maxShield);

			var st = _mono.shieldText;
			st.text = playerShieldHP.ToString() + " / " + playerShieldMaxHP.ToString();
		}


		private void SetupBoss()
		{
			_bossHP = PlayerDataManager.Instance.boss_hp;
			_bossMaxHP = PlayerDataManager.Instance.boss_hp_max;
			Utils.TrySetActive(_mono.bossSlider.gameObject, false);
			Utils.TrySetActive(_mono.bossTextGO, false);
		}

		private void SetupBossHP(float hp, float targetMaxHP)
		{
			var bs = _mono.bossSlider;
			Utils.TrySetActive(bs.gameObject, true);

			bs.maxValue = targetMaxHP;
			bs.value = hp;
			_bossHP = hp;
			_bossMaxHP = targetMaxHP;

			_bossHPFollow = hp;
			var rectTF = _mono.bossSliderFollow?.GetComponent<RectTransform>();
			var anchorMax = rectTF.anchorMax;
			anchorMax.x = Mathf.Clamp01(_bossHPFollow / _bossMaxHP);
			rectTF.anchorMax = anchorMax;
		}

		#endregion

		#region Shield & PlayerHP * BossHP

		private void UpdatePlayerShieldHP(float shieldHP, float shieldMaxHP)
		{
			SetupPlayerShield(shieldHP, shieldMaxHP);
		}

		float _playerHPFollow = 0;

		private void UpdatePlayerHP(float hp, float targetMaxHP)
		{
			_mono.hpSlider.value = hp;
			_playerHP = hp;
			_playerMaxHP = targetMaxHP;

			int playerHP = System.Convert.ToInt32(_playerHP);
			int playerMaxHP = System.Convert.ToInt32(_playerMaxHP);
			_mono.hpText.text = playerHP.ToString() + " / " + playerMaxHP.ToString();
			

			if (hp > _playerHPFollow)
			{
				_playerHPFollow = hp;
				return;
			}
		}

		private void FollowPlayerHP()
		{
			_hp = PlayerDataManager.Instance.hp;
			UpdatePlayerHP(_hp, PlayerDataManager.Instance.hp_max);
			float weight = Mathf.Clamp01(_mono.hpSpeed * Time.deltaTime + _mono.hpFixedWeight);
			_playerHPFollow = weight * _playerHP + (1 - weight) * _playerHPFollow;

			var rectTF = _mono.hpSliderFollow?.GetComponent<RectTransform>();
			if (rectTF == null)
				return;
			var anchorMax = rectTF.anchorMax;
			anchorMax.x = Mathf.Clamp01(_playerHPFollow / _playerMaxHP);
			rectTF.anchorMax = anchorMax;
		}


		float _bossHPFollow = 0;

		private void UpdateBossHP(float hp, float targetMaxHP)
		{
			var bs = _mono.bossSlider;
			if (bs.IsActive() == false)
				return;

			bs.maxValue = targetMaxHP;
			bs.value = hp;
			_bossHP = hp;
			_bossMaxHP = targetMaxHP;

			if (hp > _bossHPFollow)
			{ 
				_bossHPFollow = hp;
				return;
			}
		}

		private void FollowPlayerShield()
		{
			SetupShield();
		}

		private void FollowBossHP()
		{
			if (_mono.bossSlider.IsActive() == false)
				return;

			float weight = Mathf.Clamp01(_mono.bossSpeed * Time.deltaTime + _mono.bossFixedWeight);
			_bossHPFollow = weight * _bossHP + (1 - weight) * _bossHPFollow;

			var rectTF = _mono.bossSliderFollow?.GetComponent<RectTransform>();
			if (rectTF == null)
				return;
			var anchorMax = rectTF.anchorMax;
			anchorMax.x = Mathf.Clamp01(_bossHPFollow / _bossMaxHP);
			rectTF.anchorMax = anchorMax;
		}

		#endregion



		#region Event
		private void OnBossStartBattle()
		{
			var bossSlider = _mono.bossSlider;
			Utils.TrySetActive(bossSlider.gameObject, true);
			bossSlider.minValue = 0;
			bossSlider.maxValue = _bossMaxHP;
			//_bossMaxHP = _bossMaxHP;
			_bossHP = _bossMaxHP;
			SetupBossHP(_bossHP, _bossMaxHP);

			var bossText = _mono.bossText;
			Utils.TrySetActive(bossText.gameObject, true);
			bossText.text = "目前Boss模块还没有相应文档以及实现";

			var bossSliderGroup = bossSlider.GetComponentInParent<CanvasGroup>();
			bossSliderGroup.alpha = 0f;
			DOTween.To(() => bossSliderGroup.alpha, (x) => bossSliderGroup.alpha = x, 1, _mono.bossSliderShowTime);
		}

		private void OnBossEndBattle()
		{
			var bossText = _mono.bossText;
			var bossSlider = _mono.bossSlider;
			var bossSliderGroup = bossSlider.GetComponentInParent<CanvasGroup>();
			bossSliderGroup.alpha = 1f;
			var tween = DOTween.To(() => bossSliderGroup.alpha, (x) => bossSliderGroup.alpha = x, 0, _mono.bossSliderFadeTime);
			tween.OnComplete(
				()=> {
					Utils.TrySetActive(bossSlider.gameObject, false);
					Utils.TrySetActive(bossText.gameObject, false);
				});
		}


		private void OnBossHPChanged(float targetHP, float targetMaxHP)
		{
			UpdateBossHP(targetHP, targetMaxHP);
		}

		private void OnPlayerHPChanged(float targetHP, float targetMaxHP)
		{
			UpdatePlayerHP(targetHP, targetMaxHP);
		}

		private void OnPlayerShieldChanged(float shield, float shieldMax)
		{
			UpdatePlayerShieldHP(shield, shieldMax);
		}


		private void OnPickJinLeaf()
        {
			var amount = PlayerDataManager.Instance.foodCount;
			_mono.JinLeaf.text = amount.ToString();
        }

		private void OnPickYinLeaf()
        {
			var amount = PlayerDataManager.Instance.woodCount;
			_mono.YinLeaf.text = amount.ToString();
		}



		#endregion
	}
}