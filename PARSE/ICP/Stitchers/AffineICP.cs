﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace PARSE.ICP.Stitchers
{
    class AffineICP : Stitcher
    {
        List<PointCloud> pointClouds;
        List<PointCloud> txpointClouds;
        PointCloud pcd;

        /// <summary>
        /// Instantiates an empty point cloud, ready for us to dump some data
        /// </summary>
        public AffineICP() {
            //instantiate empty lists and point clouds 
            pointClouds = new List<PointCloud>();
            txpointClouds = new List<PointCloud>();
            pcd = new PointCloud();
        }

        /// <summary>
        /// Add a point cloud into the list of point clouds
        /// </summary>
        /// <param name="pc">Input point cloud</param>
        public override void add(PointCloud pc)
        {
            this.pointClouds.Add(pc);
        }

        /// <summary>
        /// Add a list of point clouds to the list of point clouds
        /// </summary>
        /// <param name="pcs"></param>
        public override void add(List<PointCloud> pcs)
        {
            this.pointClouds = pcs;
            Console.WriteLine("There are " + pcs.Count);
        }

        public override void stitch() {
            //todo: some form of datastructure to hold the transformation matrices

            /*
            //perform registration on each pair of clouds 
            for (int i = 0; i > pointClouds.Count -1; i++) { 
            
                //add this transform matrix to the previous one so that transformation is complete 
            }*/
        } 

        public void ICPStep() {
            double[] s = { 1, 1, 1, 0.01, 0.01, 0.01, 0.01, 0.01, 0.01 };                 //scale
            double[] p = { 0, 0, 0, 0, 0, 0, 100, 100, 100 };                           //parameters

            //jam the values into vectors so that we can multiply them and shiz
            DenseVector scale = new DenseVector(s);
            DenseVector parameters = new DenseVector(p);

            //distance error in final iteration 
            double fval_old = double.MaxValue;

            //change in distance error between two iterations 
            double fval_percep = 0;

            //todo: some array to contain the transformed points 

            //number of iterations 
            int itt = 0;

            //get the max and min points of the static points 
            double maxP = this.maxP();
            double minP = this.minP();

            double tolX = (maxP - minP) / 1000;


        }
 

        /// <summary>
        /// Return the result of the stitching
        /// </summary>
        /// <returns>The result of the stitching</returns>
        public override PointCloud getResult()
        {
            return pcd;
        }

        public override List<PointCloud> getResultList()
        {
            return txpointClouds;
        }

        public double maxP() {
            return 0;
        }

        public double minP() {
            return 0; 
        }
    }
}
