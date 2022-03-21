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
		// К версии 1.4a
	
		// Информация о скрипте
        // Метод выводит в консоль информацию о скрипте (вызывается перед загрузкой DBF файла)
        public override void ScriptInfo()
        {
            L("Этот скрипт ничего не делает, он только содержит базовые методы");
        }

        // Кодировка при чтении из файла (перекрытие может отсутствовать)
        public override Encoding FileReadEncoding { get { return Encoding.GetEncoding(1251); } }

        // Кодировка при записе в файл (перекрытие может отсутствовать)
        public override Encoding FileWriteEncoding { get { return Encoding.GetEncoding(1251); } }

        // Вызывается перед обработкой записей (перекрытие может отсутствовать)
        public override void BeforeProcess(string fileName, int RecordsCount)
        {		
			//
        }

		private int cInd = 0;
        // Главный метод скрипта, возвращает признак сохранять ли изменения в записе
        // ... вызывается для каждой записи		
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

        // Вызывается после обработки записей (перекрытие может отсутствовать)
        public override void AfterProcess() 
        { 
            //
        }
		
		private void ListOfMethodsAndParams()
		{
			string res = "VALUE";
			
			/*
				Строковые функции
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
				Проверка
				DBFScript.IsInteger, DBFScript.IsDouble,
			*/
			bool isi = IsInteger("11");
			bool isd = IsDouble("11.22");
			
			/*
				Доступные статические методы класса DBFScript.* 
			*/
			
			// Вывод в консоль
			L("Hello, world!");
			
			// Статус обработки текущей записи
			CurrentStatus = "UPDATED: ROW 11";
			
			// Диалог ввода текста; DBFScript.InputBox
			DialogResult result = InputBox("Заголовок диалога", "Введите значение:", ref res);
			
			// Диалог выбора кодировки файла
			int codePage = SelectEncodingDialog(1251);
			
			/* 
				Доступные методы объекта класса DBFScript.* 
			*/
			
			// Диалог выбора поля (из всех); DBFScript.SelectFieldDialog
			string fieldName = SelectFieldDialog("Выберите поле по которому будет проходить обработка:","Default Field Name");			
			string[] fields = SelectFieldDialog(new string[]{"Выберите поле с улицей","Выберите поле с домом"},new string[]{"STREET","HOUSE"});
		
			/* 
				Доступные методы и параметры  в BeforeProcess, ProcessRecord и AfterProcess 
			*/
			
			bool fieldExists = IsFieldExist("NAME"); // Существует ли поле?
			fieldExists = IsFieldExist(new string[]{"Field1","Field2","Field3"}); // Существуют ли поля?
            Hashtable a1 = this.Fields_Length; // Размеры полей
            Hashtable a2 = this.Fields_Type; // Типы полей
		}
	}
}