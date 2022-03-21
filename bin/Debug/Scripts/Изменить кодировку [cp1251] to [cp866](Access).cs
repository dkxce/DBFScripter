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
			L("Преобразование полей DBF из кодировки [cp1251](Win) в [cp866](DOS/Acccess)");
		}
		
		public override bool ProcessRecord(Hashtable row) // Главный метод скрипта, возвращает признак сохранять ли изменения в записе
		{	
			return true;
		}
		
		public override Encoding FileReadEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // Кодировка файла
        public override Encoding FileWriteEncoding { get { return System.Text.Encoding.GetEncoding(866); } } // Кодировка файла
	}
}