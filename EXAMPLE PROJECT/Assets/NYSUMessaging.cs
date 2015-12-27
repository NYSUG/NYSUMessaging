/******************
The MIT License (MIT)
Copyright (c) 2015 No, You Shut Up Inc.
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
********************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NYSUMessaging : MonoBehaviour {

	// Class to define all of the message groups
	public class MessageGroups {
		public const string kAll = "All";
	}

	public class Messages {
		public const string kToggleRenderer = "ToggleRenderer";
	}

	// Mapping of all objects to receive messages
	private static Dictionary<string, List<GameObject>> _messageMapping = new Dictionary<string, List<GameObject>> ();

#region Registration

	public static void RegisterForBroadcastGroup (List<string> broadcastGroupNames, GameObject go)
	{
		foreach (string broadcastGroupName in broadcastGroupNames) {
			RegisterForBroadcastGroup (broadcastGroupName, go);
		}
	}

	public static void RegisterForBroadcastGroup (string broadcastGroupName, GameObject go)
	{
		RegisterForBroadcastGroup (broadcastGroupName, go, true);
	}

	public static void RegisterForBroadcastGroup (string broadcastGroupName, GameObject go, bool forceCreate)
	{
		if (_messageMapping.ContainsKey (broadcastGroupName)) {

			// Add this GameObject to the list
			List<GameObject> gameObjects = _messageMapping [broadcastGroupName];
			gameObjects.Add (go);

		} else if (!_messageMapping.ContainsKey (broadcastGroupName) && forceCreate) {
			
			// Create the group and add it to the list
			List<GameObject> gos = new List<GameObject> ();
			gos.Add (go);

			_messageMapping.Add (broadcastGroupName, gos);

		} else {
			// Group doesn't exist, throw an error
			Debug.LogError (string.Format ("broadcastGroup {0} has not been created", broadcastGroupName));
			return;
		}
	}

	public static void UnRegisterFromBroadcastGroup (List<string> broadcastGroupNames, GameObject go)
	{
		foreach (string broadcastGroupName in broadcastGroupNames) {
			UnRegisterFromBroadcastGroup (broadcastGroupName, go);
		}
	}

	public static void UnRegisterFromBroadcastGroup (string broadcastGroupName, GameObject go)
	{
		if (!_messageMapping.ContainsKey (broadcastGroupName)) {
			Debug.LogError (string.Format ("broadcast group {0} does not exist", broadcastGroupName));
			return;
		}

		List<GameObject> gameObjects = _messageMapping [broadcastGroupName];
		gameObjects.Remove (go);
	}

#endregion

#region Broadcasting

	// Send a message to all GameObjects in the scene
	public static void SendSystemMessage (string broadcastGroupName, string msg, Dictionary<string, object> data)
	{
		// Since BroadcastMessage can only take one argument, add the message to the data object and
		// parse on the otherside
		if (data == null) {
			data = new Dictionary<string, object> ();
		}

		data.Add ("msg", msg);

		if (broadcastGroupName == MessageGroups.kAll) {

			// Send the message to the entire scene graph
			GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
			foreach (GameObject go in gos) {
				go.gameObject.SendMessage ("ReceiveSystemMessage", data, SendMessageOptions.DontRequireReceiver);
			}
		} else if (_messageMapping.ContainsKey (broadcastGroupName)) {

			// Sanitize the list
			DoGarbageCollection (broadcastGroupName);

			// Send to this broadcast group
			List<GameObject> gameObjects = _messageMapping [broadcastGroupName];
			foreach (GameObject go in gameObjects) {
				go.gameObject.SendMessage ("ReceiveSystemMessage", data, SendMessageOptions.DontRequireReceiver);
			}

		} else {
			// Error
			Debug.LogError (string.Format ("No broadcast group named {0}", broadcastGroupName));
			return;
		}
	}

	// Send a message to multiple broadcast groups
	public static void SendSystemMessage (List<string> broadcastGroupNames, string msg, Dictionary<string, object> data)
	{
		foreach (string broadcastGroupName in broadcastGroupNames) {
			SendSystemMessage (broadcastGroupName, msg, data);
		}
	}

#endregion

#region Cleanup/Pruning

	private static void DoGarbageCollection ()
	{
		// Clean up all of the broadcast group lists
		foreach (KeyValuePair<string, List<GameObject>> entry in _messageMapping) {
			List<GameObject> gameObjects = entry.Value;
			gameObjects.RemoveAll ((o) => o == null);
		}
	}

	private static void DoGarbageCollection (string broadcastGroupName)
	{
		if (broadcastGroupName == MessageGroups.kAll) {
			DoGarbageCollection ();
			return;
		}

		List<GameObject> gameObjects = _messageMapping [broadcastGroupName];
		gameObjects.RemoveAll ((o) => o == null);
	}

#endregion

}
