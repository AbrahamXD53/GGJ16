using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class AbilitieButton : MonoBehaviour {

    public float secondsForFade;

    private bool cool;
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
        //Enfriaminto en positivo
        cool = true;
        //Cambia el color a uno fuerte
        Color temp = myImageButton.color;
        temp.a = 1f;
        myImageButton.color = temp;
        //Cambia animacion a estado Activo
        myAnimator.SetBool("isActivated", true);
        //Busca el hijo Mask y lo deshabilita
        GameObject childMask = transform.GetChild(1).gameObject;
        childMask.SetActive(false);
    }

    public bool isCool()
    {
        return cool;
    }
    public void setCool()
    {
        cool = false;
    }
}
