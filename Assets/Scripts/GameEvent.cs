using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GameEvent {
     
    Dictionary<Reaction, int> reactions;
    Action<GameManager> actionTrue;
    Action<GameManager> actionFalse;
    string name;

    public GameEvent(Dictionary<Reaction, int> reactions, Action<GameManager> actionSuccess, Action<GameManager> actionFail)
    {
        this.actionTrue = actionSuccess;
        this.actionFalse = actionFail;
		this.reactions = reactions;
        name = "";
    }

    public GameEvent(string name, Dictionary<Reaction, int> reactions, Action<GameManager> actionSuccess, Action<GameManager> actionFail)
    {
        this.actionTrue = actionSuccess;
        this.actionFalse = actionFail;
        this.reactions = reactions;
        this.name = name;
    }

    public void Apply(GameManager man, Reaction react)
    {
		int randomNumber = UnityEngine.Random.Range (0, 101);
		if (react != null && reactions.ContainsKey (react) && randomNumber <= reactions [react]) {
			actionTrue (man);
            react = null;
			return;
		}
        actionFalse(man);
        react = null;
    }

    public string GetName()
    {
        return name;
    }
}
