using UnityEngine;

public class GameControl : MonoBehaviour
{
	private int _currentActiveCharacter;
	private int _currentActiveLightsSet;

	// ReSharper disable InconsistentNaming
	public GameObject[] characters;
	public int activeCharacter;

	public GameObject[] lightsSets;
	public int activeLightsSet;
	// ReSharper restore InconsistentNaming

	public void Start()
	{
	}

	public void Update()
	{
		if (_currentActiveCharacter != activeCharacter)
			ChangeCharacter();

		if (_currentActiveLightsSet != activeLightsSet)
			ChangeLightSet();
	}

	private void ChangeCharacter()
	{
		if (activeCharacter >= characters.Length) return;
		characters[_currentActiveCharacter].SetActive(false);
		_currentActiveCharacter = activeCharacter;
		characters[_currentActiveCharacter].SetActive(true);
	}

	private void ChangeLightSet()
	{
		if (activeLightsSet >= lightsSets.Length) return;
		lightsSets[_currentActiveLightsSet].SetActive(false);
		_currentActiveLightsSet = activeLightsSet;
		var lightsSet = lightsSets[_currentActiveLightsSet];
		lightsSet.SetActive(true);
		var lightScript = lightsSet.GetComponent<LightsSet>();
		RenderSettings.ambientIntensity = lightScript.IntensityMultiplier;
		RenderSettings.ambientSkyColor = lightScript.SkyColor;
		RenderSettings.skybox = lightScript.SkyboxMaterial;
	}
}