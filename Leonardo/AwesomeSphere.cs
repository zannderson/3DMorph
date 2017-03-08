using MatterHackers.Csg.Solids;
using MatterHackers.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leonardo
{
	public class AwesomeSphere : AwesomeSolid
	{
		private Sphere sphere;

		private List<Vector3> _attachPoints;
		public override List<Vector3> AttachPoints
		{
			get
			{
				if(_attachPoints == null)
				{
					Vector3 center = GetCenter();
					_attachPoints = new List<Vector3>();
					_attachPoints[0] = new Vector3(center.x + Radius, center.y, center.z);
					_attachPoints[1] = new Vector3(center.x, center.y + Radius, center.z);
					_attachPoints[2] = new Vector3(center.x, center.y, center.z + Radius);
					_attachPoints[3] = new Vector3(center.x - Radius, center.y, center.z);
					_attachPoints[4] = new Vector3(center.x, center.y - Radius, center.z);
					_attachPoints[5] = new Vector3(center.x, center.y, center.z - Radius);
				}
				return _attachPoints;
			}
		}

		#region IAwesomeSolid Methods

		public List<Vector3> GetAttachPoints()
		{
			return new List<Vector3>(AttachPoints);
		}

		public double GetAverageSize()
		{
			return Radius;
		}

		#endregion
	}
}
