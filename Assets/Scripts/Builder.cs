using UnityEngine;
using System.Collections;

public class Builder : MonoBehaviour {

	public GameObject objToBuild;
	public GameObject cube;

	void Update() {
		if(Input.GetMouseButtonDown(0)) {
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
			RaycastHit hit;
			if (Physics.Raycast(transform.position, fwd, out hit, 10)) {
				GameObject newCube;
				Vector3 hitPointRelativity = hit.collider.transform.position - hit.point;
				Vector3 hitPointRelativityAbs = new Vector3(
					Mathf.Abs(hitPointRelativity.x),
					Mathf.Abs(hitPointRelativity.y),
					Mathf.Abs(hitPointRelativity.z)
				);

				if(hitPointRelativityAbs.x > hitPointRelativityAbs.y && hitPointRelativityAbs.x > hitPointRelativityAbs.z) {
					//X axis
					if(hitPointRelativity.x >= 0) {
						newCube = (GameObject)GameObject.Instantiate(cube, hit.collider.transform.position + (Vector3.left * cube.transform.localScale.x), Quaternion.identity);
					}
					else {
						newCube = (GameObject)GameObject.Instantiate(cube, hit.collider.transform.position + (Vector3.right * cube.transform.localScale.x), Quaternion.identity);
					}
				}
				else if(hitPointRelativityAbs.y > hitPointRelativityAbs.z) {
					//Y axis
					if(hitPointRelativity.y >= 0) {
						newCube = (GameObject)GameObject.Instantiate(cube, hit.collider.transform.position + (Vector3.down * cube.transform.localScale.y), Quaternion.identity);
					}
					else {
						newCube = (GameObject)GameObject.Instantiate(cube, hit.collider.transform.position + (Vector3.up * cube.transform.localScale.y), Quaternion.identity);
					}
				}
				else {
					if(hitPointRelativity.z >= 0) {
						newCube = (GameObject)GameObject.Instantiate(cube, hit.collider.transform.position + (Vector3.back * cube.transform.localScale.z), Quaternion.identity);
					}
					else {
						newCube = (GameObject)GameObject.Instantiate(cube, hit.collider.transform.position + (Vector3.forward * cube.transform.localScale.z), Quaternion.identity);
					}
				}

				newCube.transform.parent = objToBuild.transform;
			}
		}

		if(Input.GetKeyDown("o")) {
			Save ();
		}
	}

	void Save () {
		MeshFilter[] meshFilters = objToBuild.GetComponentsInChildren<MeshFilter>();
		
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.SetActive(false);
			i++;
			Debug.Log ("test");
		}
		objToBuild.transform.GetComponent<MeshFilter>().mesh = new Mesh();
		objToBuild.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

		MeshFilter mf = (MeshFilter)objToBuild.GetComponent("MeshFilter");
		Debug.Log(Application.dataPath + '/' + "object.obj");
		ObjExporter.MeshToFile(mf, Application.dataPath + '/' + "object.obj");
	}
}
