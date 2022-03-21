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
        private List<string> fieldNames = new List<string>();

		public override void ScriptInfo()
        {
            L("���� ������ ������������ ������ ��� ���������� DBF ��� ��������� ������");
			L("����� �������� ������ �������� ������� ������� ���������� � �.�.");
            L(" ��������� ��������� 13.09.2012 ");
        }

        public override void BeforeProcess(string fileName, int RecordsCount)
        {
            // ����� ���������� �����
            do
            {
                fieldNames.Add("NAME");
                fieldNames[fieldNames.Count - 1] = SelectFieldDialog("�������� ���� �� �������� ����� ��������� ���������:", fieldNames[fieldNames.Count-1]);
                Console.WriteLine("��������� ����   : " + fieldNames[fieldNames.Count - 1]);
            }
            while (MessageBox.Show("�� ������ ������� ��� ���� ����?", "����� ����", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes);
			
			// ���������� ������
			fieldNames.Sort();
			int index = 0;
			while (index < fieldNames.Count - 1)
			{
				if (fieldNames[index] == fieldNames[index + 1])
					fieldNames.RemoveAt(index);
				else
					index++;	
			};
        }

        // replace word (regular word ext.)
        public static Regex RW(string regTXT)
        {
            return new Regex(regTXT, RegexOptions.IgnoreCase);
        }

        // ������� ����� �������, ���������� ������� ��������� �� ��������� � ������
        // ... ���������� ��� ������ ������
        public override bool ProcessRecord(Hashtable row)
        {
            foreach(string fieldName in fieldNames)
                if (IsFieldExist(fieldName))
                {
                    string nm = row[fieldName].ToString().Replace("{M", "");
                    nm = nm.Replace("{P", "");
                    nm = nm.Replace("{3", " ");
                    nm = nm.Replace("{5", " ");

                    // ����� ��
                    nm = RW(@"\b��\b\.?|\b�����\b").Replace(nm, "��.");
                    // �������� ������ ���
                    nm = RW(@"\b���(���)?\b\.?|\b��������\b").Replace(nm, "���.");
                    // ������ ��-� �-�
                    nm = RW(@"\b��?-�\b|\b������\b").Replace(nm, "��-�");
                    // �������� ��-� �-� ���� �����
                    nm = RW(@"\b��?-�\b|\b�����?\b\.?|\b��������\b").Replace(nm, "��-�");
                    // ������� ���� ��
                    nm = RW(@"\b��(��)?\b\.?|\b�������\b").Replace(nm, "��.");
                    // ������� ��� ��� �-�
                    nm = RW(@"\b(���|���|�-�)\b\.?|\b�������\b").Replace(nm, "�-�");
                    // ���������� ������� ���
                    nm = RW(@"\b���(����)?\b\.?|\b(?<!��\.(\s|\t){1,5})����������(?!(\s|\t){1,5}��)\b").Replace(nm, "���.");

                    // 1-�/� 2-�/� 3-�/�
                    nm = RW(@"\b1-?�?�\b").Replace(nm, "1-��");
                    nm = RW(@"\b1-?�?�\b").Replace(nm, "1-��");
                    nm = RW(@"\b2-?�?�\b").Replace(nm, "2-��");
                    nm = RW(@"\b2-?�?�\b").Replace(nm, "2-��");
                    nm = RW(@"\b3-?�?�\b").Replace(nm, "3-��");
                    nm = RW(@"\b3-?�?�\b").Replace(nm, "3-��");

                    // ����������
                    nm = RW(@"\b����������\b").Replace(nm, "�/�");
                    // ������� ���
                    nm = RW(@"\b���\b\.?|\b�������\b").Replace(nm, "�.");
                    // ����
                    nm = RW(@"\b����\b").Replace(nm, "�.");
                    // �������
                    nm = RW(@"\b�������\b").Replace(nm, "���.");
                    // �����
                    nm = RW(@"\b�����\b").Replace(nm, "�.");
                    // �������
                    nm = RW(@"\b�������\b").Replace(nm, "��.");
                    // PATTERNS: http://html5pattern.com/
                    // REGULAR: http://www.mikesdotnetting.com/Article/46/CSharp-Regular-Expressions-Cheat-Sheet
                    // REFER: http://www.regular-expressions.info/reference.html      
                    // SHARED: http://habrahabr.ru/post/123845/      

                    //if(nm.IndexOf("���������� ��.") < 0)
                    //    nm = RW("����������").Replace(nm, "���.");

                    //L(row[fieldName].ToString()+" --> "+nm);
                    row[fieldName] = nm;
                };
			return true;
        }

	}
}