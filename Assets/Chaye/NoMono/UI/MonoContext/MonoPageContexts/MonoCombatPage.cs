using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace NoMono
{

	public class MonoCombatPage : BaseMonoPageContext
	{
		
		[SerializeField]
		private Image _image;

		public Image combatImage { get { return _image; } }

		[SerializeField]
		private float _whiteBossSpeed = 1f;
		[SerializeField]
		private float _whiteBossFixedWeight = 0.05f;
		[SerializeField]
		private float _whiteHPSpeed = 1f;
		[SerializeField]
		private float _whiteHPFixedWeight = 0.05f;

		public float indicatorShowTime = 0.2f;
		public float indicatorFadeTime = 0.2f;
		public float bossSliderShowTime = 0.2f;
		public float bossSliderFadeTime = 0.2f;

		public float bossSpeed { get { return _whiteBossSpeed; } }
		public float bossFixedWeight { get { return _whiteBossFixedWeight; } }
		public float hpSpeed { get { return _whiteHPSpeed; } }
		public float hpFixedWeight { get { return _whiteHPFixedWeight; } }


		// 下面的条子
		public Image bottomBar;
		public Image swordIcon;
		
		public Slider shieldSlider;
		public Text shieldText;
		public Slider hpSlider;
		public GameObject hpSliderFollow;
		public Text hpText;
		
		public Slider bossSlider;
		public GameObject bossSliderFollow;
		public GameObject bossTextGO;
		public Text bossText;

		public CanvasGroup OkaneCanvas;
		public Text JinLeaf;
		public Text YinLeaf;


		private Color minWaitCol = new Color(.6f, .6f, .6f, 1.0f);

	}
}