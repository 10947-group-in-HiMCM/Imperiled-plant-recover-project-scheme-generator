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
    class Program
    {
        static void Main(string[] args)
        {
            int year = 30;
            double R2 = 0.7;
            Calculation.Cost(year,R2);
        }
       
    }
}
