using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ImageBehavior : MonoBehaviour {

	public Text messageText;
	public string messageGroup;

	// The method called by NYSUMessaging
	private void ReceiveSystemMessage (Dictionary<string, object> data)
	{
		// Toggle this renderer
		if (data ["msg"].ToString () == NYSUMessaging.Messages.kToggleRenderer) {
			// Toggle the Renderer
			ToggleRenderer ();
		}

		if (data.ContainsKey ("payload")) {
			messageText.text = data ["payload"].ToString ();
		}
	}

	private void Start ()
	{
		NYSUMessaging.RegisterForBroadcastGroup (messageGroup, this.gameObject);
	}

	private void ToggleRenderer ()
	{
		this.GetComponent<Image> ().enabled = !this.GetComponent<Image> ().enabled;
	}
}
