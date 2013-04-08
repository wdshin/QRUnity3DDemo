using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using Ionic.Zlib;
using UnityEngine;

namespace HTTP
{
	public class Response
	{
		public int status = 200;
		public string message = "OK";
		public byte[] bytes;

		List<byte[]> chunks;
		Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> ();

		public string Text {
			get {
				if (bytes == null)
					return "";
				return System.Text.UTF8Encoding.UTF8.GetString (bytes);
			}
		}

		public AssetBundleCreateRequest Asset {
			get { return AssetBundle.CreateFromMemory (bytes); }
		}

		public void AddHeader (string name, string value)
		{
			GetHeaders (name).Add (value.Trim ());
		}

		public void SetHeader (string name, string value)
		{
			var h = GetHeaders (name);
			h.Clear ();
			h.Add (value.Trim ());
		}

		public List<string> GetHeaders (string name)
		{
			name = name.Trim ();
			foreach (var i in headers.Keys) {
				if (string.Compare (name, i, true) == 0)
					return headers[i];
			}
			var h = headers[name] = new List<string> ();
			return h;
		}

		public string GetHeader (string name)
		{
			var h = GetHeaders (name);
			if (h.Count == 0)
				return string.Empty;
			return h[h.Count - 1];
		}

		public Response ()
		{
			//ReadFromStream (stream);
		}

		string ReadLine (Stream stream)
		{
			var line = new List<byte> ();
			while (true) {
				byte c = (byte)stream.ReadByte ();
				if (c == Request.EOL[1])
					break;
				line.Add (c);
			}
			var s = ASCIIEncoding.ASCII.GetString (line.ToArray ()).Trim ();
			return s;
		}

		string[] ReadKeyValue (Stream stream)
		{
			string line = ReadLine (stream);
			if (line == "")
				return null;
			else {
				var split = line.IndexOf (':');
				if (split == -1)
					return null;
				var parts = new string[2];
				parts[0] = line.Substring (0, split).Trim ();
				parts[1] = line.Substring (split + 1).Trim ();
				return parts;
			}
			
		}

		public AssetBundleCreateRequest TakeAsset ()
		{
			var b = TakeChunk ();
			if (b == null)
				return null;
			return AssetBundle.CreateFromMemory (b);
		}

		public byte[] TakeChunk ()
		{
			byte[] b = null;
			lock (chunks) {
				if (chunks.Count > 0) {
					b = chunks[0];
					chunks.RemoveAt (0);
					return b;
				}
			}
			return b;
		}

		public void ReadFromStream (Stream inputStream)
		{
			//var inputStream = new BinaryReader(inputStream);
			var top = ReadLine (inputStream).Split (new char[] { ' ' });
			var output = new MemoryStream ();
			
			if (!int.TryParse (top[1], out status))
				throw new HTTPException ("Bad Status Code");
			
			message = string.Join (" ", top, 2, top.Length - 2);
			headers.Clear ();
			
			while (true) {
				// Collect Headers
				string[] parts = ReadKeyValue (inputStream);
				if (parts == null)
					break;
				AddHeader (parts[0], parts[1]);
			}
			
			if (string.Compare (GetHeader ("Transfer-Encoding"), "chunked", true) == 0) {
				chunks = new List<byte[]> ();
				while (true) {
					// Collect Body
					string hexLength = ReadLine (inputStream);
					//Console.WriteLine("HexLength:" + hexLength);
					if (hexLength == "0") {
						lock (chunks) {
							chunks.Add (new byte[] {  });
						}
						break;
					}
					int length = int.Parse (hexLength, NumberStyles.AllowHexSpecifier);
					for (int i = 0; i < length; i++)
						output.WriteByte ((byte)inputStream.ReadByte ());
					lock (chunks) {
						if (GetHeader ("Content-Encoding").ToLower ().Contains ("gzip"))
							chunks.Add (UnZip (output));
						else
							chunks.Add (output.ToArray ());
					}
					output.SetLength (0);
					//forget the CRLF.
					inputStream.ReadByte ();
					inputStream.ReadByte ();
				}
				
				while (true) {
					//Collect Trailers
					string[] parts = ReadKeyValue (inputStream);
					if (parts == null)
						break;
					AddHeader (parts[0], parts[1]);
				}
				var unchunked = new List<byte> ();
				foreach (var i in chunks) {
					unchunked.AddRange (i);
				}
				bytes = unchunked.ToArray ();
				
			} else {
				// Read Body
				int contentLength = 0;
				int.TryParse (GetHeader ("Content-Length"), out contentLength);
				for (int i = 0; i < contentLength; i++)
					output.WriteByte ((byte)inputStream.ReadByte ());
				
				if (GetHeader ("Content-Encoding").ToLower ().Contains ("gzip")) {
					bytes = UnZip (output);
				} else {
					bytes = output.ToArray ();
				}
			}
			
		}


		byte[] UnZip (MemoryStream output)
		{
			var cms = new MemoryStream ();
			output.Seek (0, SeekOrigin.Begin);
			using (var gz = new GZipStream (output, CompressionMode.Decompress)) {
				var buf = new byte[1024];
				int byteCount = 0;
				while ((byteCount = gz.Read (buf, 0, buf.Length)) > 0) {
					cms.Write (buf, 0, byteCount);
				}
			}
			return cms.ToArray ();
		}
		
	}
}

