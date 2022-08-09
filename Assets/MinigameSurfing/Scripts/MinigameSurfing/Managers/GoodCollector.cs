using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodCollector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void Collect(GameObject go)
	{
		MinigameManager.current_count--;
		Destroy(go);
		if (go.tag.Equals(Const.tagWood))
		{ 
			NoMono.Messenger.Broadcast((uint)NoMono.EventType.Player_PickYinLeaf);
		}
		else if (go.tag.Equals(Const.tagFood))
		{
			NoMono.Messenger.Broadcast((uint)NoMono.EventType.Player_PickJinLeaf);
			NoMono.PlayerDataManager.Instance.ShieldAdd(10f);
			MinigameManager.Instance.ShieldChanged();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.rigidbody != null)
			if (collision.transform.tag.Equals(Const.tagWood) || collision.transform.tag.Equals(Const.tagFood))
				Collect(collision.rigidbody.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
			if (other.tag.Equals(Const.tagWood) || other.tag.Equals(Const.tagFood))
				Collect(other.attachedRigidbody.gameObject);
	}

}
