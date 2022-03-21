using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using DBFScripter;

namespace DBFScripting
{
	public class MainScript: DBFScripter.DBFScript
	{		
		private string fieldName = "RSTR1";
	
		public override void ScriptInfo()
        {
            L("����� ��������� ����� `>` � ���� �������, ���������� � �������.");
			L("��������� ���������� ������������ � ��� �� ����.");
			L("��������, 1>1  --> 1");
			L("��������, 1>1,2>2,3>3  --> 3");
			L("�����:\t ������� �����");
			L("������ �� 23.06.2011");
        }        

        public override void BeforeProcess(string fileName, int RecordsCount)
		{
			fieldName = SelectFieldDialog("�������� ���� �� �������� ����� ��������� ���������:", fieldName);
			Console.WriteLine("��������� ����   : "+fieldName);
		}

        public override bool ProcessRecord(Hashtable row)
        {
			Regex r = new Regex(@">", RegexOptions.IgnoreCase);
            row[fieldName] = r.Matches(row[fieldName].ToString()).Count.ToString();
            return true;
        }                
	}
}