using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilitieManager : MonoBehaviour
{

    public GameObject[] abilities;
    public string[] names;
    public int amountBeer;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ColdDown(int ability)
    {
        //BUscamos si esta enfriado el boton
        bool isCool = abilities[ability].GetComponent<AbilitieButton>().isCool();
        if (isCool)
        {
            Debug.Log("Habilidad Activada");
            //Cambia estado a enfriamiento
            abilities[ability].GetComponent<AbilitieButton>().setCool();
            //Cambiamos el color a uno obsucro
            Image myImageButton = abilities[ability].GetComponent<Image>();
            Color temp = myImageButton.color;
            temp.a = 0.5f;
            myImageButton.color = temp;
            //Cambia a estado de enfriamiento
            abilities[ability].GetComponent<Animator>().SetBool("isActivated", false);
            GameObject childMask = abilities[ability].transform.GetChild(1).gameObject;
            childMask.SetActive(true);

            //TODO verificar
            switch (ability)
            {
                //Cerveza
                case 0:
                    addPee(amountBeer);
                    break;
            }
            GamePlaying.game.player.SetTrigger(names[ability]);
        }

    }

    void addPee(int amount)
    {

    }
}
