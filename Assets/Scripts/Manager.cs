using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour {
	public GameObject playerPrefab, asteroidPrefab, clockPrefab, fieldPrefab, propelPrefab;

	const float updateRate = 33;
	const float radius = 2000;
	const float clockRotSpd = Mathf.PI / 1000 * updateRate;
	const float propelSpd = 1 * updateRate;

	public static int level = 1;
	public static float timeScale = 1;
	public static Obj player;
	public static List<Obj> asteroids = new List<Obj>();
	public static List<Obj> fields = new List<Obj>();

	// Use this for initialization
	void Start() {
		Time.fixedDeltaTime = updateRate / 1000;
		LoadLevel();
	}

	void LoadLevel() {
		if (player != null) {
			Destroy(player.go);
			foreach (Obj asteroid in asteroids) Destroy(asteroid.go);
			foreach (Obj field in fields) Destroy(field.go);
			asteroids.Clear();
			fields.Clear();
		}
		player = new Obj(Instantiate(playerPrefab), new Vector2(), new Vector2(), Mathf.PI / 2, 0);
		fields.Add(player);
		switch (level) {
		case 1:
			asteroids.Add(new Obj(Instantiate(clockPrefab), new Vector2(-200, 0), new Vector2(), 0, clockRotSpd));
			asteroids[0].immovable = true;
			asteroids.Add(new Obj(Instantiate(clockPrefab), new Vector2(200, 0), new Vector2(), Mathf.PI, clockRotSpd));
			asteroids[1].immovable = true;
			break;
		case 2:
			asteroids.Add(new Obj(Instantiate(clockPrefab), new Vector2(200, 0), new Vector2(), 0, clockRotSpd));
			asteroids.Add(new Obj(Instantiate(clockPrefab), new Vector2(400, 0), new Vector2(), 0, clockRotSpd));
			break;
		}
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButton(0)) player.rot = Mathf.Atan2(Input.mousePosition.y - Screen.height / 2, Input.mousePosition.x - Screen.width / 2);
		player.Draw();
		foreach (Obj asteroid in asteroids) asteroid.Draw();
		foreach (Obj field in fields) field.Draw();
	}

	void FixedUpdate() {
		if (level == 1 && Mathf.Abs(asteroids[0].rot - asteroids[1].rot) < 0.1
				|| level == 2 && Vector2.Distance(asteroids[0].pos, asteroids[1].pos) < 100) {
			level++;
			LoadLevel();
		}
		if (Input.GetMouseButton(0) && Random.value < 0.02f * updateRate) {
			asteroids.Add(new Obj(Instantiate(propelPrefab), player.pos,
				player.vel - propelSpd * new Vector2(Mathf.Cos(player.rot), Mathf.Sin(player.rot)) + Random.insideUnitCircle * propelSpd / 6, 0, 0));
		}
		foreach (Obj field in fields) field.prevPos = field.pos;
		foreach (Obj field in fields) if (field != player) field.UpdatePos(0);
		foreach (Obj asteroid in asteroids) asteroid.UpdatePos(0);
		timeScale = player.UpdatePos(Input.GetMouseButton(0) ? 0.2f : 0);
	}
}
