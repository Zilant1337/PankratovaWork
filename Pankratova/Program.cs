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
            double alphaLeft = 0/*1.21922*/; double alphaRight=2/*1.5*/;
            double h = 0.0001;
            List<double> y1 = new List<double>();
            List<double> y2 = new List<double>();
            List<double> y3 = new List<double>();
            y1.Add(1);
            y2.Add(1 + alphaLeft);
            y3.Add(1 + alphaLeft);

            List<double>xList=new List<double>();
            xList.Add(alphaLeft);
            for (double alpha = alphaLeft; alpha< alphaRight; alpha += h)
            {
                double k11 = f1(y1.Last(), y2.Last(), y3.Last(), alpha);
                double k12 = f2(y1.Last(), y2.Last(), y3.Last(), alpha);
                double k13 = f3(y1.Last(), y2.Last(), y3.Last(), alpha);

                double k21 = f1(y1.Last() + h * k11 / 2, y2.Last() + h * k12 / 2, y3.Last() + h * k13 / 2, alpha + h / 2);
                double k22 = f2(y1.Last() + h * k11 / 2, y2.Last() + h * k12 / 2, y3.Last() + h * k13 / 2, alpha + h / 2);
                double k23 = f3(y1.Last() + h * k11 / 2, y2.Last() + h * k12 / 2, y3.Last() + h * k13 / 2, alpha + h / 2);

                double k31 = f1(y1.Last() + h * k21 / 2, y2.Last() + h * k22 / 2, y3.Last() + h * k23 / 2, alpha + h / 2);
                double k32 = f2(y1.Last() + h * k21 / 2, y2.Last() + h * k22 / 2, y3.Last() + h * k23 / 2, alpha + h / 2);
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
            var scatter1 = plot.Add.Scatter(xList.ToArray(),y1.ToArray());
            scatter1.LegendText ="f1";
            var scatter2 = plot.Add.Scatter(xList.ToArray(),y2.ToArray());
            scatter2.LegendText = "f2";
            var scatter3 = plot.Add.Scatter(xList.ToArray(), y3.ToArray());
            scatter3.LegendText = "f3";
            plot.ShowLegend();
            plot.SavePng("Graph.png", 2560, 1440);

            DoTestStuff();
        }

        static double f1Test(double y1, double y2, double x)
        {
            return -y2+y1*(y1*y1+y2*y2-1);
        }
        static double f2Test(double y1, double y2, double x)
        {
            return y1+y2*(y1*y1+y2*y2-1);
        }
        static double f1TestAnswer(double x)
        {
            return Math.Cos(x)/Math.Sqrt(1+Math.Exp(2*x));
        }
        static double f2TestAnswer(double x)
        {
            return Math.Sin(x) / Math.Sqrt(1 + Math.Exp(2 * x));
        }
        static void DoTestStuff()
        {
            double alphaLeft = 0; double alphaRight = 5;
            List<double> Error1 = new List<double>();
            List<double> Error2 = new List<double>();
            List<double> ErrorH1 = new List<double>();
            List<double> ErrorH2 = new List<double>();

            List<double> alphaList = new List<double>();
            for (double h = 0.01; h >= 0.0001; h = h / 1.1)
            {
                List<double> y1 = new List<double>();
                List<double> y2 = new List<double>();
                List<double> y1Ans = new List<double>();
                List<double> y2Ans = new List<double>();

                y1.Add(f1TestAnswer(0));
                y2.Add(f2TestAnswer(0));
                y1Ans.Add(f1TestAnswer(0));
                y2Ans.Add(f2TestAnswer(0));

                alphaList.Add(h);

                for (double alpha = alphaLeft + h; alpha < alphaRight; alpha += h)
                {
                    double k11 = f1Test(y1.Last(), y2.Last(), alpha);
                    double k12 = f2Test(y1.Last(), y2.Last(), alpha);

                    double k21 = f1Test(y1.Last() + h * k11 / 2, y2.Last() + h * k12 / 2, alpha + h / 2);
                    double k22 = f2Test(y1.Last() + h * k11 / 2, y2.Last() + h * k12 / 2, alpha + h / 2);

                    double k31 = f1Test(y1.Last() - h * k11 / 2 + h * k21, y2.Last() - h * k12 / 2 + h * k22, alpha + h / 2);
                    double k32 = f2Test(y1.Last() - h * k11 / 2 + h * k21, y2.Last() - h * k12 / 2 + h * k22, alpha + h / 2);

                    double k41 = f1Test(y1.Last() + h * k31, y2.Last() + h * k32, alpha + h);
                    double k42 = f2Test(y1.Last() + h * k31, y2.Last() + h * k32, alpha + h);

                    y1.Add(nextY(k11, k21, k31, k41, y1.Last(), h));
                    y2.Add(nextY(k12, k22, k32, k42, y2.Last(), h));
                    y1Ans.Add(f1TestAnswer(alpha));
                    y2Ans.Add(f2TestAnswer(alpha));
                }

                double y1Delta = double.MinValue;
                for (int i = 0; i < y1.Count; i++)
                {
                    if (Math.Abs(y1[i] - y1Ans[i]) > y1Delta)
                    {
                        y1Delta = Math.Abs(y1[i] - y1Ans[i]);
                    }
                }
                Error1.Add(y1Delta);
                ErrorH1.Add(y1Delta / Math.Pow(h, 4));

                double y2Delta = double.MinValue;
                for (int i = 0; i < y2.Count; i++)
                {
                    if (Math.Abs(y2[i] - y2Ans[i]) > y2Delta)
                    {
                        y2Delta = Math.Abs(y2[i] - y2Ans[i]);
                    }
                }
                Error2.Add(y2Delta);
                ErrorH2.Add(y2Delta / Math.Pow(h, 4));
            }

            ScottPlot.Plot plot = new ScottPlot.Plot();
            plot.Add.Scatter(alphaList.ToArray(), Error1.ToArray());
            plot.SavePng("ErrorGraphTestF1.png", 2560, 1440);

            plot = new ScottPlot.Plot();
            plot.Add.Scatter(alphaList.ToArray(), ErrorH1.ToArray());
            plot.SavePng("ErrorH4GraphTestF1.png", 2560, 1440);

            plot = new ScottPlot.Plot();
            plot.Add.Scatter(alphaList.ToArray(), ErrorH2.ToArray());
            plot.SavePng("ErrorH4GraphTestF2.png", 2560, 1440);

            plot = new ScottPlot.Plot();
            plot.Add.Scatter(alphaList.ToArray(), Error2.ToArray());
            plot.SavePng("ErrorGraphTestF2.png", 2560, 1440);
        }
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
