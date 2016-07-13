using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour {
	public GameObject asteroidPrefab, clockPrefab, fieldPrefab, gunPrefab, gunNozzlePrefab, playerPrefab, propelPrefab, side1Prefab, side2Prefab;
	public UnityEngine.UI.Text message;

	public const float updateRate = 0.033f;
	public const float radius = 2000;
	public const float clockRotSpd = Mathf.PI * updateRate;

	public static Manager singleton;
	public static int level = 1;
	public static GUIStyle lblStyle;
	public static List<Obj> fields = new List<Obj>();
	public static bool waitForRelease;
	public static List<Obj> objs = new List<Obj>();

	// Use this for initialization
	void Start() {
		Time.fixedDeltaTime = updateRate;
		singleton = this;
		lblStyle = GUIStyle.none;
		lblStyle.fontSize = Screen.height / 10;
		lblStyle.normal.textColor = Color.white;
		for (int i = 0; i < 10; i++) {
			fields.Add(NewObj(fieldPrefab, new Vector2(), new Vector2(), 0));
			fields[fields.Count - 1].enable = false;
		}
		LoadLevel();
	}

	void LoadLevel() {
		message.text = "";
		waitForRelease = true;
		foreach (Obj field in fields) field.enable = false;
		foreach (Obj obj in objs) Destroy(obj.gameObject);
		objs.Clear();
		switch (level) {
		case 1:
			message.text = "Tap and hold to slow down a clock.\nSynchronize the clock hands.";
			objs.Add(NewObj(clockPrefab, new Vector2(-200, 0), new Vector2(), 0, clockRotSpd, true));
			objs.Add(NewObj(clockPrefab, new Vector2(200, 0), new Vector2(), Mathf.PI, clockRotSpd, true));
			break;
		case 2:
			message.text = "Get the green object to the blue object.";
			objs.Add(NewObj(gunPrefab, new Vector2(-400, 200), new Vector2(), 0, clockRotSpd, true));
			objs.Add(NewObj(gunPrefab, new Vector2(0, -200), new Vector2(), Mathf.PI / 2, clockRotSpd, true));
			objs.Add(NewObj(side2Prefab, new Vector2(400, 200), new Vector2(), 0, 0, true));
			objs.Add(NewObj(side1Prefab, objs[0].pos, new Vector2(600 * updateRate, 0), 0, 0));
			objs.Add(NewObj(asteroidPrefab, objs[1].pos, new Vector2(0, 600 * updateRate), 0, 0));
			objs[0].GetComponent<Gun>().shot = objs[3];
			objs[1].GetComponent<Gun>().shot = objs[4];
			break;
		case 3:
			objs.Add(NewObj(side1Prefab, new Vector2(-200, 0), new Vector2(), 0, clockRotSpd));
			objs.Add(NewObj(side2Prefab, new Vector2(200, 0), new Vector2(), 0, clockRotSpd, true));
			break;
		case 4:
			objs.Add(NewObj(gunPrefab, new Vector2(-200, 200), new Vector2(), 0, 0, true));
			objs[0].enable = false;
			objs.Add(NewObj(side2Prefab, new Vector2(200, 200), new Vector2(), 0, 0, true));
			objs.Add(NewObj(side1Prefab, objs[0].pos, new Vector2(100 * updateRate, 0), 0, clockRotSpd));
			break;
		case 5:
			objs.Add(NewObj(side1Prefab, new Vector2(-200, 0), new Vector2(), 0, clockRotSpd));
			objs.Add(NewObj(side2Prefab, new Vector2(200, 0), new Vector2(), 0, clockRotSpd));
			break;
		}
	}

	Obj NewObj(GameObject prefab, Vector2 pos, Vector2 vel, float rot, float velRot = 0, bool immovable = false) {
		GameObject go = Instantiate(prefab) as GameObject;
		Obj o = go.GetComponent<Obj>();
		o.pos = pos;
		o.vel = vel;
		o.dilatedVel = vel;
		o.rot = rot;
		o.velRot = velRot;
		o.immovable = immovable;
		return o;
	}

	void FixedUpdate() {
		if (level == 1 && Mathf.Abs(objs[0].rot % (Mathf.PI * 2) - objs[1].rot % (Mathf.PI * 2)) < 0.5
				|| level == 2 && objs[3].pos.x > objs[2].pos.x
				|| (level == 3 || level == 5) && Vector2.Distance(objs[0].pos, objs[1].pos) < 100
				|| level == 4 && objs[2].pos.x > objs[1].pos.x - 100) {
			level++;
			LoadLevel();
		}
		if (Input.GetMouseButton(0) && Input.touchCount == 0 && !waitForRelease) {
			Obj field = fields[0];
			field.prevPos = field.pos;
			field.pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) * 100;
			if (!field.enable) field.prevPos = field.pos;
			field.dilatedVel = field.vel = field.pos - field.prevPos;
			field.enable = true;
		} else {
			Obj field = fields[0];
			if (!Input.GetMouseButton(0)) waitForRelease = false;
			field.enable = false;
		}
		if (Input.touchCount == 0) waitForRelease = false;
		if (!waitForRelease) {
			int j = 0;
			for (int i = 0; i < Input.touchCount; i++) {
				Obj field = fields[j];
				if (Input.GetTouch(i).phase == TouchPhase.Ended) {
					field.enable = false;
					fields.Add(field);
					fields.RemoveAt(j);
				} else {
					field.prevPos = field.pos;
					field.pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position) * 100;
					if (Input.GetTouch(i).phase == TouchPhase.Began) field.prevPos = field.pos;
					field.dilatedVel = field.vel = field.pos - field.prevPos;
					field.enable = true;
					j++;
				}
			}
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
