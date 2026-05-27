using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ContextCommands
{
	private static int AutoSortThreshold = 999;

	[MenuItem("CONTEXT/Transform/Auto Order in Layer")]
	private static void AutoOrderInLayer(MenuCommand command)
	{
		int n = 0;

		Transform rootTransform = command.context as Transform;
		if (rootTransform == null)
			return;

		Debug.Log("Start - Auto Order in Layer");

		AutoOrderInLayer_Helper(rootTransform, ref n);

		Debug.Log("Complete - Auto Order in Layer");
	}

	private static void AutoOrderInLayer_Helper(Transform t, ref int n)
	{
		if (n > AutoSortThreshold)
			return;

		SpriteRenderer sprite = t.GetComponent<SpriteRenderer>();
		if (sprite != null)
		{
			if (sprite.sortingOrder >= 0 && sprite.sortingOrder <= AutoSortThreshold)
			{
				Debug.Log(string.Format("Setting sort order of {0} to {1}", t.name, n));
				sprite.sortingOrder = n;
				++n;
			}
		}

		foreach (Transform childObject in t)
		{
			AutoOrderInLayer_Helper(childObject, ref n);
		}
	}
}
