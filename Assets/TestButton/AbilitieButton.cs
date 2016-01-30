using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilitieButton : MonoBehaviour {

    public float secondsForFade;

    private float seconds;
    private Image myImageButton;
	// Use this for initialization
	void Start () {
        //Cambiar color a desactivado
        myImageButton = GetComponent<Image>();
        Color temp = myImageButton.color;
        temp.a = 0.5f;
        myImageButton.color = temp;

        seconds = 1 / secondsForFade;

        Animator myAnimator = GetComponent<Animator>();
        myAnimator.SetFloat("myspeed", seconds);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ActiveAbilitie()
    {
        Color temp = myImageButton.color;
        temp.a = 1f;
        myImageButton.color = temp;
    }
}
