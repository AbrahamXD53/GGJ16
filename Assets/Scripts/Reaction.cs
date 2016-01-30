using UnityEngine;
using System.Collections;
using System;

public class Reaction {


    public Reaction(int CoolDown, Action<GameManager> action)
    {
        this.action = action;
        this.CoolDown = CoolDown;
    }

    int CoolDown;
    int Counter;
    Action<GameManager> action;
    public void addSecond()
    {
        Counter--;
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
