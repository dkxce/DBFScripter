using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using DBFScripter;

namespace DBFScripting
{
	public class MainScript: DBFScripter.DBFScript
	{		
		public override void ScriptInfo() // ���������� � �������
		{
			L("�������������� ����� DBF �� ��������� [cp1251](Win) � [cp866](DOS/Acccess)");
		}
		
		public override bool ProcessRecord(Hashtable row) // ������� ����� �������, ���������� ������� ��������� �� ��������� � ������
		{	
			return true;
		}
		
		public override Encoding FileReadEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // ��������� �����
        public override Encoding FileWriteEncoding { get { return System.Text.Encoding.GetEncoding(866); } } // ��������� �����
	}
}