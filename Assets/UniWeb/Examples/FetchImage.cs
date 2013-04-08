using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FetchImage : MonoBehaviour
{
	List<string> msgs = new List<string>();
	string url = "http://www.differentmethods.com/wp-content/uploads/2011/05/react.jpg";

	IEnumerator Start ()
	{
		//Security.PrefetchSocketPolicy("www.differentmethods.com", 843);
		Application.RegisterLogCallback(Logger);
		var d = gameObject.AddComponent<HTTP.DiskCache> ();
		var r = new HTTP.Request ("GET", url);
		var h = d.Fetch (r);
		while (!h.isDone)
			yield return new WaitForEndOfFrame ();
		if (h.request.exception != null) {
			Debug.LogError (h.request.exception);
			
		} else {
			var tex = new Texture2D (512, 512);
			tex.LoadImage (h.request.response.bytes);
			renderer.material.SetTexture ("_MainTex", tex);
		}
		yield return new WaitForSeconds(1);
		DoItAnotherWay ();
	}
	
	void Logger(string condition, string msg, LogType type) {
		msgs.Add(condition);
	}

	void DoItAnotherWay ()
	{
		url = "http://unity3d.com/support/documentation/Images/manual/Platform%20Dependent%20Compilation-0.jpg";
		var www = gameObject.AddComponent<HTTP.SimpleWWW> ();
		www.Send(new HTTP.Request ("GET", url), HandleResponse);
	}

	void HandleResponse (HTTP.Response response)
	{
		var tex = new Texture2D (512, 512);
		tex.LoadImage (response.bytes);
		renderer.material.SetTexture ("_MainTex", tex);
	}
	
	void OnGUI() {
		GUILayout.BeginVertical();
		foreach(var i in msgs) 
			GUILayout.Label(i);
		GUILayout.EndVertical();
	}
}
