using UnityEngine;
using System.Collections;

public class Builder : MonoBehaviour {

	public GameObject objToBuild;
	public GameObject objToPlace;
	public Texture2D crosshair;
	public int crosshairWidth;
	public int crosshairHeight;
	public Material previewMaterial;
	public GameObject previewTemplate;

	private GameObject previousTemplate;
	private bool previousShown = false;


	void OnGUI() {
		GUI.DrawTexture(new Rect((Screen.width - crosshairWidth) / 2, (Screen.height - crosshairHeight) /2, crosshairWidth, crosshairHeight), crosshair);
	}

	void Update() {
		previewTemplate.GetComponent<MeshFilter>().mesh = objToPlace.GetComponent<MeshFilter>().mesh;

		if(previousShown) {
			GameObject.Destroy(previousTemplate);
			previousShown = false;
		}

		bool mouseClicked = false;

		if(Input.GetMouseButtonDown(0)) {
			mouseClicked = true;
		}

		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		if (Physics.Raycast(transform.position, fwd, out hit, 10)) {
			Vector3 newPosition;
			Vector3 hitPointRelativity = hit.collider.transform.position - hit.point;
			Vector3 hitPointRelativityAbs = new Vector3(
				Mathf.Abs(hitPointRelativity.x),
				Mathf.Abs(hitPointRelativity.y),
				Mathf.Abs(hitPointRelativity.z)
			);

			if(hitPointRelativityAbs.x > hitPointRelativityAbs.y && hitPointRelativityAbs.x > hitPointRelativityAbs.z) {
				//X axis
				if(hitPointRelativity.x >= 0) {
					newPosition = hit.collider.transform.position + (Vector3.left * objToPlace.transform.localScale.x);
				}
				else {
					newPosition = hit.collider.transform.position + (Vector3.right * objToPlace.transform.localScale.x);
				}
			}
			else if(hitPointRelativityAbs.y > hitPointRelativityAbs.z) {
				//Y axis
				if(hitPointRelativity.y >= 0) {
					newPosition = hit.collider.transform.position + (Vector3.down * objToPlace.transform.localScale.y);
				}
				else {
					newPosition = hit.collider.transform.position + (Vector3.up * objToPlace.transform.localScale.y);
				}
			}
			else {
				if(hitPointRelativity.z >= 0) {
					newPosition = hit.collider.transform.position + (Vector3.back * objToPlace.transform.localScale.z);
				}
				else {
					newPosition = hit.collider.transform.position + (Vector3.forward * objToPlace.transform.localScale.z);
				}
			}

			if(mouseClicked) {
				GameObject newObj = (GameObject)GameObject.Instantiate(objToPlace, newPosition, Quaternion.identity);
				newObj.transform.parent = objToBuild.transform;
			}
			else {
				GameObject newObj = (GameObject)GameObject.Instantiate(previewTemplate, newPosition, Quaternion.identity);
				previousTemplate = newObj;
				previousShown = true;
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
