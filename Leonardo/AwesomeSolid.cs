using MatterHackers.Csg;
using MatterHackers.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leonardo
{
	public abstract class AwesomeSolid : CsgObject
	{
		public abstract List<Vector3> AttachPoints
		{
			get;
		}

		public abstract double AverageSize
		{
			get;
		}

		public abstract CsgObject InnerObject
		{
			get;
		}

		public AwesomeSolid(string name) : base(name)
		{

		}

		public AwesomeSolid(Dictionary<string, string> properties) : base(properties)
		{

		}

		public AwesomeSolid(Vector3 size, string name = "", bool createCentered = true) : base(name)
		{

		}
	}
}
