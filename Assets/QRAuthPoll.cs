using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QRAuthPoll : MonoBehaviour {

	public string url="http://localhost:8000/cs_poll_auth2";
	List<string> msgs = new List<string>();
	
	public string id;
	public string password;
	
	public bool polling=false;
	
	public string qrCode;
	
	public GameObject goMsg;
	
	public GameObject goThisPanel;
	public GameObject goNextPanel;
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void StartPoll()
	{
		polling=true;
		Debug.Log("QRAuthPoll.StartPoll");
		StartCoroutine(Poll());
	}
	
	public void StopPoll()
	{
		polling=false;
	}	
	
//	public void OnSubmitLogin()
//	{
//		Debug.Log("OnSubmitLogin");
//		StartCoroutine(Poll());
//	}
	
	public IEnumerator Poll()
	{
		Debug.Log("SUBMIT");
		//Security.PrefetchSocketPolicy("www.differentmethods.com", 843);
		Application.RegisterLogCallback(Logger);
		//var d = gameObject.AddComponent<HTTP.DiskCache> ();
		//var r = new HTTP.Request ("GET", url);
		var www = gameObject.AddComponent<HTTP.SimpleWWW> ();
		string URL=url+"?qr_code="+qrCode;
		Debug.Log(URL);
		HTTP.Request req=new HTTP.Request ("GET", URL);
		req.AddHeader("Accept","application/json");
		www.Send(req, HandleResponse);
		
		//yield return new WaitForSeconds(1);
		yield return new WaitForSeconds(60);
		
		if ( polling ) Poll();
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
			string code=(string)t["sc_poll_auth2"];
//			//qrCodeImage = QRCodeProcessor.Encode((string)code, _width, _height);
			if ( code == "success" )
			{
				Debug.Log("OK AUTHED");	
				
				if ( goMsg )
				{
					UILabel ui=goMsg.GetComponent<UILabel>();
					if ( ui )
					{
						ui.text="LOGIN OK";
					}
				}
				
				if ( goNextPanel )
				{
					goNextPanel.SetActiveRecursively(true);
				}
				if ( goThisPanel )
				{
					goThisPanel.SetActiveRecursively(false);
				}												
			}
			else
			{
					QRViewCodeAuth s=GetComponent<QRViewCodeAuth>();
					if ( s )
					{
						s.Request();
					}				
			}
		}
		//
		
		
		//var tex = new Texture2D (512, 512);
		//tex.LoadImage (response.bytes);
		//renderer.material.SetTexture ("_MainTex", tex);
	}	
	
	void Logger(string condition, string msg, LogType type) {
		msgs.Add(condition);
	}
}
