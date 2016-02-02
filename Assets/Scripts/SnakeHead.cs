using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SnakeHead : MonoBehaviour {

    public int defaultSize;
    public GameObject body;
    public GameObject apple;
    public GameObject scoreText;

    private Vector3 position;
    private Vector3 rotation;
    private List<SnakeBody> snake = new List<SnakeBody>();
    private int frame;
    private int length;
    private int score;
    private float speed;
    private float screenH;
    private float screenW;
    private int screenHpx;
    private int screenWpx;

    private bool gameStopped;
    private float alpha = 0f;

    private string name = "Enter name";
    
    void Awake () {
	QualitySettings.vSyncCount = 0;
	Application.targetFrameRate = 30;
    }

    void OnGUI() {
	GUIStyle style = new GUIStyle();
	style.alignment = TextAnchor.MiddleCenter;
	style.fontSize = (int) screenH * 6;
	style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	GUI.Label(new Rect(Screen.width / 2 - 40, 50, 80, 30), "Score : " + score, style);

	if (gameStopped) {
	    GUIStyle fadeIn;
	    fadeIn = new GUIStyle();
	    fadeIn.alignment = TextAnchor.MiddleCenter;
	    fadeIn.fontSize = (int) screenHpx / 20;
	    fadeIn.normal.textColor = new Color (1.0f, 1.0f, 1.0f, alpha);
	    GUI.Label(new Rect(screenWpx / 10, 1.5f * screenHpx / 10,
					  screenWpx - screenWpx * 2 / 10, screenHpx / 10),
				 "Name :", fadeIn);
	    name = GUI.TextField(new Rect(screenWpx / 10, screenHpx /2 - 3 * screenHpx / 10,
					  screenWpx - screenWpx * 2 / 10, screenHpx / 10),
				 name, fadeIn);
	    if (alpha < 1.0f)
		alpha+= 0.02f;
	    if (GUI.Button(new Rect(screenWpx / 10, screenHpx /2 - screenHpx / 10,
				    screenWpx - screenWpx * 2 / 10, screenHpx / 10),
			   "Play again", fadeIn)) {
		addScore(name == "Enter name" ? "Anonymous" : name, score);
		SceneManager.LoadScene("GameScene");		
	    }
	    else if (GUI.Button(new Rect(screenWpx / 10, screenHpx / 2 + screenHpx / 10,
					 screenWpx - screenWpx * 2 / 10, screenHpx / 10),
				"Back to menu", fadeIn)) {
		addScore(name == "Enter name" ? "Anonymous" : name, score);
		SceneManager.LoadScene("MenuScene");
	    }
	}
    }
    
    void Start () {
	if (PlayerPrefs.HasKey("LastNameEntered")) {
	    name = PlayerPrefs.GetString("LastNameEntered");
	}
	frame = 0;
	speed = 0.1f;
	position = new Vector3(0,0,0);
	rotation = new Vector3(0,0,0);
	gameStopped = false;

	transform.position = position;
	for (int i = 0; i < defaultSize; i++) {
	    Instantiate(body, position,
				   Quaternion.Euler(rotation.x, rotation.y, rotation.z));
	}
	screenH = Camera.main.orthographicSize * 2.0f;
	screenW = screenH * Screen.width / Screen.height;
	Instantiate(apple, randomPositionInBounds(), Quaternion.Euler(0, 0, 0));
	screenHpx = Screen.height;
	screenWpx = Screen.width;
    }
    
    void Update () {
	// Detecting back button press
	if (Input.GetKeyDown(KeyCode.Escape)) {
	    SceneManager.LoadScene("MenuScene");
	}

	if (gameStopped)
	    return;
	
	/* Moving the whole snake
	** Optimized rolling list to avoid multiple updates each frame
	** and eventual framerate drops
	*/
	SnakeBody part = getLastElem();
	snake.RemoveAt(snake.Count - 1);
	snake.Insert(0, part);
	part.setRotation(rotation);
	part.setPosition(transform.position);

	// Turning one side or an other depending on if the user touches the screen
	int fingerCount = 0;
        foreach (Touch touch in Input.touches) {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;            
        }
	if (fingerCount > 0 || Input.GetKey("space")) // Spacebar to debug on PC
	    rotation.z = (rotation.z - 10) % 360;
	else
	    rotation.z = (rotation.z + 10) % 360;
	transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
	position += transform.up * speed;

	// Passing through the edges of the screen
	if (position.x > screenW / 2)
	    position.x -= screenW;
	else if (-position.x > screenW / 2)
	    position.x += screenW;
	if (position.y > screenH / 2)
	    position.y -= screenH;
	else if (-position.y > screenH / 2)
	    position.y += screenH;
	transform.position = position;

	frame += 1;
    }

    void OnTriggerEnter2D(Collider2D other) {
	if (other.gameObject.tag == "dot") {
	    Debug.Log("Found apple !");
	    Destroy(other.gameObject);
	    for (int i = 0; i < 4; i++) {
		Instantiate(body, getLastElem().getPosition(),
			    Quaternion.Euler(rotation.x, rotation.y, rotation.z));
	    }
	    Instantiate(apple, randomPositionInBounds(), Quaternion.Euler(0, 0, 0));
	    if (speed < 0.12f)
		speed += 0.001f;
	    score += 1;
	}
	else if (frame > 60 && other.gameObject.tag == "body")
	{
	    gameStopped = true;
	    GameObject[] parts = GameObject.FindGameObjectsWithTag("body");
	    ((SpriteRenderer) gameObject.GetComponent(typeof(SpriteRenderer))).enabled = false;
	    foreach (GameObject part in parts) {
		Rigidbody2D rb = (Rigidbody2D) part.GetComponent(typeof(Rigidbody2D));
		Vector2 force = (Vector2) randomPositionInBounds();
		force.Normalize();
		force *= 0.01f;
		rb.AddForce(force);
	    }
	}
    }

    private Vector3 randomPositionInBounds() {
	Vector3 temp;
	
	temp.x = (Random.value * 2 - 1) * (screenW - 0.25f) / 2;
	temp.y = (Random.value * 2 - 1) * (screenH - 0.25f) / 2;
	temp.z = 0;
	while (Vector3.Distance(temp, transform.position) < 2f) {
	    temp.x = (Random.value * 2 - 1) * screenW / 2;
	    temp.y = (Random.value * 2 - 1) * screenH / 2;
	}
	return (temp);
    }
    
    private SnakeBody getLastElem() {
	SnakeBody tmp = null;
	
	foreach (SnakeBody part in snake) {
	    tmp = part;
	}
 	return tmp;
    }

    public void addScore(string nm, int sc) {
	int score = sc;
	string name = nm;
	int oldScore;
	string oldName;

	PlayerPrefs.SetString("LastNameEntered", name);
	for (int i = 1; i < 11; i++) {
	    if (PlayerPrefs.HasKey(i + "HScore")) {
		if (PlayerPrefs.GetInt(i + "HScore") < score && score != 0) {
		    oldScore = PlayerPrefs.GetInt(i + "HScore");
		    oldName = PlayerPrefs.GetString(i + "HScoreName");
		    PlayerPrefs.SetInt(i + "HScore", score);
		    PlayerPrefs.SetString(i + "HScoreName", name);
		    score = oldScore;
		    name = oldName;
		}
	    }
	    else if (score != 0) {
		PlayerPrefs.SetInt(i+"HScore", score);
		PlayerPrefs.SetString(i+"HScoreName", name);
		score = 0;
		name = "";
	    }	       
	}
    }
    
    public void addToList(SnakeBody part) {
	part.setPrevious(getLastElem());
	snake.Add(part);
    }

    public Vector3 getPosition() {
	return transform.position;
    }

}
