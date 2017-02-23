using MatterHackers.Csg.Solids;
using MatterHackers.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leonardo
{
    public class AwesomeBox : Box
    {
        public AwesomeBox(Vector3 size, string name = "", bool createCentered = true) : base(size, name, createCentered)
        {
            Translation = new Vector3(0.0, 0.0, 0.0);
        }

        public AwesomeBox(double sizeX, double sizeY, double sizeZ, string name = "", bool createCentered = true) : base(sizeX, sizeY, sizeZ, name, createCentered)
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
                    _corners = new Vector3[8];
                    _corners[0] = Translation;
                    _corners[1] = new Vector3(Translation.x + XSize, Translation.y, Translation.z);
                }

                return _corners;
            }
        }

        private Vector3 _translation;
        public Vector3 Translation
        {
            get { return _translation; }
            set
            {
                _corners = null; //just empty it out and it'll get re-filled next time the property is accessed
                _translation = value;
            }
        }
    }
}
