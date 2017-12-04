using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Blink : MonoBehaviour
{
	SkinnedMeshRenderer skinnedMeshRenderer;
	private float defaultTopLidValue;
	private float defaultBottomLidValue;

	public int topLidIndex;
	public int bottomLidIndex;
	public float topLidCloseValue;
	public float bottomLidCloseValue;

	void Awake()
	{
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		defaultTopLidValue = skinnedMeshRenderer.GetBlendShapeWeight(topLidIndex);
		defaultBottomLidValue = skinnedMeshRenderer.GetBlendShapeWeight(bottomLidIndex);
	}

	void Start()
	{
		StartCoroutine(DoBlink());
	}

	private IEnumerator DoBlink()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(1.0f, 8.0f));

			skinnedMeshRenderer.SetBlendShapeWeight(topLidIndex, topLidCloseValue);
			skinnedMeshRenderer.SetBlendShapeWeight(bottomLidIndex, bottomLidCloseValue);
			yield return new WaitForSeconds(0.1f);

			skinnedMeshRenderer.SetBlendShapeWeight(topLidIndex, defaultTopLidValue);
			skinnedMeshRenderer.SetBlendShapeWeight(bottomLidIndex, defaultBottomLidValue);
		}
	}
}
