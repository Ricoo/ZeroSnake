using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuHandler : MonoBehaviour {
    public string CurrentMenu;
    private int screenHpx;
    private int screenWpx;
    private GUIStyle style;
    private GUIStyle style2;
    
    void Start() {
	CurrentMenu = "main";
	screenHpx = Screen.height;
	screenWpx = Screen.width;
	style = new GUIStyle();
	style.alignment = TextAnchor.MiddleCenter;
	style.fontSize = (int) screenHpx / 20;
	style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	style2 = new GUIStyle();
	style2.alignment = TextAnchor.MiddleCenter;
	style2.fontSize = (int) screenHpx / 30;
	style2.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
    }
    
    void OnGUI() {
	if (CurrentMenu == "main") {
	    menuMain();
	}
	else if (CurrentMenu == "hs") {
	    menuHiScore();
	}
    }
    
    void Update() {
	if (Input.GetKeyDown(KeyCode.Escape)) {
	    Application.Quit();
	}
    }

    private void menuMain() {
	if (GUI.Button(new Rect(screenWpx / 10, screenHpx /2 - screenHpx / 10,
				screenWpx - screenWpx * 2 / 10, screenHpx / 10), "Play Game", style)) {
	    SceneManager.LoadScene("GameScene");
	}
   	else if (GUI.Button(new Rect(screenWpx / 10, screenHpx / 2 + screenHpx / 10,
				     screenWpx - screenWpx * 2 / 10, screenHpx / 10), "High Scores", style)) {
	    CurrentMenu = "hs";
	}
	
    }
    
    private void menuHiScore()
    {
	string text = "";
	for (int i = 1; i < 11; i++) {
	    if (PlayerPrefs.HasKey(i + "HScore")) {
		text += i + "- ";
		text += PlayerPrefs.GetString(i + "HScoreName");
		text += ": ";
		text += PlayerPrefs.GetInt(i + "HScore");
		text += "points";
	    }
	    else {
		text += i + " ---";
	    }
	    text += "\n";
	}
	GUI.Label(new Rect(screenWpx / 10, screenHpx / 10,
			  screenWpx - screenWpx * 2 / 10, screenHpx - 2* screenHpx / 10), text, style2);
	if (GUI.Button(new Rect(screenWpx / 10, screenHpx - 2 * screenHpx / 10,
				screenWpx - screenWpx * 2 / 10, screenHpx / 10), "Back", style))
	{
	    CurrentMenu = "main";
	}
    }
    
}
