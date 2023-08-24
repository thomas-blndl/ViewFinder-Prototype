using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    // Declare and initialize a new List of GameObjects called currentCollisions.
	public static List<GameObject> collidingObjects = new List<GameObject>();
	
	private void OnTriggerEnter (Collider col) {
        if(col.tag == "Player" || col.gameObject.layer == 8) return;

		collidingObjects.Add(col.gameObject);

        // if (col.gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
        // {
        //     mr.material.color = Color.green;
        // }

	}

	private void OnTriggerExit (Collider col) {

		collidingObjects.Remove (col.gameObject);
        // if (col.gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
        // {
        //     mr.material.color = Color.red;
        // }
	}
}
