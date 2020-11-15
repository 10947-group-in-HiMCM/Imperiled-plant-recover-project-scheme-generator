using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using System;
using System.Threading;
using IronPython.Modules;
using IronPython;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using MSWord = Microsoft.Office.Interop.Excel;
using System.Threading.Tasks;
using IronPython.Modules.Bz2;
using Microsoft.Office.Core;



namespace CSharpCallPython
{
    class Calculation
    {
        public static double Cost(int year,double theGOF)
        {
            //net layer
            wfapp1.SocketHelper socketHelper = new wfapp1.SocketHelper("127.0.0.1", 9091);

            //data layer
            
            var T = new int[41];//Enter all the period of the plant
            int[] pin = { 0, 3, 3, 3, 3, 3, 3, 4, 3, 5, 3, 5, 5, 5, 5, 3, 3, 5, 5, 16, 3, 3, 5, 5, 6, 3, 5, 5, 5, 5, 5, 3, 5, 9, 24, 5, 11, 5, 5, 5, 5 };
            pin.CopyTo(T, 0);
            var theLatestTimeForInvestment = new int[41];
            var ValueOfExpectation = new double[41];
            var Benefits = new double[41];
            var FeasilbilityOfSuccess = new double[41];
            var AverageFOS = new double[41];
            //entering all the cost of flower per year
            string pathOne = "/Users/yanyibo/Desktop/CostDataIO.csv";
            modeling.CsvHandler ch = new modeling.CsvHandler(pathOne);
            var costData = new double[41, 26];
            var theExpectedScheme = new double[10010, 41, year+1];
            var theExpectedR2 = new double[10010];//the R2 of expected scheme
            var theExpectedYearCostScheme = new double[10010, 41, year+1];
            var theTotalCostOfEveryExpectedScheme = new double[10010];
            var theEveryYear = new double[10010, year+1];
            var theExpectedA = new double[10010];
            var theExpectedB = new double[10010];
            var theExpectedC = new double[10010];
            for (int i = 1; i <= 40; i++)
                for (int j = 1; j <= 25; j++)
                {
                    costData[i, j] = ch.data(j - 1, i - 1);
                }
            // enetering all the index of the flower
            var theIndexOfFower = new double[41];
            for (int i = 1; i <= 40; i++)
            {
                theIndexOfFower[i] = ch.data(25, i -1);
            }
            //entering all the benefits of flower
            string pathTwo = "/Users/yanyibo/Desktop/Expectation.csv";
            modeling.CsvHandler ch2 = new modeling.CsvHandler(pathTwo);
            for (int j = 1; j <= 40; j++)
            {
                Benefits[j] = ch2.data(0, j - 1);
            }

            //entering all the feasibility of success
            for (int i = 1; i <= 40; i++)
            {
                FeasilbilityOfSuccess[i] = ch2.data(1, i - 1);
            }

            //the lastest time to invest
            for (int i = 1; i <= 40; i++)
            {
                theLatestTimeForInvestment[i] = year - T[i] + 1;
            }

            //the calcualtion of value of expectation
            for (int i = 1; i <= 40; i++)
            {
                ValueOfExpectation[i] = FeasilbilityOfSuccess[i] * Benefits[i];
            }

            // Average value of expectation
            for (int i = 0; i < 40; i++)
            {
                AverageFOS[i] = Math.Round(ValueOfExpectation[i] / T[i], 2);
            }
            var fangcha = new double[100001];
            int countOfScheme = 0;//count the amount of scheme that fit the real situation
            //main
            for (int i = 1; i <= 10000; i++)
            {

                var theStartpoint = new int[41];
                //Monte Carlo method to generate the different scheme
                for (int j = 1; j <= 40; j++)
                {
                    Random theStart = new Random();
                    int starpoint = theStart.Next(1, theLatestTimeForInvestment[j]);
                    theStartpoint[j] = starpoint;
                }

                //Scheme generator
                var theScheme = new double[41, year+1];
                for (int k = 0; k <= 40; k++)
                    for (int j = 0; j <= year; j++)
                        theScheme[k, j] = 0;
                for (int k = 1; k <= 40; k++)
                    for (int j = theStartpoint[k]; j <= theStartpoint[k] + T[k] - 1; j++)
                    {
                        theScheme[k, j] = 1;
                    }

                // The value of expectation in the scheme that is been generated
                var Expectation = new double[41, year+1];
                for (int k = 1; k <= 40; k++)
                    for (int j = 1; j <= year; j++)
                        Expectation[k, j] = theScheme[k, j] * AverageFOS[k];

                //Average Expectation in the scheme
                var AEIS = new double[year+1];
                for (int j = 0; j <= year; j++)
                    AEIS[j] = 0;
                for (int k = 1; k <= year ; k++)
                {
                    for (int j = 1; j <= 40; j++)
                    {
                        AEIS[k] += Expectation[j, k];
                    }
                    AEIS[k] = Math.Round(AEIS[k], 2);
                }
                double AverageExpectation = 0;
                for (int k = 1; k <= year; k++)
                    AverageExpectation += AEIS[k];
                AverageExpectation /= year;

                // The First election base on Expectation
                double variance = 0;
                double denominator = 0;
                for (int k = 1; k <= year; k++)
                    denominator += Math.Pow((AEIS[k] - AverageExpectation), 2);
                variance = denominator / year;

                // Entering the second elecation--> using R2
                var TheYearCostScheme = new double[41, year+1];
                if (variance <= 0.5)
                {
                    int count = 1;
                    for (int k = 1; k <= 40; k++)
                    {
                        for (int j = theStartpoint[k]; j <= theStartpoint[k] + T[k]; j++)
                        {
                            var inflation = modeling.inflation.inflationReturn(j,year);
                            TheYearCostScheme[k, j] = costData[k, count] * theScheme[k, j] * inflation;
                            count++;
                        }
                        count = 0;
                    }
                    var theEveryYearCost = new double[year+1];
                    for (int k = 1; k <= year; k++)
                        for (int j = 1; j <= 40; j++)
                        {
                            theEveryYearCost[k] += Math.Round(TheYearCostScheme[j, k]);
                        }
                    Console.WriteLine();
                    //R2 calculation
                    //Write the every year cost data in to the txt file
                    var everyyearCost = new string[year+1];
                    for (int k = 1; k <= year; k++)
                    {
                        everyyearCost[k] = Convert.ToString(theEveryYearCost[k]);
                    }
                    //TextWriter.program.Writer(everyyearCost);
                    string senddata = "";
                    for (int m = 0; m < everyyearCost.Length; m++)
                    {
                        if (everyyearCost[m] == null)
                            continue;
                        string endstr = ",";
                        if (m == everyyearCost.Length - 1)
                            endstr = "";
                        senddata += everyyearCost[m].ToString() + endstr;
                    }
                    string result = socketHelper.sendProcess("127.0.0.1", 9090, "data", senddata);
                    result.Remove('\0');
                    string[] resultProcessed = result.Split(',');
                    double a = Convert.ToDouble(resultProcessed[0]);
                    Math.Round(a, 2);
                    double b = Convert.ToDouble(resultProcessed[1]);
                    Math.Round(b, 2);
                    double c = Convert.ToDouble(resultProcessed[2]);
                    Math.Round(c, 2);
                    var TheFitingPoints = new double[year+1];
                    for (int k = 1; k <= year; k++)
                    {
                        TheFitingPoints[k] = a * Math.Log(b * k + c);
                    }

                    // The calculation of R2
                    var EveryYearCost = new int[year+1];
                    for (int k = 1; k <= year; k++)
                    {
                        theEveryYearCost[k] = Convert.ToDouble(everyyearCost[k]);
                    }
                    double R2 = modeling.curveFit.Fiting(TheFitingPoints, theEveryYearCost,year);
                    //Whether the R2 have above the expected value
                    Console.WriteLine(R2);
                    if (R2 > theGOF)
                    {
                        countOfScheme++;
                        for (int k = 1; k <= 40; k++)
                            for (int j = 1; j <= year ; j++)
                            {
                                theExpectedScheme[countOfScheme, k, j] = theScheme[k, j];
                            }
                        theExpectedR2[countOfScheme] = R2;
                        // the expected scheme's paying scheme
                        //record the data
                        for (int k = 1; k <= 40; k++)
                            for (int j = 1; j <= year; j++)
                            {
                                theExpectedYearCostScheme[countOfScheme, k, j] = TheYearCostScheme[k, j];
                            }
                        double thetotalCost = 0;
                        for (int k = 1; k <= year; k++)
                        {
                            thetotalCost += theEveryYearCost[k];
                            theTotalCostOfEveryExpectedScheme[countOfScheme] = thetotalCost;
                        }
                        for (int k = 1; k <= year; k++)
                        {
                            theEveryYear[countOfScheme, k] = theEveryYearCost[k];
                        }
                        theExpectedA[countOfScheme] = a;
                        theExpectedB[countOfScheme] = b;
                        theExpectedC[countOfScheme] = c;
                        

                    }
                    else
                        continue;


                }
                else
                    continue;
            }

            //find the best answer
            int theBestSolution = 1;
            for (int i = 1; i <= 10000; i++)
            {
                if (theTotalCostOfEveryExpectedScheme[i] < theTotalCostOfEveryExpectedScheme[theBestSolution] && theTotalCostOfEveryExpectedScheme[i] != 0)
                    theBestSolution = i;
                else
                    continue;
            }
            // OutPut
            Console.WriteLine("The fitting line: "+theExpectedA[theBestSolution]+"log"+"("+theExpectedB[theBestSolution]+"x+"+theExpectedC[theBestSolution]+")");
            Console.WriteLine();
            Console.WriteLine("The goodness of fitting: "+theExpectedR2[theBestSolution]);
            Console.WriteLine();
            //OutPut the shceme
            for (int k = 1; k <= 40; k++)
            {
                Console.WriteLine("1-Flowering Plants-"+theIndexOfFower[k]);

                for (int j = 1; j <= year; j++)
                {
                    Console.Write(theExpectedScheme[theBestSolution,k,j]+" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("The every year cost are:");
            for(int i=1;i<=year;i++)
                Console.WriteLine(theEveryYear[theBestSolution,i]);
            return theExpectedR2[theBestSolution];
            


        }

    }
}

