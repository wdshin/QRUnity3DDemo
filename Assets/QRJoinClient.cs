using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QRJoinClient : MonoBehaviour {

	public string url="http://localhost/join_service";
	List<string> msgs = new List<string>();
	
	public string id;
	public string password;
	
	public GameObject goPanelAuth;
	public GameObject goPanelJoin;
	
	public GameObject goPanelJoinQR;
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
	
	public void OnBack()
	{
		if ( goPanelJoin ) goPanelJoin.SetActiveRecursively(false);		
		if ( goPanelAuth ) goPanelAuth.SetActiveRecursively(true);
	}
	
	public void OnSubmitJoin()
	{
		Debug.Log("QRJoinClient.OnSubmitJoin");
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
				
		Debug.Log("SUBMIT");
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
			string code=(string)t["sc_ack_join_service"];
			//qrCodeImage = QRCodeProcessor.Encode((string)code, _width, _height);
			if ( code == "already_joined" )
			{
				if ( goMsg )
				{
					UILabel ui=goMsg.GetComponent<UILabel>();
					if ( ui )
					{
						ui.text="Already Joined";
					}
				}
				
//				if ( goPanelAuth ) goPanelAuth.SetActiveRecursively(false);
//				if ( goViewQRAuth ) 
//				{
//					goViewQRAuth.SetActiveRecursively(true);
//					QRViewCodeJoin s=goViewQRJoin.GetComponent<QRViewCodeJoin>();
//					if ( s )
//					{
//						s.id=id;
//					}					
//				}
			}
			else if ( code == "joined_ok" )
			{
				Debug.Log("JOINED_OK");
				if ( goMsg )
				{
					UILabel ui=goMsg.GetComponent<UILabel>();
					if ( ui )
					{
						ui.text="Joined Successfully!";
					}
				}
				
				if ( goPanelAuth ) goPanelAuth.SetActiveRecursively(false);
				if ( goPanelJoin ) goPanelJoin.SetActiveRecursively(false);
				
				if ( goPanelJoinQR )
				{
					goPanelJoinQR.SetActiveRecursively(true);
					Debug.Log("JOINED_OK 2");
					
					if ( goViewQRJoin )
					{
						goViewQRJoin.SetActiveRecursively(true);	
						QRViewCodeJoin s=goViewQRJoin.GetComponent<QRViewCodeJoin>();
						if ( s )
						{
							Debug.Log("JOINED_OK 3");
							if ( goID )
							{
								Debug.Log("JOINED_OK 4");
								UIInput ui=goID.GetComponent<UIInput>();
								if ( ui )
								{
									id=ui.text;
									Debug.Log("goViewQRJoin : id="+id);
								}
							}
									
							s.id=id;
						}
					}
				}				
				//if ( goPanelAuth ) goPanelAuth.SetActiveRecursively(true);
				//if ( goPanelJoin ) goPanelJoin.SetActiveRecursively(false);
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
