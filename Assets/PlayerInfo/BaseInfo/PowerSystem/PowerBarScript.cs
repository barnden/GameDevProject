using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBarScript : MonoBehaviour
{
	[SerializeField]
	Power powerObject;

	private Slider slider;
	private ParticleSystem particleSys;
	
	public float FillSpeed = 0.5f;
	public float targetProgress = 0;
	
	private void Awake()
	{
		slider = gameObject.GetComponent<Slider>();
		particleSys = GameObject.Find("FillEffects").GetComponent<ParticleSystem>();
	}

	void Update()
    {
		slider.value = powerObject.getPower() / powerObject.getMaxPower();

        if (slider.value < targetProgress)
		{
			slider.value += FillSpeed * Time.deltaTime;
			if (!particleSys.isPlaying)
			{
				particleSys.Play();
			}
		}
		else
		{
			particleSys.Stop();
		}
    }
}
