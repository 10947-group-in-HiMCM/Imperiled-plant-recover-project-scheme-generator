using System;
namespace modeling
{
    public class inflation
    {
        public static double inflationReturn(int year,int theNumberOfyear)
        {
            var inflationData = new double[theNumberOfyear+1];
            string path = "/Users/yanyibo/Desktop/inflation.csv";
            modeling.CsvHandler ch2 = new modeling.CsvHandler(path);
            for (int i = 1; i <= theNumberOfyear; i++)
            {
                inflationData[i] = ch2.data(1, i-1);
            }
            return (1+(inflationData[year]/100));
        }
    }
}
