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
            double xLeft = 0.001; double xRight=0.2;
            double h = 0.0001;
            List<double> y1 = new List<double>();
            List<double> y2 = new List<double>();
            /*List<double> y1Ans = new List<double>();
            List<double> y2Ans = new List<double>();*/
            y1.Add(2);
            y2.Add(2);
            /*
            y1Ans.Add(f1ans(xLeft));
            y2Ans.Add(f2ans(xLeft));
            */
            List<double>xList=new List<double>();
            for (double eta = xLeft; eta< xRight; eta += h)
            {
                /* double k11 = f1(y1.Last(),y2.Last(),x);
                 double k21 = f1(y1.Last() + h * k11 / 3, y2.Last() + h * k11 / 3,x + h / 3);
                 double k31 = f1(y1.Last() - h * k11 / 3 + h * k21, y2.Last() - h * k11 / 3 + h * k21, x + 2 * h / 3);
                 double k41 = f1(y1.Last() + h * k11 - h * k21 + h * k31, y2.Last() + h * k11 - h * k21 + h * k31, x + h) ;

                 double k12 = f2(y1.Last(), y2.Last(), x);
                 double k22 = f2(y1.Last() + h * k12 / 3, y2.Last() + h * k12 / 3, x + h / 3);
                 double k32 = f2(y1.Last() - h * k12 / 3 + h * k22, y2.Last() - h * k12 / 3 + h * k22, x + 2 * h / 3);
                 double k42 = f2(y1.Last() + h * k12 - h * k22 + h * k32, y2.Last() + h * k12 - h * k22 + h * k32, x + h);*/

                double k11 = f1(y1.Last(), y2.Last());
                double k12 = f2(y1.Last(), eta);

                double k21 = f1(y1.Last() + h * k11 / 3, y2.Last() + h * k12 / 3);
                double k22 = f2(y1.Last() + h * k11 / 3, eta + h / 3);
                
                double k31 = f1(y1.Last() - h * k11 / 3 + h * k21, y2.Last() - h * k12 / 3 + h * k22);
                double k32 = f2(y1.Last() - h * k11 / 3 + h * k21, eta + 2 * h / 3);
                
                double k41 = f1(y1.Last() + h * k11 - h * k21 + h * k31, y2.Last() + h * k12 - h * k22 + h * k32);
                double k42 = f2(y1.Last() + h * k11 - h * k21 + h * k31, eta + h);





                y1.Add(nextY(k11, k21, k31, k41, y1.Last(), h));
                y2.Add(nextY(k12, k22, k32, k42, y2.Last(), h));
                /*y1Ans.Add(f1ans(x));
                y2Ans.Add(f2ans(x));*/
                xList.Add(eta);
            }
            ScottPlot.Plot plot = new ScottPlot.Plot();
            plot.Add.Scatter(xList.ToArray(),y1.ToArray());
            /*plot.Add.Scatter(xList.ToArray(), y1Ans.ToArray());*/
            /*plot.SavePng("GraphY1.png",2560,1440);*/
            /*plot=new ScottPlot.Plot();*/
            plot.Add.Scatter(xList.ToArray(),y2.ToArray());
            /*plot.Add.Scatter(xList.ToArray(),y2Ans.ToArray());*/
            plot.SavePng("Graph.png", 2560, 1440);
            /*Console.Write("y1: ");
            foreach(var y in y1)
            {
                Console.Write(y.ToString());
            }
            Console.Write("\n\nГотовое y1: ");
            foreach (var y in y1Ans)
            {
                Console.Write(y.ToString());
            }
            Console.Write("\n\ny2: ");
            foreach(var y in y2)
            {
                Console.Write(y.ToString());
            }
            Console.Write("\n\nГотовое y2: ");
            foreach (var y in y2Ans)
            {
                Console.Write(y.ToString());
            }
            Console.Read();*/
        }
        /*
        static double f1(double y1, double y2, double x)
        {
            return y1 + 1 / y2 - Math.Exp(x);
        }
        static double f2(double y1, double y2, double x)
        {
            return y2 - 1 / y1 - Math.Exp(-x);
        }
        */
        static double f1(double x1, double x2)
        {
            return x2-(x1*(Math.Pow(x1,2)/3-1));
        }
        static double f2(double x1, double eta)
        {
            return -eta*x1;
        }
        static double f1ans(double x)
        {
            return Math.Exp(x);
        }
        static double f2ans(double x) 
        {  
            return Math.Exp(-x); 
        }
        static double nextY(double k1, double k2, double k3, double k4, double prevY,double h)
        {
            return prevY+h*(k1+3*k2+3*k3+k4)/8;
        }
    }
}
