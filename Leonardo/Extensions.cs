using MatterHackers.Csg.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leonardo
{
	public static class Extensions
	{
		public static string GetScadOutputRecursive(this OpenSCadOutput oso, AwesomeSolid awesome, int level = 0)
		{
			return oso.GetScadOutputRecursive(awesome.InnerObject);
		}
	}
}
