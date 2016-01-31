using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AbilitieButton : MonoBehaviour {

    public float secondsForFade;

    private float seconds;
    private Image myImageButton;
    private Animator myAnimator;
    // Use this for initialization
    void Start () {
        //Cambiar color a desactivado
        myImageButton = GetComponent<Image>();
        Color temp = myImageButton.color;
        temp.a = 0.5f;
        myImageButton.color = temp;

        seconds = 1 / secondsForFade;

        myAnimator = GetComponent<Animator>();
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

        myAnimator.SetBool("isActivated", true);
        GameObject childMask = transform.GetChild(1).gameObject;
        childMask.SetActive(false);
    }

    public void Onclick()
    {
        myAnimator.SetBool("isActivated", false);
        GameObject childMask = transform.GetChild(1).gameObject;
        childMask.SetActive(true);
    }
}
