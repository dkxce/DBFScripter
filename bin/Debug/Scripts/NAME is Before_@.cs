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
        private string fieldName = "NAME";

		public override void ScriptInfo()
        {
            L("���� ������ ��������� � ���� ������ ��� ������� ����� @");
        }

        public override void BeforeProcess(string fileName, int RecordsCount)
        {
            fieldName = SelectFieldDialog("�������� ���� �� �������� ����� ��������� ���������:", fieldName);
            Console.WriteLine("��������� ����   : " + fieldName);
        }

        public override bool ProcessRecord(Hashtable row)
        {
            if (IsFieldExist(fieldName))
            {
                string nm = row[fieldName].ToString().Trim();
                string[] bdog = nm.Split(new string[] {"@"},StringSplitOptions.None);
                row[fieldName] = bdog[0];
            };
			return true;
        }
	}
}