using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// Esta clase se encarga de:
// controlar las variables criticas del juego (Salud, Avance y Orina)

public class GameManager : MonoBehaviour {

    public const int INITIAL_LUCK = 50;
	public const int INITIAL_PROGRESS = 50;
	public const int FAIL_PROGRESS = 30;
	public const int DELTA_PEE = 20;
    public const int PEE_TIME = 5;

    // Duración del juego (Nivel) en segundos
    const float GAME_DURATION = 60.0f;

    const float LUCK_MULTIPLIER = 2;
    

    float luck;
	int pee;
	int progress;

	int teamGoals = 0;
	int enemyGoals = 0;

    float elapsedTime;
	float turnTime;

    float peeTime = PEE_TIME;
    bool isPeeing = false;

	bool isGameOver = false;

	Reaction nextTurnReaction = null;

    #region Reactions
    static Dictionary<string, Reaction> reactions = new Dictionary<string, Reaction>{
		{"beer",
            new Reaction(2,
                k =>
                {
                    k.AddPee(DELTA_PEE);
                    k.AddLuck(LUCK_MULTIPLIER * k.currentLevel.deltaLuck);
                    Debug.Log("Cerveza, pipi +" + DELTA_PEE + ", suerte +" + LUCK_MULTIPLIER * k.currentLevel.deltaLuck);
                }
            )
        },
        {"flag",
            new Reaction(5,
                k =>
                {
                    k.AddLuck(k.currentLevel.deltaLuck);
                    Debug.Log("Bandera, suerte +" + k.currentLevel.deltaLuck);
                }
            )
        },
        {"shout",
            new Reaction(7,
                k =>
                {
                    k.AddLuck(k.currentLevel.deltaLuck);
                    Debug.Log("Grito, suerte +" + k.currentLevel.deltaLuck);
                }
            )
        },
        {"celebrate",
            new Reaction(10,
                k =>
                {
                    k.AddLuck(LUCK_MULTIPLIER * k.currentLevel.deltaLuck);
                    Debug.Log("Celebración, suerte +" + LUCK_MULTIPLIER * k.currentLevel.deltaLuck);
                }
            )
        }
    };
    #endregion

    #region GameEvents
    static Dictionary<string, GameEvent> events = new Dictionary<string, GameEvent>{
		{"ProGoal",
            new GameEvent(
                new Dictionary<Reaction, int>{
                    {reactions["beer"], 50},
                    {reactions["shout"], 70},
                    {reactions["flag"], 50},
                    {reactions["celebrate"], 80}
                },
                k => { k.teamGoals++; k.progress = INITIAL_PROGRESS; Debug.Log("Gol Anotado"); },
                k => { k.progress = 70; Debug.Log("Gol Fallado"); }
            )
        },
        {"ContraGoal",
            new GameEvent(
                new Dictionary<Reaction, int>{
                    {reactions["beer"], 50},
                    {reactions["shout"], 30},
                    {reactions["flag"], 50},
                    {reactions["celebrate"], 10}
                },
                k => { k.enemyGoals++;  k.progress = INITIAL_PROGRESS; Debug.Log("Gol enemigo Anotado"); },
                k => { k.progress = 0; Debug.Log("Gol enemigo Fallado"); }
            )
        },
        {"ProPass",
            new GameEvent(
                new Dictionary<Reaction, int>{
                    {reactions["beer"], 20},
                    {reactions["shout"], 80},
                    {reactions["flag"], 50},
                    {reactions["celebrate"], 20}
                },
                k => { k.AddProgress(2 * k.currentLevel.deltaProgress); Debug.Log("Pase Exitoso"); },
                k => { k.AddProgress(- k.currentLevel.deltaProgress);Debug.Log("Pase Fallado"); }
            )
        },
        {"ContraPass",
            new GameEvent(
                new Dictionary<Reaction, int>{
                    {reactions["beer"], 20},
                    {reactions["shout"], 40},
                    {reactions["flag"], 60},
                    {reactions["celebrate"], 80}
                },
                k => { k.AddProgress(-2 * k.currentLevel.deltaProgress); Debug.Log("Pase enemigo Exitoso"); },
                k => { k.AddProgress(+ k.currentLevel.deltaProgress);Debug.Log("Pase enemigo Fallado"); }
            )
        },
        {"ProSweep",
            new GameEvent(
                new Dictionary<Reaction, int>{
                    {reactions["beer"], 30},
                    {reactions["shout"], 60},
                    {reactions["flag"], 60},
                    {reactions["celebrate"], 80}
                },
                k => { k.AddProgress(k.currentLevel.deltaProgress); Debug.Log("Barrida Exitosa"); },
                k => { k.AddProgress(-k.currentLevel.deltaProgress); Debug.Log("Barrida Fallada"); }
            )
        },
        {"ContraSweep",
            new GameEvent(
                new Dictionary<Reaction, int>{
                    {reactions["beer"], 30},
                    {reactions["shout"], 40},
                    {reactions["flag"], 70},
                    {reactions["celebrate"], 70}
                },
                k => { k.AddProgress(-k.currentLevel.deltaProgress); Debug.Log("Barrida enemigo Exitosa"); },
                k => { k.AddProgress(k.currentLevel.deltaProgress); Debug.Log("Barrida enemigo Fallada"); }
            )
        },
        {"ProPenalty",
            new GameEvent(
                new Dictionary<Reaction, int>{
                    {reactions["beer"], 50},
                    {reactions["shout"],20},
                    {reactions["flag"], 40},
                    {reactions["celebrate"], 70}
                },
                k => { k.progress = 100; Debug.Log("Penal Exitoso"); },
                k => { Debug.Log("Penal Fallado"); }
            )
        },
        {"ContraPenalty",
            new GameEvent(
                new Dictionary<Reaction, int>{
                    {reactions["beer"], 50},
                    {reactions["shout"], 50},
                    {reactions["flag"], 30},
                    {reactions["celebrate"], 70}
                },
                k => { k.progress = 0; Debug.Log("Penal enemigo Exitoso"); },
                k => { Debug.Log("Penal enemigo Fallado"); }
            )
        }

    };

