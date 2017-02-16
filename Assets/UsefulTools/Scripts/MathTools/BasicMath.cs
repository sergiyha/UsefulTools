using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BasicMath
{
	public class LeastCommonMultiple
	{
		public static int gcf(int a, int b)
		{
			while (b != 0)
			{
				int temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}
	}
}
