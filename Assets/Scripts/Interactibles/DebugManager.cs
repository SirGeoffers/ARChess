using UnityEngine;
using System.Collections;

public class DebugManager : MonoBehaviour {

    GameObject focusedObject;

	void Start () {
	
	}
	
	void Update () {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {

            GameObject go = hit.transform.gameObject;

            // Stop gazing at last focused object
            if (focusedObject != null) {
                focusedObject.SendMessage("GazeExited");
            }

            // Start gazing at new focused object
            focusedObject = go;
            focusedObject.SendMessage("GazeEntered");

            // Select current focused object
            if (Input.GetMouseButtonDown(0) && focusedObject != null) {
                focusedObject.SendMessage("OnSelect");
            }

        } else {

            // Stop gazing at focused object
            if (focusedObject != null) {
                focusedObject.SendMessage("GazeExited");
                focusedObject = null;
            }

        }

    }
}
