using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuantumConcepts.Formats.StereoLithography;
using System.Windows.Media.Media3D;

namespace MagicMorpher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _fileContents;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_PickFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                DisplayFile(ofd.FileName);
            }

        }

        private void DisplayFile(string filename)
        {
            STLDocument doc = STLDocument.Read(new FileStream(filename, FileMode.Open));

            MeshGeometry3D mg = new MeshGeometry3D();
            
            foreach (var facet in doc.Facets)
            {
                for (int i = 0; i < facet.Vertices.Count; i++)
                {
                    mg.Positions.Add(new Point3D(facet.Vertices[i].X, facet.Vertices[i].Y, facet.Vertices[i].Z));
					mg.Normals.Add(new Vector3D(facet.Normal.X, facet.Normal.Y, facet.Normal.Z));
				}
            }

            ModelVisual3D m3d = new ModelVisual3D();
            m3d.Content = new GeometryModel3D(mg, new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(175, 175, 175)))) { BackMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(255, 0, 0))) };

            HelixViewer.Children.Add(m3d);
        }
    }
}
