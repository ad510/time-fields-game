﻿using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour {
	public GameObject asteroidPrefab, clockPrefab, fieldPrefab, gunPrefab, gunNozzlePrefab, playerPrefab, propelPrefab, side1Prefab, side2Prefab;

	public const float updateRate = 0.033f;
	public const float radius = 2000;
	public const float clockRotSpd = Mathf.PI * updateRate;

	public static Manager singleton;
	public static int level = 1;
	public static Obj field;
	public static List<Obj> objs = new List<Obj>();

	// Use this for initialization
	void Start() {
		Time.fixedDeltaTime = updateRate;
		singleton = this;
		field = AddObj(fieldPrefab, new Vector2(), new Vector2(), 0);
		field.enable = false;
		objs.Remove(field);
		LoadLevel();
	}

	void LoadLevel() {
		foreach (Obj obj in objs) Destroy(obj.gameObject);
		objs.Clear();
		switch (level) {
		case 1:
			AddObj(clockPrefab, new Vector2(-200, 0), new Vector2(), 0, clockRotSpd, true);
			AddObj(clockPrefab, new Vector2(200, 0), new Vector2(), Mathf.PI, clockRotSpd, true);
			break;
		case 2:
			AddObj(gunPrefab, new Vector2(0, 200), new Vector2(), 0, clockRotSpd, true);
			AddObj(gunPrefab, new Vector2(200, 0), new Vector2(), Mathf.PI / 2, clockRotSpd, true);
			AddObj(side2Prefab, new Vector2(400, 200), new Vector2(), 0, 0, true);
			AddObj(side1Prefab, objs[0].pos, new Vector2(600 * updateRate, 0), 0, 0);
			AddObj(asteroidPrefab, objs[1].pos, new Vector2(0, 600 * updateRate), 0, 0);
			objs[0].GetComponent<Gun>().shot = objs[3];
			objs[1].GetComponent<Gun>().shot = objs[4];
			break;
		case 3:
			AddObj(side1Prefab, new Vector2(200, 0), new Vector2(), 0, clockRotSpd);
			AddObj(side2Prefab, new Vector2(400, 0), new Vector2(), 0, clockRotSpd);
			break;
		case 4:
			AddObj(gunPrefab, new Vector2(-200, 200), new Vector2(), 0, 0, true);
			AddObj(side2Prefab, new Vector2(200, 200), new Vector2(), 0, 0, true);
			AddObj(side1Prefab, objs[0].pos, new Vector2(100 * updateRate, 0), 0, clockRotSpd);
			break;
		}
	}

	Obj AddObj(GameObject prefab, Vector2 pos, Vector2 vel, float rot, float velRot = 0, bool immovable = false) {
		GameObject go = Instantiate(prefab) as GameObject;
		Obj o = go.GetComponent<Obj>();
		o.pos = pos;
		o.vel = vel;
		o.dilatedVel = vel;
		o.rot = rot;
		o.velRot = velRot;
		o.immovable = immovable;
		objs.Add(o);
		return o;
	}

	void FixedUpdate() {
		if (level == 1 && Mathf.Abs(objs[0].rot % (Mathf.PI * 2) - objs[1].rot % (Mathf.PI * 2)) < 0.1
				|| level == 2 && objs[3].pos.x > objs[2].pos.x
				|| level == 3 && Vector2.Distance(objs[0].pos, objs[1].pos) < 100
				|| level == 4 && objs[2].pos.x > objs[1].pos.x) {
			level++;
			LoadLevel();
		}
		if (Input.GetMouseButton(0)) {
			field.prevPos = field.pos;
			field.pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) * 100;
			if (!field.enable) field.prevPos = field.pos;
			field.dilatedVel = field.vel = field.pos - field.prevPos;
			field.enable = true;
		} else {
			field.enable = false;
		}
		foreach (Obj obj in objs) obj.UpdatePrevPos();
		foreach (Obj obj in objs) obj.UpdatePos();
		if (level == 2) {
			if (Vector2.Distance(objs[3].pos, objs[4].pos) < 50) {
				objs[3].pos.x = -10000;
				objs[4].pos.x = -20000;
			}
		}
		if (level == 4 && objs[2].rot % (Mathf.PI * 2) < objs[2].prevRot % (Mathf.PI * 2)) objs[2].pos = objs[0].pos;
	}
}
