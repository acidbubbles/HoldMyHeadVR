using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioStartRandomize : MonoBehaviour
{
	private AudioSource _source;

	// ReSharper disable InconsistentNaming
	public float clipDuration;
	// ReSharper restore InconsistentNaming

	public void Start()
	{
		_source = GetComponent<AudioSource>();
		_source.time = Random.Range(0, clipDuration);
		_source.Play();
	}
}
