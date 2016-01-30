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
    // Duración del juego (Nivel) en segundos
    const float GAME_DURATION = 60.0f;

    float luck;
	int pee;
	int progress;

	int teamGoals = 0;
	int enemyGoals = 0;

    float elapsedTime;
	float turnTime;

	bool isGameOver = false;

	public int levelNumber = 0;

	// Esta instancia tiene referencias a las reacciones y eventos disponibles en cada nivel,
	// así como la variación de luck, pee y progress y el retardo de turno.
	Level currentLevel;

	Reaction nextTurnReaction = null;

    static Reaction beer = new Reaction(5,
        k =>
        {
            k.pee += DELTA_PEE;
            k.luck += 2 * k.currentLevel.deltaLuck;
            Debug.Log("Tomaste cerveza, pipi +" + DELTA_PEE + " y suerte +" + 2 * k.currentLevel.deltaLuck);
        }
    );

    #region Reactions
    static Dictionary<string, Reaction> reactions = new Dictionary<string, Reaction>{
		{"beer",
        new Reaction(5,
            k =>
            {
                k.pee += DELTA_PEE;
                k.luck += 2 * k.currentLevel.deltaLuck;
                Debug.Log("Tomaste cerveza, pipi +" + DELTA_PEE + " y suerte +" + 2 * k.currentLevel.deltaLuck);
            }
        )
        }
	};
    #endregion

    #region GameEvents
    static Dictionary<string, GameEvent> events = new Dictionary<string, GameEvent>{
		{"teamGoal",
        new GameEvent(
            new Dictionary<Reaction, int>{
                {beer, 50},
            },
            k => { k.teamGoals++; k.progress = INITIAL_PROGRESS; Debug.Log("Gol Anotado"); },
            k => { k.progress -= FAIL_PROGRESS; Debug.Log("Gol Fallado"); }
        )
        },
        {"pass",
        new GameEvent(
            new Dictionary<Reaction, int>{
                {beer, 50},
            },
            k => {k.AddProgress(k.currentLevel.deltaProgress); Debug.Log("Pase Exitoso"); },
            k => {k.AddProgress(-k.currentLevel.deltaProgress); Debug.Log("Pase Fallado"); }
        )
        }
    };
    #endregion
	Dictionary<int, GameEvent> levelEvents = new Dictionary<int, GameEvent> ();

    static List<Level> levels = new List<Level>(){
        new Level(1, 5, -1 ,10, new List<Reaction> { reactions["beer"] }, new Dictionary<GameEvent, int> {{events["pass"],80}}),
        new Level(1, 5, -1 ,10, null, null),
        new Level(1, 5, -1 ,10, null, null),
		/*new Level(1, 5, 1 ,10, new List<Reaction>{reactions["beer"], reactions["flag"]},
			new Dictionary<GameEvent, int>{{events["pass"], 80}, {events["sweep"], 70}})*/
	};

	public static GameManager instance = null;         

	void Awake()
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

			if (reactions["beer"].IsCool ())
				Debug.Log ("Cerveza recargada");

			// Utiliza reacción
			if (nextTurnReaction == null && Input.GetKey (KeyCode.Space) && reactions["beer"].IsCool()) {
				nextTurnReaction = reactions["beer"];
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

				if (nextTurnReaction != null) {
					nextTurnReaction.Apply (this);
					nextTurnReaction = null;
				}

				// Si el progress llega a 100 se dispara el evento gol de nuestro equipo.
				if (progress >= 100) {
					events["teamGoal"].Apply (this, reactions["beer"]); // La acción de teamGoalEvent modifica el valor de progress y Score.
					Debug.Log("Tiro a gol equipo nuestro");

				}
				else if (progress <= 0) {
					events["enemyGoal"].Apply (this, reactions["beer"]); // La acción de enemyGoalEvent modifica el valor de progress y Score.
					Debug.Log("Tiro a gol equipo enemigo");
				}

				Debug.Log ("Paso un turno, La suerte es: " + luck.ToString() + " El avance es: " + progress.ToString() + " La pipi: " + pee.ToString());
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
		while (eventsTime < GAME_DURATION) {
			eventsTime += currentLevel.eventDelay + Random.Range (0, currentLevel.eventDelay/2);
			levelEvents.Clear ();
			levelEvents.Add (eventsTime, currentLevel.GetEvent ());
		}
		Debug.Log (events.ToString ());
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

	void ChangeProgress(int amount){
		int randomNumber = Random.Range (0, 101);
		Debug.Log (randomNumber);
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

}
