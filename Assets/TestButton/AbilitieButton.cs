using UnityEngine;
using System.Collections;

public class AbilitieButton : MonoBehaviour {

    public float secondsForFade;

    private float seconds;
	// Use this for initialization
	void Start () {
        seconds = 1 / secondsForFade;

        Animator myAnimator = GetComponent<Animator>();
        myAnimator.SetFloat("myspeed", seconds);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
