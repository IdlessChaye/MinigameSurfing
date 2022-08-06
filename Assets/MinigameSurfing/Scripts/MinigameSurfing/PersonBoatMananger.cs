using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBoatMananger : MonoBehaviour
{
	private static PersonBoatMananger _instance;
	public static PersonBoatMananger Instance => _instance;

	public PersonBoatStatus PersonBoatStatus => _personBoatStatus;


	private BoatController _boatController;
	private ActiveRagdoll.ActiveRagdoll _activeRagdoll;

	private PersonBoatStatus _personBoatStatus = PersonBoatStatus.PersonWalk;

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		_boatController = BoatController.Instance;
		_activeRagdoll = ActiveRagdoll.ActiveRagdoll.Instance;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			if (_personBoatStatus == PersonBoatStatus.PersonWalk)
			{
				SetPersonBoatStatus(PersonBoatStatus.PersonSurfing);
			}
			else if (_personBoatStatus == PersonBoatStatus.PersonSurfing)
			{
				SetPersonBoatStatus(PersonBoatStatus.PersonWalk);
			}
		}

		// 人拉滑板沉入海 就滑板
		if (IsBoatInWater() && ActiveRagdoll.Gripper.IsHoldBoat && _personBoatStatus == PersonBoatStatus.PersonWalk)
		{
			_activeRagdoll.GripModule.UnGrip();
			SetPersonBoatStatus(PersonBoatStatus.PersonSurfing);
		}

	}

	private bool IsBoatInWater()
	{
		return _boatController.buoyancy.isInWater;
	}

	public void SetPersonBoatStatus(PersonBoatStatus status)
	{
		_personBoatStatus = status;
		//BroadcastMessage
		switch (_personBoatStatus)
		{
			case PersonBoatStatus.PersonWalk:
				_activeRagdoll.AnimationModule.SetRigidbodyIsKe(false);
				_boatController.SetIsMotoring(false);
				_activeRagdoll.CameraModule.PrepareCamera();
				ResetPosAndRotOfPlayerAndBoat();
				break;
			case PersonBoatStatus.PersonSurfing:
				_activeRagdoll.AnimationModule.SetRigidbodyIsKe(true);
				_boatController.SetIsMotoring(true);
				_activeRagdoll.CameraModule.PrepareCamera();
				break;
		}
	}

	private void ResetPosAndRotOfPlayerAndBoat()
	{
		_activeRagdoll.ResetStatus();
		_boatController.ResetStatus();
	}

	private void LateUpdate()
	{
		if (PersonBoatMananger.Instance.PersonBoatStatus == PersonBoatStatus.PersonSurfing)
		{
			
		}
	}
}

public enum PersonBoatStatus
{
	PersonWalk,
	PersonSurfing,
	None,
}