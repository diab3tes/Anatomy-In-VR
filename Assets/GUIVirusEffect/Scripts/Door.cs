using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	public Vector3 closedPosition;
	public Vector3 openPosition;

	public bool opened = false;
	public bool opening = false;

	float doorTime = 0;

	bool mouseOver = false;
	bool windowOpen = false;

	void FixedUpdate() {
		if(opening) {
			if(doorTime < 1) {
				doorTime += Time.deltaTime;
			}
			else {
				opening = false;
			}

			transform.position = Vector3.Lerp(closedPosition, openPosition, doorTime); //Smoothly moves the door to the open position
		}
	}

	void OnMouseEnter() { // Checks if the mouse is over the door's collider
		mouseOver = true;
	}

	void OnMouseExit() { // Checks if the mouse has exited the door's collider
		mouseOver = false;
	}

	void OnMouseUp() { //Creates the warning here
		if(mouseOver) {
			if(!opened && !windowOpen) {
				//Creates a Warning window that will call the WindowSpam function if you press either the Yes or No button
				VirusGUI.PopupVariables vars = new VirusGUI.PopupVariables("Virus Detected!", "This game has detected a type of Door virus. Scan?", gameObject, "WindowSpam", "", "WindowSpam", "");
				VirusGUI.PopupWindow window = new VirusGUI.PopupWindow(vars, new Vector2(Screen.width * .5f - 125, Screen.height * .5f - 75));
				window.popupVariables.closeButton = false;
				windowOpen = true;
			}
		}
	}

	public void OpenDoor() {
		opening = true;
		opened = true;
		//Removes the window spam and tells shows you that you were succesful at opening the door
		VirusGUI.virus.RemovePopupsWithFunctionObject(gameObject);
		VirusGUI.PopupVariables vars = new VirusGUI.PopupVariables("Success!", "You have successfully removed the Trojan from Door(0)!");
		new VirusGUI.PopupWindow(vars, new Vector2(Screen.width * .5f - 125, Screen.height * .5f - 75));
	}

	public void WindowSpam() {
		//Opens 3 windows that are the same
		//If you press Yes, it calls the OpenDoor function
		//If you press No, it calls the CancelWindow function
		VirusGUI.PopupVariables vars = new VirusGUI.PopupVariables("You've done it now!", "Door(0) has received a Trojan. Delete Trojan?", gameObject, "OpenDoor", "", "CancelWindow", "");
		VirusGUI.MultiWindow multi = new VirusGUI.MultiWindow(3, 5, vars);
		multi.popupVariables.runFromCursor = true;
	}

	public void CancelWindow() {
		windowOpen = false;
		VirusGUI.virus.RemovePopupsWithFunctionObject(gameObject);
	}

	void OnPopupX() {
		CancelWindow();
	}

	[ContextMenu("Set Closed Position")]
	void ClosedPosition() { closedPosition = transform.position; }

	[ContextMenu("Set Open Position")]
	void OpenPosition() { openPosition = transform.position; }

	void OnDrawGizmosSelected() {
		Gizmos.DrawLine(closedPosition, openPosition);
	}
}
