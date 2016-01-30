using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// Esta clase se encarga de:
// controlar las variables criticas del juego (Salud, Avance y Orina)

public class GameManager : MonoBehaviour {
	
	int luck;
	int pee;
	int progress;

	public int levelNumber = 0;
	int teamGoals = 0;
	int enemyGoals = 0;
	
	// Duración del juego (Nivel) en segundos
	const float GAME_DURATION = 90.0f;
	float elapsedTime;

	float turnTime;

	bool isGameOver = false;

	// Estos niveles son de prueba
	static List<Level> levels = new List<Level>(){new Level(1.0f, 2.0f, 3 ,4), new Level(1.0f, 2.0f, 3 ,4), new Level(1.0f, 2.0f, 3 ,4)};
	// Esta instancia tiene referencias a las reacciones y eventos disponibles en cada nivel,
	// así como la variación de luck, pee y progress y el retardo de turno.
	Level currentLevel;

	//GameEvent CurrentEvent = null;
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
			Debug.Log (elapsedTime.ToString ());
			if (elapsedTime >= GAME_DURATION) {
				Debug.Log ("Se acabo el Tiempo");
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
				ChangeProgress(currentLevel.deltaProgress);

				// Si el progress llega a 100 se dispara el evento gol de nuestro equipo.
				if (progress >= 100) {
					// TeamGoal.action(this) // La acción de TeamGoal modifica el valor de progress y Score.
				}
				else if (progress <= 0) {
					// EnemyGoal.action(this) // La acción de EnemyGoal modifica el valor de progress y Score. 
				}
			}

		}
	}

	void LoadLevel (int levelNumber){
		// Aquí se inicializan los valores del nivel
		currentLevel = levels[levelNumber];
		Debug.Log ("Se cargo el nivel " + levelNumber.ToString());
		// reiniciar el tiempo de juego 
		elapsedTime = 0;

		// reiniciar el puntaje
		teamGoals = enemyGoals = 0;

		// Cargar eventos
	}

	void AddLuck(int amount){
		luck += amount;
		if (luck >= 100)
			luck = 100;
		if (luck <= 0)
			luck = 0;
	}

	void ChangeProgress(int amount){
		int randomNumber = (int)Random.Range (0.0f, 100.0f);
		if (randomNumber < luck) {
			progress -= currentLevel.deltaProgress;
			if (progress < 0)
				progress = 0;
		} 
		else {
			progress += currentLevel.deltaProgress;
			if (progress > 100)
				progress = 100;
		}
	}

}
