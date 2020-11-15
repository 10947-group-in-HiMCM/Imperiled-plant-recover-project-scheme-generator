using System;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Reflection;
using _Excel = Microsoft.Office.Interop.Excel;
using MSWord = Microsoft.Office.Interop.Excel;  
using Microsoft.Office.Core;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace modeling
{
	class CsvHandler
	{
		//存储数据的矩阵
		List<List<double>> dataMatrix = new List<List<double>>();

		//构造函数
		public CsvHandler(string file)
		{
			try
			{
				//Open stream
				StreamReader sr = new StreamReader(file);
				string line = "";
				// 从文件读取并显示行，直到文件的末尾 
				while ((line = sr.ReadLine()) != null)
				{
					if (line.Length < 1)
						continue;

					//新行
					List<double> row = new List<double>();

					//读取一行
					string[] words = line.Split(',');
					for (int i = 0; i < words.Length; i++)
					{
						string iworld = words[i];
						double v = double.Parse(iworld);
						row.Add(v);
					}

					//存储列
					dataMatrix.Add(row);
				}
				//关闭文件
				sr.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine("文件有问题");
				Console.WriteLine(e.Message);
			}
		}

		//获取数据
		public double data(int x, int y)
		{
			if (y >= 0 && y < dataMatrix.Count)
			{
				List<double> rowdata = dataMatrix[y];
				if (x >= 0 && x < rowdata.Count)
				{
					return rowdata[x];
				}
			}
			return 0;
		}

	}
}
