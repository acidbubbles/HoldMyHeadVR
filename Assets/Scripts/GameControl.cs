using UnityEngine;

public class GameControl : MonoBehaviour
{
	private int _currentActiveCharacter = 0;

	// ReSharper disable InconsistentNaming
	public GameObject[] characters;
	public int activeCharacter;
	// ReSharper restore InconsistentNaming

	public void Start()
	{
	}

	public void Update()
	{
		if (_currentActiveCharacter != activeCharacter)
			ChangeCharacter();
	}

	private void ChangeCharacter()
	{
		if (activeCharacter >= characters.Length) return;
		characters[_currentActiveCharacter].SetActive(false);
		_currentActiveCharacter = activeCharacter;
		characters[_currentActiveCharacter].SetActive(true);
	}
}