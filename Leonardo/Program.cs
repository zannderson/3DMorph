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
		private const double _largest = 200.0;

        private static double _startTime;

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

        /*
		 * Pick a point (random, something on an existing shape, something from a group of points)
		 * Do a thing there (random, all the same thing with random parameters, 
		 * Repeat
		 */

        /*
		 * Pi day chat with Dan:
		 * -Maybe just do all spheres
		 * -They'd look good
		 * -And you could be more random
		 * -Subtraction could still lead to voids and interesting shapes
		 * -Need to automate "picture taking" - GET TO WORK ON THIS
		 * -THERE IS SO MUCH WORK TO DO TO MAKE THIS COOLER
		 * -Need to automate more of the process in general
		 * 
		 * Methods for doing:
		 * -Size
		 * -Shape(?)
		 * -Position
		 * -Relative position
		 * 
		 * Include:
		 * -Fixed
		 * -Random
		 * -Mathematical function
		 * -Relative to others
		 * 
		 * Possibly instead of just spheres, maybe try doing just cubes or just cones or cylinders?
		 */


        #region Main and Related

        static void Main(string[] args)
		{
			_rand = new Random();
            _startTime = DateTime.Now.Ticks;
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

            //Union u = DoItWithSubdivision();

            //CsgObject theThing = StringEmUp();

            //NGonExtrusion what = new NGonExtrusion(20, 13, 15);

            //DoASingleOne(CutUpASphereHybrid);
            //DoABunchSpheresOnly(100);
            DoABunchOneMethod(100, StringEmUpCrazy);
        }

        private static void DoASingleOne(Func<CsgObject> thisOne)
        {
            CsgObject thing = thisOne();

            OpenSCadOutput.Save(thing, "output.scad");
        }

        private static void DoABunch(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                //Console.Out.WriteLine(i);
                try
                {

                    CsgObject theThing = null;
                    double moreThanOne = _rand.NextDouble();
                    if (moreThanOne >= 0)
                    {
                        Union u = new Union();
                        int howManyMethods = _rand.Next(5);
                        List<CsgObject> unions = new List<CsgObject>();
                        List<CsgObject> differences = new List<CsgObject>();
                        for (int j = 0; j < howManyMethods; j++)
                        {
                            double addOrRemove = _rand.NextDouble();
                            if (addOrRemove <= 0.75 || j == 0)
                            {
                                var addMe = GimmeAComposition();
                                unions.Add(addMe);
                            }
                            else
                            {
                                var diffMe = GimmeAComposition();
                                differences.Add(diffMe);
                            }
                        }
                        foreach (var thing in unions)
                        {
                            u.Add(thing);
                        }
                        theThing = u;
                        if (differences.Count > 0)
                        {
                            Difference d = new Difference(theThing, differences[0]);
                            for (int j = 1; j < differences.Count; j++)
                            {
                                d.AddToSubtractList(differences[j]);
                            }
                            theThing = d;
                        }
                    }
                    else
                    {
                        theThing = GimmeAComposition();
                    }

                    DateTime now = DateTime.Now;
                    if (theThing == null)
                    {
                        theThing = GimmeAComposition();
                    }
                    //Console.Out.WriteLine("Writing file {0}", i);
                    OpenSCadOutput.Save(theThing, string.Format("{0}.scad", now.ToString("yyMMddHHmmssff")));
                }
                catch (Exception e)
                {
                    //DON'T CARE!!! HAHAHA
                    Console.Out.WriteLine("EXCEPTION: {0}", e.Message);
                }
            }
        }

        private static void DoABunchSpheresOnly(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                //Console.Out.WriteLine(i);
                try
                {

                    CsgObject theThing = null;
                    double moreThanOne = _rand.NextDouble();
                    if (moreThanOne >= 0)
                    {
                        Union u = new Union();
                        int howManyMethods = _rand.Next(5);
                        List<CsgObject> unions = new List<CsgObject>();
                        List<CsgObject> differences = new List<CsgObject>();
                        for (int j = 0; j < howManyMethods; j++)
                        {
                            double addOrRemove = _rand.NextDouble();
                            if (addOrRemove <= 0.75 || j == 0)
                            {
                                var addMe = GimmeACompositionSpheresOnly();
                                unions.Add(addMe);
                            }
                            else
                            {
                                var diffMe = GimmeACompositionSpheresOnly();
                                differences.Add(diffMe);
                            }
                        }
                        foreach (var thing in unions)
                        {
                            u.Add(thing);
                        }
                        theThing = u;
                        if (differences.Count > 0)
                        {
                            Difference d = new Difference(theThing, differences[0]);
                            for (int j = 1; j < differences.Count; j++)
                            {
                                d.AddToSubtractList(differences[j]);
                            }
                            theThing = d;
                        }
                    }
                    else
                    {
                        theThing = GimmeACompositionSpheresOnly();
                    }

                    DateTime now = DateTime.Now;
                    if (theThing == null)
                    {
                        theThing = GimmeACompositionSpheresOnly();
                    }
                    //Console.Out.WriteLine("Writing file {0}", i);
                    OpenSCadOutput.Save(theThing, string.Format("{0}.scad", now.ToString("yyMMddHHmmssff")));
                }
                catch (Exception e)
                {
                    //DON'T CARE!!! HAHAHA
                    Console.Out.WriteLine("EXCEPTION: {0}", e.Message);
                }
            }
        }

        private static void DoABunchOneMethod(int howMany, Func<CsgObject> method)
        {
            for (int i = 0; i < howMany; i++)
            {
                //Console.Out.WriteLine(i);
                try
                {

                    CsgObject theThing = null;
                    double moreThanOne = _rand.NextDouble();
                    if (moreThanOne >= 0)
                    {
                        Union u = new Union();
                        int howManyMethods = _rand.Next(5);
                        List<CsgObject> unions = new List<CsgObject>();
                        List<CsgObject> differences = new List<CsgObject>();
                        for (int j = 0; j < howManyMethods; j++)
                        {
                            double addOrRemove = _rand.NextDouble();
                            if (addOrRemove <= 0.75 || j == 0)
                            {
                                var addMe = method();
                                unions.Add(addMe);
                            }
                            else
                            {
                                var diffMe = method();
                                differences.Add(diffMe);
                            }
                        }
                        foreach (var thing in unions)
                        {
                            u.Add(thing);
                        }
                        theThing = u;
                        if (differences.Count > 0)
                        {
                            Difference d = new Difference(theThing, differences[0]);
                            for (int j = 1; j < differences.Count; j++)
                            {
                                d.AddToSubtractList(differences[j]);
                            }
                            theThing = d;
                        }
                    }
                    else
                    {
                        theThing = method();
                    }

                    DateTime now = DateTime.Now;
                    if (theThing == null)
                    {
                        theThing = method();
                    }
                    //Console.Out.WriteLine("Writing file {0}", i);
                    OpenSCadOutput.Save(theThing, string.Format("{0}.scad", now.ToString("yyMMddHHmmssff")));
                }
                catch (Exception e)
                {
                    //DON'T CARE!!! HAHAHA
                    Console.Out.WriteLine("EXCEPTION: {0}", e.Message);
                }
            }
        }

        #endregion Main and Related

        #region Generation Methods

        private static CsgObject StringEmUp()
        {
            /*
			 * Pick a primitive type (or a pattern or random)
			 * Pick a start and end size (or go up and down...)
			 * Pick a distance?
			 * Pick a line/parabola/periodic function
			 * Go for it
			 */

            Union u = new Union();

            double start = GimmeABoundedDouble(5, 50);
            double end = GimmeABoundedDouble(60, 200);

            int howMany = _rand.Next(5, 20);

            double sizeIncrement = (start - end) / (double)howMany;

            double z = 0;
            List<double> radii = GimmeSomeBoundedDoubles(start, end, howMany);
            for (int i = 0; i < howMany; i++)
            {
                double radius = radii[i] * (end - start);
                Sphere awe = new Sphere(radius);
                Translate t = new Translate(awe as CsgObject, new Vector3(0, 0, z));
                u.Add(t);
                z += radius;
            }
            return u;
        }

        private static CsgObject StringEmUpCrazy()
        {
            Union u = new Union();
            int howMany = _rand.Next(5, 20);
            double radius = GimmeABoundedDouble(_smallest, _largest);
            Vector3 center = Vector3.Zero;
            List<double> radii = GimmeSomeBoundedDoubles(0, 100, howMany);
            for (int i = 0; i < howMany; i++)
            {
                Vector3 direction = GimmeADirection();
                Vector3 translation = center + direction * radius;
                center = center + direction * radius;
                double percentChange = GimmeABoundedDouble(0, 2) * radii[i];
                double biggerOrSmaller = GimmeABoundedDouble(0, 1);
                radius = biggerOrSmaller >= 0.5 ? radius + radius * percentChange : radius - radius * percentChange;
                Sphere awe = new Sphere(radius);
                Translate t = new Translate(awe, translation);
                u.Add(t);
            }
            return u;

        }

        private static CsgObject VaryOnlyXYorZ()
        {

        }

        private static CsgObject AtomDiagram()
        {

        }

        private static CsgObject SingleComposition()
        {
            //TODO: make some bounds on how crazy or tame this goes with relative sizes...IE does the main thing
            //get more or less emphasis?
            CsgObject returnMe = null;
            Union u = new Union();

            CsgObject box = new Box(new Vector3(150, 150, 150));

            int whichOne = _rand.Next(2);
            List<CsgObject> unions = new List<CsgObject>();
            List<CsgObject> differences = new List<CsgObject>();

            //TODO: Order prolly matters here? Maybe? So right now we're gonna build lists, do unions, then
            //do differences. Maybe later we should do it in whatever order they happen in, or at least
            //consider it...

            //foreach (Vector3 corner in )
            //{
            //int which = _rand.Next(2);
            //switch (which)
            //{
            //	case 0:
            //		solid = new Box(new Vector3(150, 150, 150));
            //		break;
            //	case 1:
            //		solid = new Sphere(150);
            //		break;
            //	default:
            //		solid = new Box(new Vector3(150, 150, 150));
            //		break;
            //}
            //}

            u.Add(box as CsgObject);

            double averageSize = (box.XSize + box.YSize + box.ZSize) / 3.0;

            List<Vector3> boxVertexes = new List<Vector3>();

            Vector3 boxCenter = box.GetCenter();
            double halfX = box.XSize / 2;
            double halfY = box.YSize / 2;
            double halfZ = box.ZSize / 2;

            for (int i = 0; i < 2; i++)
            {
                double x = -1 * halfX + i * box.XSize;
                for (int j = 0; j < 2; j++)
                {
                    double y = -1 * halfY + j * box.YSize;
                    for (int k = 0; k < 2; k++)
                    {
                        double z = -1 * halfZ + k * box.ZSize;
                        Vector3 newVertex = new Vector3(x, y, z);
                        boxVertexes.Add(newVertex);
                    }
                }
            }

            foreach (Vector3 attachPoint in boxVertexes)
            {
                CsgObject joinThis = null;
                double doWhat = _rand.NextDouble();
                if (doWhat < 0.33)
                {
                    joinThis = GimmeAPrimitive(GimmeABoundedDouble(averageSize * 0.25, averageSize * 0.75), attachPoint);
                    differences.Add(joinThis);
                }
                else if (doWhat < 0.85)
                {
                    joinThis = GimmeAPrimitive(GimmeABoundedDouble(averageSize * 0.25, averageSize * 0.75), attachPoint);
                    unions.Add(joinThis);
                }
                else
                {
                    joinThis = null;
                }
            }

            foreach (var thing in unions)
            {
                u.Add(thing);
                returnMe = u;
            }
            if (differences.Count > 0)
            {
                Difference d = new Difference(returnMe, differences[0]);
                for (int i = 1; i < differences.Count; i++)
                {
                    d.AddToSubtractList(differences[i]);
                }
                returnMe = d;
            }



            return returnMe;
        }

        private static CsgObject MakeRandomSpheres()
        {
            int makeThisMany = _rand.Next(20);
            Union u = new Union();
            Sphere[] spheres = new Sphere[makeThisMany];
            Vector3[] translations = new Vector3[makeThisMany];
            for (int i = 0; i < makeThisMany; i++)
            {
                Sphere newS = null;
                Sphere attachToMe = null;
                int whichToAttachTo = -1;
                if (i > 0)
                {
                    whichToAttachTo = _rand.Next(i);
                    attachToMe = spheres[whichToAttachTo];
                }
                newS = new Sphere(GimmeABoundedDouble(_smallest, _largest));
                Translate t = null;
                Vector3 translation = Vector3.NegativeInfinity;
                if (attachToMe != null)
                {
                    double aRadius = attachToMe.Radius;
                    double bRadius = newS.Radius;

                    double overlap = GimmeABoundedDouble(0.05, 0.95);
                    Vector3 direction = GimmeADirection();
                    double maxLength = aRadius + bRadius;
                    double minLength = Math.Abs(aRadius - bRadius);
                    double range = maxLength - minLength;

                    double goThisFar = range * overlap;

                    translation = translations[whichToAttachTo] + direction * goThisFar;

                    t = new Translate(newS, translation);
                }
                else
                {
                    translation = new Vector3(GimmeABoundedDouble(0, _xMax), GimmeABoundedDouble(0, _yMax), GimmeABoundedDouble(0, _zMax));
                    t = new Translate(newS, translation);
                }
                translations[i] = translation;
                spheres[i] = newS;

                double diffMe = GimmeABoundedDouble(0, 1);
                if (diffMe >= 0.75)
                {
                    Difference d = new Difference(u, new Translate(newS, translation));
                    u = new Union();
                    u.Add(d);
                }
                else
                {
                    u.Add(new Translate(newS, translation));
                }
            }
            return u;
        }

        private static CsgObject SphereWithSurfaceSpheres()
        {
            Union u = new Union();
            Sphere mainSphere = new Sphere(GimmeABoundedDouble(_smallest, _largest));
            u.Add(mainSphere);
            int howManyBumps = _rand.Next(20);
            //size range is 0.25-0.75 of main sphere's radius
            //double howMuchToUse = GimmeABoundedDouble(0.1, 1);
            for (int i = 0; i < howManyBumps; i++)
            {
                Vector3 direction = GimmeADirection();
                double radius = GimmeABoundedDouble(0.25, 0.75) * mainSphere.Radius;
                Sphere bump = new Sphere(radius);
                Translate t = new Translate(bump, mainSphere.GetCenter() + direction * mainSphere.Radius);
                u.Add(t);
            }
            return u;
        }

        private static CsgObject SphereWithSmartSurfaceSpheres()
        {
            Union u = new Union();
            Sphere mainSphere = new Sphere(GimmeABoundedDouble(_smallest, _largest));
            u.Add(mainSphere);
            int howManyBumps = _rand.Next(5) + 2;
            for (int i = 0; i < howManyBumps; i++)
            {
                Vector3 direction = GimmeADirection();
                double radius = GimmeABoundedDouble(0.25, 0.75) * mainSphere.Radius;
                Sphere bump = new Sphere(radius);
                Translate t = new Translate(bump, mainSphere.GetCenter() + direction * mainSphere.Radius);
                u.Add(t);

                double doOpposite = GimmeABoundedDouble(0, 1);
                double doOneAxis = GimmeABoundedDouble(0, 1);
                double doOtherAxis = GimmeABoundedDouble(0, 1);

                Vector3 orthogonalOne = CrossProduct(direction, new Vector3(direction.z, direction.x, direction.y));
                Vector3 orthogonalTwo = CrossProduct(direction, orthogonalOne);

                if (doOpposite >= 0.25)
                {
                    double diff = GimmeABoundedDouble(0, 1);
                    Sphere opposite = new Sphere(radius);
                    Translate to = new Translate(opposite, mainSphere.GetCenter() - direction * mainSphere.Radius);
                    if (diff >= 0.75)
                    {
                        Difference d = new Difference(u, to);
                        u = new Union();
                        u.Add(d);
                    }
                    else
                    {
                        u.Add(to);
                    }
                }
                if (doOneAxis >= 0.66)
                {
                    double diff = GimmeABoundedDouble(0, 1);
                    Sphere oneOne = new Sphere(radius);
                    Sphere oneTwo = new Sphere(radius);
                    Translate tOne = new Translate(oneOne, mainSphere.GetCenter() + orthogonalOne * mainSphere.Radius);
                    Translate tTwo = new Translate(oneOne, mainSphere.GetCenter() - orthogonalOne * mainSphere.Radius);
                    if (diff >= 0.75)
                    {
                        Difference d = new Difference(u, tOne);
                        Difference d2 = new Difference(d, tTwo);
                        u = new Union();
                        u.Add(d2);
                    }
                    else
                    {
                        u.Add(tOne);
                        u.Add(tTwo);
                    }
                }
                if (doOtherAxis >= 0.66)
                {
                    double diff = GimmeABoundedDouble(0, 1);
                    Sphere oneOne = new Sphere(radius);
                    Sphere oneTwo = new Sphere(radius);
                    Translate tOne = new Translate(oneOne, mainSphere.GetCenter() + orthogonalTwo * mainSphere.Radius);
                    Translate tTwo = new Translate(oneOne, mainSphere.GetCenter() - orthogonalTwo * mainSphere.Radius);
                    if (diff >= 0.5)
                    {
                        Difference d = new Difference(u, tOne);
                        Difference d2 = new Difference(d, tTwo);
                        u = new Union();
                        u.Add(d2);
                    }
                    else
                    {
                        u.Add(tOne);
                        u.Add(tTwo);
                    }
                }
            }
            return u;
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

        private static CsgObject CutUpASphereRegular()
        {
            Sphere s = new Sphere(GimmeABoundedDouble(100, 200));

            List<Vector3> compassPoints = GimmeSphereCompassPoints(s);

            Difference d = new Difference(s);
            foreach (var point in compassPoints)
            {
                Sphere cutItOut = new Sphere(GimmeABoundedDouble(40, 90));
                Translate t = new Translate(cutItOut, point);
                d.AddToSubtractList(t);
            }

            return d;
        }

        private static CsgObject CutUpASphereRandom()
        {
            Sphere mainSphere = new Sphere(GimmeABoundedDouble(_smallest, _largest));
            Difference d = new Difference(mainSphere);
            int howManyBumps = _rand.Next(20);
            //size range is 0.25-0.75 of main sphere's radius
            //double howMuchToUse = GimmeABoundedDouble(0.1, 1);
            for (int i = 0; i < howManyBumps; i++)
            {
                Vector3 direction = GimmeADirection();
                double radius = GimmeABoundedDouble(0.25, 0.65) * mainSphere.Radius;
                Sphere bump = new Sphere(radius);
                Translate t = new Translate(bump, mainSphere.GetCenter() + direction * mainSphere.Radius);
                d.AddToSubtractList(t);
            }
            return d;
        }

        private static CsgObject CutUpASphereHybrid()
        {
            Sphere mainSphere = new Sphere(GimmeABoundedDouble(_smallest, _largest));
            Difference d = new Difference(mainSphere);
            int howManyBumps = _rand.Next(5) + 2;
            for (int i = 0; i < howManyBumps; i++)
            {
                Vector3 direction = GimmeADirection();
                double radius = GimmeABoundedDouble(0.25, 0.75) * mainSphere.Radius;
                Sphere bump = new Sphere(radius);
                Translate t = new Translate(bump, mainSphere.GetCenter() + direction * mainSphere.Radius);
                d.AddToSubtractList(t);

                double doOpposite = GimmeABoundedDouble(0, 1);
                double doOneAxis = GimmeABoundedDouble(0, 1);
                double doOtherAxis = GimmeABoundedDouble(0, 1);

                Vector3 orthogonalOne = CrossProduct(direction, new Vector3(direction.z, direction.x, direction.y));
                Vector3 orthogonalTwo = CrossProduct(direction, orthogonalOne);

                if (doOneAxis >= 0.66)
                {
                    double diff = GimmeABoundedDouble(0, 1);
                    Sphere oneOne = new Sphere(radius);
                    Sphere oneTwo = new Sphere(radius);
                    Translate tOne = new Translate(oneOne, mainSphere.GetCenter() + orthogonalOne * mainSphere.Radius);
                    Translate tTwo = new Translate(oneOne, mainSphere.GetCenter() - orthogonalOne * mainSphere.Radius);
                    d.AddToSubtractList(tOne);
                    d.AddToSubtractList(tTwo);
                }
                if (doOtherAxis >= 0.66)
                {
                    double diff = GimmeABoundedDouble(0, 1);
                    Sphere oneOne = new Sphere(radius);
                    Sphere oneTwo = new Sphere(radius);
                    Translate tOne = new Translate(oneOne, mainSphere.GetCenter() + orthogonalTwo * mainSphere.Radius);
                    Translate tTwo = new Translate(oneOne, mainSphere.GetCenter() - orthogonalTwo * mainSphere.Radius);
                    d.AddToSubtractList(tOne);
                    d.AddToSubtractList(tTwo);
                }
            }
            return d;
        }

        #endregion Generation Methods

        #region Helper Methods - Values

        private static double GimmeADouble()
        {
            //TODO: Make this have different execution paths - random, by some function, etc...
            return 2.0 + _rand.NextDouble() * 8.0;
        }

        private static double GimmeASize()
        {
            //TODO: Make this have different execution paths - random, by some function, etc...
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

        private static double GimmeABoundedDouble(double min, double max)
        {
            if (min >= max)
            {
                throw new Exception("You dummy! Make the max strictly larger than the min.");
            }
            double range = max - min;
            return _rand.NextDouble() * range + min;
        }

        private static List<double> GimmeSomeBoundedDoubles(double min, double max, int howMany)
        {
            if(min >= max)
            {
                throw new Exception("Min must be strictly greater than max. You did it wrong.");
            }

            List<double> doubles = new List<double>();
            double range = max - min;
            double increment = range / howMany;

            Func<double, double> whichOne = null;
            int pickOne = _rand.Next(5);
            switch (pickOne)
            {
                case 0:
                    whichOne = Math.Sin;
                    break;
                case 1:
                    whichOne = Math.Cos;
                    break;
                case 2:
                    whichOne = Math.Sqrt;
                    break;
                case 3:
                    whichOne = Square;
                    break;
                case 4:
                    whichOne = Cube;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < howMany; i++)
            {
                double input = i * increment;
                if(pickOne <= 1)
                {
                    input = DegreeToRadian(input);
                }
                doubles.Add(whichOne(DegreeToRadian(input)));
            }
            return doubles;
        }
        
        private static double DistanceBetweenPoints(Vector3 a, Vector3 b)
        {
            return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
        }

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static double Square(double input)
        {
            return input * input;
        }

        private static double Cube(double input)
        {
            return input * input * input;
        }

        #endregion Helper Methods - Values

        #region Helper Methods - Csg/Vectors

        private static CsgObject GimmeAPrimitive(double size, Vector3 center)
        {
			CsgObject newPrimitive = null;
			//primitive type
			int type = _rand.Next(4);
            double height = GimmeABoundedDouble(size * 0.1, size * 2.0);

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
                    double depth = GimmeABoundedDouble(size * 0.1, size * 2.0);
                    double width = GimmeABoundedDouble(size * 0.1, size * 2.0);
					newPrimitive = new Box(width, depth, size);
					break;
				default:
					newPrimitive = new Box(size, size, size);
					break;
			}

            newPrimitive = new SetCenter(newPrimitive, center);

			return newPrimitive;
		}

		private static CsgObject GimmeAPrimitive(double size)
		{
            return GimmeAPrimitive(size, new Vector3(0, 0, 0));
		}

		private static CsgObject GimmeAPrimitive()
		{
			return GimmeAPrimitive(GimmeASize());
		}

		private static CsgObject GimmeAComposition()
		{
			return GimmeAComposition(GimmeASize());
        }

        private static CsgObject GimmeACompositionSpheresOnly()
        {
            CsgObject returnThis = GimmeAPrimitive();
            int whichOne = _rand.Next(5);
            switch (whichOne)
            {
                case 0:
                    returnThis = StringEmUp();
                    break;
                case 1:
                    returnThis = CutUpASphereHybrid();
                    break;
                case 2:
                    returnThis = SphereWithSmartSurfaceSpheres();
                    break;
                case 3:
                    returnThis = MakeRandomSpheres();
                    break;
                case 4:
                    returnThis = StringEmUpCrazy();
                    break;
                case 5:
                    returnThis = SphereWithSurfaceSpheres();
                    break;
                case 6:
                    returnThis = CutUpASphereRandom();
                    break;
                case 7:
                    returnThis = CutUpASphereRegular();
                    break;
                default:
                    returnThis = new Sphere(GimmeASize());
                    break;
            }
            return returnThis;
        }

        private static CsgObject GimmeAComposition(double size)
		{
			CsgObject returnThis = GimmeAPrimitive();
			int whichOne = _rand.Next(5);
			switch (whichOne)
			{
				case 0:
					returnThis = StringEmUp();
					break;
				case 1:
					returnThis = SingleComposition();
					break;
				case 2:
					returnThis = GimmeAPrimitive();
					break;
				case 3:
					returnThis = MakeRandomSpheres();
					break;
				case 4:
					returnThis = StringEmUpCrazy();
					break;
				default:
					returnThis = GimmeAPrimitive();
					break;
			}
			return returnThis;
		}

        private static Vector3 GimmeADirection()
        {
            Vector3 direction = new Vector3(GimmeABoundedDouble(0, 1) - 0.5, GimmeABoundedDouble(0, 1) - 0.5, GimmeABoundedDouble(0, 1) - 0.5);
            direction.Normalize();
            return direction;
        }

        private static Vector3 CrossProduct(Vector3 v1, Vector3 v2)
        {
            double x, y, z;
            x = v1.y * v2.z - v2.y * v1.z;
            y = (v1.x * v2.z - v2.x * v1.z) * -1;
            z = v1.x * v2.y - v2.x * v1.y;

            var rtnvector = new Vector3(x, y, z);
            rtnvector.Normalize(); //optional
            return rtnvector;
        }

        private static List<Vector3> GimmeSphereCompassPoints(Sphere sphere)
        {
            List<Vector3> points = new List<Vector3>();

            Vector3 center = sphere.GetCenter();
            double radius = sphere.Radius;

            points.Add(new Vector3(center.x + radius, center.y, center.z));
            points.Add(new Vector3(center.x - radius, center.y, center.z));
            points.Add(new Vector3(center.x, center.y + radius, center.z));
            points.Add(new Vector3(center.x, center.y - radius, center.z));
            points.Add(new Vector3(center.x, center.y, center.z + radius));
            points.Add(new Vector3(center.x, center.y, center.z - radius));

            return points;
        }

        #endregion Helper Methods - Csg/Vectors

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
