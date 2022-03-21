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
		public override void ScriptInfo() // Информация о скрипте
		{
			L("Проверка несовпадений TMC_CODES");
			L("Например, 27191	@E0+746+02605@E0+746+02766");
			L("Например, 27192	@E0+746+02767@E0+746-02605");
			L("Обязательные поля: LINK_ID, TMC_CODES1");
			L("Автор:\t Каримов Артем");
			L("Версия от 22.06.2011");
		}
			
		public override bool ProcessRecord(Hashtable row) // Главный метод скрипта, возвращает признак изменена ли запись
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
		
		public override Encoding FileReadEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // Кодировка файла
        public override Encoding FileWriteEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // Кодировка файла
		
		private int errors; // Счетчик
		private System.IO.StreamWriter sw; // Лог
		private string swfn; // Имя файла лога
		
		public override void BeforeProcess(string fileName, int RecordsCount)// Перед обработкой	
		{
			errors = 0;
			swfn = fileName.Replace(System.IO.Path.GetExtension(fileName), "_failcheck.txt");
            sw = new System.IO.StreamWriter(swfn, false, System.Text.Encoding.GetEncoding(1251));
            sw.WriteLine("Обработка файла: " + (fileName));
            sw.WriteLine("Начата: " + System.DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));
            sw.WriteLine(" - - - - - ");
            sw.WriteLine("LINK_ID\tTMC_CODES1");
			sw.Flush();
		}
		
		public override void AfterProcess()// После обработки
		{			
			sw.WriteLine("Всего найдено несовпадений: " + errors.ToString());
            sw.WriteLine(" - - - - - ");
            sw.WriteLine("Закончена: " + System.DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));			
			sw.Flush();
            sw.Close();
			
			L("Всего найдено несовпадений: " + errors.ToString());
			L("Протокол сохранен в файл: "+System.IO.Path.GetFileName(swfn));
		}
	}
}