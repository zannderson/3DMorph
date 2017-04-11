using MatterHackers.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leonardo
{
	public interface IAwesomeSolid
	{
		List<Vector3> GetAttachPoints();

		double GetAverageSize();
	}
}
