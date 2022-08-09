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
		NoMono.Messenger.Broadcast((uint)NoMono.EventType.Player_PickYinLeaf);
		Debug.Log("Pick");
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.rigidbody != null && collision.transform.tag.Equals(Const.tagWood))
			Collect(collision.rigidbody.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null && other.tag.Equals(Const.tagWood))
			Collect(other.attachedRigidbody.gameObject);
	}

}
