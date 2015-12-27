using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainCanvasBehavior : MonoBehaviour {

	public InputField payload;

	public void SendMessageButtonPressed (string messageGroup)
	{
		Dictionary<string, object> data = new Dictionary<string, object> ();

		if (!string.IsNullOrEmpty (payload.text)) {
			data.Add ("payload", payload.text);
		}

		NYSUMessaging.SendSystemMessage (messageGroup, NYSUMessaging.Messages.kToggleRenderer, (string.IsNullOrEmpty (payload.text)) ? null : data);
	}
}
