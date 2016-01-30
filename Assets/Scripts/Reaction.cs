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
    public bool IsCool()
    {
        return Counter <= 0;
    }
    public void Apply(GameManager mgr)
    {
        Counter = CoolDown;
        action(mgr);
    }

    
}
