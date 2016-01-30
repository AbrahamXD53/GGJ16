using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GameEvent : MonoBehaviour {
     
    Dictionary<Reaction, int> reactions;
    Action<GameManager> actionTrue;
    Action<GameManager> actionFalse;
    public GameEvent(Dictionary<Reaction, int> reactions, Action<GameManager> actionSuccess, Action<GameManager> actionFail)
    {
        this.actionTrue = actionSuccess;
        this.actionFalse = actionFail;
    }
    public void Apply(GameManager man, Reaction react)
    {
        if (react != null && reactions.ContainsKey(react) && new System.Random().Next(100) <= reactions[react]) actionTrue(man);
        actionFalse(man);
    }
}
