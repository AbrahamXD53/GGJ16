using UnityEngine;
using System.Collections.Generic;

public class Level{

	public int turnDelay;
	public int eventDelay;
	public int deltaLuck;
	public int deltaProgress;

    //List of Events and its probabilities in the Level
    Dictionary<GameEvent, int> EventProbability;
    
    //List of valid reactions in this level
    List<Reaction> reactions;

	public Level(int turnDelay, int deltaLuck, int deltaProgress)
	{
		this.turnDelay = turnDelay;
		this.deltaLuck = deltaLuck;
		this.deltaProgress = deltaProgress;
	}

    public Level(int turnDelay, int deltaLuck, int deltaProgress,
        List<Reaction> validReactions, Dictionary<GameEvent, int> EventProbability)
    {
		this.turnDelay = turnDelay;
		this.deltaLuck = deltaLuck;
		this.deltaProgress = deltaProgress;
        this.reactions = validReactions;
        this.EventProbability = EventProbability;
	}
   
    //Function that iterates through the Events and returns one
    public GameEvent GetEvent()
    {

        List<GameEvent> events = new List<GameEvent>();
        
        foreach (var pair in EventProbability) 
            if (Random.Range(0, 101) <= pair.Value) events.Add(pair.Key);
        
        if(events.Count == 0) return null;

        return events[Random.Range(0, events.Count)];

    }

}
