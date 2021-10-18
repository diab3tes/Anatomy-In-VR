using UnityEngine;
using System.Collections;

//Created By: Steve Wigley (a.k.a. GIB3D)
//Date Created: January 19, 2011
//Purpose: To add some uniqueness to your game by having pop-ups!

public class VirusGUI : MonoBehaviour {
	public static VirusGUI virus;

	public GUISkin virusGUI;
	public PopupWindow[] popupWindows;
	public MultiWindow[] multiWindows;

	public enum WindowType { Error, Warning };

	public VirusGUI() {
		virus = this;
	}

	void Update() {
		foreach(MultiWindow a in multiWindows) {
			a.Update();
		}
	}

	void OnGUI() {
		GUI.skin = virusGUI;

		int i = 0;
		foreach(PopupWindow a in popupWindows) {
			if(a.popupVariables.stayOnScreen) { a.rect = StayOnScreen(a.rect); }
			a.rect = GUI.Window(i, a.rect, Popup, a.popupVariables.title);
			i++;
		}
	}

	void Popup(int windowID) {
		PopupWindow window = popupWindows[windowID];

		#region Window Effects
		if(window.popupVariables.runFromCursor) {
			Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

			if(window.rect.Contains(mouse)) {
				window.rect = MoveRect(window.rect, new Vector2(((window.rect.x + (window.rect.width * .5f)) - mouse.x) * window.popupVariables.runSpeed, ((window.rect.y + (window.rect.height * .5f)) - mouse.y) * window.popupVariables.runSpeed));
			}
		}

		if(window.popupVariables.spazWindow) {
			if(window.popupVariables.spazTime < 1) {
				window.popupVariables.spazTime += Time.deltaTime * window.popupVariables.spazSpeed;
			}
			else {
				window.popupVariables.spazTime = 0;
				window.rect = MoveRect(window.rect, Random.insideUnitCircle * window.popupVariables.spazMagnitude);
			}
		}
		#endregion

		GUILayout.Label(window.popupVariables.message, GUILayout.ExpandHeight(true));

		GUILayout.BeginArea(new Rect(window.rect.width * .5f - 45, window.rect.height - 24, 90, 24));

		switch(window.popupVariables.windowType) {
			case WindowType.Error:
				if(GUILayout.Button("Ok", GUILayout.ExpandWidth(true))) {
					if(window.popupVariables.yesFunction != "") {
						window.popupVariables.functionObject.SendMessage(window.popupVariables.yesFunction, window.popupVariables.yesParameter, SendMessageOptions.DontRequireReceiver);
					}
					Remove(window);
					return;
				}
				break;

			case WindowType.Warning:
				if(window.popupVariables.functionObject) {
					GUILayout.BeginHorizontal();

					if(GUILayout.Button("Yes", GUILayout.MinWidth(40), GUILayout.MaxWidth(40))) {
						if(window.popupVariables.yesFunction != "") {
							window.popupVariables.functionObject.SendMessage(window.popupVariables.yesFunction, window.popupVariables.yesParameter, SendMessageOptions.DontRequireReceiver);
						}
						Remove(window);
						return;
					}
					if(GUILayout.Button("No", GUILayout.MinWidth(40), GUILayout.MaxWidth(40))) {
						if(window.popupVariables.noFunction != "") {
							window.popupVariables.functionObject.SendMessage(window.popupVariables.noFunction, window.popupVariables.noParameter, SendMessageOptions.DontRequireReceiver);
						}
						Remove(window);
						return;
					}

					GUILayout.EndHorizontal();
				}
				else {
					if(GUILayout.Button("Ok", GUILayout.ExpandWidth(true))) {
						Remove(window);
						return;
					}
				}


				break;
		}
		GUILayout.EndArea();

		GUI.Box(new Rect(0, 0, window.rect.width, 20), "");

		if(window.popupVariables.closeButton) {
			GUI.color = Color.red;
			if(GUI.Button(new Rect(window.rect.width - 56, 0, 50, 20), "X")) {
				if(window.popupVariables.functionObject) {
					window.popupVariables.functionObject.SendMessage("OnPopupX", SendMessageOptions.DontRequireReceiver);
				}
				Remove(window);
				return;
			}
		}

		GUI.DragWindow(new Rect(0, 0, window.rect.width, 20));
	}

	//Adds and Removes windows from the Array variables
	public PopupWindow Add(PopupWindow window) {
		ArrayList list = new ArrayList(popupWindows);
		list.Add(window);
		popupWindows = list.ToArray(typeof(PopupWindow)) as PopupWindow[];
		return window;
	}
	public MultiWindow Add(MultiWindow multiWindow) {
		ArrayList list = new ArrayList(multiWindows);
		list.Add(multiWindow);
		multiWindows = list.ToArray(typeof(MultiWindow)) as MultiWindow[];
		return multiWindow;
	}
	public void Remove(PopupWindow window) {
		ArrayList list = new ArrayList(popupWindows);
		list.Remove(window);
		popupWindows = list.ToArray(typeof(PopupWindow)) as PopupWindow[];
	}
	public void Remove(MultiWindow multiWindow) {
		ArrayList list = new ArrayList(multiWindows);
		list.Remove(multiWindow);
		multiWindows = list.ToArray(typeof(MultiWindow)) as MultiWindow[];
	}

