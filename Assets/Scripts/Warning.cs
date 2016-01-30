using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Warning : MonoBehaviour
{

    private Camera m_camera;
    public float epCount = 10;
    public float duration = 3;
    private float interval;
    public Text txtWarning;
    private bool canClic = false;

    IEnumerator coldDown()
    {
        canClic = false;
        yield return new WaitForSeconds(2f);
        canClic = true;
    }
    void Start()
    {
        m_camera = GetComponent<Camera>();
        interval = duration / epCount;
        StartCoroutine(coldDown());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) && canClic)
        {
            if (epCount > 1)
            {
                iTween.ShakePosition(txtWarning.transform.parent.GetChild(1).gameObject, new Vector3(Random.Range(0, 20), Random.Range(0, 20), 0), duration * 10);
                StartCoroutine(Epilepsia());
            }
            else
            {
                //Cargar la escena chida
                print("Cargar la chida");
                SceneManager.LoadScene(1);
                SceneManager.LoadScene(2,LoadSceneMode.Additive);
            }
        }

    }

    IEnumerator Epilepsia()
    {
        while ((epCount--) > 1)
        {
            m_camera.backgroundColor = new Color(Random.Range(20, 100) / 100f, Random.Range(20, 100) / 100f, Random.Range(20, 100) / 100f, 1);
            print(m_camera.backgroundColor);
            yield return new WaitForSeconds(interval);
        }
        m_camera.backgroundColor = new Color(1, 1, 1);
        txtWarning.text = "Ah!... También puede causar epilepsia a personas fotosensibles no me pregunten no soy 100tifiko";
        StartCoroutine(coldDown());
        yield return 0;
    }
}
