using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ScottPlot;

namespace Pankratova
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double xLeft = 0.001; double xRight = 0.2;
            double h = 0.0001;
            List<double> y1 = new List<double>();
            List<double> y2 = new List<double>();
            List<double> y1Ans = new List<double>();
            List<double> y2Ans = new List<double>();
            y1.Add(2);
            y2.Add(2);

            List<double> xList = new List<double>();
            for (double eta = xLeft; eta < xRight; eta += h)
            {
                double k11 = F1(y1.Last(), y2.Last());
                double k12 = F2(y1.Last(), eta);

                double k21 = F1(y1.Last() + h * k11 / 3, y2.Last() + h * k12 / 3);
                double k22 = F2(y1.Last() + h * k11 / 3, eta + h / 3);

                double k31 = F1(y1.Last() - h * k11 / 3 + h * k21, y2.Last() - h * k12 / 3 + h * k22);
                double k32 = F2(y1.Last() - h * k11 / 3 + h * k21, eta + 2 * h / 3);

                double k41 = F1(y1.Last() + h * k11 - h * k21 + h * k31, y2.Last() + h * k12 - h * k22 + h * k32);
                double k42 = F2(y1.Last() + h * k11 - h * k21 + h * k31, eta + h);

                y1.Add(NextY(k11, k21, k31, k41, y1.Last(), h));
                y2.Add(NextY(k12, k22, k32, k42, y2.Last(), h));
                xList.Add(eta);
            }
            ScottPlot.Plot plot = new ScottPlot.Plot();
            plot.Add.Scatter(xList.ToArray(), y1.ToArray());
            plot.Add.Scatter(xList.ToArray(), y2.ToArray());
            plot.SavePng("Graph.png", 2560, 1440);
            GetTestGraphs();
            DoErrorGraphs();
        }
        static double F1Test(double y1, double y2, double x)
        {
            return y1 + 1 / y2 - Math.Exp(x);
        }
        static double F2Test(double y1, double y2, double x)
        {
            return y2 - 1 / y1 - Math.Exp(-x);
        }

        static double F1(double x1, double x2)
        {
            return x2 - (x1 * (Math.Pow(x1, 2) / 3 - 1));
        }
        static double F2(double x1, double eta)
        {
            return -eta * x1;
        }
        static double F1TestFinal(double x)
        {
            return Math.Exp(x);
        }
        static double F2TestFinal(double x)
        {
            return Math.Exp(-x);
        }
        static double NextY(double k1, double k2, double k3, double k4, double prevY, double h)
        {
            return prevY + h * (k1 + 3 * k2 + 3 * k3 + k4) / 8;
        }
        static void DoErrorGraphs()
        {
            double xLeft = 0; double xRight = 3;
            List<double> Err1 = new List<double>();
            List<double> Err2 = new List<double>();


            List<double> xList = new List<double>();
            for (double h = 0.01; h >= 0.0001; h = h / 1.1)
            {
                List<double> y1 = new List<double>();
                List<double> y2 = new List<double>();
                List<double> y1Ans = new List<double>();
                List<double> y2Ans = new List<double>();

                y1.Add(F1TestFinal(0));
                y2.Add(F2TestFinal(0));
                y1Ans.Add(F1TestFinal(0));
                y2Ans.Add(F2TestFinal(0));

                xList.Add(h);

                for (double x = xLeft+h; x < xRight; x += h)
                {
                    double k11 = F1Test(y1.Last(), y2.Last(), x);
                    double k12 = F2Test(y1.Last(), y2.Last(), x);

                    double k21 = F1Test(y1.Last() + h * k11 / 3, y2.Last() + h * k12 / 3, x + h / 3);
                    double k22 = F2Test(y1.Last() + h * k11 / 3, y2.Last() + h * k12 / 3, x + h / 3);

                    double k31 = F1Test(y1.Last() - h * k11 / 3 + h * k21, y2.Last() - h * k12 / 3 + h * k22, x + 2 * h / 3);
                    double k32 = F2Test(y1.Last() - h * k11 / 3 + h * k21, y2.Last() - h * k12 / 3 + h * k22, x + 2 * h / 3);

                    double k41 = F1Test(y1.Last() + h * k11 - h * k21 + h * k31, y2.Last() + h * k12 - h * k22 + h * k32, x + h);
                    double k42 = F2Test(y1.Last() + h * k11 - h * k21 + h * k31, y2.Last() + h * k12 - h * k22 + h * k32, x + h);

                    y1.Add(NextY(k11, k21, k31, k41, y1.Last(), h));
                    y2.Add(NextY(k12, k22, k32, k42, y2.Last(), h));
                    y1Ans.Add(F1TestFinal(x));
                    y2Ans.Add(F2TestFinal(x));
                }

                double y1Delta=double.MinValue;
                for (int i = 0; i < y1.Count; i++)
                {
                    if (Math.Abs(y1[i] - y1Ans[i])>y1Delta)
                    {
                        y1Delta = Math.Abs(y1[i] - y1Ans[i]);
                    }
                }
                Err1.Add(y1Delta);

                double y2Delta = double.MinValue;
                for (int i = 0; i < y2.Count; i++)
                {
                    if (Math.Abs(y2[i] - y2Ans[i]) > y2Delta)
                    {
                        y2Delta = Math.Abs(y2[i] - y2Ans[i]);
                    }
                }
                Err2.Add(y2Delta);
            }

            ScottPlot.Plot plot = new ScottPlot.Plot();
            plot.Add.Scatter(xList.ToArray(), Err1.ToArray());
            plot.SavePng("ErrorGraphY1.png", 2560, 1440);

            plot = new ScottPlot.Plot();
            plot.Add.Scatter(xList.ToArray(), Err2.ToArray());
            plot.SavePng("ErrorGraphY2.png", 2560, 1440);
        }
        static void GetTestGraphs()
        {
            double xLeft = 0; double xRight = 3;
            double h = 0.0001;
            List<double> y1 = new List<double>();
            List<double> y2 = new List<double>();
            List<double> y1Ans = new List<double>();
            List<double> y2Ans = new List<double>();

            y1.Add(F1TestFinal(0));
            y2.Add(F2TestFinal(0));
            y1Ans.Add(F1TestFinal(0));
            y2Ans.Add(F2TestFinal(0));
            List<double> xList = new List<double>();

            for (double eta = xLeft; eta < xRight; eta += h)
            {
                double k11 = F1Test(y1.Last(), y2.Last(), eta);
                double k12 = F2Test(y1.Last(), y2.Last(), eta);

                double k21 = F1Test(y1.Last() + h * k11 / 3, y2.Last() + h * k12 / 3, eta + h / 3);
                double k22 = F2Test(y1.Last() + h * k11 / 3, y2.Last() + h * k12 / 3, eta + h / 3);

                double k31 = F1Test(y1.Last() - h * k11 / 3 + h * k21, y2.Last() - h * k12 / 3 + h * k22, eta + 2 * h / 3);
                double k32 = F2Test(y1.Last() - h * k11 / 3 + h * k21, y2.Last() - h * k12 / 3 + h * k22, eta + 2 * h / 3);

                double k41 = F1Test(y1.Last() + h * k11 - h * k21 + h * k31, y2.Last() + h * k12 - h * k22 + h * k32, eta + h);
                double k42 = F2Test(y1.Last() + h * k11 - h * k21 + h * k31, y2.Last() + h * k12 - h * k22 + h * k32, eta + h);

                y1.Add(NextY(k11, k21, k31, k41, y1.Last(), h));
                y2.Add(NextY(k12, k22, k32, k42, y2.Last(), h));
                y1Ans.Add(F1TestFinal(eta));
                y2Ans.Add(F2TestFinal(eta));
                xList.Add(eta);
            }
            ScottPlot.Plot plot = new ScottPlot.Plot();
            plot.Add.Scatter(xList.ToArray(), y1.ToArray());
            plot.Add.Scatter(xList.ToArray(), y1Ans.ToArray());
            plot.SavePng("TestGraphsY1.png", 2560, 1440);
            plot = new ScottPlot.Plot();
            plot.Add.Scatter(xList.ToArray(), y2.ToArray());
            plot.Add.Scatter(xList.ToArray(), y2Ans.ToArray());
            plot.SavePng("TestGraphsY2.png", 2560, 1440);
            plot = new ScottPlot.Plot();
            plot.Add.Scatter(xList.ToArray(), y1.ToArray());
            plot.Add.Scatter(xList.ToArray(), y1Ans.ToArray());
            plot.Add.Scatter(xList.ToArray(), y2.ToArray());
            plot.Add.Scatter(xList.ToArray(),y2Ans.ToArray());
            plot.SavePng("TestGraphs.png", 2560, 1440);
        }
    }
}
