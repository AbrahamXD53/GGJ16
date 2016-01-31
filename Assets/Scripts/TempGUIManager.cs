using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TempGUIManager : MonoBehaviour {
    GameManager gameManager;

    public Text txtLuck;
    public Text txtProgress;
    public Text txtPee;

    public Text txtBeer;
    public Text txtShout;
    public Text txtFlag;
    public Text txtCelebrate;

    public Text txtEventComing;

    public Text txtTime;
    public Text txtEvent;

    public Text txtEventOutput;

    public Text txtReactionOutput;

    public Text txtProScore;
    public Text txtContraScore;

    public Text txtLevel;

    public Text txtIsPeing;

    public Text txtProGoal;
    public Text txtContraGoal;

    public Text txtActiveReaction;

    // Use this for initialization
    void Start()
    {
        gameManager = GameManager.instance;
    }
	
	// Update is called once per frame
	void Update () {
        txtLuck.text = gameManager.GetLuck().ToString();
        txtPee.text = gameManager.GetPee().ToString();
        txtProgress.text = gameManager.GetProgress().ToString();

        txtTime.text = ((int)gameManager.GetElapsedTime()).ToString();

        txtBeer.enabled = GameManager.reactions[GameManager.BEER].IsCool();
        txtShout.enabled = GameManager.reactions[GameManager.SHOUT].IsCool();
        txtFlag.enabled = GameManager.reactions[GameManager.FLAG].IsCool();
        txtCelebrate.enabled = GameManager.reactions[GameManager.CELEBRATE].IsCool();

        txtEventComing.enabled = txtEvent.enabled = gameManager.EventComing();
        txtEvent.text = gameManager.NextEvent();

        txtProScore.text = gameManager.GetScore(GameManager.PRO_PLAYER).ToString();
        txtContraScore.text = gameManager.GetScore(GameManager.CONTRA_PLAYER).ToString();

        txtEventOutput.text = GameManager.eventOutput;
        txtReactionOutput.text = GameManager.reactionOutput;

        txtIsPeing.enabled = gameManager.IsPeing();

        txtLevel.text = gameManager.GetLevelNumber().ToString();

        txtProGoal.enabled = gameManager.ProGoalNear();
        txtContraGoal.enabled = gameManager.ContraGoalNear();

        txtActiveReaction.text = gameManager.GetActiveReaction();
    }
}
