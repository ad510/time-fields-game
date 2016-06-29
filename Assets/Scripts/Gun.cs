using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
	GameObject nozzle;
	public Obj shot;
	float rot;

	// Use this for initialization
	void Start() {
	}

	void OnDestroy() {
		Destroy(nozzle);
	}

	public void UpdateObj(float mul) {
		Obj o = GetComponent<Obj>();
		if (nozzle == null) {
			nozzle = Instantiate(Manager.singleton.gunNozzlePrefab) as GameObject;
			rot = o.prevRot;
		}
		if ((o.rot - rot + Mathf.PI * 2) % (Mathf.PI * 2) < (o.prevRot - rot + Mathf.PI * 2) % (Mathf.PI * 2)) shot.pos = o.pos;
	}
	
	// Update is called once per frame
	void Update() {
		if (nozzle != null) {
			nozzle.transform.position = transform.position + Vector3.forward;
			nozzle.transform.rotation = Quaternion.Euler(0, 0, rot * Mathf.Rad2Deg - 90);
		}
	}
}
