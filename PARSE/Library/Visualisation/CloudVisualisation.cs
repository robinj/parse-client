﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

using HelixToolkit.Wpf;

using Microsoft.Kinect;

namespace PARSE
{
    /*CloudVisualisation Class
     * - Displays a cloud visualisation based on the received point cloud structure
     * - Currently uses int arrays. Will use PointCloud type */

    public class CloudVisualisation
    {

        //Constants for visualisation
        private int     depthFrameWidth = 640;
        private int     depthFrameHeight = 480;
        private int     cx = 640 / 2;
        private int     cy = 480 / 2;
        private int     tooCloseDepth = 0;
        private int     tooFarDepth = 4095;
        private int     unknownDepth = -1;
        private double  scale = 0.001;
        private double  fxinv = 1.0 / 480;
        private double  fyinv = 1.0 / 480;
        private double  ddt = 200;

        //Geometry for visualisation
        //This is public to be accessed for XML serialization.s
        List<PointCloud>    clouds;
        bool?               texture;
        
        //Geometry for visualisation computation.
        int[]               rawDepth;
        Point3D[]           depthFramePoints;
        Point[]             textureCoordinates;

        public CloudVisualisation()
        {
            //parameterless
        }

        public CloudVisualisation(List<PointCloud> pc, bool? texture)
        {
            this.clouds = pc;
            this.texture = texture;

            textureCoordinates = new Point[depthFrameHeight * depthFrameWidth];
            depthFramePoints = new Point3D[depthFrameHeight * depthFrameWidth];

            render();

        }
        
        public void render()
        {
            //create depth coordinates

            for (int i = 0; i < this.clouds.Count; i++)
            {
                this.rawDepth = this.clouds[i].rawDepth;
                runDemoModel(i);
                createDepthCoords();

                System.Diagnostics.Debug.WriteLine(this.clouds[i].rawDepth.Length);

                switch (i)
                {
                    case 0:
                        this.Model.Geometry = createMesh();

                        if (texture == false)
                        {
                            this.Model.Material = this.Model.BackMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));
                        }
                        else
                        {
                            this.Model.Material = this.Model.BackMaterial = new DiffuseMaterial(new ImageBrush(this.clouds[i].bs));
                        }
                        
                        this.Model.Transform = new TranslateTransform3D(-1, -2, 1);
                        break;
                    case 1:
                        this.Model2.Geometry = createMesh();

                        if (texture == false)
                        {
                            this.Model2.Material = this.Model2.BackMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));
                        }
                        else
                        {
                            this.Model2.Material = this.Model2.BackMaterial = new DiffuseMaterial(new ImageBrush(this.clouds[i].bs));
                        }

                        this.Model2.Transform = new TranslateTransform3D(0, -2, 1);
                        break;
                    case 2:
                        this.Model3.Geometry = createMesh();

