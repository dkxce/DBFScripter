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
			L("Получение целой части от дома");
			L("Например, 15К1ВЛ2 --> 15");
			L("Например, 21С1 --> 21");
			L("Номер дома берется из поля NAME, результат обработки возращается в NAME1");
			L("Обязательные поля: NAME (string), NAME1 (string)");
			L("Автор:\t Каримов Артем");
			L("Версия от 22.06.2011");
		}
		
		public override bool ProcessRecord(Hashtable row) // Главный метод скрипта, возвращает признак изменена ли запись
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