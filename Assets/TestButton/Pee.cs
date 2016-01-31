using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pee : MonoBehaviour {

    //Variable que buscara el int del nivel de pipi, valor de 0 a 100
    //Sustituir por la variable real
    [Range (0, 100)]
    public int nivelPipi;
    public float secondsForDoingPee;
    
    private Slider peeBar;
	// Use this for initialization
	void Start () {
        peeBar = GetComponent<Slider>();

	}
	
	// Update is called once per frame
	void Update () {
        peeBar.value = nivelPipi;
        if (nivelPipi >= 100)
            emptyPee();
	}

    //Agrega el valor de la cerveza ingeridas
    public void setAmountPee(int amount)
    {
        nivelPipi = +amount;
    }
    //Cuando se vaya al baño
    public void emptyPee()
    {
        //Animacion para vaciar la barra
        StartCoroutine("emptyBlader");
        //nivelPipi = 0;
    }
    IEnumerator emptyBlader ()
    {
        float timeAnimation = 0f;
        while(timeAnimation < secondsForDoingPee)
        {
            timeAnimation += Time.deltaTime;
            float lerpValue = timeAnimation / secondsForDoingPee;
            peeBar.value = Mathf.Lerp(100, 0, lerpValue);
            yield return null;
        }
        nivelPipi = 0;
    }
}
