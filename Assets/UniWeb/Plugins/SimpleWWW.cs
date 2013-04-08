using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace HTTP {
	public class SimpleWWW : MonoBehaviour {
		
		public delegate void ResponseDelegate(HTTP.Response response);
		
		
		public void Send(Request request, ResponseDelegate responseDelegate) {
			StartCoroutine(_Send(request, responseDelegate));
		}
		
		IEnumerator _Send(Request request, ResponseDelegate responseDelegate) {
			request.Send();
			while(!request.isDone)
				yield return new WaitForEndOfFrame();
			if(request.exception != null) {
				Debug.LogError(request.exception);	
			} else {
				responseDelegate(request.response);
			}
		}
		
	}
}