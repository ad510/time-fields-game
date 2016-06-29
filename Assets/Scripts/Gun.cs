using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
	public Obj shot;
	public float interval = 2, reload = 0;

	// Use this for initialization
	void Start() {
	
	}

	public void UpdateObj(float mul) {
		reload += Manager.updateRate * mul;
		if (reload >= interval) {
			reload -= interval;
			shot.pos = GetComponent<Obj>().pos;
		}
	}
	
	// Update is called once per frame
	void Update() {
		// transparency: http://answers.unity3d.com/questions/684997/change-transparency-of-a-sprite.html
	}
}
