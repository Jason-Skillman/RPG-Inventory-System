using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	//Singleton
	public static CameraFollow main;

    public Transform target;
    [Range(0.01f, 1.0f)]
    public float speed = 0.1f;

    
    public void Awake() {
        DontDestroyOnLoad(gameObject);

		//Singleton
		if(!main)
			main = this;
		else
			Destroy(gameObject);
	}

    void Update() {
        if(target) {
			//Smooth camera movement
			this.transform.position = Vector3.Lerp(this.transform.position, target.position, speed) + new Vector3(0, 0, -10);
        }
    }
}
