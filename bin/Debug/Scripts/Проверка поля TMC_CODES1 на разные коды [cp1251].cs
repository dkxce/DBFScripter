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
			L("�������� ������������ TMC_CODES");
			L("��������, 27191	@E0+746+02605@E0+746+02766");
			L("��������, 27192	@E0+746+02767@E0+746-02605");
			L("������������ ����: LINK_ID, TMC_CODES1");
			L("�����:\t ������� �����");
			L("������ �� 22.06.2011");
		}
			
		public override bool ProcessRecord(Hashtable row) // ������� ����� �������, ���������� ������� �������� �� ������
		{			
			if(row["TMC_CODES1"].ToString().Length == 0) return false;
			
			string tmc_codes1 = row["TMC_CODES1"].ToString();			
			CurrentStatus = "Current TMC: "+tmc_codes1;			
			
			if (tmc_codes1.Length > 14)
            {
                string c1 = tmc_codes1.Substring(8, 5);
                string c2 = tmc_codes1.Substring(21, 5);
                if (c1 != c2)
                {
					errors++;                    
					sw.WriteLine(row["LINK_ID"].ToString() + "\t" + tmc_codes1);
                };
            };			
			return false;
		}
		
		public override Encoding FileReadEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // ��������� �����
        public override Encoding FileWriteEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // ��������� �����
		
		private int errors; // �������
		private System.IO.StreamWriter sw; // ���
		private string swfn; // ��� ����� ����
		
		public override void BeforeProcess(string fileName, int RecordsCount)// ����� ����������	
		{
			errors = 0;
			swfn = fileName.Replace(System.IO.Path.GetExtension(fileName), "_failcheck.txt");
            sw = new System.IO.StreamWriter(swfn, false, System.Text.Encoding.GetEncoding(1251));
            sw.WriteLine("��������� �����: " + (fileName));
            sw.WriteLine("������: " + System.DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));
            sw.WriteLine(" - - - - - ");
            sw.WriteLine("LINK_ID\tTMC_CODES1");
			sw.Flush();
		}
		
		public override void AfterProcess()// ����� ���������
		{			
			sw.WriteLine("����� ������� ������������: " + errors.ToString());
            sw.WriteLine(" - - - - - ");
            sw.WriteLine("���������: " + System.DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));			
			sw.Flush();
            sw.Close();
			
			L("����� ������� ������������: " + errors.ToString());
			L("�������� �������� � ����: "+System.IO.Path.GetFileName(swfn));
		}
	}
}