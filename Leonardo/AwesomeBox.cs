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
    public class AwesomeBox : AwesomeSolid
    {
        public AwesomeBox(Vector3 size, string name = "", bool createCentered = true) : base(size, name, createCentered)
        {
            Translation = new Vector3(0.0, 0.0, 0.0);
        }

        private Vector3[] _corners;
        public Vector3[] Corners
        {
            get
            {
                if(_corners == null)
                {
                    var center = GetCenter();
                    _corners = new Vector3[8];
                    _corners[0] = new Vector3(center.x - XSize / 2, center.y - YSize / 2, center.z - ZSize / 2);
                    _corners[1] = new Vector3(center.x + XSize / 2, center.y - YSize / 2, center.z - ZSize / 2);
                    _corners[2] = new Vector3(center.x - XSize / 2, center.y + YSize / 2, center.z - ZSize / 2);
                    _corners[3] = new Vector3(center.x - XSize / 2, center.y - YSize / 2, center.z + ZSize / 2);
                    _corners[4] = new Vector3(center.x + XSize / 2, center.y + YSize / 2, center.z - ZSize / 2);
                    _corners[5] = new Vector3(center.x - XSize / 2, center.y + YSize / 2, center.z + ZSize / 2);
                    _corners[6] = new Vector3(center.x + XSize / 2, center.y - YSize / 2, center.z + ZSize / 2);
                    _corners[7] = new Vector3(center.x + XSize / 2, center.y + YSize / 2, center.z + ZSize / 2);
				}
                return _corners;
            }
        }

		private Vector3[] _sideCenters;
		public Vector3[] SideCenters
		{
			get
			{
				if(_sideCenters == null)
				{
					_sideCenters = new Vector3[6];
					_sideCenters[0] = new Vector3(Translation.x + XSize * 0.5, Translation.y + YSize * 0.5, Translation.z);
					_sideCenters[1] = new Vector3(Translation.x + XSize * 0.5, Translation.y, Translation.z + ZSize * 0.5);
					_sideCenters[2] = new Vector3(Translation.x, Translation.y + YSize * 0.5, Translation.z + ZSize * 0.5);
					_sideCenters[3] = new Vector3(Translation.x + XSize - XSize * 0.5, Translation.y + YSize - YSize * 0.5, Translation.z + ZSize);
					_sideCenters[3] = new Vector3(Translation.x + XSize, Translation.y + YSize - YSize * 0.5, Translation.z + ZSize - ZSize * 0.5);
					_sideCenters[3] = new Vector3(Translation.x + XSize - XSize * 0.5, Translation.y + YSize, Translation.z + ZSize - ZSize * 0.5);
				}
				return _sideCenters;
			}
		}

		public List<Vector3> GetAttachPoints()
		{
			List<Vector3> attachPoints = new List<Vector3>(Corners);
			attachPoints.AddRange(SideCenters);
			return attachPoints;
		}

		public double GetAverageSize()
		{
			return XSize + YSize + ZSize / 3.0;
		}

		public override AxisAlignedBoundingBox GetAxisAlignedBoundingBox()
		{
			throw new NotImplementedException();
		}

		private Vector3 _translation;
        public Vector3 Translation
        {
            get { return _translation; }
            set
            {
				_sideCenters = null;
                _corners = null; //just empty it out and it'll get re-filled next time the property is accessed
                _translation = value;
            }
        }

		public override List<Vector3> AttachPoints
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override double AverageSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override CsgObject InnerObject
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
