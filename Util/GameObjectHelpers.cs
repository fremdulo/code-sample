using UnityEngine;

public class GameObjectHelpers : MonoBehaviour
{
	public void DestroySelf()
	{
		Destroy(gameObject);
	}

	public void ActivateSelf()
	{
		gameObject.SetActive(true);
	}

	public void DeactivateSelf()
	{
		gameObject.SetActive(true);
	}

	public void InstantiateObject(GameObject obj, Transform spawnPos)
	{
		Instantiate(obj, spawnPos.position, Quaternion.identity);
	}

	public void InstantiateObject(GameObject obj)
	{
		Instantiate(obj, Vector3.zero, Quaternion.identity);
	}
}
