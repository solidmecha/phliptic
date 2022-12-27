using UnityEngine;
using System.Collections;

public class RotateIt : MonoBehaviour {

    float counter;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        counter += Time.deltaTime;
        transform.Rotate(new Vector3(0, 360,0) * Time.deltaTime);
        if (counter >= .5f)
        {
            float y = Mathf.Round(transform.eulerAngles.y);
            transform.eulerAngles = new Vector2(0, y);
            Destroy(this);
        }
	
	}
}
