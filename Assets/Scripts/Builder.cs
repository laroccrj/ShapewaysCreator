using UnityEngine;
using System.Collections;
using OAuth;

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
		}
		objToBuild.transform.GetComponent<MeshFilter>().mesh = new Mesh();
		objToBuild.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

		MeshFilter mf = (MeshFilter)objToBuild.GetComponent("MeshFilter");
		Debug.Log(Application.dataPath + '/' + "object.obj");
		ObjExporter.MeshToFile(mf, Application.dataPath + '/' + "object.obj");
		string obj = ObjExporter.MeshToString(mf);
		StartCoroutine(Upload(obj));

	}

	IEnumerator Upload(string file) {
		Debug.Log("test1");
		Manager oauthManager = new Manager(
			"747d6d77f805eaca20f1535050eaa3568396b685",
			"c27d5555ecc20c1fd69d3a6338bc51c259d5ef11",
			"8d58f1c79911ae7196f5cd9c0f0bdbc686fec32b",
			"1e770408c0fdd5507b614b321fb8b1123bb9a9e7"
			);
		byte[] fileEncoded = System.Text.Encoding.UTF8.GetBytes(file);
		oauthManager._params["fileName"] = "Object Test";
		oauthManager._params["hasRightsToModel"] = "1";
		oauthManager._params["acceptTermsAndConditions"] = "1";
		oauthManager._params["file"] = System.Text.Encoding.UTF8.GetString(fileEncoded);
		string oauthHeaders = oauthManager.GenerateAuthzHeader("http://api.shapeways.com/models/v1" , "POST");

		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("fileName", "Object Test");
		wwwForm.AddField("hasRightsToModel", 1);
		wwwForm.AddField("acceptTermsAndConditions", 1);
		Debug.Log(file);
		wwwForm.AddBinaryData("file", fileEncoded);
		
		Hashtable headers = wwwForm.headers;
		headers["Authorization"] = oauthHeaders;
		Debug.Log("Keys");
		foreach (DictionaryEntry de in headers)
		{        
			string fieldName = de.Key as string;         
			Debug.Log(fieldName); 
		}
		Debug.Log (headers["Content-type"]);

		Debug.Log (System.Text.Encoding.UTF8.GetString(wwwForm.data));

		Debug.Log("Starting request");
		WWW www = new WWW("http://api.shapeways.com/models/v1", wwwForm.data, headers);
		yield return www;

		Debug.Log("Response is in");
		Debug.Log(www.text);

	}

	static byte[] GetBytes(string str)
	{
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	static string GetString(byte[] bytes)
	{
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}
}
