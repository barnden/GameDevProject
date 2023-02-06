using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBarScript : MonoBehaviour
{
	private Slider slider;
	private ParticleSystem particleSys;
	
	public float FillSpeed = 0.5f;
	private float targetProgress = 0;
	
	private void Awake()
	{
		slider = gameObject.GetComponent<Slider>();
		particleSys = GameObject.Find("FillEffects").GetComponent<ParticleSystem>();
	}
    // Start is called before the first frame update
    void Start()
    {
        IncrementProgress(0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value < targetProgress)
		{
			slider.value += FillSpeed * Time.deltaTime;
			if (!particleSys.isPlaying)
				particleSys.Play();
		}
		else
		{
			particleSys.Stop();
		}
    }
	
	//Add progress to bar
	public void IncrementProgress(float newProgress)
	{
		targetProgress = slider.value + newProgress;
	}
}