	//Searches listed Popup Windows and removes the ones with the same functionObject
	public void RemovePopupsWithFunctionObject(GameObject functionObject) {
		foreach(PopupWindow a in popupWindows) {
			if(a.popupVariables.functionObject == functionObject) { Remove(a); }
		}
	}

	public static Rect MoveRect(Rect rect, Vector2 move) {
		rect.x += move.x;
		rect.y += move.y;

		return rect;
	}
	public static Rect StayOnScreen(Rect rect) {
		if(rect.x < 0) { rect.x = 0; }
		else if(rect.x + rect.width > Screen.width) { rect.x = Screen.width - rect.width; }

		if(rect.y < 0) { rect.y = 0; }
		else if(rect.y + rect.height > Screen.height) { rect.y = Screen.height - rect.height; }

		return rect;
	}
	public static Vector2 RandomPointOnScreen() {
		return new Vector2(Random.Range(0, Screen.width + 1), Random.Range(0, Screen.height + 1));
	}

	[System.Serializable]
	public class PopupVariables {
		public string title = "";
		public string message = "";

		public WindowType windowType = WindowType.Error;

		public bool stayOnScreen = true;
		public bool closeButton = true;

		#region Effects
		//Run From Cursor
		public bool runFromCursor = false; //If you're cursor gets close, it moves away
		public float runSpeed = .1f;

		//Spaz Window - Moves the window to random positions
		public bool spazWindow = false;
		public float spazSpeed = 5;
		public float spazMagnitude = 100;
		[HideInInspector]
		public float spazTime = 0;
		#endregion

		//These variables will be used with SendMessage
		public GameObject functionObject; //Object to call the function on

		public string yesFunction = ""; //When Yes is clicked, this function will be called // This will also be used for the Ok button
		public string noFunction = ""; //When No is clicked, this function will be called

		//Optional function parameters
		public string yesParameter = ""; // This will also be used for the Ok button
		public string noParameter = "";

		public PopupVariables() { }
		public PopupVariables(string title, string message) {
			this.title = title;
			this.message = message;
		}
		public PopupVariables(string title, string message, WindowType windowType) {
			this.title = title;
			this.message = message;

			this.windowType = windowType;
		}
		public PopupVariables(string title, string message, GameObject functionObject, string okFunction, string okParameter) {
			this.title = title;
			this.message = message;

			this.windowType = WindowType.Error;

			this.functionObject = functionObject;
			this.yesFunction = okFunction;
			this.yesParameter = okParameter;
		}
		public PopupVariables(string title, string message, GameObject functionObject, string yesFunction, string yesParameter, string noFunction, string noParameter) {
			this.title = title;
			this.message = message;

			this.windowType = WindowType.Warning;

			this.functionObject = functionObject;
			this.yesFunction = yesFunction;
			this.yesParameter = yesParameter;
			this.noFunction = noFunction;
			this.noParameter = noParameter;
		}
	}

	[System.Serializable]
	public class PopupWindow {
		public PopupVariables popupVariables;

		public Rect rect = new Rect(0, 0, 250, 150);

		public PopupWindow() { }
		public PopupWindow(PopupVariables popupVariables){
			this.popupVariables = popupVariables;
			virus.Add(this);
		}
		public PopupWindow(PopupVariables popupVariables, Rect windowRect){
			this.popupVariables = popupVariables;
			rect = windowRect;
			virus.Add(this);
		}
		public PopupWindow(PopupVariables popupVariables, Vector2 position){
			this.popupVariables = popupVariables;
			rect.x = position.x;
			rect.y = position.y;
			virus.Add(this);
		}
	}

	[System.Serializable]
	public class MultiWindow {
		public PopupVariables popupVariables;

		public int desiredAmount = 1;

		[HideInInspector]
		public int currentAmount = 0;

		public float popupSpeed = 10;
		float time = 1;

		public MultiWindow() { }
		public MultiWindow(int windowAmount, PopupVariables popupVariables){
			desiredAmount = windowAmount;
			this.popupVariables = popupVariables;
			virus.Add(this);
		}
		public MultiWindow(int windowAmount, int popupSpeed, PopupVariables popupVariables){
			desiredAmount = windowAmount;
			this.popupVariables = popupVariables;
			this.popupSpeed = popupSpeed;
			virus.Add(this);
		}

		public void Update() {
			if(currentAmount < desiredAmount) {
				if(time < 1) {
					time += Time.deltaTime * popupSpeed;
				}
				else {
					time = 0;
					currentAmount++;

					new PopupWindow(popupVariables, RandomPointOnScreen());
				}
			}
			else {
				virus.Remove(this);
			}
		}
	}
}
