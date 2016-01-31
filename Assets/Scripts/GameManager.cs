using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// Esta clase se encarga de:
// controlar las variables criticas del juego (Salud, Avance y Orina)

public class GameManager : MonoBehaviour {

    #region Variables para Debug
    public static string eventOutput = "";
    public static string reactionOutput = "";
    public int randomNumber = 0;
    #endregion

    #region Constants
    public const int INITIAL_LUCK = 50;
    public const int INITIAL_PROGRESS = 50;
    public const int FAIL_PROGRESS = 30;
    public const int DELTA_PEE = 20;
    public const int PEE_TIME = 5;

    public const string PRO_PLAYER = "pro";
    public const string CONTRA_PLAYER = "contra";

    #region Nombres de eventos
    public const string PRO_GOAL = "ProGoal";
    public const string CONTRA_GOAL = "ContraGoal";
    public const string PRO_PASS = "ProPass";
    public const string CONTRA_PASS = "ContraPass";
    public const string PRO_SWEEP = "ProSweep";
    public const string CONTRA_SWEEP = "ContraSweep";
    public const string PRO_PENALTY = "ProPenalty";
    public const string CONTRA_PENALTY = "ContraPenalty";
    #endregion

    #region Nombres de Reacciones
    public const string BEER = "beer";
    public const string SHOUT = "shout";
    public const string FLAG = "flag"; 
    public const string CELEBRATE = "celebrate";
    #endregion
    #endregion

    // Duración del juego (Nivel) en segundos
    const float GAME_DURATION = 45.0f;

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
    bool inGame = false;

	Reaction nextTurnReaction = null;

    bool goalComing = false; 
    #region Reactions
    public static Dictionary<string, Reaction> reactions = new Dictionary<string, Reaction>{
        {BEER,
            new Reaction(BEER, 2,
                k =>
                {
                    k.AddPee(DELTA_PEE);
                    k.AddLuck(LUCK_MULTIPLIER * -k.currentLevel.deltaLuck);
                    // Debug
                    reactionOutput = "Cerveza, pipi +" + DELTA_PEE + " suerte +" + -k.currentLevel.deltaLuck;
                }
            )
        },
        {FLAG,
            new Reaction(FLAG, 5,
                k =>
                {
                    k.AddLuck(-k.currentLevel.deltaLuck);
                   reactionOutput = "Bandera, suerte +" + -k.currentLevel.deltaLuck;
                }
            )
        },
        {SHOUT,
            new Reaction(SHOUT, 7,
                k =>
                {
                    k.AddLuck(-k.currentLevel.deltaLuck);
                     reactionOutput = "Grito, suerte +" + -k.currentLevel.deltaLuck;
                }
            )
        },
        {CELEBRATE,
            new Reaction(CELEBRATE, 10,
                k =>
                {
                    k.AddLuck(LUCK_MULTIPLIER * -k.currentLevel.deltaLuck);
                     reactionOutput = "Celebración, suerte +" + LUCK_MULTIPLIER * -k.currentLevel.deltaLuck;
                }
            )
        }
    };
    #endregion

