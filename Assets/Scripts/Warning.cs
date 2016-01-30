using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Warning : MonoBehaviour {


    private Camera m_camera;
    public float epCount=10;
    public float duration = 3;
    private float interval;
    public Text txtWarning;
	void Start() {
        m_camera = GetComponent<Camera>();
        interval = duration / epCount;
    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.anyKeyDown || (Input.touchCount>0 && Input.GetTouch(0).phase==TouchPhase.Began))
        {
            iTween.ShakePosition(txtWarning.transform.parent.GetChild(1).gameObject, new Vector3(Random.Range(0, 20), Random.Range(0, 20), 0), duration*10);
            StartCoroutine(Epilepsia());
        }
	}
    
    IEnumerator Epilepsia()
    {
        print("called");
        while((epCount--)>1)
        {
            m_camera.backgroundColor = new Color(Random.Range(20,100)/100f, Random.Range(20, 100) / 100f, Random.Range(20, 100) / 100f, 1);
            print(m_camera.backgroundColor);
            yield return new WaitForSeconds(interval);
        }
        m_camera.backgroundColor = new Color(1,1,1);
        txtWarning.text = "Ah!... También puede causar epilepsia a personas fotosensibles no me pregunten no soy 100tifiko";
        yield return 0;
    }
}
