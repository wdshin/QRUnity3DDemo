using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QRAuthClient : MonoBehaviour {

	public string url="http://localhost/join_service";
	List<string> msgs = new List<string>();
	
	public string id;
	public string password;
	
	public GameObject goPanelAuth;
	public GameObject goPanelJoin;
	public GameObject goViewQRAuth;
	public GameObject goViewQRJoin;

	public GameObject goID;
	public GameObject goPassword;
	
	public GameObject goMsg;
			
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnEnable()
	{
		if ( goMsg )
		{
			UILabel ui=goMsg.GetComponent<UILabel>();
			if ( ui )
			{
				ui.text="ENTER USER NAME AND PASSWORD";
			}
		}		
	}
	
	public void OnSubmitJoin()
	{
		Debug.Log("QRAuthClient.OnSubmitJoin");
		if ( goPanelAuth ) goPanelAuth.SetActiveRecursively(false);
		if ( goPanelJoin ) goPanelJoin.SetActiveRecursively(true);
		//if ( goViewQRJoin ) goViewQRJoin.SetActiveRecursively(true);
	}	
	
	public void OnSubmitLogin()
	{
		Debug.Log("QRAuthClient.OnSubmitLogin");
		StartCoroutine(Submit());
	}
	
	public IEnumerator Submit()
	{
		if ( goID )
		{
			UIInput ui=goID.GetComponent<UIInput>();
			if ( ui )
			{
				id=ui.text;
			}
		}
		if ( goPassword )
		{
			UIInput ui=goPassword.GetComponent<UIInput>();
			if ( ui )
			{
				password=ui.text;
			}
		}
				
		Debug.Log("QRAuthClient.SUBMIT");
		//Security.PrefetchSocketPolicy("www.differentmethods.com", 843);
		Application.RegisterLogCallback(Logger);
		//var d = gameObject.AddComponent<HTTP.DiskCache> ();
		//var r = new HTTP.Request ("GET", url);
		var www = gameObject.AddComponent<HTTP.SimpleWWW> ();
		string URL=url+"?username="+id+"&password="+password;
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
			string code=(string)t["sc_ack_login"];
			//qrCodeImage = QRCodeProcessor.Encode((string)code, _width, _height);
			if ( code == "joined_qr2" )
			{
				// VIEW QRVIEWCODE
				if ( goPanelAuth ) goPanelAuth.SetActiveRecursively(false);
				if ( goViewQRAuth ) goViewQRAuth.SetActiveRecursively(true);
				
				
			}
			else if ( code == "not_joined_qr2" )
			{
				if ( goPanelAuth ) goPanelAuth.SetActiveRecursively(false);
				
				// 2차 인증 가입페이지로 유도
				
				
				//if ( goViewQRJoin ) goViewQRJoin.SetActiveRecursively(true);
				// LOGIN OK
				
				
			}
			else if ( code == "incorrect" )
			{
				if ( goMsg )
				{
					UILabel ui=goMsg.GetComponent<UILabel>();
					if ( ui )
					{
						ui.text="INCORRECT USERNAME OR PASSWORD";
					}
				}
				// DO NOTHING
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
