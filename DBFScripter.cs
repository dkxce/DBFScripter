using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DBFScripter
{
    // Базовый абстрактный класс для скрипта
    public abstract class DBFScript
    {
        #region STATIC
        // Диалог ввода/корректировки значения
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 38, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            form.Dispose();
            return dialogResult;
        }

        // Диалог выбора кодировки файла
        public static int SelectEncodingDialog(int defaultEncoding)
        {
            if (defaultEncoding == 0) defaultEncoding = 1251;

            Form form = new Form();

            Label label = new Label();
            ComboBox textBox = new ComboBox();
            textBox.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox.Items.Add("Windows 1251");
            textBox.Items.Add("DOS 866");

            label.Text = "Выберите кодировку файла:";
            textBox.SelectedIndex = defaultEncoding == 866 ? 1 : 0;

            Button buttonOk = new Button();

            form.Text = "Выбор кодировки файла";

            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;

            label.SetBounds(9, 20, 472, 13);
            textBox.SetBounds(12, 38, 472, 20);
            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;

            buttonOk.SetBounds(409, 72, 75, 23);

            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(496, 107);
            form.Controls.AddRange(new Control[] { label, textBox });
            form.Controls.AddRange(new Control[] { buttonOk });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;

            DialogResult dialogResult = form.ShowDialog();
            int selI = textBox.SelectedIndex;
            form.Dispose();
            return selI == 0 ? 1251 : 866;
        }

        // Вывод информации в консоль
        public static void L(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*  " + text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Проверка целого числа
        public static bool IsInteger(string value)
        {
            int ii = 0;
            return int.TryParse(value, out ii);
        }

        // Проверка дробного числа
        public static bool IsDouble(string value)
        {
            string val = value;
            double d = 0;
            if (double.TryParse(val, out d)) return true;
            if (double.TryParse(val.Replace(",", "."), out d)) return true;
            if (double.TryParse(val.Replace(".", ","), out d)) return true;
            return false;
        }
        #endregion STATIC

        #region INTERNAL
        // Имена полей
        internal string[] Fields_Name = new string[0]; // внутр.
        // Размеры полей
        internal Hashtable fieldsLength = new Hashtable(); // внутр.
        // Типы полей
        internal Hashtable fieldsType = new Hashtable(); // внутр.
        #endregion INTERNAL

        #region ДИАЛОГОВЫЕ ОКНА
        // Диалог выбора поля из списка
        public string SelectFieldDialog(string promptText, string defaultField)
        {
            return SelectFieldDialog(new string[] { promptText }, new string[] { defaultField })[0];
        }        
        // Диалог выбора полей из списка
        public string[] SelectFieldDialog(string[] promptTexts, string[] defaultFields)
        {
            if ((promptTexts == null) || (defaultFields == null) || (promptTexts.Length == 0) || (defaultFields.Length == 0) || (promptTexts.Length != defaultFields.Length))
                throw new Exception("Отсутствует promptTexts, defaultFields или их длина разная!");

            Form form = new Form();
            Label[] labels = new Label[promptTexts.Length];
            ComboBox[] textBoxes = new ComboBox[promptTexts.Length];
            for (int i = 0; i < promptTexts.Length; i++)
            {
                labels[i] = new Label();
                textBoxes[i] = new ComboBox();
                textBoxes[i].DropDownStyle = ComboBoxStyle.DropDownList;
                foreach (string field in Fields_Name)
                    textBoxes[i].Items.Add(field);

                labels[i].Text = promptTexts[i];
                textBoxes[i].Text = defaultFields[i];
            };                        
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = promptTexts.Length == 1 ? "Выбор поля" : "Выбор полей";

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            for (int i = 0; i < promptTexts.Length; i++)
            {
                labels[i].SetBounds(9, 20+i*42, 472, 13);
                textBoxes[i].SetBounds(12, 38+i*42, 472, 20);
                labels[i].AutoSize = true;
                textBoxes[i].Anchor = textBoxes[i].Anchor | AnchorStyles.Right;
            };
            buttonOk.SetBounds(328, 72 + (promptTexts.Length - 1) * 42, 75, 23);
            buttonCancel.SetBounds(409, 72 + (promptTexts.Length - 1) * 42, 75, 23);
            
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(496, 107 + (promptTexts.Length - 1) * 42);
            for (int i = 0; i < promptTexts.Length; i++)
                form.Controls.AddRange(new Control[] { labels[i], textBoxes[i] });
            form.Controls.AddRange(new Control[] { buttonOk, buttonCancel });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, labels[0].Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            string[] values = new string[promptTexts.Length];
            for (int i = 0; i < promptTexts.Length; i++) values[i] = textBoxes[i].Text;
            form.Dispose();
            return dialogResult == DialogResult.OK ? values : defaultFields;
        }
        #endregion ДИАЛОГОВЫЕ ОКНА

        #region ПРОВЕРКИ ПОЛЕЙ
        // Существует ли поле
        public bool IsFieldExist(string fieldName)
        {
            bool res = false;
            foreach (string field in Fields_Name) if (fieldName == field) res = true;
            return res;
        }
        // Существуют ли поля
        public bool IsFieldExist(string[] fieldNames)
        {
            foreach (string s in fieldNames) if (!IsFieldExist(s)) return false;
            return true;
        }
        #endregion ПРОВЕРКИ ПОЛЕЙ

        #region ОБЯЗАТЕЛЬНЫЕ МЕТОДЫ
        // Информация о скрипте (обязательно)
        public abstract void ScriptInfo();

        // Главный метод скрипта, возвращает признак сохранять ли изменения в записе
        // ... вызывается для каждой записи (обязательно)
        public abstract bool ProcessRecord(Hashtable row);
        #endregion ОБЯЗАТЕЛЬНЫЕ МЕТОДЫ

        #region НЕОБЯЗАТЕЛЬНЫЕ МЕТОДЫ
        // Вызывается перед обработкой записей
        public virtual void BeforeProcess(string fileName, int RecordsCount) { }

        // Вызывается после обработки записей
        public virtual void AfterProcess() { }
        
        // Кодировка при чтении из файла
        public virtual Encoding FileReadEncoding { get { return Encoding.GetEncoding(1251); } }
		
		// Кодировка при записе в файл
        public virtual Encoding FileWriteEncoding { get { return Encoding.GetEncoding(1251); } }
        #endregion ОБЯЗАТЕЛЬНЫЕ МЕТОДЫ

        #region PROPERTIES

        // Статус обработки текущей записи
        public string CurrentStatus = "";

        // Предыдущая запись
        public Hashtable prevRow = null;     

        // Имена полей
        public string[] Fields_Names { get { return (string[])Fields_Name.Clone(); } }        

        // Размеры полей
        public Hashtable Fields_Length { get { return (Hashtable)fieldsLength.Clone(); } }        

        // Типы полей
        public Hashtable Fields_Type { get { return (Hashtable)fieldsType.Clone(); } }

        #endregion PROPERTIES
    }

    public sealed class DBFScripter
    {
        [STAThread]
        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.ProductVersion;

            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            
            //
            // Access Functions SQL 
            //
            // http://www.techonthenet.com/access/functions
            //
            //            
            Console.Title = "DataBase File DBFScripter by Artem Karimov v" + version;
            Console.WriteLine(Console.Title);

            
            System.Threading.Thread.Sleep(500);

            DBFScript SCR; // Our Script Object
            string scriptFileName = "";
            string dataFileName = "";
            
            // Check Command Line
            foreach (string arg in args)
            {
                if(System.IO.File.Exists(arg) && ((System.IO.Path.GetExtension(arg).ToLower()==".csm") || (System.IO.Path.GetExtension(arg).ToLower()==".cs"))) scriptFileName = arg;
                if (System.IO.File.Exists(arg) && (System.IO.Path.GetExtension(arg).ToLower() == ".dbf")) dataFileName = arg;
            };

            if (scriptFileName == "")
            {
                Console.Write("Выберите скрипт для выполнения... ");
                SelectScript sc = new SelectScript();
                sc.listBox1.Items.Clear();
                List<string> files = new List<string>();
                files.AddRange(System.IO.Directory.GetFiles(GetCurrentDir()+@"\Scripts\", "*.csm"));
                files.AddRange(System.IO.Directory.GetFiles(GetCurrentDir() + @"\Scripts\", "*.cs"));
                files.Sort();
                foreach (string file in files) sc.listBox1.Items.Add(System.IO.Path.GetFileName(file));
                if(sc.ShowDialog() != DialogResult.OK) return;
                scriptFileName = sc.currentFile;
                if (!System.IO.File.Exists(scriptFileName)) return;
                sc.Dispose();

                Console.SetCursorPosition(0, Console.CursorTop);
                for (int i = 0; i < 35; i++) Console.Write(" ");
                Console.SetCursorPosition(0, Console.CursorTop);
            };
            
            Console.Write("Файл скрипта: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(System.IO.Path.GetFileName(scriptFileName));
            Console.ForegroundColor = ConsoleColor.White;

            //
            // http://www.csscript.net/
            //            
            try
            {
                System.Reflection.Assembly asm = CSScriptLibrary.CSScript.Load(scriptFileName);
                CSScriptLibrary.AsmHelper script = new CSScriptLibrary.AsmHelper(asm);
                SCR = (DBFScript)script.CreateObject("DBFScripting.MainScript");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Файл \"" + System.IO.Path.GetFileName(scriptFileName) + "\" не является скриптом или версия скрипта устарела!\n\n"+ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            };

            Console.WriteLine("");
            bool existScriptInfo = true;
            SCR.ScriptInfo();
            Console.WriteLine("");            

            if (!existScriptInfo)
            {
                Console.CursorVisible = true;
                Console.WriteLine("Продолжить выполнение скрипта [Y/N]?");
                bool loop = true;
                while (loop)
                {
                    ConsoleKeyInfo cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.N) return;
                    if (cki.Key == ConsoleKey.Y) loop = false;
                };
                Console.CursorVisible = false;
            };
            System.Threading.Thread.Sleep(250);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Выберите файл данных...";
            Console.Write(ofd.Title);
            ofd.DefaultExt = "*.dbf";
            ofd.Filter = "Data Base Files (*.dbf)|*.dbf";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            dataFileName = ofd.FileName;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < ofd.Title.Length; i++) Console.Write(" ");
            Console.SetCursorPosition(0, Console.CursorTop);
            ofd.Dispose();            

            Console.Write("Файл данных\t : ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(System.IO.Path.GetFileName(dataFileName));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Кодировка чтения : " + SCR.FileReadEncoding.WebName + " [cp" + SCR.FileReadEncoding.CodePage + "]");
            Console.WriteLine("Кодировка записи : " + SCR.FileWriteEncoding.WebName + " [cp" + SCR.FileWriteEncoding.CodePage + "]");
            
            // http://www.sql.ru/forum/actualthread.aspx?tid=664774
            // http://nansoft.ru/blog/csharp/30.html

             //║ Байты :              Описание                            
             //║══════════════════════════════════════════════════════════
             //║ 00    :Типы файлов с данными:                            
             //║       : FoxBASE+/dBASE III +, без memo - 0х03            
             //║       : FoxBASE+/dBASE III +, с memo   - 0х83            
             //║       : FoxPro/dBASE IV, без memo      - 0х03            
             //║       : FoxPro с memo                  - 0хF5            
             //║       : dBASE IV с memo                - 0x8B            
             //║----------------------------------------------------------
             //║ 01-03 :Последнее изменение (ГГММДД)                      ║
             //║----------------------------------------------------------║
             //║ 04-07 :Число записей в файле                             ║
             //║----------------------------------------------------------║
             //║ 08-09 :Положение первой записи с данными                 ║
             //║----------------------------------------------------------║
             //║ 10-11 :Длина одной записи с данными (включая признак     ║
             //║       :удаления)                                         ║
             //║----------------------------------------------------------║
             //║ 12-27 :Зарезервированы                                   ║
             //║----------------------------------------------------------║
             //║ 28    :1-есть структ.составной инд.файл (типа .CDX),0-нет║
             //║----------------------------------------------------------║
             //║ 29-31 :Зарезервированы                                   ║
             //║----------------------------------------------------------║
             //║ 32-n  :Подзаписи полей**                                 ║
             //║----------------------------------------------------------║
             //║ n+1   :Признак завершения записи заголовка (0х0D)        ║
             //════════════════════════════════════════════════════════════

            System.IO.FileStream fs = new System.IO.FileStream(dataFileName, System.IO.FileMode.Open);            

            // Read File Version
            fs.Position = 0;
            int ver = fs.ReadByte();

            // Read Records Count
            fs.Position = 04;
            byte[] bb = new byte[4];                        
            fs.Read(bb,0,4);
            int total = BitConverter.ToInt32(bb, 0);

            // Read DataRecord 1st Position
            fs.Position = 8;
            bb = new byte[2];            
            fs.Read(bb, 0, 2);
            short dataRecord_1st_Pos = BitConverter.ToInt16(bb, 0);
            int FieldsCount = (((bb[0] + (bb[1] * 0x100)) - 1) / 32) - 1;

            // Read DataRecord Length
            fs.Position = 10;
            bb = new byte[2];            
            fs.Read(bb, 0, 2);
            short dataRecord_Length = BitConverter.ToInt16(bb, 0);

            // Read Заголовки Полей
            fs.Position = 32;
            SCR.Fields_Name = new string[FieldsCount]; // Массив названий полей
            SCR.fieldsLength.Clear(); // Массив размеров полей
            SCR.fieldsType.Clear();   // Массив типов полей
            byte[]   Fields_Dig    = new byte[FieldsCount];   // Массив размеров дробной части
            int[]    Fields_Offset = new int[FieldsCount];    // Массив отступов
            bb = new byte[32 * FieldsCount]; // Описание полей: 32 байтa * кол-во, начиная с 33-го            
            fs.Read(bb, 0, bb.Length);
            int FieldsLength = 0;
            for (int x = 0; x < FieldsCount; x++)
            {
                SCR.Fields_Name[x] = System.Text.Encoding.Default.GetString(bb, x * 32, 10).TrimEnd(new char[] { (char)0x00 });
                SCR.fieldsType.Add(SCR.Fields_Name[x], "" + (char)bb[x * 32 + 11]);
                SCR.fieldsLength.Add(SCR.Fields_Name[x], (int)bb[x * 32 + 16]);
                Fields_Dig[x] = bb[x * 32 + 17];
                Fields_Offset[x] = 1 + FieldsLength;
                FieldsLength = FieldsLength + (int)SCR.fieldsLength[SCR.Fields_Name[x]];

                // loadedScript.fieldsType[Fields_Name[x]] == "L" -- System.Boolean
                // loadedScript.fieldsType[Fields_Name[x]] == "D" -- System.DateTime
                // loadedScript.fieldsType[Fields_Name[x]] == "N" -- System.Int32 (FieldDigs[x] == 0) / System.Decimal (FieldDigs[x] != 0)
                // loadedScript.fieldsType[Fields_Name[x]] == "F" -- System.Double
                // loadedScript.fieldsType[Fields_Name[x]] == "C" -- System.String
                // loadedScript.fieldsType[Fields_Name[x]] == def -- System.String
            };

            SCR.BeforeProcess(dataFileName,total);

            // FOREACH ALL DATA    
            Console.WriteLine("Начато в         : " + DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));
            Console.Write("Обработано\t : ");
            for (int record_no = 0; record_no < total; record_no++)
            {
                // read fieldsFrom 
                string[] FieldValues = new string[FieldsCount];
                Hashtable recordDataOld = new Hashtable();
                for(int y=0;y<FieldValues.Length;y++)
                {
                    fs.Position = dataRecord_1st_Pos + (dataRecord_Length * record_no) + Fields_Offset[y];
                    bb = new byte[(int)SCR.fieldsLength[SCR.Fields_Name[y]]];
                    fs.Read(bb, 0, bb.Length);
                    FieldValues[y] = SCR.FileReadEncoding.GetString(bb).Trim().TrimEnd(new char[] { (char)0x00 });
                    recordDataOld.Add(SCR.Fields_Name[y], FieldValues[y]);
                };

                // process record
                Hashtable recordNewData = (Hashtable)recordDataOld.Clone();
                bool dataChanged = false;
                SCR.CurrentStatus = "";
                try { dataChanged = SCR.ProcessRecord(recordNewData); }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка при обработке на записе " + (record_no + 1).ToString() + " из " + total.ToString() + "     ");
                    Console.WriteLine(ex.ToString());
                    System.Threading.Thread.Sleep(350);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Обработано\t : ");
                };
                
                // result analyse
                if(dataChanged)
                foreach (System.Collections.DictionaryEntry elemNew in recordNewData)
                {
                    if ((elemNew.Value.ToString() != recordDataOld[elemNew.Key].ToString()) || (SCR.FileReadEncoding.CodePage != SCR.FileWriteEncoding.CodePage))
                        for(int y=0;y<FieldValues.Length;y++)
                            if (elemNew.Key.ToString() == SCR.Fields_Name[y])
                            {
                                // write fieldTo
                                fs.Position = dataRecord_1st_Pos + (dataRecord_Length * record_no) + Fields_Offset[y];
                                bb = new byte[(int)SCR.fieldsLength[SCR.Fields_Name[y]]]; // for(int x=0;x<bb.Length;x++) bb[x] = 0;
                                byte[] nnn = SCR.FileWriteEncoding.GetBytes(elemNew.Value.ToString());
                                for (int x = 0; (x < nnn.Length) && (x < bb.Length); x++) bb[x] = nnn[x];
                                fs.Write(bb, 0, bb.Length);
                            };
                };

                // save previous row
                SCR.prevRow = recordNewData;

                Console.SetCursorPosition(19, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.Magenta;
                if (record_no == (total - 1)) Console.ForegroundColor = ConsoleColor.White;
                Console.Write( ((double)(record_no + 1) / (double)total * 100).ToString("0.00").Replace(",", ".") + "% (" + (record_no + 1).ToString() + " из " + total.ToString()+")     ");
                Console.ForegroundColor = ConsoleColor.White;

                string cs = SCR.CurrentStatus;
                if (cs.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.SetCursorPosition(0, Console.CursorTop + 2);                                        
                    Console.SetCursorPosition(0, Console.CursorTop);
                    if (cs.Length > 76) cs = cs.Substring(0, 76);                    
                    Console.Write("> " + cs+" ");
                    for (int i = cs.Length; i < 77; i++) Console.Write(" ");
                    Console.SetCursorPosition(0, Console.CursorTop - 3);
                    Console.ForegroundColor = ConsoleColor.White;
                };
            };
            Console.WriteLine();
            fs.Close();
            
            Console.WriteLine("Завершено в      : " + DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));
            for (int i = 0; i < 79; i++) Console.Write(" ");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Готово!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();            
            SCR.AfterProcess();
            Console.ReadLine();                     
        }

        /// <summary>
        ///     Получение папки, из которой запущено приложение
        /// </summary>
        /// <returns>Полный путь к папки с \ на конце</returns>
        public static string GetCurrentDir()
        {
            // DBFReaderW w = new DBFReaderW("");
            
            string fname = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString();
            fname = fname.Replace("file:///", "");
            fname = fname.Replace("/", @"\");
            fname = fname.Substring(0, fname.LastIndexOf(@"\") + 1);
            return fname;
        }
    }


    /// <summary>
    ///     Чтение DBF файла и перезапись его строк
    /// </summary>
    public class DBFReaderW
    {
        public class FieldsInfo
        {
            private FieldInfo[] info;

            public FieldsInfo(FieldInfo[] fieldsInfo) { this.info = fieldsInfo; }

            /// <summary>
            ///     Число полей
            /// </summary>
            public int Count { get { return info.Length; } }

            /// <summary>
            ///     Получение поля по номеру
            /// </summary>
            /// <param name="index">номер поля, от нуля</param>
            /// <returns>информация о поле</returns>
            public FieldInfo this[int index] { get { return info[index]; } }

            /// <summary>
            ///     Получение поля по имени
            /// </summary>
            /// <param name="index">имя поля</param>
            /// <returns>информация о поле</returns>
            public FieldInfo this[string index]
            {
                get
                {
                    foreach (FieldInfo inf in info)
                        if (inf.Name.ToLower() == index.ToLower())
                            return inf;
                    return null;
                }
            }

            /// <summary>
            ///     Список имен полей
            /// </summary>
            public string[] Names
            {
                get
                {
                    string[] result = new string[info.Length];
                    for (int i = 0; i < result.Length; i++)
                        result[i] = info[i].Name;
                    return result;
                }
            }
        }

        public class FieldInfo
        {
            private string name = "";
            private int length = 0;
            private string type = "";

            public FieldInfo(string name, string type, int length)
            {
                this.name = name;
                this.type = type;
                this.length = length;
            }

            public string Name { get { return name; } }
            public string Type { get { return type; } }
            public int Length { get { return length; } }
        }

        private bool fReadOnly = false; // открыть как ReadOnly
        private Encoding fEncIn = Encoding.GetEncoding(1251); // Кодировка чтения
        private Encoding fEncOut = Encoding.GetEncoding(1251); // Кодировка записи

        private System.IO.FileStream fs; // FileStream

        private int fieldsCount; // Число полей в файле
        private string[] fields_Name; // Массив названий полей
        private int[] fields_Length; // Массив размеров полей
        private string[] fields_Type;   // Массив типов полей
        private byte[] fields_Dig;   // Массив размеров дробной части
        private int[] fields_Offset;    // Массив отступов

        private int dataRecords_Count; // Число записей в файле
        private short dataRecord_1st_Pos; // Позиция первого элемента в файле
        private short dataRecord_Length; // Длина записи, байт        

        /// <summary>
        ///     Кодировка файла при чтении
        /// </summary>
        public Encoding EncodingRead { get { return fEncIn; } set { fEncIn = value; } }

        /// <summary>
        ///     Кодировка файла при записи
        /// </summary>
        public Encoding EncodingWrite { get { return fEncOut; } set { fEncOut = value; } }

        /// <summary>
        ///     Поля таблицы
        /// </summary>
        public FieldsInfo Fields 
        { 
            get 
            {
                FieldInfo[] result = new FieldInfo[fieldsCount];
                for (int i = 0; i < fieldsCount; i++)
                    result[i] = new FieldInfo(fields_Name[i], fields_Type[i], fields_Length[i]);
                return new FieldsInfo(result);
            } 
        }

        /// <summary>
        ///     Создаем экземпляр класса чтения DBF
        /// </summary>
        /// <param name="dataFileName">Имя файла</param>
        public DBFReaderW(string dataFileName) { OpenFile(dataFileName); }

        /// <summary>
        ///     Создаем экземпляр класса чтения DBF
        /// </summary>
        /// <param name="dataFileName">Имя файла</param>
        /// <param name="ReadOnly">Открывать файл только для чтения</param>
        public DBFReaderW(string dataFileName, bool ReadOnly) 
        { 
            this.fReadOnly = ReadOnly;
            OpenFile(dataFileName);                      
        }

        private void OpenFile(string dataFileName)
        {
            fs = new System.IO.FileStream(dataFileName, System.IO.FileMode.Open, fReadOnly ? System.IO.FileAccess.Read : System.IO.FileAccess.ReadWrite);

            // Read File Version
            fs.Position = 0;
            int verOfFile = fs.ReadByte();

            // Read Records Count
            fs.Position = 04;
            byte[] bb = new byte[4];
            fs.Read(bb, 0, 4);
            dataRecords_Count = BitConverter.ToInt32(bb, 0);

            // Read DataRecord 1st Position
            fs.Position = 8;
            bb = new byte[2];
            fs.Read(bb, 0, 2);
            dataRecord_1st_Pos = BitConverter.ToInt16(bb, 0);
            fieldsCount = (((bb[0] + (bb[1] * 0x100)) - 1) / 32) - 1;

            // Read DataRecord Length
            fs.Position = 10;
            bb = new byte[2];
            fs.Read(bb, 0, 2);
            dataRecord_Length = BitConverter.ToInt16(bb, 0);

            // Read Заголовки Полей
            fs.Position = 32;
            fields_Name = new string[fieldsCount]; // Массив названий полей
            fields_Length = new int[fieldsCount]; // Массив размеров полей
            fields_Type = new string[fieldsCount];   // Массив типов полей
            fields_Dig = new byte[fieldsCount];   // Массив размеров дробной части
            fields_Offset = new int[fieldsCount];    // Массив отступов
            bb = new byte[32 * fieldsCount]; // Описание полей: 32 байтa * кол-во, начиная с 33-го            
            fs.Read(bb, 0, bb.Length);
            int allFieldsLength = 0;
            for (int x = 0; x < fieldsCount; x++)
            {
                fields_Name[x] = System.Text.Encoding.Default.GetString(bb, x * 32, 10).TrimEnd(new char[] { (char)0x00 });
                fields_Type[x] = "" + (char)bb[x * 32 + 11];
                fields_Length[x] = (int)bb[x * 32 + 16];
                fields_Dig[x] = bb[x * 32 + 17];
                fields_Offset[x] = 1 + allFieldsLength;
                allFieldsLength = allFieldsLength + fields_Length[x];
            };  
        }

        /// <summary>
        ///     Число записей в таблице
        /// </summary>
        public int Count { get { return dataRecords_Count; } }

        /// <summary>
        ///     Получение/запись строки таблицы
        /// </summary>
        /// <param name="index">номер строки таблицы, нумерация от нуля</param>
        /// <returns>строка таблицы</returns>
        public Hashtable this [int index]
        {
            get
            {
                Hashtable result = new Hashtable();
                for (int i = 0; i < fields_Name.Length; i++)
                {
                    fs.Position = dataRecord_1st_Pos + (dataRecord_Length * index) + fields_Offset[i];
                    byte[] bb = new byte[fields_Length[i]];
                    fs.Read(bb, 0, bb.Length);
                    string elem = fEncIn.GetString(bb).Trim().TrimEnd(new char[] { (char)0x00 });
                    result.Add(fields_Name[i], elem);
                };
                return result;
            }

            set
            {
                if (fReadOnly) throw new Exception("ERROR: File Opened as ReadOnly!");
                foreach (System.Collections.DictionaryEntry elem in value)
                {
                    for (int i = 0; i < fields_Name.Length; i++)
                        if (elem.Key.ToString() == fields_Name[i])
                        {
                            fs.Position = dataRecord_1st_Pos + (dataRecord_Length * index) + fields_Offset[i];
                            byte[] bb = new byte[fields_Length[i]];
                            byte[] nnn = fEncOut.GetBytes(elem.Value.ToString());
                            for (int x = 0; (x < nnn.Length) && (x < bb.Length); x++) bb[x] = nnn[x];
                            fs.Write(bb, 0, bb.Length);
                        };
                };
            }
        }

        /// <summary>
        ///     Заканчиваем работу с файлом
        /// </summary>
        public void Dispose() { fs.Close(); }        
    }
}


//string cd = GetCurrentDir();
//System.Data.OleDb.OleDbConnection oq = new System.Data.OleDb.OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + cd + ";Extended Properties=dBASE IV;User ID=Admin;Password=");
//System.Data.OleDb.OleDbConnection oq2 = new System.Data.OleDb.OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + cd + ";Extended Properties=dBASE IV;User ID=Admin;Password=");
//oq.Open();
//oq2.Open();
//System.Data.OleDb.OleDbCommand oc = new System.Data.OleDb.OleDbCommand("SELECT COUNT(*) FROM INPUTFILE", oq);
//System.Data.OleDb.OleDbDataReader dr = oc.ExecuteReader();            

//int mx = 0;
//if (dr.Read()) mx = Convert.ToInt32(dr[0]);
//dr.Close();


//oc = new System.Data.OleDb.OleDbCommand("SELECT [NAME],[NAME1],[OBJECTID] FROM INPUTFILE", oq);
//dr = oc.ExecuteReader();
//int ttl = 0;
//while (dr.Read())
//{
//    string nam = dr[0].ToString();
//    int no = 0;
//    int val = 0;
//    int i = 1;
//    if (!int.TryParse(nam, out no))
//        while (int.TryParse(nam.Substring(0, i++), out val))
//            no = val;
//    ttl++;
//    new System.Data.OleDb.OleDbCommand("UPDATE INPUTFILE SET NAME1 = " + no.ToString() + " WHERE OBJECTID = " + dr[1].ToString() + ";", oq2).ExecuteScalar();
//    Console.Clear();
//    Console.WriteLine("Records fetched: " + ttl.ToString() + " из " + mx.ToString());
//};
//dr.Close();
//oq.Close();
//oq2.Close();
//Console.WriteLine("DONE!");
//Console.ReadLine();