using UnityEngine;
using System.Collections;

public class Obj : MonoBehaviour {
	public Vector2 pos, prevPos, vel, dilatedVel;
	public float rot, prevRot, velRot;
	public bool enable = true, immovable;

	void Start() {
		if (immovable) {
			LineRenderer line = gameObject.AddComponent<LineRenderer>();
			line.material.shader = Shader.Find("Sprites/Default");
			line.material.color = Color.gray;
			line.SetWidth(0.5f, 0.5f);
			line.SetVertexCount(2);
			line.SetPosition(0, Vector3.forward * 2 + (Vector3)pos / 100);
			line.SetPosition(1, Vector3.forward * 2 + (Vector3)pos);
		}
	}

	public void UpdatePrevPos() {
		prevPos = pos;
		prevRot = rot;
	}

	public void UpdatePos() {
		Obj field = null;
		float dist = 0, mul = 1;
		// find closest field
		foreach (Obj f in Manager.fields) {
			if (f.enable) {
				float d = Vector2.Distance(pos, f.prevPos);
				float m = Mathf.Clamp01(d / 500 + 0.4f); // possible to compute this after loop, or use a weighted average instead (sum up all delta v's won't work b/c 2 close fields would repel player)
				if (m < 1 && (field == null || d < dist)) {
					field = f;
					dist = d;
					mul = m;
				}
			}
		}
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
