using System;
namespace modeling
{
    public class curveFit
    {
        public static double Fiting(double[] a, double[] b,int theNumebrOfyear)//a 拟合数值 b 待拟合数值
        {
            double SSE = 0;
            double TheSum = 0;
            for (int i = 1; i <= theNumebrOfyear; i++)
                TheSum += b[i];
            double theAverage = TheSum/30;
            for (int i = 1; i <= theNumebrOfyear; i++)
            {
                SSE += Math.Pow((b[i] - theAverage), 2);
            }
            double RSS = 0;
            for (int i = 1; i <= theNumebrOfyear; i++)
            {
                RSS += Math.Pow((a[i] - b[i]), 2);
            }
            return (1 - (RSS/SSE));
        }
    }
}
