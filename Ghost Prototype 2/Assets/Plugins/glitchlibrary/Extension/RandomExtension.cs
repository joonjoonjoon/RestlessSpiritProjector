using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class RandomExtension
{
	public static IEnumerable<int> Range(int min, int max, int count)
	{
		var result = new List<int>();

		while (result.Distinct().Count() < count) 
		{
			result.Add(Random.Range(min, max));
		}

		return result.Distinct();
	}

    public static Vector2 insideRect(Rect rect)
    {
        float x = Random.value * rect.width;
        float y = Random.value * rect.height;
        return new Vector2(x + rect.x, y + rect.y);
    }

    public static float sign()
    {
        return (Random.Range(0, 2) - 0.5f) * 2f;
    }
    
    public static Color color()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}