    #endregion
	Dictionary<int, GameEvent> levelEvents = new Dictionary<int, GameEvent> ();
    bool eventComing = false;
    int eventPeriod = 4;

    #region Levels
    static List<Level> levels = new List<Level>(){
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}}),
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}}),
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}}),
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}}),
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}}),
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}}),
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}}),
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}}),
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}}),
        new Level(1, 5, -1 ,10, new List<Reaction>(reactions.Values), new Dictionary<GameEvent, int> {{events["ProPass"],80}})
		/*new Level(1, 5, 1 ,10, new List<Reaction>{reactions["beer"], reactions["flag"]},
			new Dictionary<GameEvent, int>{{events["pass"], 80}, {events["sweep"], 70}})*/
	};
    #endregion
    public int levelNumber = 0;

    // Esta instancia tiene referencias a las reacciones y eventos disponibles en cada nivel,
    // así como la variación de luck, pee y progress y el retardo de turno.
    Level currentLevel;

    public static GameManager instance = null;         

	void Awake ()
	{
		// Verifica si existe instance
		if (instance == null)
			// si no existe instance asigna a esta
			instance = this;
		else if (instance != this) {
			Destroy (gameObject);   
		}

		// Que no se elimine este objeto al cargar escenas
		DontDestroyOnLoad (gameObject);

		// Cargar Nivel Inicial
		LoadLevel(levelNumber);
	}

	void Update () {
		if (!isGameOver) {
			turnTime -= Time.deltaTime;
			elapsedTime += Time.deltaTime;
            if (isPeeing)
            {
                peeTime -= Time.deltaTime;
                if (peeTime <= 0)
                {
                    peeTime = PEE_TIME;
                    isPeeing = false;
                }
            }
            // Utiliza reacción
            if (nextTurnReaction == null) {
                if (Input.GetKey(KeyCode.A) && currentLevel.HasReaction(reactions["beer"]) && reactions["beer"].IsCool()) {
                    nextTurnReaction = reactions["beer"];
                }
            }

			// Fin del nivel
			if (elapsedTime >= GAME_DURATION) {
				
				if (teamGoals > enemyGoals) {
					// Nivel superado, cargar siguiente
					levelNumber++;
					// Ultimo nivel superado
					if (levelNumber == levels.Count)
						SceneManager.LoadScene("EndScene");
					LoadLevel (levelNumber);
				} 
				else {
					isGameOver = true;
					SceneManager.LoadScene("GameOverScene");
				}
			}

			// Cada turno
			if (turnTime <= 0) {
                // Reinicia tiempo para siguiente turno
                turnTime += currentLevel.turnDelay;

				Reaction.Update ();

				// Modificar los valores de la suerte y del progreso de acuerdo al nivel
				AddLuck (currentLevel.deltaLuck);
				ChangeProgress (currentLevel.deltaProgress);
                if (pee >= 100)
                    isPeeing = true;

                // Si no es periodo de evento y se utilizo una reacción aplicar efecto
				if (nextTurnReaction != null && ! isPeeing && ! eventComing) {
					nextTurnReaction.Apply (this);
					nextTurnReaction = null;
				}

				// Si el progress llega a 100 se dispara el evento gol de nuestro equipo.
				if (progress >= 100) {
                    Debug.Log("Tiro a gol equipo nuestro");
                    events["ProGoal"].Apply (this, reactions["beer"]); // La acción de teamGoalEvent modifica el valor de progress y Score.
				}
				else if (progress <= 0) {
                    Debug.Log("Tiro a gol equipo enemigo");
                    events["ContraGoal"].Apply (this, reactions["beer"]); // La acción de enemyGoalEvent modifica el valor de progress y Score.
				}

                // Aplicar eventos en función del tiempo
                if (levelEvents.ContainsKey((int)elapsedTime))
                {
                    Debug.Log("Ocurrio un evento en " + elapsedTime);
                    levelEvents[(int)elapsedTime].Apply(this, nextTurnReaction);
                    levelEvents.Remove((int)elapsedTime);
                    nextTurnReaction = null;
                    eventComing = false;
                }

                // Checa si ocurrira un evento pronto 
                if (levelEvents.ContainsKey((int)elapsedTime + eventPeriod))
                {
                    eventComing = true;
                }

                Debug.Log ("Suerte: " + luck.ToString() + " Avance: " + progress.ToString() + " pipi: " + pee.ToString());
                if (eventComing)
                    Debug.Log("Evento se acerca.");
            }

		}
	}

	void LoadLevel (int levelNumber){
		// Aquí se inicializan los valores del nivel
		currentLevel = levels[levelNumber];
		// reiniciar el tiempo de juego 
		elapsedTime = 0;

		// reiniciar variables
		teamGoals = enemyGoals = pee = 0;
		luck = INITIAL_LUCK;
		progress = INITIAL_PROGRESS;

		// Cargar eventos
		int eventsTime = 0;
        levelEvents.Clear();
        while (eventsTime < GAME_DURATION)
        {
            eventsTime += currentLevel.eventDelay + Random.Range(0, currentLevel.eventDelay / 2);
            GameEvent newEvent = currentLevel.GetEvent();
            if (newEvent != null)
                levelEvents.Add(eventsTime, newEvent);
        }

        // Resetear cooldowns
        foreach (KeyValuePair<string, Reaction> kvp in reactions)
        {
            kvp.Value.Cool();
        }
    }

	void AddLuck(float amount){
		luck += amount;
		if (luck >= 100)
			luck = 100;
		if (luck <= 0)
			luck = 0;
	}

	void AddProgress(int amount){
		progress += currentLevel.deltaProgress;
		if (progress > 100)
			progress = 100;
		if (progress < 0)
			progress = 0;
	}

    void AddPee(int amount)
    {
        pee += amount;
        if (pee >= 100)
            pee = 100;
        if (pee <= 0)
            pee = 0;
    }

    void ChangeProgress(int amount){
		int randomNumber = Random.Range (0, 101);
		//Debug.Log (randomNumber);
		if (randomNumber <= luck) {
			AddProgress (currentLevel.deltaProgress);
		} 
		else {
			AddProgress (-currentLevel.deltaProgress);
		}
	}

	// Gets para que usen los valores con el UI
	float GetLuck(){
		return luck;
	}
		
	int GetPee(){
		return pee;
	}

	int GetProgress(){
		return progress;
	}

    float GetElapsedTime()
    {
        return elapsedTime;
    }

    bool IsPeing()
    {
        return isPeeing;
    }
}
