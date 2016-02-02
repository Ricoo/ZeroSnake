using UnityEngine;
using System.Collections;

public class SnakeBody : MonoBehaviour {

    private Vector3 position;
    private Vector3 rotation;
    private SnakeBody previous;

    void Start() {
	GameObject go = GameObject.Find("snake-head");
	SnakeHead head = (SnakeHead) go.GetComponent(typeof(SnakeHead));
	head.addToList((SnakeBody) gameObject.GetComponent(typeof(SnakeBody)));
    }
    
    public void setPrevious(SnakeBody prev) {
	previous = prev;
    }

    public SnakeBody getPrevious() {
	return this.previous;
    }

    public Vector3 getPosition() {
	return transform.position;
    }

    public Vector3 getRotation() {
	return rotation;
    }

    public void setPosition(Vector3 pos) {
	position = pos;
	transform.position = position;
    }

    public void setRotation(Vector3 rot) {
	rotation = rot;
	transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    public void Destroy() {
	Destroy(gameObject);
    }
}
