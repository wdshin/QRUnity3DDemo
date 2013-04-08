using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Antares.QRCode;

public class QRViewCode : MonoBehaviour {

	List<string> msgs = new List<string>();

	public Texture2D qrCodeImage;
	string url = "http://localhost:8000/cs_req_join_qr2?service_id=123";

    private int _width = 512;
    private int _height = 512;
    	
	// Use this for initialization
	IEnumerator Start ()
	{
		//Security.PrefetchSocketPolicy("www.differentmethods.com", 843);
		Application.RegisterLogCallback(Logger);
		//var d = gameObject.AddComponent<HTTP.DiskCache> ();
		//var r = new HTTP.Request ("GET", url);
		var www = gameObject.AddComponent<HTTP.SimpleWWW> ();
		HTTP.Request req=new HTTP.Request ("GET", url);
		req.AddHeader("Accept","application/json");
		www.Send(req, HandleResponse);
		
		yield return new WaitForSeconds(1);
		//DoItAnotherWay ();
	}
	
	void HandleResponse (HTTP.Response response)
	{
		//System.Text.UTF8Encoding.
		System.Text.Encoding enc = System.Text.Encoding.UTF8;//System.Text.Encoding.ASCII;
		string myString = enc.GetString(response.bytes);

		Debug.Log((string)myString);
		object o=MiniJSON.JsonDecode(myString);
		if ( o != null )
		{
			Hashtable t=(Hashtable)o;
			string code=(string)t["sc_res_join_qr2"];
			qrCodeImage = QRCodeProcessor.Encode((string)code, _width, _height);
			
		}
		//
		
		
		//var tex = new Texture2D (512, 512);
		//tex.LoadImage (response.bytes);
		//renderer.material.SetTexture ("_MainTex", tex);
	}	
	
	void Logger(string condition, string msg, LogType type) {
		msgs.Add(condition);
	}
	
	// Update is called once per frame
	void Update () {
        //transform.rotation *= Quaternion.Euler(0.1f, 0.2f, 0.3f);

        this.renderer.material.mainTexture = qrCodeImage;	
	}
}
