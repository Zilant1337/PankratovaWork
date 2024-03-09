using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;

namespace Pankratova
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double alphaLeft = 1.21922; double alphaRight=1.5;
            double h = 0.0001;
            List<double> y1 = new List<double>();
            List<double> y2 = new List<double>();
            List<double> y3 = new List<double>();
            y1.Add(1);
            y2.Add(1 + alphaLeft);
            y3.Add(1 + alphaLeft);


            List<double>xList=new List<double>();
            for (double alpha = alphaLeft; alpha< alphaRight; alpha += h)
            {
                double k11 = f1(y1.Last(), y2.Last(), y3.Last(), alpha);
                double k12 = f2(y1.Last(), y2.Last(), y3.Last(), alpha);
                double k13 = f3(y1.Last(), y2.Last(), y3.Last(), alpha);

                double k21 = f1(y1.Last() + h * k11 / 2, y2.Last() + h * k12 / 2, y3.Last() + h * k13 / 2, alpha + h / 2);
                double k22 = f2(y1.Last() + h * k11 / 2, y2.Last() + h * k12 / 2, y3.Last() + h * k13 / 2, alpha + h / 2);
                double k23 = f3(y1.Last() + h * k11 / 2, y2.Last() + h * k12 / 2, y3.Last() + h * k13 / 2, alpha + h / 2);

                double k31 = f1(y1.Last() + h * k21/2, y2.Last() + h * k22/2, y3.Last() + h * k23 / 2, alpha + h / 2);
                double k32 = f2(y1.Last() + h * k21/2, y2.Last() + h * k22/2, y3.Last() + h * k23 / 2, alpha + h / 2);
                double k33 = f3(y1.Last() + h * k21 / 2, y2.Last() + h * k22 / 2, y3.Last() + h * k23 / 2, alpha + h / 2);

                double k41 = f1(y1.Last() + h * k31, y2.Last() + h * k32, y3.Last() + h * k33, alpha + h);
                double k42 = f2(y1.Last() + h * k31, y2.Last() + h * k32, y3.Last() + h * k33, alpha + h);
                double k43 = f3(y1.Last() + h * k31, y2.Last() + h * k32, y3.Last() + h * k33, alpha + h);

                y1.Add(nextY(k11, k21, k31, k41, y1.Last(), h));
                y2.Add(nextY(k12, k22, k32, k42, y2.Last(), h));
                y3.Add(nextY(k13, k13, k13, k13, y3.Last(), h));
                xList.Add(alpha);
            }
            ScottPlot.Plot plot = new ScottPlot.Plot();
            plot.Add.Scatter(xList.ToArray(),y1.ToArray());
            plot.Add.Scatter(xList.ToArray(),y2.ToArray());
            plot.Add.Scatter(xList.ToArray(), y3.ToArray());
            plot.SavePng("Graph.png", 2560, 1440);
        }

        /*static double f1(double y1, double y2, double x)
        {
            return y1 + 1 / y2 - Math.Exp(x);
        }
        static double f2(double y1, double y2, double x)
        {
            return y2 - 1 / y1 - Math.Exp(-x);
        }*/

        static double f1(double y1, double y2,double y3, double alpha)
        {
            return 1+y1*y1*y2-(y3+1)*y1;
        }
        static double f2(double y1, double y2, double y3, double alpha)
        {
            return y1*y3-y1*y1*y2;
        }
        static double f3(double y1, double y2, double y3, double alpha)
        {
            return -y1*y3+alpha;
        }

        static double nextY(double k1, double k2, double k3, double k4, double prevY,double h)
        {
            return prevY+h*(k1+2*k2+2*k3+k4)/6;
        }
    }
}
