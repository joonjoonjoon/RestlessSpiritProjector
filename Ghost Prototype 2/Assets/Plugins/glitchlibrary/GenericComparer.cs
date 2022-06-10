
using System.Collections;
using System.Collections.Generic;
using System;

public class GenericComparer<T> : IEqualityComparer<T>
{
	public GenericComparer(Func<T, T, bool> cmp)
	{
		this.cmp = cmp;
	}
	public bool Equals(T x, T y)
	{
		return cmp(x, y);
	}

	public int GetHashCode(T obj)
	{
		return obj.GetHashCode();
	}

	public Func<T, T, bool> cmp { get; set; }
}