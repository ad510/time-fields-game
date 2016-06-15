using UnityEngine;
using System.Collections;

public class Obj {
	public Vector2 pos, prevPos, vel, dilatedVel;
	public float rot, velRot;
	public GameObject go;

	public Obj(GameObject go, Vector2 pos, Vector2 vel, float rot) {
		this.go = go;
		this.pos = pos;
		this.vel = vel;
		this.dilatedVel = vel;
		this.rot = rot;
	}

	public float UpdatePos(float fwd, float velRot) {
		Obj field = null;
		float dist = 0, mul = 1;
		// update velocity
		vel.x += fwd * Mathf.Cos(rot);
		vel.y += fwd * Mathf.Sin(rot);
		// find closest field
		foreach (Obj f in Manager.fields) {
			if (f != this) {
				float d = Vector2.Distance(pos, f.prevPos);
				float m = Mathf.Clamp01(d / 500 + 0.2f); // possible to compute this after loop, or use a weighted average instead (sum up all delta v's won't work b/c 2 close fields would repel player)
				if (m < 1 && (field == null || d < dist)) {
					field = f;
					dist = d;
					mul = m;
				}
			}
		}
		// update position and rotation
		dilatedVel = field == null ? vel : field.dilatedVel + (vel - field.dilatedVel) * mul;
		pos += dilatedVel / Manager.timeScale;
		rot += velRot * mul / Manager.timeScale;
		if (rot < 0) rot += Mathf.PI * 2;
		if (rot > Mathf.PI * 2) rot -= Mathf.PI * 2;
		return mul;
	}

	public void Draw() {
		go.transform.position = (pos - Manager.player.pos) / 100;
		go.transform.rotation = Quaternion.Euler(0, 0, rot * Mathf.Rad2Deg - 90);
	}
}
