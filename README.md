### NYSUMessaging
A utility for using a message passing based system in Unity3D. This utility was developed for my game Atomic Space Command. 
Check it out: http://atomicspacecommand.net

## Purpose
This utility is useful for keeping your reference count as low as possible while being able to transport events and data 
across the game without having to traverse the entire scene graph. This is losely based on the methodology for Apple's GCD

## Usage
Game Objects can register or deregister for message groups using the following command:

    NYSUMessaging.RegisterForBroadcastGroup ("MainMenuUI", this.gameObject);

One an object has been registered add a method called ReceiveSystemMessage like this:

    private void ReceiveSystemMessage (Dictionary<string, object> data)
	{
		// Toggle this renderer
		if (data ["msg"].ToString () == NYSUMessaging.Messages.kToggleRenderer) {
			// Toggle the Renderer
			ToggleRenderer ();
		}
	}

You can send messages and data to groups using the SendSystemMessage method:

    Dictionary<string, object> data = new Dictionary<string, object> ();
    data.add ("payload", "This can be anything, a string, a GameObject, etc");
    
    NYSUMessaging.SendSystemMessage ("MainMenuUI", NYSUMessaging.Messages.kToggleRenderer, data);

See the example project for more usage details
