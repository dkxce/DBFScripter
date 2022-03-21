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
		// � ������ 1.4a
	
		// ���������� � �������
        // ����� ������� � ������� ���������� � ������� (���������� ����� ��������� DBF �����)
        public override void ScriptInfo()
        {
            L("���� ������ ������ �� ������, �� ������ �������� ������� ������");
        }

        // ��������� ��� ������ �� ����� (���������� ����� �������������)
        public override Encoding FileReadEncoding { get { return Encoding.GetEncoding(1251); } }

        // ��������� ��� ������ � ���� (���������� ����� �������������)
        public override Encoding FileWriteEncoding { get { return Encoding.GetEncoding(1251); } }

        // ���������� ����� ���������� ������� (���������� ����� �������������)
        public override void BeforeProcess(string fileName, int RecordsCount)
        {		
			//
        }

		private int cInd = 0;
        // ������� ����� �������, ���������� ������� ��������� �� ��������� � ������
        // ... ���������� ��� ������ ������		
        public override bool ProcessRecord(Hashtable row)
        {
			if(IsFieldExist("NAME"))
			{
				row["NAME"] = row["NAME"].ToString().Trim();
				CurrentStatus = "NAME: "+row["NAME"].ToString().Trim()+" Previous NAME: "+(prevRow == null? "" : prevRow["NAME"].ToString().Trim());
			}
			else
			{
				CurrentStatus = "UPDATED: "+(cInd++).ToString();
			};
			return false;
        }

        // ���������� ����� ��������� ������� (���������� ����� �������������)
        public override void AfterProcess() 
        { 
            //
        }
		
		private void ListOfMethodsAndParams()
		{
			string res = "VALUE";
			
			/*
				��������� �������
			*/
			string s = "my text";
			bool bl = false;
			string[] rex = new string[0];
			int ind = 0; int startIndex = 0; int count = 5;
			
			s = string.Format("{0} {1} {2}", new object[] { 0, "any text here", 2.2 });
			bl = string.IsNullOrEmpty(s);
			bl = s.StartsWith("text");
			bl = s.EndsWith("text");
			bl = s.Contains("text");
			ind = s.IndexOf("text");
			ind = s.IndexOf("text",startIndex);
			ind = s.IndexOf("text",startIndex,count);
			ind = s.LastIndexOf("text");
			ind = s.LastIndexOf("text",startIndex);
			ind = s.LastIndexOf("text",startIndex,count);
			ind = s.Length;
			res = s.Insert(startIndex,"text");
			res = s.Remove(startIndex);
			res = s.Remove(startIndex,count);
			res = s.Replace("what","");
			rex = s.Split(new string[]{";"},StringSplitOptions.None);
			rex = s.Split(new string[]{";"},StringSplitOptions.RemoveEmptyEntries);
			res = s.Substring(startIndex);
			res = s.Substring(startIndex,count);
			res = s.ToLower();
			res = s.ToUpper();
			res = s.Trim();
			res = s.TrimStart();
			res = s.TrimEnd();
			
			/*
				��������
				DBFScript.IsInteger, DBFScript.IsDouble,
			*/
			bool isi = IsInteger("11");
			bool isd = IsDouble("11.22");
			
			/*
				��������� ����������� ������ ������ DBFScript.* 
			*/
			
			// ����� � �������
			L("Hello, world!");
			
			// ������ ��������� ������� ������
			CurrentStatus = "UPDATED: ROW 11";
			
			// ������ ����� ������; DBFScript.InputBox
			DialogResult result = InputBox("��������� �������", "������� ��������:", ref res);
			
			// ������ ������ ��������� �����
			int codePage = SelectEncodingDialog(1251);
			
			/* 
				��������� ������ ������� ������ DBFScript.* 
			*/
			
			// ������ ������ ���� (�� ����); DBFScript.SelectFieldDialog
			string fieldName = SelectFieldDialog("�������� ���� �� �������� ����� ��������� ���������:","Default Field Name");			
			string[] fields = SelectFieldDialog(new string[]{"�������� ���� � ������","�������� ���� � �����"},new string[]{"STREET","HOUSE"});
		
			/* 
				��������� ������ � ���������  � BeforeProcess, ProcessRecord � AfterProcess 
			*/
			
			bool fieldExists = IsFieldExist("NAME"); // ���������� �� ����?
			fieldExists = IsFieldExist(new string[]{"Field1","Field2","Field3"}); // ���������� �� ����?
            Hashtable a1 = this.Fields_Length; // ������� �����
            Hashtable a2 = this.Fields_Type; // ���� �����
		}
	}
}