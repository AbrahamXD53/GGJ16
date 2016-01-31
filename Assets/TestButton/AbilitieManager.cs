using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilitieManager : MonoBehaviour
{

    public GameObject[] abilities;
    public string[] names;
    public int amountBeer;
    GameManager gameManager;
    string[] reactions = new string[] {GameManager.BEER,GameManager.SHOUT,GameManager.FLAG,GameManager.CELEBRATE };
    // Use this for initialization
    void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ColdDown(int ability)
    {
        //BUscamos si esta enfriado el boton
        // bool isCool = abilities[ability].GetComponent<AbilitieButton>().isCool();
        Debug.Log(GameManager.reactions[reactions[ability]].IsCool());
        if (GameManager.reactions[reactions[ability]].IsCool())
        {   
            //Cambia estado a enfriamiento
           // abilities[ability].GetComponent<AbilitieButton>().setCool();            
            
            gameManager.SetActiveReaction(reactions[ability]);
            GamePlaying.game.player.SetTrigger(names[ability]);
            Debug.Log(GameManager.reactions[GameManager.BEER].GetTimeToCool());
        }
    }

    public void AnimationColdDown(int ability)
    {
        //Cambiamos el color a uno obsucro
        Image myImageButton = abilities[ability].GetComponent<Image>();
        Color temp = myImageButton.color;
        temp.a = 0.5f;
        myImageButton.color = temp;
        //Cambia a estado de enfriamiento
        abilities[ability].GetComponent<Animator>().SetBool("isActivated", false);
        GameObject childMask = abilities[ability].transform.GetChild(1).gameObject;
        childMask.SetActive(true);
    }

}
