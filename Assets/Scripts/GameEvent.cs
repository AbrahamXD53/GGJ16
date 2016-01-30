using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GameEvent {
     
    Dictionary<Reaction, int> reactions;
    Action<GameManager> actionTrue;
    Action<GameManager> actionFalse;
    public GameEvent(Dictionary<Reaction, int> reactions, Action<GameManager> actionSuccess, Action<GameManager> actionFail)
    {
        this.actionTrue = actionSuccess;
        this.actionFalse = actionFail;
		this.reactions = reactions;
    }
    public void Apply(GameManager man, Reaction react)
    {
		int randomNumber = UnityEngine.Random.Range (0, 101);
		if (react != null && reactions.ContainsKey (react) && randomNumber <= reactions [react]) {
			actionTrue (man);
			return;
		}
        actionFalse(man);
    }
}