                        if (texture == false)
                        {
                            this.Model3.Material = this.Model3.BackMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));
                        }
                        else
                        {
                            this.Model3.Material = this.Model3.BackMaterial = new DiffuseMaterial(new ImageBrush(this.clouds[i].bs));
                        }

                        //translate then rotate
                        Transform3DCollection tc = new Transform3DCollection(2);
           
                        this.Model3.Transform = new TranslateTransform3D(-1, -1, 1);
                        //this.Model3.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 0), 0), 0, -2, 0);
                        break;
                    case 3:
                        this.Model4.Geometry = createMesh();

                        if (texture == false)
                        {
                            this.Model4.Material = this.Model4.BackMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));
                        }
                        else
                        {
                            this.Model4.Material = this.Model4.BackMaterial = new DiffuseMaterial(new ImageBrush(this.clouds[i].bs));
                        }
                        
                        this.Model4.Transform = new TranslateTransform3D(2, -2, 1);
                        break;
                } 

            }

        }

        public void createDepthCoords()
        {

            for (int iy = 0; iy < 480; iy++)
            {
                for (int ix = 0; ix < 640; ix++)
                {
                    int i = (iy * 640) + ix;

                    if (rawDepth[i] == unknownDepth || rawDepth[i] < tooCloseDepth || rawDepth[i] > tooFarDepth)
                    {
                        this.rawDepth[i] = -1;
                        this.depthFramePoints[i] = new Point3D();
                    }
                    else
                    {
                        double zz = this.rawDepth[i] * scale;
                        double x = (cx - ix) * zz * fxinv;
                        double y = zz;
                        double z = (cy - iy) * zz * fyinv;
                        this.depthFramePoints[i] = new Point3D(x, y, z);
                    }
                }
            }
        }

        public MeshGeometry3D createMesh()
        {
            var triangleIndices = new List<int>();
            for (int iy = 0; iy + 1 < depthFrameHeight; iy++)
            {
                for (int ix = 0; ix + 1 < depthFrameWidth; ix++)
                {
                    int i0 = (iy * depthFrameWidth) + ix;
                    int i1 = (iy * depthFrameWidth) + ix + 1;
                    int i2 = ((iy + 1) * depthFrameWidth) + ix + 1;
                    int i3 = ((iy + 1) * depthFrameWidth) + ix;

                    var d0 = this.rawDepth[i0];
                    var d1 = this.rawDepth[i1];
                    var d2 = this.rawDepth[i2];
                    var d3 = this.rawDepth[i3];

                    var dmax0 = Math.Max(Math.Max(d0, d1), d2);
                    var dmin0 = Math.Min(Math.Min(d0, d1), d2);
                    var dmax1 = Math.Max(d0, Math.Max(d2, d3));
                    var dmin1 = Math.Min(d0, Math.Min(d2, d3));

                    if (dmax0 - dmin0 < ddt && dmin0 != -1)
                    {
                        triangleIndices.Add(i0);
                        triangleIndices.Add(i1);
                        triangleIndices.Add(i2);
                    }

                    if (dmax1 - dmin1 < ddt && dmin1 != -1)
                    {
                        triangleIndices.Add(i0);
                        triangleIndices.Add(i2);
                        triangleIndices.Add(i3);
                    }
                }
            }

            return new MeshGeometry3D()
            {
                Positions = new Point3DCollection(this.depthFramePoints),
                TextureCoordinates = new System.Windows.Media.PointCollection(this.textureCoordinates),
                TriangleIndices = new Int32Collection(triangleIndices)
            };
        }

        /// <summary>
        /// Sanity check.
        /// </summary>

        public void runDemoModel(int pos)
        {

            // Create a mesh builder and add a box to it
            var meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddBox(new Point3D(0, 0, 1), 1, 2, 0.5);
            meshBuilder.AddBox(new Rect3D(0, 0, 1.2, 0.5, 1, 0.4));

            // Create a mesh from the builder (and freeze it)
            var mesh = meshBuilder.ToMesh(true);

            // Create some materials
            var greenMaterial = MaterialHelper.CreateMaterial(Colors.Green);

            switch(pos) {

                case 0: this.Model = new GeometryModel3D { Geometry = mesh, Transform = new TranslateTransform3D(0, 0, 0), Material = greenMaterial, BackMaterial = greenMaterial }; break;
                case 1: this.Model2 = new GeometryModel3D { Geometry = mesh, Transform = new TranslateTransform3D(0, 0, 0), Material = greenMaterial, BackMaterial = greenMaterial }; break;
                case 2: this.Model3 = new GeometryModel3D { Geometry = mesh, Transform = new TranslateTransform3D(0, 0, 0), Material = greenMaterial, BackMaterial = greenMaterial }; break;
                case 3: this.Model4 = new GeometryModel3D { Geometry = mesh, Transform = new TranslateTransform3D(0, 0, 0), Material = greenMaterial, BackMaterial = greenMaterial }; break;
            }

        }

        /// <summary>
        /// Gets or sets the sanity model.
        /// </summary>
        /// <value>The model.</value>

        public GeometryModel3D Model { get; set; }
        public GeometryModel3D BaseModel { get; set; }
        public GeometryModel3D Model2 { get; set; }
        public GeometryModel3D Model3 { get; set; }
        public GeometryModel3D Model4 { get; set; }

    }
}
