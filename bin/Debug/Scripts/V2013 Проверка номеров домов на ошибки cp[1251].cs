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
            L("Этот скрипт проверяет правильность нумерации домов (V2)");
            L(" последние изменения 09.10.2013 ");
        }

        public override Encoding FileReadEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // Кодировка файла
        public override Encoding FileWriteEncoding { get { return System.Text.Encoding.GetEncoding(1251); } } // Кодировка файла

        private int errors; // Счетчик
        private System.IO.StreamWriter sw; // Лог
        private string swfn; // Имя файла лога

        public override void BeforeProcess(string fileName, int RecordsCount)
        {
            fieldName = SelectFieldDialog("Выберите поле по которому будет проходить обработка:", fieldName);
			fieldOut = SelectFieldDialog("Выберите поле в который будет сохранен результат:", fieldOut);
            Console.WriteLine("Обработка поля   : " + fieldName + " в " + fieldOut);

            swfn = fileName.Replace(System.IO.Path.GetExtension(fileName), "_badHouses.txt");
            sw = new System.IO.StreamWriter(swfn, false, System.Text.Encoding.GetEncoding(1251));
            sw.WriteLine("Обработка файла: " + (fileName));
            sw.WriteLine("Начата: " + System.DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));
            sw.WriteLine(" - - - - - ");
            sw.WriteLine(fieldName);
            sw.Flush();
        }

        public override void AfterProcess()// После обработки
        {
            sw.WriteLine(" - - - - - ");
            sw.WriteLine("Всего найдено с ошибками: " + errors.ToString());
            sw.WriteLine("Закончена: " + System.DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));
            sw.Flush();
            sw.Close();
            L("Всего найдено с ошибками: " + errors.ToString());
            L("Протокол сохранен в файл: " + System.IO.Path.GetFileName(swfn));
        }

        public static bool isHouse(string inputHouse)
            {
                string strRegex =
                      @"^[0-9]+([a-jl-r-tzA-JL-R-Zа-ил-рт-яА-ИЛ-РТ-Я][0-9]{0,3})?(([кКkK]|[CcСс]|СТР|ВЛ)[0-9]{0,}([a-jl-r-tzA-JL-R-Zа-ил-рт-яА-ИЛ-РТ-Я][0-9]{0,3})?(([кКkK]|[CcСс]|СТР|ВЛ)[0-9]{0,}([a-jl-r-tzA-JL-R-Zа-ил-рт-яА-ИЛ-РТ-Я][0-9]{0,3})?)?)?" +
                      "(/[0-9]+([a-jl-r-tzA-JL-R-Zа-ил-рт-яА-ИЛ-РТ-Я][0-9]{0,3})?(([кКkK]|[CcСс]|СТР|ВЛ)[0-9]{0,}([a-jl-r-tzA-JL-R-Zа-ил-рт-яА-ИЛ-РТ-Я][0-9]{0,3})?(([кКkK]|[CcСс]|СТР|ВЛ)[0-9]{0,}([a-jl-r-tzA-JL-R-Zа-ил-рт-яА-ИЛ-РТ-Я][0-9]{0,3})?)?)?)?$";
                Regex re = new Regex(strRegex);
                if (re.IsMatch(inputHouse))
                    return (true);
                else
                    return (false);
            }

        // Главный метод скрипта, возвращает признак сохранять ли изменения в записе
        // ... вызывается для каждой записи
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