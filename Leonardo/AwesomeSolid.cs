using MatterHackers.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leonardo
{
	public abstract class AwesomeSolid : CsgSolid
	{
		public abstract List<Vector3> AttachPoints
		{
			get;
		}

		public abstract double AverageSize
		{
			get;
		}
	}
}
