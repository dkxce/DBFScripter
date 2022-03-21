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
			L("��������� ����� ����� �� ����");
			L("��������, 15�1��2 --> 15");
			L("��������, 21�1 --> 21");
			L("����� ���� ������� �� ���� NAME, ��������� ��������� ����������� � NAME1");
			L("������������ ����: NAME (string), NAME1 (string)");
			L("�����:\t ������� �����");
			L("������ �� 22.06.2011");
		}
		
		public override bool ProcessRecord(Hashtable row) // ������� ����� �������, ���������� ������� �������� �� ������
		{			
			int newVal = 0;
			int tryVal = 0;
			int i = 1;
			if (!int.TryParse(row["NAME"].ToString(), out newVal))
				while (int.TryParse(row["NAME"].ToString().Substring(0, i++), out tryVal))
					newVal = tryVal;
			row["NAME1"] = newVal.ToString();
			return true;
		}
	}
}