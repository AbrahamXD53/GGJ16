using UnityEngine;
using System.Collections;

public class AnimationCOntrolloer : MonoBehaviour {
    AbilitieManager animaciones;
	// Use this for initialization
	void Start () {
        animaciones = GameObject.FindObjectOfType<AbilitieManager>().GetComponent<AbilitieManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void animacion(int i)
    {
        animaciones.AnimationColdDown(i);
    }
}
