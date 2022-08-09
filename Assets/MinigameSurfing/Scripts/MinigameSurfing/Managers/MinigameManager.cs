using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActiveRagdoll;
using NoMono;
public class MinigameManager : MonoBehaviour
{

	public static MinigameManager Instance;

	private PersonBoatMananger _personBoatMananger;

	public GameObject[] floatingGoods;
	public float generate_radius;
	public float island_radius;
	public float generate_rate = 0.5f;
	public int generate_count = 30;
	public static int current_count = 0;
	private Transform _transform;
	private WaitForSeconds _wfs;
	public bool isGenerate = false;

	private void Awake()
	{
		Instance = this;
		AwakeSettings();
		_personBoatMananger = GetComponent<PersonBoatMananger>();
		_transform = transform;
		_wfs = new WaitForSeconds(generate_rate);
	}

	private void Start()
	{
		Debug.Log("Sta");
		StartAVGEngineOnce(Const.avgScriptTutorial);
		
		if (isGenerate)
		{
			StartCoroutine(GenerateFloatingGoods());
		}

		
	}

	public void ShieldChanged()
	{
		var shield = PlayerDataManager.Instance.shield;
		var shield_max = PlayerDataManager.Instance.shield_max;

		NoMono.Messenger.Broadcast((uint)NoMono.EventType.Player_ShieldChanged, shield, shield_max);
	}

	private void AwakeSettings()
	{
		Cursor.lockState = CursorLockMode.Confined;
		Application.targetFrameRate = 60;
	}

	private void StartAVGEngine(string scriptName)
	{
		IdlessChaye.IdleToolkit.AVGEngine.PachiGrimoire.I.StartAVGEngine(scriptName);
	}

	public void StartAVGEngineOnce(string scriptName)
	{
		if (Const.HasMark(scriptName) == false)
			StartAVGEngine(scriptName);
		Const.markList.Add(scriptName);
	}

	private GameObject FloatGoodsFactory(GameObject go)
	{
		var pos = generate_radius * Random.insideUnitCircle;
		while (Vector2.Distance(pos, Vector2.zero) < island_radius)
		{
			pos =  generate_radius * Random.insideUnitCircle;
		}

		var pos3 = new Vector3(pos.x, 0, pos.y);
		var rot = Quaternion.identity;
		var floatingGood = GameObject.Instantiate(go, pos3, rot, _transform);
		return floatingGood;
	}

	IEnumerator GenerateFloatingGoods()
	{
		while(true)
		{
			if (current_count < generate_count)
			{
				current_count++;
				var rand_index = Random.Range(0, floatingGoods.Length);
				var prefeb = floatingGoods[rand_index];
				var go = FloatGoodsFactory(prefeb);
			}
			yield return _wfs;
		}
	}


}