    #region GameEvents
    public static Dictionary<string, GameEvent> events = new Dictionary<string, GameEvent>{
		{PRO_GOAL,
            new GameEvent(PRO_GOAL,
                new Dictionary<Reaction, int>{
                    {reactions[BEER], 50},
                    {reactions[SHOUT], 70},
                    {reactions[FLAG], 50},
                    {reactions[CELEBRATE], 80}
                },
                k => { k.teamGoals++; k.progress = INITIAL_PROGRESS;  eventOutput = "Gol Anotado"; },
                k => { k.progress = 70; eventOutput = "Gol Fallado"; }
            )
        },
        {CONTRA_GOAL,
            new GameEvent(CONTRA_GOAL,
                new Dictionary<Reaction, int>{
                    {reactions[BEER], 50},
                    {reactions[SHOUT], 90},
                    {reactions[FLAG], 50},
                    {reactions[CELEBRATE], 10}
                },
                k => { k.progress = 0; eventOutput = "Gol enemigo Fallado"; },
                k => { k.enemyGoals++;  k.progress = INITIAL_PROGRESS; eventOutput = "Gol enemigo Anotado";}
            )
        },
        {PRO_PASS,
            new GameEvent(PRO_PASS,
                new Dictionary<Reaction, int>{
                    {reactions[BEER], 20},
                    {reactions[SHOUT], 80},
                    {reactions[FLAG], 50},
                    {reactions[CELEBRATE], 20}
                },
                k => { k.AddProgress(2 * k.currentLevel.deltaProgress);eventOutput = "Pase Exitoso"; },
                k => { k.AddProgress(- k.currentLevel.deltaProgress);eventOutput = "Pase Fallado"; }
            )
        },
        {CONTRA_PASS,
            new GameEvent(CONTRA_PASS,
                new Dictionary<Reaction, int>{
                    {reactions[BEER], 20},
                    {reactions[SHOUT], 40},
                    {reactions[FLAG], 60},
                    {reactions[CELEBRATE], 80}
                },
                k => { k.AddProgress(+ k.currentLevel.deltaProgress);eventOutput = "Pase enemigo Fallado"; },
                k => { k.AddProgress(-2 * k.currentLevel.deltaProgress); eventOutput = "Pase enemigo Exitoso"; }
            )
        },
        {PRO_SWEEP,
            new GameEvent(PRO_SWEEP,
                new Dictionary<Reaction, int>{
                    {reactions[BEER], 30},
                    {reactions[SHOUT], 60},
                    {reactions[FLAG], 60},
                    {reactions[CELEBRATE], 80}
                },
                k => { k.AddProgress(k.currentLevel.deltaProgress);eventOutput = "Barrida Exitosa"; },
                k => { k.AddProgress(-k.currentLevel.deltaProgress); eventOutput = "Barrida Fallada"; }
            )
        },
        {CONTRA_SWEEP,
            new GameEvent(CONTRA_SWEEP,
                new Dictionary<Reaction, int>{
                    {reactions[BEER], 30},
                    {reactions[SHOUT], 40},
                    {reactions[FLAG], 70},
                    {reactions[CELEBRATE], 70}
                },
                k => { k.AddProgress(k.currentLevel.deltaProgress); eventOutput = "Barrida enemigo Fallada"; },
                k => { k.AddProgress(-k.currentLevel.deltaProgress); eventOutput = "Barrida enemigo Exitosa"; }
            )
        },
        {PRO_PENALTY,
            new GameEvent(PRO_PENALTY,
                new Dictionary<Reaction, int>{
                    {reactions[BEER], 50},
                    {reactions[SHOUT],20},
                    {reactions[FLAG], 40},
                    {reactions[CELEBRATE], 70}
                },
                k => { k.progress = 100; eventOutput = "Penal Exitoso"; },
                k => { eventOutput = "Penal Fallado"; }
            )
        },
        {CONTRA_PENALTY,
            new GameEvent(CONTRA_PENALTY,
                new Dictionary<Reaction, int>{
                    {reactions[BEER], 50},
                    {reactions[SHOUT], 50},
                    {reactions[FLAG], 30},
                    {reactions[CELEBRATE], 70}
                },
                k => { eventOutput = "Penal enemigo Fallado"; },
                k => { k.progress = 0; eventOutput = "Penal enemigo Exitoso"; }    
            )
        }

    };

    #endregion
	Dictionary<int, GameEvent> levelEvents = new Dictionary<int, GameEvent> ();
    bool eventComing = false;
    int eventPeriod = 2;

    #region Levels
    static List<Level> levels = new List<Level>(){
        new Level(2, 10, -2 ,10, new Dictionary<GameEvent, int> {
            { events[PRO_PASS],70},
            { events[PRO_SWEEP],60},
            { events[PRO_PENALTY],50}
        }),
        new Level(2, 9, -3 ,10, new Dictionary<GameEvent, int> {
            { events[PRO_PASS],60},
            { events[CONTRA_PASS],50},
            { events[PRO_PENALTY],70}
        }),
        new Level(1, 9, -4, 8, new Dictionary<GameEvent, int> {
            { events[PRO_PASS],50},
            { events[CONTRA_PASS],50},
            { events[PRO_SWEEP],50},
            { events[CONTRA_SWEEP],50},
            { events[PRO_PENALTY],40}
        }),
        new Level(1, 8, -5, 6, new Dictionary<GameEvent, int> {
            { events[PRO_PASS],50},
            { events[CONTRA_PASS],60},
            { events[PRO_SWEEP],50},
            { events[CONTRA_SWEEP],60},
            { events[PRO_PENALTY],40},
            { events[CONTRA_PENALTY],70}
        }),
    };
    #endregion
    int levelNumber = 0;

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

