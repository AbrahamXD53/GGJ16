using UnityEngine;
using System.Collections;

public class ShowGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        iTween.MoveTo(gameObject, iTween.Hash("y", 0, "time", 1));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
