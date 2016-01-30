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

	int luck;
	int pee;
	int progress;

	public int levelNumber = 0;
	int teamGoals = 0;
	int enemyGoals = 0;
	
	// Duración del juego (Nivel) en segundos
	const float GAME_DURATION = 60.0f;
	float elapsedTime;

	float turnTime;

	bool isGameOver = false;

	Reaction nextTurnReaction = null;

	static Reaction beer = new Reaction(5, k => { k.pee += 20; k.luck += 5; Debug.Log("Tomaste cerveza, pipi +20 y suerte +5");});

	static GameEvent teamGoalEvent = new GameEvent(new Dictionary<Reaction, int>{{beer, 90}},
		k => {k.teamGoals++; k.progress = INITIAL_PROGRESS; Debug.Log("Gol Anotado");},
		k => {k.progress-=FAIL_PROGRESS; Debug.Log("Gol Fallado");}
	);

	static GameEvent enemyGoalEvent = new GameEvent(new Dictionary<Reaction, int>{{beer, 90}},
		k => {k.enemyGoals++; k.progress = INITIAL_PROGRESS; Debug.Log("Gol Anotado");},
		k => {k.progress+=FAIL_PROGRESS; Debug.Log("Gol Fallado");}
	);

	// Estos niveles son de prueba
	static List<Level> levels = new List<Level>(){
		new Level(1, -1 ,10), 
		new Level(1, -3 ,5), 
		new Level(1, -3 ,5)
	};

	// Esta instancia tiene referencias a las reacciones y eventos disponibles en cada nivel,
	// así como la variación de luck, pee y progress y el retardo de turno.
	Level currentLevel;

	int eventIndex;
	//List<GameEvent> events = new List<GameEvent> ();

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

			// Utiliza reacción
			if (nextTurnReaction == null && Input.GetKey (KeyCode.Space) && beer.IsCool()) {
				nextTurnReaction = beer;
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

				// Modificar los valores de la suerte y del progreso de acuerdo al nivel
				AddLuck (currentLevel.deltaLuck);
				ChangeProgress (currentLevel.deltaProgress);

				if (nextTurnReaction != null) {
					nextTurnReaction.Apply (this);
					nextTurnReaction = null;
				}

				// Si el progress llega a 100 se dispara el evento gol de nuestro equipo.
				if (progress >= 100) {
					teamGoalEvent.Apply (this, beer); // La acción de teamGoalEvent modifica el valor de progress y Score.
					Debug.Log("Tiro a gol equipo nuestro");

				}
				else if (progress <= 0) {
					enemyGoalEvent.Apply (this, beer); // La acción de enemyGoalEvent modifica el valor de progress y Score.
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
		teamGoals = enemyGoals = pee = eventIndex = 0;
		luck = INITIAL_LUCK;
		progress = INITIAL_PROGRESS;

		// Cargar eventos
		int eventsTime = 0;
		//while (eventsTime < GAME_DURATION) {
			
		//}
	}

	void AddLuck(int amount){
		luck += amount;
		if (luck >= 100)
			luck = 100;
		if (luck <= 0)
			luck = 0;
	}

	void ChangeProgress(int amount){
		int randomNumber = Random.Range (0, 101);
		Debug.Log (randomNumber);
		if (randomNumber <= luck) {
			progress += currentLevel.deltaProgress;
			if (progress > 100)
				progress = 100;
		} 
		else {
			progress -= currentLevel.deltaProgress;
			if (progress < 0)
				progress = 0;
		}
	}

}
