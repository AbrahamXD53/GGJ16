using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GamePlaying : MonoBehaviour
{

    GameManager gameManager;
    public Image match;
    public Image nextEvent;
    public Image modifier;
    public Text score;

    public Sprite[] eventImageArray;
    void Start()
    {
        gameManager = GameManager.instance;
    }
    string GetTimeString(int secs)
    {
        var restante = GameManager.GAME_DURATION - secs;
        return string.Format("%s s");
    }
    Sprite GetEventSprite(string name)
    {
        switch (name)
        {
            case GameManager.PRO_GOAL:
                return eventImageArray[0];
            case GameManager.PRO_PASS:
                return eventImageArray[1];
            case GameManager.PRO_PENALTY:
                return eventImageArray[2];
            case GameManager.PRO_SWEEP:
                return eventImageArray[3];
            case GameManager.CONTRA_GOAL:
                return eventImageArray[4];
            case GameManager.CONTRA_PASS:
                return eventImageArray[5];
            case GameManager.CONTRA_PENALTY:
                return eventImageArray[6];
            case GameManager.CONTRA_SWEEP:
                return eventImageArray[7];
            default:
                break;
        }
        return null;
    }
    void Update()
    {
        match.rectTransform.position = new Vector3(5.25f - (5.25f * gameManager.GetProgress() / 50), match.rectTransform.position.y, match.rectTransform.position.z);

        nextEvent.enabled = gameManager.IsEventComing();
        if (nextEvent.enabled)
            nextEvent.sprite = GetEventSprite(gameManager.NextEvent());
        if(SoundManager.Instance!=null)
            SoundManager.Instance.ChangeSong(gameManager.GetProgress());
    }
}
