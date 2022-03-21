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
		private string fieldOut = "ERROR";

		public override void ScriptInfo()
        {
            L("���� ������ ��������� ������������ ��������� ����� (V2)");
            L(" ��������� ��������� 09.10.2013 ");
        }

        public override Encoding FileReadEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // ��������� �����
        public override Encoding FileWriteEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // ��������� �����

        private int errors; // �������
        private System.IO.StreamWriter sw; // ���
        private string swfn; // ��� ����� ����

        public override void BeforeProcess(string fileName, int RecordsCount)
        {
            fieldName = SelectFieldDialog("�������� ���� �� �������� ����� ��������� ���������:", fieldName);
			fieldOut = SelectFieldDialog("�������� ���� � ������� ����� �������� ���������:", fieldOut);
            Console.WriteLine("��������� ����   : " + fieldName + " � " + fieldOut);

            swfn = fileName.Replace(System.IO.Path.GetExtension(fileName), "_badHouses.txt");
            sw = new System.IO.StreamWriter(swfn, false, System.Text.Encoding.GetEncoding(1251));
            sw.WriteLine("��������� �����: " + (fileName));
            sw.WriteLine("������: " + System.DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));
            sw.WriteLine(" - - - - - ");
            sw.WriteLine(fieldName);
            sw.Flush();
        }

        public override void AfterProcess()// ����� ���������
        {
            sw.WriteLine(" - - - - - ");
            sw.WriteLine("����� ������� � ��������: " + errors.ToString());
            sw.WriteLine("���������: " + System.DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));
            sw.Flush();
            sw.Close();
            L("����� ������� � ��������: " + errors.ToString());
            L("�������� �������� � ����: " + System.IO.Path.GetFileName(swfn));
        }

        public static bool isHouse(string inputHouse)
            {
                string strRegex =
                      @"^[0-9]+([a-jl-r-tzA-JL-R-Z�-��-��-��-��-��-�][0-9]{0,3})?(([��kK]|[Cc��]|���|��)[0-9]{0,}([a-jl-r-tzA-JL-R-Z�-��-��-��-��-��-�][0-9]{0,3})?(([��kK]|[Cc��]|���|��)[0-9]{0,}([a-jl-r-tzA-JL-R-Z�-��-��-��-��-��-�][0-9]{0,3})?)?)?" +
                      "(/[0-9]+([a-jl-r-tzA-JL-R-Z�-��-��-��-��-��-�][0-9]{0,3})?(([��kK]|[Cc��]|���|��)[0-9]{0,}([a-jl-r-tzA-JL-R-Z�-��-��-��-��-��-�][0-9]{0,3})?(([��kK]|[Cc��]|���|��)[0-9]{0,}([a-jl-r-tzA-JL-R-Z�-��-��-��-��-��-�][0-9]{0,3})?)?)?)?$";
                Regex re = new Regex(strRegex);
                if (re.IsMatch(inputHouse))
                    return (true);
                else
                    return (false);
            }

        // ������� ����� �������, ���������� ������� ��������� �� ��������� � ������
        // ... ���������� ��� ������ ������
        public override bool ProcessRecord(Hashtable row)
        {
            if (IsFieldExist(fieldName))
            {
                string nm = row[fieldName].ToString();
                if (!isHouse(nm))
                {
                    errors++;
                    sw.WriteLine(nm);
					if(fieldOut != fieldName) row[fieldOut] = "1";
					return true;
                }
				else
				{
					if(fieldOut != fieldName) row[fieldOut] = "0";
					return true;
				};
            };
			return false;
        }
	}
}