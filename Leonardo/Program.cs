using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatterHackers.Csg;
using MatterHackers.Csg.Operations;
using MatterHackers.Csg.Processors;
using MatterHackers.Csg.Solids;
using MatterHackers.Csg.Transform;
using MatterHackers.VectorMath;

namespace Leonardo
{
	class Program
	{
		private static Random _rand;

		//these dimentions are in mm
		private const double _xMax = 200.0;
		private const double _yMax = 200.0;
		private const double _zMax = 300.0;
		private const double _smallest = 5.0;

		/* So, a general way of doing things:
		 * -Choose some way of how to pick primitives
		 * -Choose some way of how to pick how many
		 * -Choose some way of how to pick sizes
		 * -Choose a method of picking positioning
		 * -Try varying the order of these and/or letting one decision influence the next
		 * -This is where Iris can help it to do some of its own thinking 
		 * 
		 * -Make sumthin' and purposely cut sumthin' else out of it, or a few sumthins...
		 * -Play with proportionality of volumes
		 */

		/* Composition principles:
		 * Mass vs space
		 * Volumes (how many)
		 * Negative volume
		 * Concave vs convex vs flat, combinations
		 * Light and shade (not sure how we can use that...)
		 * Orientation
		 * Proportion
		 */


        /* New Framing for the Process:
         * A series of decisions.
         * -How many primitives?
         * -Which primitives?
         * -Rotate?
         * -Make more after rotate?
         * -Translations?
         * -Rotate/translate and do more?
         * -How many iterations?
         * -Which steps in each iteration?
         * -How do we determine placement? Random? One one side or the other? Segregate? Integrate?
         * -How to make sure they're connected? Put one, note the size, put another that you know will connect...Do them symmetric...
         * -Place in ascending/descending size order
         * -Any cutouts? How?
         * 
         * Make these decisions in different orders
         * How do we incorporate some of the 
         */

		static void Main(string[] args)
		{
			_rand = new Random();
            //int numBodies =  5 + _rand.Next(45);

            //Union u = new Union();

            //for (int i = 0; i < numBodies; i++)
            //{
            //	CsgObject newPrimitive = GimmeAPrimitive();

            //	//placement
            //	Vector3 translate = new Vector3(GetATransform(), GetATransform(), GetATransform());

            //	newPrimitive = new SetCenter(newPrimitive, translate);

            //	u = new Union(u, newPrimitive);
            //}

            Union u = DoItWithSubdivision();
            
            
			OpenSCadOutput.Save(u, "output.scad");
		}

        private static CsgObject SingleComposition()
        {
            CsgObject returnMe = null;



            return returnMe;
        }

		private static Union DoItWithSubdivision()
		{
			//int howManyAxes = _rand.Next(3);
			Union u = new Union();

			Array axes = Enum.GetValues(typeof(Axes));
			Axes whichOne = (Axes)axes.GetValue(_rand.Next(axes.Length));
            
			double[][] axisValues = new double[3][];
			axisValues[0] = GimmeRandomDoubles(_rand.Next(15), _xMax);
			axisValues[1] = GimmeRandomDoubles(_rand.Next(15), _yMax);
			axisValues[2] = GimmeRandomDoubles(_rand.Next(15), _zMax);
			switch (whichOne)
			{
				case Axes.X:
					int xPoints = _rand.Next(50);
					axisValues[0] = Subdivide(_xMax, xPoints);
					break;
				case Axes.Y:
					int yPoints = _rand.Next(50);
					axisValues[1] = Subdivide(_yMax, yPoints);
					break;
				case Axes.Z:
					int zPoints = _rand.Next(50);
					axisValues[2] = Subdivide(_yMax, zPoints);
					break;
				default:
					break;
			}
			for (int i = 0; i < axisValues[0].Length; i++)
			{
				for (int j = 0; j < axisValues[1].Length; j++)
				{
					for (int k = 0; k < axisValues[2].Length; k++)
					{
						CsgObject newPrimitive = GimmeAPrimitive();
						Vector3 translate = new Vector3(axisValues[0][i], axisValues[1][j], axisValues[2][k]);
						newPrimitive = new SetCenter(newPrimitive, translate);
						u = new Union(u, newPrimitive);
					}
				}
			}

			return u;
		}

		private static double GetADouble()
		{
			return 2.0 + _rand.NextDouble() * 8.0;
		}

		private static double GetASize()
		{
			return 5.0 + _rand.NextDouble() * 25.0;
		}

		private static double GetATransform()
		{
			return 0 + _rand.NextDouble() * 200.0;
		}

		private static double[] Subdivide(double size, int numPoints)
		{
			double[] returnArray = new double[numPoints];
			double increment = size / numPoints;
			returnArray[0] = increment / 2;
			for (int i = 1; i < numPoints; i++)
			{
				returnArray[i] = returnArray[i - 1] + increment;
			}
			return returnArray;
		}

		private static double[] GimmeRandomDoubles(int howMany, double max)
		{
			double[] returnArray = new double[howMany];
			for (int i = 0; i < howMany; i++)
			{
				returnArray[i] = _rand.NextDouble() * max;
			}
			return returnArray;
		}

		private static CsgObject GimmeAPrimitive()
		{
			CsgObject newPrimitive = null;
			//primitive type
			int type = _rand.Next(4);
			double size = GetASize();
			double height = GetASize();

			switch (type)
			{
				case 0: //cube
					newPrimitive = new Box(size, size, size);
					break;
				case 1: //sphere
					newPrimitive = new Sphere(size);
					break;
				case 2: //cone
					newPrimitive = new Cylinder(size, 0, height);
					break;
				case 3: //cylinder
					newPrimitive = new Cylinder(size, height);
					break;
				case 4: //rectangular solid
					double depth = GetASize();
					double width = GetASize();
					newPrimitive = new Box(width, depth, size);
					break;
				default:
					newPrimitive = new Box(size, size, size);
					break;
			}

			return newPrimitive;
		}

		/* Ideas:
		 * have it choose n points equidistant from one another in 3-space, put stuff there of the same or
		 * different or a handful of sizes
		 * have it place stuff so that it knows where the last one was, and make the size a function
		 * of the distance from the last one and its size...
		 * have it change orientation of stuff, because that makes things more interesting - come up with
		 * smart ways for it to do this
		 * it should maintain some kind of knowledge of what it made - how many primitives are there, what are
		 * they, what sizes are they, what's their translation (location), what rotation do they have
		 * pick a size or size range and make a bunch of things those sizes
		 * pick an interesting x, y, or z point and put a bunch of stuff there, ranging in the other two
		 * maybe use a mathematical equation to produce sizes or positions...
		 */

	}

	public enum Axes
	{
		X,
		Y,
		Z
	}

	public enum Primitives
	{
		Cube,
		Cone,
		Cylinder,
		Sphere,
		Rectangular
	}
}