    void Update() {
        if (inGame && !isGameOver)
        {
            turnTime -= Time.deltaTime;
            elapsedTime += Time.deltaTime;

            if (isPeeing)
            {
                peeTime -= Time.deltaTime;
                if (peeTime <= 0)
                {
                    peeTime = PEE_TIME;
                    isPeeing = false;
                    pee = 0;
                }
            }

            // Utiliza reacción (Modifican para que esto se haga con click)         
            if (Input.GetKey(KeyCode.A))
            {
                SetActiveReaction(GameManager.BEER);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                SetActiveReaction(GameManager.SHOUT);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                SetActiveReaction(GameManager.FLAG);
            }
            else if (Input.GetKey(KeyCode.F))
            {
                SetActiveReaction(GameManager.CELEBRATE);
            }

            // Fin del nivel
            if (elapsedTime >= GAME_DURATION)
            {

                if (teamGoals > enemyGoals)
                {
                    // Nivel superado, cargar siguiente
                    levelNumber++;
                    inGame = false;

                    // Ultimo nivel superado
                    if (levelNumber == levels.Count)
                    {
                        isGameOver = true;
                        SceneManager.LoadScene("EndScene");
                    }

                    LoadLevel(levelNumber);
                    return;
                }
                else {
                    inGame = false;
                    isGameOver = true;
                    SceneManager.LoadScene("GameOverScene");
                }
            }

            // Cada turno
            if (turnTime <= 0)
            {
                // Reinicia tiempo para siguiente turno
                turnTime += currentLevel.turnDelay;

                Reaction.Update();

                // Modificar los valores de la suerte y del progreso de acuerdo al nivel
                AddLuck(currentLevel.deltaLuck);
                ChangeProgress(currentLevel.deltaProgress);
                if (pee >= 100)
                    isPeeing = true;

                // Si no es periodo de evento y se utilizo una reacción aplicar efecto
                if (nextTurnReaction != null && !isPeeing && !eventComing && !goalComing)
                {
                    nextTurnReaction.Apply(this);
                    nextTurnReaction = null;
                }

                // Si el progress es cercano al 100, prepararse para reaccionar a tiro de gol.
                if (ProGoalNear() || ContraGoalNear())
                {
                    goalComing = true;
                }

                // Si estuviste cerca de Gol pero ya no 
                if (!ProGoalNear() && !ContraGoalNear() && goalComing)
                {
                    goalComing = false;
                }

                // Aplicar eventos en función del tiempo
                if (levelEvents.ContainsKey((int)elapsedTime))
                {
                    if (!goalComing)
                        levelEvents[(int)elapsedTime].Apply(this, nextTurnReaction);
                    levelEvents.Remove((int)elapsedTime);
                    eventComing = false;
                }

                // Si el progress llega a 100 se dispara el evento gol de nuestro equipo.
                if (progress >= 100)
                {
                    events[PRO_GOAL].Apply(this, nextTurnReaction); // La acción de teamGoalEvent modifica el valor de progress y Score.
                    goalComing = false;
                }
                else if (progress <= 0)
                {
                    events[CONTRA_GOAL].Apply(this, nextTurnReaction); // La acción de enemyGoalEvent modifica el valor de progress y Score.
                    goalComing = false;
                }

                // Checa si ocurrira un evento pronto 
                if (levelEvents.ContainsKey((int)elapsedTime + eventPeriod))
                {
                    eventComing = true;
                }
            }
        } 
        else if (!isGameOver)
        {
            Debug.Log("Presiona para comenzar.");
            if (Input.anyKey) inGame = true;
        }
	}

	void LoadLevel (int levelNumber){
		// Aquí se inicializan los valores del nivel
		currentLevel = levels[levelNumber];
		// reiniciar el tiempo de juego 
		elapsedTime = 0;

		// Inicializar variables
		teamGoals = enemyGoals = pee = 0;
		luck = INITIAL_LUCK;
		progress = INITIAL_PROGRESS;
        turnTime = currentLevel.turnDelay;
        nextTurnReaction = null;
        eventComing = false;
        goalComing = false;

		// Cargar eventos
		int eventsTime = 0;
        levelEvents.Clear();
        while (eventsTime < GAME_DURATION)
        {
            int Offset = Random.Range(-currentLevel.eventDelay / 2, currentLevel.eventDelay / 2);
            eventsTime += currentLevel.eventDelay + Offset;
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
		progress += amount;
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
        // Sustituir después
        //int randomNumber = Random.Range(0, 101);
        randomNumber = Random.Range (0, 101);
		//Debug.Log (randomNumber);
		if (randomNumber <= luck) {
			AddProgress (currentLevel.deltaProgress);
		} 
		else {
			AddProgress (-currentLevel.deltaProgress);
		}
	}
	// Gets para que usen los valores con el UI
	public float GetLuck(){
		return luck;
	}
		
	public int GetPee(){
		return pee;
	}

	public int GetProgress(){
		return progress;
	}

    public  float GetElapsedTime()
    {
        return elapsedTime;
    }

    public bool IsPeing()
    {
        return isPeeing;
    }

    public bool IsEventComing()
    {
        return eventComing;
    }

    public string NextEvent()
    {
        if (levelEvents.Count > 0)
            return levelEvents.OrderBy(k => k.Key).First().Value.GetName();
        return "";
    }

    public int GetScore(string team)
    {
        if (team == PRO_PLAYER)
        {
            return teamGoals;
        }
        else if (team == CONTRA_PLAYER)
        {
            return enemyGoals;
        }
        return -1;
    }

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public bool ProGoalNear()
    {
        return (progress >= 85 && progress <= 99);
    }

    public bool ContraGoalNear()
    {
        return (progress >= 1 && progress <= 15);
    }

    public string GetActiveReaction()
    {
        if (nextTurnReaction != null)
        {
            return nextTurnReaction.GetName();
        }
        return "";
    }

    public void SetActiveReaction(string name)
    {
        if (nextTurnReaction == null && reactions[name].IsCool())
        {
            nextTurnReaction = reactions[name];
        }
    }

    public bool GameOver()
    {
        return isGameOver;
    }
}
