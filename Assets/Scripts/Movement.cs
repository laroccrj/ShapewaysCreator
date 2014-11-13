using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public int speed;

	void Update () {
		if(Input.GetKey("w")) {
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
		}

		if(Input.GetKey("a")) {
			transform.Translate(Vector3.left * Time.deltaTime * speed);
		}

		if(Input.GetKey("s")) {
			transform.Translate(Vector3.back * Time.deltaTime * speed);
		}

		if(Input.GetKey("d")) {
			transform.Translate(Vector3.right * Time.deltaTime * speed);
		}

		if(Input.GetKey(KeyCode.Space)) {
			transform.Translate(Vector3.up * Time.deltaTime * speed);
		}

		if(Input.GetKey(KeyCode.LeftControl)) {
			transform.Translate(Vector3.down * Time.deltaTime * speed);
		}
	}

}
