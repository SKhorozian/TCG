using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexagonMetrics
{
	public const float outerRadius = 0.5f;
	public const float innerRadius = outerRadius * 0.866025404f;

	public static int GetDistantce (Vector2 a, Vector2 b) {

		int dx = Mathf.Abs((int)a.x - (int)b.x);
		int dy = Mathf.Abs((int)a.y - (int)b.y);

		return dx + Mathf.Max (0, (dy-dx)/2);
	}

}
