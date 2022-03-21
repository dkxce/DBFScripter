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
            L(" последние изменения 09.10.2013 ");
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
                    nm = nm.Replace("@", " ");

                    // STANDARD FROM OCT 2013
                    nm = RW(@"\b1-?Ы?Й\b").Replace(nm, "1-й");
                    nm = RW(@"\b1-?А?Я\b").Replace(nm, "1-я");
                    nm = RW(@"\b2-?О?Й\b").Replace(nm, "2-й");
                    nm = RW(@"\b2-?А?Я\b").Replace(nm, "2-я");
                    nm = RW(@"\b3-?И?Й\b").Replace(nm, "3-й");
                    nm = RW(@"\b3-?Я?Я\b").Replace(nm, "3-я");
                    nm = RW(@"\b4-?Ы?Й\b").Replace(nm, "4-й");
                    nm = RW(@"\b4-?А?Я\b").Replace(nm, "4-я");
                    nm = RW(@"\b5-?Ы?Й\b").Replace(nm, "5-й");
                    nm = RW(@"\b5-?А?Я\b").Replace(nm, "5-я");
                    nm = RW(@"\b6-?О?Й\b").Replace(nm, "6-й");
                    nm = RW(@"\b6-?А?Я\b").Replace(nm, "6-я");
                    nm = RW(@"\b7-?О?Й\b").Replace(nm, "7-й");
                    nm = RW(@"\b7-?А?Я\b").Replace(nm, "7-я");
                    nm = RW(@"\b8-?О?Й\b").Replace(nm, "8-й");
                    nm = RW(@"\b8-?А?Я\b").Replace(nm, "8-я");
                    nm = RW(@"\b9-?Ы?Й\b").Replace(nm, "9-й");
                    nm = RW(@"\b9-?А?Я\b").Replace(nm, "9-я");
                    nm = RW(@"\b10-?Ы?Й\b").Replace(nm, "10-й");
                    nm = RW(@"\b10-?А?Я\b").Replace(nm, "10-я");

                    nm = RW(@"\bб(лв|ул)\b\.?|\bбульвар\b").Replace(nm, "б-р");
                    //nm = RW(@"\bвладение\b|\bвл(ад)\b\.?").Replace(nm, "ВЛ");
                    nm = RW(@"\bгород\b|\bгор\b\.?").Replace(nm, "г.");
                    nm = RW(@"\bдеревня\b|\bдер\b\.?").Replace(nm, "д.");
                    //nm = RW(@"\bздание\b").Replace(nm, "ЗД");
                    nm = RW(@"\bквартал\b|\bк-л\b|\bкв(арт)\b\.?").Replace(nm, "кв-л");
                    //nm = RW(@"\bкорпус\b|\b(к|кор|корп)\b\.?").Replace(nm, "К");
                    nm = RW(@"\bлин\b\.?").Replace(nm, "линия");
                    nm = RW(@"\bмикрорайон\b|\bм/р\b|\bмк/р\b|\bмк/р-н\b").Replace(nm, "мкр.");
                    nm = RW(@"\bнаб(ер|ереж)?\b\.?|\b(?<!ул\.(\s|\t){1,5})набережная(?!(\s|\t){1,5}ул)\b").Replace(nm, "наб");
                    nm = RW(@"\bобласть\b").Replace(nm, "обл.");
                    nm = RW(@"\bозеро\b").Replace(nm, "оз.");
                    nm = RW(@"\bпоселок\b|\bпос-к\b").Replace(nm, "п.");
                    nm = RW(@"\bпер(еул)?\b\.?|\bпереулок\b|\bп-к\b").Replace(nm, "переул");
                    nm = RW(@"\bпл(ощ)?\b\.?|\bплощадь\b").Replace(nm, "пл.");
                    nm = RW(@"\bплощадка\b").Replace(nm, "пл-ка");
                    nm = RW(@"\bпр?-д\b|\bпроезд\b").Replace(nm, "пр-д");
                    nm = RW(@"\bпр?-т\b|\bпросп?\b\.?|\bпроспект\b").Replace(nm, "пр-кт");
                    nm = RW(@"\bрека\b").Replace(nm, "р.");
                    nm = RW(@"\bреспублика\b").Replace(nm, "Респ");
                    nm = RW(@"\bрайон\b").Replace(nm, "р-н");
                    nm = RW(@"\bстанция\b|\bст-я\b").Replace(nm, "ст.");
                    nm = RW(@"\bстаница\b").Replace(nm, "ст-ца");
                    //nm = RW(@"\bстроение\b|\bстр\b\.?").Replace(nm, "С");
                    nm = RW(@"\bтерритория\b|\bтерр-ия\b").Replace(nm, "тер");
                    nm = RW(@"\bтупик\b").Replace(nm, "туп.");
                    nm = RW(@"\bулица\b").Replace(nm, "ул.");
                    nm = RW(@"\bучасток\b").Replace(nm, "уч.");
                    nm = RW(@"\bшос\b\.?|\bшоссе\b").Replace(nm, "ш.");

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
