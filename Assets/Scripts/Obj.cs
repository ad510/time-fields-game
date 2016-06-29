using UnityEngine;
using System.Collections;

public class Obj : MonoBehaviour {
	public Vector2 pos, prevPos, vel, dilatedVel;
	public float rot, prevRot, velRot;
	public bool enable = true, immovable;

	public void UpdatePrevPos() {
		prevPos = pos;
		prevRot = rot;
	}

	public void UpdatePos() {
		Obj field = Manager.field.enable ? Manager.field : null;
		float mul = field == null ? 1 : Mathf.Clamp01(Vector2.Distance(pos, field.prevPos) / 500 + 0.2f);
		// update position and rotation
		dilatedVel = field == null || immovable ? vel : field.dilatedVel + (vel - field.dilatedVel) * mul;
		pos += dilatedVel;
		rot += velRot * mul;
		// update components
		Gun gun = GetComponent<Gun>();
		if (gun != null) gun.UpdateObj(mul);
	}

	void Update() {
		transform.position = pos / 100;
		transform.rotation = Quaternion.Euler(0, 0, rot * Mathf.Rad2Deg - 90);
		GetComponent<Renderer>().enabled = enable;
	}
}
