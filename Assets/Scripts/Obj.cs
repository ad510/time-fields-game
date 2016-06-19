﻿using UnityEngine;
using System.Collections;

public class Obj {
	public Vector2 pos, prevPos, vel, dilatedVel;
	public float rot, prevRot, velRot;
	public bool alwaysOnscreen, immovable;
	public GameObject go;

	public Obj(Object go, Vector2 pos, Vector2 vel, float rot, float velRot = 0, bool alwaysOnscreen = false, bool immovable = false) {
		this.go = go as GameObject;
		this.pos = pos;
		this.vel = vel;
		this.dilatedVel = vel;
		this.rot = rot;
		this.velRot = velRot;
		this.alwaysOnscreen = alwaysOnscreen;
		this.immovable = immovable;
	}

	public void UpdatePrevPos() {
		prevPos = pos;
		prevRot = rot;
	}

	public float UpdatePos(float fwd) {
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
		dilatedVel = field == null || immovable ? vel : field.dilatedVel + (vel - field.dilatedVel) * mul;
		pos += dilatedVel / Manager.timeScale;
		rot += velRot * mul / Manager.timeScale;
		if (rot < 0) rot += Mathf.PI * 2;
		if (rot > Mathf.PI * 2) rot -= Mathf.PI * 2;
		return mul;
	}

	public void Draw() {
		go.transform.position = (pos - Manager.player.pos) / 100;
		if (alwaysOnscreen) {
			Vector3 extents = go.GetComponent<SpriteRenderer>().sprite.bounds.extents;
			float halfWidth = Camera.main.orthographicSize * Screen.width / Screen.height - extents.x;
			float halfHeight = Camera.main.orthographicSize - extents.y;
			if (Mathf.Abs(go.transform.position.x) > halfWidth) go.transform.position *= Mathf.Abs(halfWidth / go.transform.position.x);
			if (Mathf.Abs(go.transform.position.y) > halfHeight) go.transform.position *= Mathf.Abs(halfHeight / go.transform.position.y);
		}
		go.transform.rotation = Quaternion.Euler(0, 0, rot * Mathf.Rad2Deg - 90);
	}
}
