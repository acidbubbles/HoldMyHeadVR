using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MouthAudioSource : MonoBehaviour
{
	private readonly List<Ticket> _tickets = new List<Ticket>();
	private AudioSource _source;

	public class Ticket
	{
		public bool IsHumping { get; set; }

		public void Update(bool isHumping)
		{
			IsHumping = isHumping;
		}
	}

	public Ticket GetTicket()
	{
		var ticket = new Ticket();
		_tickets.Add(ticket);
		return ticket;
	}

	public void Start()
	{
		_source = GetComponent<AudioSource>();
	}

	public void Update()
	{
		var isHumping = _tickets.Any(t => t.IsHumping);
		if (isHumping && !_source.isPlaying)
		{
			_source.Play();
			Debug.Log("Start");
		}
		else if (!isHumping && _source.isPlaying)
		{
			_source.Stop();
			Debug.Log("Stop");
		}
	}
}
