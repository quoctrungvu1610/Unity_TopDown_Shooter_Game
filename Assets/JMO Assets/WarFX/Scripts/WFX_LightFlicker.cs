using UnityEngine;
using System.Collections;

/**
 *	Rapidly sets a light on/off.
 *	
 *	(c) 2015, Jean Moreno
**/

[RequireComponent(typeof(Light))]
public class WFX_LightFlicker : MonoBehaviour
{
	public float time = 0.17f;
	
	private float timer;

    //private void OnEnable()
    //{
    //    GetComponent<Light>().enabled = true;
    //}


    private void Update()
    {
        time -= Time.deltaTime;
		if(time <= 0)
		{
			time = 0.17f;
			this.gameObject.SetActive(false);
        }
    }


 //   void Start ()
	//{
	//	timer = time;
	//	StartCoroutine("Flicker");
	//}
	
	//IEnumerator Flicker()
	//{
	//	while(true)
	//	{
	//		GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
			
	//		do
	//		{
	//			timer -= Time.deltaTime;
	//			yield return null;
	//		}
	//		while(timer > 0);
	//		timer = time;

	//	}
	//}
}
