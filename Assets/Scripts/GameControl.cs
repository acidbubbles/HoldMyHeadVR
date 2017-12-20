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

	public Transform podium;
	public float podiumHeight;
	// ReSharper restore InconsistentNaming

	public void Start()
	{
		podiumHeight = podium.transform.position.y;
	}

	public void Update()
	{
		podium.transform.position = new Vector3(podium.position.x, podiumHeight, podium.position.z);
		if (_currentActiveCharacter != activeCharacter)
			ChangeCharacter();

		if (_currentActiveLightsSet != activeLightsSet)
			ChangeLightSet();
	}

	private void ChangeCharacter()
	{
		if (activeCharacter < 0 || activeCharacter >= characters.Length)
		{
			activeCharacter = _currentActiveCharacter;
			return;
		}

		characters[_currentActiveCharacter].SetActive(false);
		_currentActiveCharacter = activeCharacter;
		characters[_currentActiveCharacter].SetActive(true);
	}

	private void ChangeLightSet()
	{
		if (activeLightsSet < 0 || activeLightsSet >= lightsSets.Length)
		{
			activeLightsSet = _currentActiveLightsSet;
			return;
		}
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