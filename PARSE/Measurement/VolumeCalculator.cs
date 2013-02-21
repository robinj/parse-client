﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Emgu.CV.CvEnum;
using System.Windows.Media.Media3D;
using PARSE.ICP;

namespace PARSE
{
    public static class VolumeCalculator
    {
        private static List<Point3D> plane;

        private static double getBoundingBoxVolume(double xmin, double xmax, double ymin, double ymax, double zmin, double zmax)
        {
            return ((xmax - xmin) * (ymax - ymin) * (zmax - zmin));
        }

        private static double volume0thApprox(PointCloud pc)
        {
            double xmin = pc.getxMin();
            double xmax = pc.getxMax();
            double ymin = pc.getyMin();
            double ymax = pc.getyMax();
            double zmin = pc.getzMin();
            double zmax = pc.getzMax();
            double volume = getBoundingBoxVolume(xmin,xmax,ymin,ymax,zmin,zmax);
            System.Diagnostics.Debug.WriteLine("Volume Pre Multi: " + volume);
            volume = UnitConvertor.convertPCM(volume);
            return volume;
        }

        //only works on an amorphus blob
        public static List<List<Point3D>> volume1stApprox(PointCloud pc)
        {
            double xmin = pc.getxMin();
            double xmax = pc.getxMax();
            double zmin = pc.getzMin();
            double zmax = pc.getzMax();
            double[] limits = { xmin, zmin, xmax, zmax };

            double ymin = pc.getyMin();
            double ymax = pc.getyMax();
            double increment = (ymax - ymin) / 30;
            double volume = 0;
            List<List<Point3D>> planes = new List<List<Point3D>>();

            for (double i = ymin + (increment / 2); i <= ymax - (increment / 2); i = i + increment)
            {
                List<Point3D> plane = pc.getKDTree().getAllPointsAt(i, increment / 2, limits);
                if (plane.Count != 0)
                {
                    Console.WriteLine("Plane is not empty");
                    plane = PointSorter.rotSort(plane);
                    planes.Add(plane);
                    plane.Add(plane[0]); //a list eating its own head, steve matthews would be proud

                    double innerVolume = 0;

                    for (int j = 0; j < plane.Count - 1; j++)
                    {
                        innerVolume = innerVolume + ((plane[j].X * plane[j + 1].Z) - (plane[j + 1].X * plane[j].Z));
                        //Console.WriteLine("1: "+plane[j].Z+", 2: "+plane[j + 1].Z);
                    }

                    innerVolume = Math.Abs(innerVolume / 2);

                    innerVolume = innerVolume * increment;


                    volume = innerVolume;
                }
                else
                {
                    Console.WriteLine("Plane EMPTY!!! BAD THINGS WILL HAPPEN");
                }
            }
            System.Diagnostics.Debug.WriteLine("Volume Pre Multi: " + volume);
            volume = UnitConvertor.convertPCM(volume);
            System.Diagnostics.Debug.WriteLine("Better Volume Patient Volume: " + volume);
            return planes;
        }
        
        public static double calculateVolume(PointCloud pc)
        {
            System.Diagnostics.Debug.WriteLine("Upper Bound on Patient Volume: " + volume0thApprox(pc));
            return 0;//volume1stApprox(pc);
        }

        public static List<Point3D> getPlane()
        {
            return plane;
        }
    }
}
