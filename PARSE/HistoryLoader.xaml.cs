﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Media.Media3D;
using System.IO;
using System.Windows.Controls.DataVisualization.Charting;

namespace PARSE
{
    /// <summary>
    /// Interaction logic for RuntimeLoader.xaml
    /// </summary>
    /// 

    public partial class HistoryLoader : Window
    {

        //persistently store our list of planes
        private List<List<Point3D>> storedPlanes;

        //publicly accessible area list from previous calculations
        public List<double> areaList;
        
        public HistoryLoader()
        {
            InitializeComponent();
            storedPlanes = new List<List<Point3D>>();
            this.Loaded += new RoutedEventHandler(HistoryLoader_Loaded);
        }

        private void HistoryLoader_Loaded(object Sender, RoutedEventArgs e)
        {
            //place relative to coreloader
            this.Left = this.Owner.Left + 20;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - 30;
            this.Height = this.Owner.Width * 0.225;
            this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;

            //check if a scan event is in place
            if (storedPlanes.Count == 0)
            {
                //in the case when we have no planes to show, modify the ui.
                //Volume Label Setting
                bodyimg.Visibility = Visibility.Collapsed;
                planeNo.Visibility = Visibility.Collapsed;
                viewborder.Visibility = Visibility.Collapsed;
                hvpcanvas.Visibility = Visibility.Hidden;
                planeChooser.Visibility = Visibility.Collapsed;
                vollabel.Visibility = Visibility.Collapsed;
                voLconclabel.Visibility = Visibility.Collapsed;

                voloutput.Visibility = Visibility.Collapsed;
                heightlabel.Visibility = Visibility.Collapsed;
                heightoutput.Visibility = Visibility.Collapsed;
                otherlabel.Visibility = Visibility.Collapsed;

                scanno.Visibility = Visibility.Collapsed;
                scantime.Visibility = Visibility.Collapsed;
                scanfileref.Visibility = Visibility.Collapsed;
                scanvoxel.Visibility = Visibility.Collapsed;
                maxarea.Visibility = Visibility.Collapsed;
                totalarea.Visibility = Visibility.Collapsed;
                totalperimiter.Visibility = Visibility.Collapsed;
                btnresults.Visibility = Visibility.Collapsed;
                btnrescan.Visibility = Visibility.Collapsed;

                //Circumference Label Setting
                circumlabel.Visibility = Visibility.Collapsed;
                circumoutput.Visibility = Visibility.Collapsed;
                limbselect.Visibility = Visibility.Collapsed;
                limbselecthdr.Visibility = Visibility.Hidden;
                planelabel.Visibility = Visibility.Collapsed;
                scanlabel.Visibility = Visibility.Collapsed;
                viewborder2.Visibility = Visibility.Collapsed;

                planeChooser2.Visibility = Visibility.Collapsed;
                limbimg.Visibility = Visibility.Collapsed;
                limbrender.Visibility = Visibility.Collapsed;

                scanno2.Visibility = Visibility.Collapsed;
                scantime2.Visibility = Visibility.Collapsed;
                scanfileref2.Visibility = Visibility.Collapsed;
                scanvoxel2.Visibility = Visibility.Collapsed;
                maxarea2.Visibility = Visibility.Collapsed;
                totalarea2.Visibility = Visibility.Collapsed;
                totalperimiter2.Visibility = Visibility.Collapsed;
                btnresults2.Visibility = Visibility.Collapsed;
                btnrescan2.Visibility = Visibility.Collapsed;

            }

            this.scanvoxel.Content = "VH Ratio: " + ((Convert.ToDouble(this.voloutput.Content) / Convert.ToDouble(this.heightoutput.Content)));
            this.scanfileref.Content = "BMI Measure: " + (66 / (Convert.ToDouble(this.heightoutput.Content)));

        }

