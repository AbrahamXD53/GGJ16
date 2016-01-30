using UnityEngine;
using System.Collections;

public class SuperAnimator : MonoBehaviour {

    public Vector3 rotation;
    public Vector3 scale;
    public float speedScale=1;
    public float speedRotation=1;

    private Vector3 initialScale;
    private Vector3 initialRotation;
	void Start () {
        initialRotation = transform.rotation.eulerAngles;
        initialScale = transform.localScale;
	}
	
	void Update () {
        transform.localScale = initialScale+ (scale * Mathf.Sin(speedScale * Time.timeSinceLevelLoad));
        transform.localRotation= Quaternion.Euler(initialRotation+ (rotation * Mathf.Sin(speedRotation * Time.timeSinceLevelLoad)));
    }
}
