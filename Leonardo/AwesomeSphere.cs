using MatterHackers.Csg.Solids;
using MatterHackers.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatterHackers.Csg;

namespace Leonardo
{
	public class AwesomeSphere : AwesomeSolid
	{
		private Sphere _sphere;

		public override CsgObject InnerObject
		{
			get { return _sphere; }
		}

		public AwesomeSphere(double radius, string name = "") : base(name)
		{
			_sphere = new Sphere(radius, name);
		}

		private List<Vector3> _attachPoints;
		public override List<Vector3> AttachPoints
		{
			get
			{
				if(_attachPoints == null)
				{
					Vector3 center = _sphere.GetCenter();
					_attachPoints = new List<Vector3>();
					_attachPoints[0] = new Vector3(center.x + _sphere.Radius, center.y, center.z);
					_attachPoints[1] = new Vector3(center.x, center.y + _sphere.Radius, center.z);
					_attachPoints[2] = new Vector3(center.x, center.y, center.z + _sphere.Radius);
					_attachPoints[3] = new Vector3(center.x - _sphere.Radius, center.y, center.z);
					_attachPoints[4] = new Vector3(center.x, center.y - _sphere.Radius, center.z);
					_attachPoints[5] = new Vector3(center.x, center.y, center.z - _sphere.Radius);
				}
				return _attachPoints;
			}
		}

		public override double AverageSize
		{
			get
			{
				return _sphere.Radius;
			}
		}

		public override AxisAlignedBoundingBox GetAxisAlignedBoundingBox()
		{
			return _sphere.GetAxisAlignedBoundingBox();
		}

		public override string ToString()
		{
			return _sphere.ToString();
		}
	}
}