        public void visualisePlanes(List<List<Point3D>> planes, double planeIndex)
        {
            //Set relevant UI components to visisble
            bodyimg.Visibility = Visibility.Visible;
            planeNo.Visibility = Visibility.Visible;
            viewborder.Visibility = Visibility.Visible;
            hvpcanvas.Visibility = Visibility.Visible;
            planeChooser.Visibility = Visibility.Visible;
            vollabel.Visibility = Visibility.Visible;
            voLconclabel.Visibility = Visibility.Visible;
            voloutput.Visibility = Visibility.Visible;
            heightlabel.Visibility = Visibility.Visible;
            heightoutput.Visibility = Visibility.Visible;
            otherlabel.Visibility = Visibility.Visible;
            scanno.Visibility = Visibility.Visible;
            scantime.Visibility = Visibility.Visible;
            scanfileref.Visibility = Visibility.Visible;
            scanvoxel.Visibility = Visibility.Visible;
            maxarea.Visibility = Visibility.Visible;
            totalarea.Visibility = Visibility.Visible;
            totalperimiter.Visibility = Visibility.Visible;
            btnresults.Visibility = Visibility.Visible;
            btnrescan.Visibility = Visibility.Visible;

            //Set relevant ui components to collapsed
            noresults.Visibility = Visibility.Collapsed;
            newscan.Visibility = Visibility.Collapsed;

            planeNo.Text = "Plane Outline: " + (int)planeIndex;

            System.Diagnostics.Debug.WriteLine("Number of caught planes: " + planes.Count);

            if (storedPlanes.Count == 0)
            {
                storedPlanes = planes;
                storedPlanes.Reverse();
                planeChooser.Maximum = storedPlanes.Count;
            }

            double xmin = 0;
            double xmax = 0;
            double zmin = 0;
            double zmax = 0;

            int i = (int)planeIndex;

            double[] x = new double[storedPlanes[i].Count];
            double[] z = new double[storedPlanes[i].Count];

            for (int j = 0; j < storedPlanes[i].Count; j++)
            {

                //Boundary check of points.
                if (storedPlanes[i][j].X > xmax)
                {
                    xmax = storedPlanes[i][j].X;
                }

                if (storedPlanes[i][j].Z > zmax)
                {
                    zmax = storedPlanes[i][j].Z;
                }

                if (storedPlanes[i][0].X < xmin)
                {
                    xmin = storedPlanes[i][0].X;
                }
                if (storedPlanes[i][j].X < xmin)
                {
                    xmin = storedPlanes[i][j].X;
                }

                if (storedPlanes[i][0].Z < zmin)
                {
                    zmin = storedPlanes[i][0].Z;
                }
                if (storedPlanes[i][j].Z < zmin)
                {
                    zmin = storedPlanes[i][j].Z;
                }

                //assign to arrays
                x[j] = storedPlanes[i][j].X;
                z[j] = storedPlanes[i][j].Z;

            }

            //write points to plane renderer class for visualisation.
            this.DataContext = new PlaneVisualisation(x, z);

            System.Diagnostics.Debug.WriteLine("Planes visualised");
            System.Diagnostics.Debug.WriteLine("xmin: " + xmin);
            System.Diagnostics.Debug.WriteLine("zmin: " + zmin);
            System.Diagnostics.Debug.WriteLine("xmax: " + xmax);
            System.Diagnostics.Debug.WriteLine("zmax: " + zmax);

            //setData
            ((LineSeries)(volchart.Series[0])).ItemsSource =
                new KeyValuePair<DateTime, int>[]{
                    new KeyValuePair<DateTime,int>(DateTime.Now, 100),
                    new KeyValuePair<DateTime,int>(DateTime.Now.AddMonths(1), 130),
                    new KeyValuePair<DateTime,int>(DateTime.Now.AddMonths(2), 150),
                    new KeyValuePair<DateTime,int>(DateTime.Now.AddMonths(3), 125),
                    new KeyValuePair<DateTime,int>(DateTime.Now.AddMonths(4),155) };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void planeChooser_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            visualisePlanes(storedPlanes, e.NewValue);
            double circum = CircumferenceCalculator.calculate(storedPlanes, (int) e.NewValue);
            this.totalarea.Content = "Total Area: " + Math.Round(areaList[(int)e.NewValue],4) + "m\u00B2";
            this.maxarea.Content = "Plane " + (int) e.NewValue;
            this.totalperimiter.Content = "Circumference: " + circum + "m";

        }
    }
}