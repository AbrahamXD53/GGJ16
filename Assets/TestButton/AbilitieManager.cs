using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilitieManager : MonoBehaviour {

    public GameObject[] abilities;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ColdDown(int ability)
    {
        Debug.Log("Boton Presonado");
        Image myImageButton = abilities[ability].GetComponent<Image>();
        Color temp = myImageButton.color;
        temp.a = 0.5f;
        myImageButton.color = temp;

        abilities[ability].GetComponent<Animator>().SetBool("isActivated", false);
        GameObject childMask = abilities[ability].transform.GetChild(1).gameObject;
        childMask.SetActive(true);
    }
}
