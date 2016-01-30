using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Reaction {

    static List<Reaction> reactions = new List<Reaction>();
    int CoolDown;
    int Counter;
    Action<GameManager> action;


    public static void Update()
    {
        foreach (var reaction in reactions) reaction.addSecond();
    }
    public Reaction(int CoolDown, Action<GameManager> action)
    {
        reactions.Add(this);
        this.action = action;
        this.CoolDown = CoolDown;
    }


    public void addSecond()
    {
        Counter--;
		if (Counter < 0)
			Counter = 0;
    }

    //function to be called before Apply function
    public bool IsCool()
    {
        return Counter <= 0;
    }

    /// <summary>
    /// A function that applies the reaction directly to a GameManager
    /// </summary>
    /// <param name="mgr">
    /// the manager to apply the function 
    /// </param>
    public void Apply(GameManager mgr)
    {
        Counter = CoolDown;
        action(mgr);
    }

    
}
