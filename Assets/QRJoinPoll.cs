using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QRJoinPoll : MonoBehaviour {

	public string url="http://localhost:8000/cs_poll_join2";
	List<string> msgs = new List<string>();
	
	public string id;
	public string password;
	
	public bool polling=false;
	
	public string qrCode;
	
	//public GameObject goViewQRJoin;
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
		Debug.Log("OnSubmitJoin");
		StartCoroutine(Poll());
	}
	
	public void StopPoll()
	{
		polling=false;
	}
	
	public IEnumerator Poll()
	{
		Debug.Log("Polling");
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
		
		yield return new WaitForSeconds(60);
		
		if ( polling ) Poll();
		//DoItAnotherWay ();
	}
	
	public IEnumerator CheckJoinQR2()
	{
		Debug.Log("Polling CheckJoinQR2");
		//Security.PrefetchSocketPolicy("www.differentmethods.com", 843);
		Application.RegisterLogCallback(Logger);
		//var d = gameObject.AddComponent<HTTP.DiskCache> ();
		//var r = new HTTP.Request ("GET", url);
		var www = gameObject.AddComponent<HTTP.SimpleWWW> ();
		string URL="http://localhost:7000/check_join_qr2?qr="+qrCode+"&username="+id;
		Debug.Log(URL);
		HTTP.Request req=new HTTP.Request ("GET", URL);
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
			object o2=t["sc_poll_join2"];
			
			if ( o2 != null )
			{
				string code=(string)o2;
				//			//qrCodeImage = QRCodeProcessor.Encode((string)code, _width, _height);
				if ( code == "success" )
				{
					Debug.Log("OK JOINED");
					
					//location.replace("http://localhost:8080/check_join_qr2?QR="+join_qr);
					polling=false;
					StartCoroutine(CheckJoinQR2());
					
					// OK joined
				}
				else 
				{
					// failed to join
					//if ( goViewQRJoin ) 
					//{
						//goViewQRJoin.SetActiveRecursively(true);
						QRViewCodeJoin s=GetComponent<QRViewCodeJoin>();
						if ( s )
						{
							s.Request();
						}
					//}
					
				}
			}
			
			object o3=t["sc_check_join_qr2"];
			if ( o3 != null )
			{
				Debug.Log("CHECK JOIN QR2 results");
				string code=(string)o3;
				if ( code == "ok" )
				{
					if ( goMsg )
					{
						UILabel ui=goMsg.GetComponent<UILabel>();
						if ( ui )
						{
							ui.text="JOINED OK";
						}
					}
					//yield return new WaitForSeconds(1);			
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
					if ( goMsg )
					{
						UILabel ui=goMsg.GetComponent<UILabel>();
						if ( ui )
						{
							ui.text="FAILED TO JOIN";
						}
					}					
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
