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
			Debug.Log(_personBoatStatus.ToString());
		}
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
				break;
			case PersonBoatStatus.PersonSurfing:
				_activeRagdoll.AnimationModule.SetRigidbodyIsKe(true);
				_boatController.SetIsMotoring(true);
				break;
		}
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