using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//un cambio menor
public class Reaction {

    static List<Reaction> reactions = new List<Reaction>();
    int coolDown;
    int counter;
    Action<GameManager> action;


    public static void Update()
    {
        foreach (var reaction in reactions) reaction.addSecond();
    }
    public Reaction(int CoolDown, Action<GameManager> action)
    {
        reactions.Add(this);
        this.action = action;
        this.coolDown = CoolDown;
    }


    public void addSecond()
    {
        counter--;
		if (counter < 0)
			counter = 0;
    }

    //function to be called before Apply function
    public bool IsCool()
    {
        return counter <= 0;
    }

    /// <summary>
    /// A function that applies the reaction directly to a GameManager
    /// </summary>
    /// <param name="mgr">
    /// the manager to apply the function 
    /// </param>
    public void Apply(GameManager mgr)
    {
        counter = coolDown;
        action(mgr);
    }

    public void Cool()
    {
        counter = 0;
    }
}
