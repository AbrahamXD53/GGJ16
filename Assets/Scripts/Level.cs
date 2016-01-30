using UnityEngine;
using System.Collections;

public class Level{

	public float turnDelay;
	public float turnPeriod;
	public int deltaLuck;
	public int deltaProgress;

	public Level (float turnDelay, float turnPeriod, int deltaLuck, int deltaProgress){
		this.turnDelay = turnDelay;
		this.turnPeriod = turnPeriod;
		this.deltaLuck = deltaLuck;
		this.deltaProgress = deltaProgress;
	}
}
