using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour {
	public GameObject playerPrefab, asteroidPrefab, clockPrefab, fieldPrefab, propelPrefab;

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
		fields.Add(player);
		asteroids.Add(new Obj(Instantiate(clockPrefab) as GameObject, new Vector2(-200, 0), new Vector2(), 0));
		asteroids[0].velRot = playerRotSpd;
		asteroids[0].immovable = true;
		asteroids.Add(new Obj(Instantiate(clockPrefab) as GameObject, new Vector2(200, 0), new Vector2(), Mathf.PI));
		asteroids[1].velRot = playerRotSpd;
		asteroids[1].immovable = true;
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButton(0)) player.rot = Mathf.Atan2(Input.mousePosition.y - Screen.height / 2, Input.mousePosition.x - Screen.width / 2);
		player.Draw();
		foreach (Obj asteroid in asteroids) asteroid.Draw();
		foreach (Obj field in fields) field.Draw();
	}

	void FixedUpdate() {
		if (Mathf.Abs(asteroids[0].rot - asteroids[1].rot) < 0.1) return;
		if (Input.GetMouseButton(0) && Random.value < 0.02f * updateRate) {
			asteroids.Add(new Obj(Instantiate(propelPrefab) as GameObject, player.pos,
				player.vel - propelSpd * new Vector2(Mathf.Cos(player.rot), Mathf.Sin(player.rot)) + Random.insideUnitCircle * propelSpd / 6, 0));
		}
		foreach (Obj field in fields) field.prevPos = field.pos;
		foreach (Obj field in fields) if (field != player) field.UpdatePos(0);
		foreach (Obj asteroid in asteroids) asteroid.UpdatePos(0);
		timeScale = player.UpdatePos(Input.GetMouseButton(0) ? 0.2f : 0);
	}

	Vector2 RandInsideCircle(float min, float max) {
		Vector2 ret;
		do {
			ret = Random.insideUnitCircle * max;
		} while (ret.sqrMagnitude < min * min);
		return ret;
	}
}
