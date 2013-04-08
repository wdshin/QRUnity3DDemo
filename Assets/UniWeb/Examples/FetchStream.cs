using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FetchStream : MonoBehaviour
{
	
	string url = "http://nodes.differentmethods.com:1337/";

	IEnumerator Start ()
	{
#if UNITY_WEBPLAYER
		//this needs to talk to your policy server.
		Security.PrefetchSocketPolicy("www.differentmethods.com", 843);
#endif
		var r = new HTTP.Request ("GET", url);
		r.Send();
		
		while (true) {
			if(r.exception != null) { 	//some error occured.
				Debug.Log(r.exception.ToString());
				break;	
			}
			if(r.state != HTTP.RequestState.Waiting) { //there might be some chunks available
				var chunk = r.response.TakeChunk();
				if(chunk != null) { //a chunk is ready
					if(chunk.Length == 0) { //no more chunks will be coming (an empty chunk is the terminator)
						break;	
					}
					Debug.Log(System.Text.ASCIIEncoding.ASCII.GetString(chunk));	
				}
			}
			yield return new WaitForEndOfFrame ();
		}
		Debug.Log("Stream has finished");
		
		
	}
	
	
}
