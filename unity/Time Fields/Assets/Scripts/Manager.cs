using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour {
	public GameObject playerPrefab, asteroidPrefab, fieldPrefab, propelPrefab;

	const float updateRate = 33;
	const float radius = 2000;
	const float playerRotSpd = Mathf.PI / 1000 * updateRate;
	const float propelSpd = 1 * updateRate;

	public static float timeScale = 1;
	public static Obj player;
	public static List<Obj> asteroids = new List<Obj>();
	public static List<Obj> fields = new List<Obj>();

	// Use this for initialization
	void Start() {
		Time.fixedDeltaTime = updateRate / 1000;
		player = new Obj(Instantiate(playerPrefab) as GameObject, new Vector2(), new Vector2(), Mathf.PI / 2);
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButton(0)) player.rot = Mathf.Atan2(Input.mousePosition.y - Screen.height / 2, Input.mousePosition.x - Screen.width / 2);
		while (fields.Count < 20) {
			Vector2 p = RandInsideCircle(1000, 2000);
			Vector2 v = RandInsideCircle(0, 0.15f * updateRate);
			if (Random.value < 0.1) {
				fields.Add(new Obj(Instantiate(fieldPrefab) as GameObject, player.pos + p, player.dilatedVel + v, 0));
			} else {
				Obj asteroid = new Obj(Instantiate(asteroidPrefab) as GameObject, player.pos + p, player.dilatedVel + v, Random.value * Mathf.PI * 2);
				asteroid.velRot = Random.Range(-playerRotSpd * 2, playerRotSpd * 2);
				asteroids.Add(asteroid);
			}
		}
		player.Draw();
		for (int i = 0; i < asteroids.Count; i++) {
			asteroids[i].Draw();
			if (Vector2.Distance(asteroids[i].pos, player.pos) > radius) {
				Destroy(asteroids[i].go);
				asteroids.RemoveAt(i);
				i--;
			}
		}
		for (int i = 0; i < fields.Count; i++) {
			fields[i].Draw();
			if (Vector2.Distance(fields[i].pos, player.pos) > radius) {
				Destroy(fields[i].go);
				fields.RemoveAt(i);
				i--;
			}
		}
	}

	void FixedUpdate() {
		if (timeScale < updateRate / 1500) return;
		if (Input.GetMouseButton(0) && Random.value < 0.02f * updateRate) {
			asteroids.Add(new Obj(Instantiate(propelPrefab) as GameObject, player.pos,
				player.vel - propelSpd * new Vector2(Mathf.Cos(player.rot), Mathf.Sin(player.rot)) + Random.insideUnitCircle * propelSpd / 6, 0));
		}
		foreach (Obj field in fields) field.prevPos = field.pos;
		foreach (Obj field in fields) field.UpdatePos(0, 0);
		foreach (Obj asteroid in asteroids) asteroid.UpdatePos(0, asteroid.velRot);
		timeScale = player.UpdatePos(Input.GetMouseButton(0) ? 0.2f : 0, 0);
	}

	Vector2 RandInsideCircle(float min, float max) {
		Vector2 ret;
		do {
			ret = Random.insideUnitCircle * max;
		} while (ret.sqrMagnitude < min * min);
		return ret;
	}
}
