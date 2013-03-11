﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PARSE
{
    static class HeightCalculator
    {
        public static double getHeight(PointCloud pc)
        {
            double ymin = pc.getyMin();
            double ymax = pc.getyMax();
            double height = ymax - ymin;
            //Console.WriteLine(height);
            height = UnitConvertor.convertPCM(height,1);
            //Environment.Exit(1);
            return height;
        }
    }
}
