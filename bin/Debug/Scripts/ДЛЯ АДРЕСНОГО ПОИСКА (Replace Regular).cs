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
            L("Этот скрипт обрабатывает СТРОКИ при подготовке DBF для адресного поиска");
			L("УЛИЦА ПЕРЕУЛОК ПРОЕЗД ПРОСПЕКТ ПЛОЩАДЬ БУЛЬВАР НАБЕРЕЖНАЯ и т.д.");
            L(" последние изменения 13.09.2012 ");
        }

        public override void BeforeProcess(string fileName, int RecordsCount)
        {
            // Выбор нескольких полей
            do
            {
                fieldNames.Add("NAME");
                fieldNames[fieldNames.Count - 1] = SelectFieldDialog("Выберите поле по которому будет проходить обработка:", fieldNames[fieldNames.Count-1]);
                Console.WriteLine("Обработка поля   : " + fieldNames[fieldNames.Count - 1]);
            }
            while (MessageBox.Show("Вы хотите выбрать еще одно поле?", "Выбор поля", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes);
			
			// Исключение дублей
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

        // Главный метод скрипта, возвращает признак сохранять ли изменения в записе
        // ... вызывается для каждой записи
        public override bool ProcessRecord(Hashtable row)
        {
            foreach(string fieldName in fieldNames)
                if (IsFieldExist(fieldName))
                {
                    string nm = row[fieldName].ToString().Replace("{M", "");
                    nm = nm.Replace("{P", "");
                    nm = nm.Replace("{3", " ");
                    nm = nm.Replace("{5", " ");

                    // УЛИЦА УЛ
                    nm = RW(@"\bУЛ\b\.?|\bУЛИЦА\b").Replace(nm, "ул.");
                    // ПЕРЕУЛОК ПЕРЕУЛ ПЕР
                    nm = RW(@"\bПЕР(ЕУЛ)?\b\.?|\bПЕРЕУЛОК\b").Replace(nm, "пер.");
                    // ПРОЕЗД ПР-Д П-Д
                    nm = RW(@"\bПР?-Д\b|\bПРОЕЗД\b").Replace(nm, "пр-д");
                    // ПРОСПЕКТ ПР-Т П-Т ПРОС ПРОСП
                    nm = RW(@"\bПР?-Т\b|\bПРОСП?\b\.?|\bПРОСПЕКТ\b").Replace(nm, "пр-т");
                    // ПЛОЩАДЬ ПЛОЩ ПЛ
                    nm = RW(@"\bПЛ(ОЩ)?\b\.?|\bПЛОЩАДЬ\b").Replace(nm, "пл.");
                    // БУЛЬВАР БЛВ БУЛ Б-Р
                    nm = RW(@"\b(блв|бул|б-р)\b\.?|\bбульвар\b").Replace(nm, "б-р");
                    // НАБЕРЕЖНАЯ НАБЕРЕЖ НАБ
                    nm = RW(@"\bНАБ(ЕРЕЖ)?\b\.?|\b(?<!ул\.(\s|\t){1,5})НАБЕРЕЖНАЯ(?!(\s|\t){1,5}ул)\b").Replace(nm, "наб.");

                    // 1-Й/Я 2-Й/Я 3-Й/Я
                    nm = RW(@"\b1-?Ы?Й\b").Replace(nm, "1-ый");
                    nm = RW(@"\b1-?А?Я\b").Replace(nm, "1-ая");
                    nm = RW(@"\b2-?О?Й\b").Replace(nm, "2-ой");
                    nm = RW(@"\b2-?А?Я\b").Replace(nm, "2-ая");
                    nm = RW(@"\b3-?И?Й\b").Replace(nm, "3-ий");
                    nm = RW(@"\b3-?Я?Я\b").Replace(nm, "3-яя");

                    // МИКРОРАЙОН
                    nm = RW(@"\bМИКРОРАЙОН\b").Replace(nm, "м/р");
                    // ДЕРЕВНЯ ДЕР
                    nm = RW(@"\bДЕР\b\.?|\bДЕРЕВНЯ\b").Replace(nm, "д.");
                    // СЕЛО
                    nm = RW(@"\bСЕЛО\b").Replace(nm, "с.");
                    // ПОСЕЛОК
                    nm = RW(@"\bПОСЕЛОК\b").Replace(nm, "пос.");
                    // ГОРОД
                    nm = RW(@"\bГОРОД\b").Replace(nm, "г.");
                    // СТАНЦИЯ
                    nm = RW(@"\bСТАНЦИЯ\b").Replace(nm, "ст.");
                    // PATTERNS: http://html5pattern.com/
                    // REGULAR: http://www.mikesdotnetting.com/Article/46/CSharp-Regular-Expressions-Cheat-Sheet
                    // REFER: http://www.regular-expressions.info/reference.html      
                    // SHARED: http://habrahabr.ru/post/123845/      

                    //if(nm.IndexOf("НАБЕРЕЖНАЯ ул.") < 0)
                    //    nm = RW("НАБЕРЕЖНАЯ").Replace(nm, "наб.");

                    //L(row[fieldName].ToString()+" --> "+nm);
                    row[fieldName] = nm;
                };
			return true;
        }

	}
}