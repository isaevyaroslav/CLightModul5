using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLightModul5
{
    class Task3
    {
        /*
         3 Заключительное консольное творческое задание
            (25 баллов) (Программа для предприятия)
            Дедлайн: 4.02
            Разработать консольное приложение для вымышленного предприятия, реализовать в нем то,
            что по вашему мнению должно максимально упростить работу.
            Пользователь: администратор 45 лет. Женщина. Разведена. 3 детей. Психически не устойчива
            и может разнести ПК если компухтер не делает, как ей надо. Особая осторожность.
            Примеры предприятий:
            1 Магазин сантехники
            2
            Частная клиника
            3
            Продуктовый магазин
            4
            Отель
            5
            Администрирование клуба(с учетом выручки)
        */
        static string dataDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;//"C:/";
        static string dataDirectoryName = "predpriyatieData";
        static string wrongDirectory = "Директория с именем: {0}  не существует. \n Обратитесь в техническую поддержку программы, пожалуйста.\n";

        static string delimiter = "\t";
        static string delimiterReplace = " ";
        static string[] workersHeads = {"Номер","Фамилия","Имя","Отчество", "Телефон", "Должность","Зарплата","График работы"};
        static string workersDataFileName = "workers.csv";
        static string[] productsHeads = { "Номер", "Артикул", "Наименование", "Поставщик", "Колличество", "Стоимость", "Цена"};
        static string productsDataFileName = "products.csv";
        static string[] moneyHeads = { "Номер", "Название", "Артикул", "Колличество", "Стоимость"};
        static string moneyDataFileName = "money.csv";
        static string[,] workersDataArray;
        static string[,] productsDataArray;
        static string[,] moneyDataArray;

        static string greeting = "\nДобро пожаловать в программу \"Продуктовый магазин 1.0\".";
        static string description = "\nПрограмма сделает управление Вашим магазином простым и понятным.\n";
        static bool exit = false;
        static int blockNumber = 0;
        static string showBlocksTitle = "Разделы программы: ";
        static string showCommandsTitle = "\nСписок доступных комманд: ";
        static string[,] blocks =
            {
                {"Начало", "Приветствие, описание программы, разделы и выбор раздела программы."},
                {"Сотрудники", "Управление сотрудниками Вашего магазина."},
                {"Продукты", "Управление продуктами Вашего магазина."},
                {"Деньги", "Доходы и расходы Вашего магазина."},
                {"Выход", "Выход из программы."},
            };
        static string showCommandFormat = "{0}) {1} - {2}";
        static string setBlockTask = "\nВведите номер или название раздела программы: ";
        static string setCommandTask = "\nВведите номер или название комманды: ";
        static string wrongBlock = "\nВы ввели неправильный номер или название раздела.\n";
        static string wrongCommand = "\nВы ввели неправильный номер или название комманды.\n";
        static string insertTaskFormat = "\nЗаполните поле {0}: ";
        static string insertTaskSucсess = "\nВы успешно добавили запись:";
        

        static void Main(string[] args)
        {
            LoadData();
            while (!exit)
            {
                SwitchBlock();
                if (!exit) {
                    ShowCommands(blocks, showCommandFormat, showBlocksTitle);
                    SetCommandNumber(blocks, ref blockNumber, setBlockTask, wrongBlock);
                    Console.Clear();
                }
            }
        }
        static void LoadData()
        {
            CreateCoreDirectory();
            workersDataArray = GetDataArray(workersDataFileName, workersHeads);
            productsDataArray = GetDataArray(productsDataFileName, productsHeads);
            moneyDataArray = GetDataArray(moneyDataFileName, moneyHeads);
        }
        static void CreateCoreDirectory()
        {
            DirectoryInfo pathDirectory = new DirectoryInfo(@dataDirectoryPath);
            DirectoryInfo coreDirectory = new DirectoryInfo(@dataDirectoryPath+@dataDirectoryName);
            pathDirectory.CreateSubdirectory(@dataDirectoryName);
        }
        static string[,] GetDataArray(string fileName, string[] fileHeaders)
        {
            FileInfo file = CreateFile(dataDirectoryPath + dataDirectoryName + "/" + fileName);
            if(file.Length == 0)
            {
                AppendFileLine(file, fileHeaders);
            }
            string[,] dataArray = GetMatrixFromFile(file, delimiter[0]);
            return dataArray;
        }
        static FileInfo CreateFile(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists)
            {
                file.Create().Close();
                file.Refresh();
            }
            return file;
        }
        static void AppendFileLine(FileInfo file, string[] line)
        {
            StreamWriter streamWriter = file.AppendText();
            streamWriter.WriteLine(string.Join(delimiter, ReplaceInArray(line, delimiter, delimiterReplace)));
            streamWriter.Close();
        }
        static string[,] GetMatrixFromFile(FileInfo file, char delimiter)
        {
            string[] fileLines =  File.ReadAllLines(file.FullName);
            int matrixHeight = fileLines.Length;
            int matrixWidth = fileLines[0].Split(delimiter).Length;
            string[,] matrix = new string[matrixHeight, matrixWidth];
            string[] currentLineArray;
            for (int row = 0; row < matrixHeight; row++)
            {
                currentLineArray = fileLines[row].Split(delimiter);
                for (int col = 0; col < matrixWidth; col++)
                {
                    matrix[row, col] = currentLineArray[col];
                }
            }

            return matrix;
        }
        static string[] ReplaceInArray (string[] dataArray, string find, string replace)
        {
            StringBuilder stringBuilder;
            for (int stringNumber = 0; stringNumber < dataArray.Length; stringNumber++)
            {
                stringBuilder = new StringBuilder(dataArray[stringNumber]);
                stringBuilder.Replace(find, replace);
                dataArray[stringNumber] = stringBuilder.ToString();
            }
            return dataArray;
        }
        static void SwitchBlock()
        {
            switch (blockNumber)
            {
                case 0:
                    StartCommand(greeting, description, blocks, showCommandFormat, showBlocksTitle);
                    break;
                case 1:
                    WorkersBlock(false);
                    break;
                case 2:
                    ProductsBlock(false);
                    break;
                case 3:
                    break;
                case 4:
                    exit = true;
                    Console.WriteLine("\nВы вышли из программы.\nДо свиданья.\n");
                    break;
            }
        }

        static void WorkersBlock(bool exit)
        {
            string[,] commands =
            {
                {"Начало","Показывает список комманд раздела." },
                {"Добавить","Добавить нового сотрудника в список." },
                {"Удалить","Удалить сотрудника из списка." },
                {"Изменить","Изменить данные о сотруднике." },
                {"Показать","Показать всех сотрудников в списке." },
                {"Найти","Найти сотрудника в списке." },
                {"Выход","Выход в меню выбора раздела программы." }
            };
            int commandNumber = 0;
            while (!exit)
            {
                SwitchBlockCommand(
                    commandNumber,
                    "Добро пожаловать в раздел управления сотрудниками.",
                    "\nВ данном разделе Вы сможете легко редактировать список сотрудников.\n",
                    commands,
                    workersDataFileName,
                    ref workersDataArray,
                    workersHeads,
                    ref exit);
                if (!exit)
                {
                    ShowCommands(commands, showCommandFormat, showCommandsTitle);
                    SetCommandNumber(commands, ref commandNumber, setCommandTask, wrongCommand);
                }
                Console.Clear();
            }
        }
        static void ProductsBlock(bool exit)
        {
            string[,] commands =
            {
                {"Начало","Показывает список комманд раздела." },
                {"Добавить","Добавить новое наименование продукта в список." },
                {"Удалить","Удалить продукт из списка." },
                {"Изменить","Изменить данные о продукте." },
                {"Показать","Показать все продукты в списке." },
                {"Найти","Найти продукты в списке." },
                {"Выход","Выход в меню выбора раздела программы." }
            };
            int commandNumber = 0;
            while (!exit)
            {
                SwitchBlockCommand(
                    commandNumber,
                    "Добро пожаловать в раздел управления продуктами.",
                    "\nВ данном разделе Вы сможете легко редактировать список продуктов.\n",
                    commands,
                    productsDataFileName,
                    ref productsDataArray,
                    productsHeads,
                    ref exit);
                if (!exit)
                {
                    ShowCommands(commands, showCommandFormat, showCommandsTitle);
                    SetCommandNumber(commands, ref commandNumber, setCommandTask, wrongCommand);
                }
                Console.Clear();
            }
        }
        static void MoneyBlock(bool exit)
        {
            string[,] commands =
            {
                {"Начало","Показывает список комманд раздела." },
                {"Добавить","Добавить новую денежную операцию в список." },
                {"Удалить","Удалить денежную операцию из списка." },
                {"Изменить","Изменить денежную операцию." },
                {"Показать","Показать все денежные операции в списке." },
                {"Найти","Найти денежную операцию в списке." },
                {"Выход","Выход в меню выбора раздела программы." }
            };
            int commandNumber = 0;
            while (!exit)
            {
                SwitchBlockCommand(
                    commandNumber,
                    "Добро пожаловать в раздел управления прибылью и убытками.",
                    "\nВ данном разделе Вы сможете легко редактировать список денежных операций.\n",
                    commands,
                    moneyDataFileName,
                    ref moneyDataArray,
                    moneyHeads,
                    ref exit);
                if (!exit)
                {
                    ShowCommands(commands, showCommandFormat, showCommandsTitle);
                    SetCommandNumber(commands, ref commandNumber, setCommandTask, wrongCommand);
                }
                Console.Clear();
            }
        }
        static void SwitchBlockCommand
            (
            int commandNumber,
            string greeting,
            string description,
            string[,] commands, 
            string dataFileName, 
            ref string[,] dataArray,
            string[] dataHeads,
            ref bool exit
            )
        {

            int lineNumber;
            string filePath = dataDirectoryPath + dataDirectoryName + "/" + dataFileName;
            switch (commandNumber)
            {
                case 0:
                    StartCommand(
                        greeting,
                        description,
                        commands,
                        showCommandFormat,
                        showCommandsTitle
                        );
                    break;
                case 1:
                    AddDataLine(ref dataArray, dataFileName, dataHeads);
                    break;
                case 2:
                    Console.WriteLine("\nЧтобы найти строку, которую нужно удалить:");
                    lineNumber = FindDataLineNumber(dataArray, dataHeads);
                    ShowDataLine(dataHeads, GetMatrixRow(dataArray, lineNumber));
                    Console.Write("Вы действительно хотите удалить эту строку? (Да/Нет):");
                    string userUnswer = Console.ReadLine();
                    if(userUnswer.ToLower() == "да")
                    {
                        DelMatrixRow(ref dataArray, lineNumber);
                        WriteMatrixToFile(dataArray, filePath, delimiter, delimiterReplace);
                        Console.WriteLine("\nСтрока успешно удалена.\n");
                    }
                    else
                    {
                        Console.WriteLine("\nСтрока не удалена.\n");
                    }
                    break;
                case 3:
                    Console.WriteLine("\nЧтобы найти строку, которую нужно редактировать выберите ");
                    lineNumber = FindDataLineNumber(dataArray, dataHeads);
                    if (!(lineNumber < dataArray.GetLength(0)))
                    {
                        Console.WriteLine("Строка с таким полем не найдена.");
                    }
                    else
                    {
                        Console.WriteLine("\nВы собираетесь редактировать строку: ");
                        ShowDataLine(dataHeads, GetMatrixRow(dataArray, lineNumber));
                        EditDataField(ref dataArray, lineNumber, dataHeads);
                        ShowDataLine(dataHeads, GetMatrixRow(dataArray, lineNumber));
                        WriteMatrixToFile(dataArray, filePath, delimiter, delimiterReplace);
                    }                    
                    break;
                case 4:
                    ShowAllData(ref dataArray, dataHeads);
                    break;
                case 5:
                    lineNumber = FindDataLineNumber(dataArray, dataHeads);
                    if(lineNumber < dataArray.GetLength(0) && lineNumber != 0)
                    {
                        ShowDataLine(dataHeads, GetMatrixRow(dataArray, lineNumber));
                    }
                    else
                    {
                        Console.WriteLine("Строка с таким полем не найдена.");
                    }
                    break;
                case 6:
                    exit = true;
                    break;
            }
        }

        
        

        static void AddDataLine(ref string[,]dataMatrix, string fileName, string[] dataHeads)
        {
            string[] newLine = GetNewLine(dataHeads, GetNextId(ref dataMatrix));
            AddMatrixRow(ref dataMatrix, newLine);
            AppendFileLine(CreateFile(dataDirectoryPath + dataDirectoryName + "/" + fileName), newLine);
            Console.WriteLine(insertTaskSucсess);
            ShowDataLine(dataHeads, newLine);

        }
        static void EditDataField(ref string[,]dataArray, int lineNumber, string[] dataHeads)
        {
            int colEdit = FindColNumber(dataHeads, "Доступные поля для редактирования: ");
            Console.WriteLine("Текущее значение поля "+dataHeads[colEdit]+" - "+dataArray[lineNumber, colEdit]);
            Console.Write("Введите новое значение: ");
            dataArray[lineNumber, colEdit] = Console.ReadLine();
        }
        static void WriteMatrixToFile(string[,] matrix, string filePath, string delimiter, string delimiterReplace)
        {
            string[] matrixLines = new string[matrix.GetLength(0)];
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                matrixLines[row] = string.Join(delimiter, ReplaceInArray(GetMatrixRow(matrix, row), delimiter, delimiterReplace));
            }
            File.WriteAllLines(filePath, matrixLines);
        }
        static void ShowAllData(ref string[,] dataArray, string[] dataHeads)
        {
            if(dataArray.GetLength(0) == 1)
            {
                Console.WriteLine("Записи не найдены");
            }
            else
            {
                for (int row = 1; row < dataArray.GetLength(0); row++)
                {
                    ShowDataLine(dataHeads, GetMatrixRow(dataArray, row));
                }
            }
        }
        static int FindDataLineNumber(string[,] dataArray, string[] dataHeads)
        {
            int findColNumber = FindColNumber(dataHeads);
            string wantedValue = GetUserString("Введите искомое значение: ");
            int lineNumber = MatrixFindRow(dataArray, findColNumber, wantedValue);
            return lineNumber;
        }
        static int FindColNumber(string[] dataHeads, string searchTitle = "Выберите поле списка: ")
        {
            string wantedCol;
            int colNumber = 0;
            do
            {
                Console.WriteLine("Доступные поля для поиска: ");
                ShowHeads(dataHeads);
                Console.Write("Введите номер или название поля: ");
                wantedCol = Console.ReadLine().ToLower();
                if(int.TryParse(wantedCol, out colNumber)){
                    if(colNumber < dataHeads.Length)
                    {
                        return colNumber;
                    }
                }
                else
                {
                    for (int col = 0; col < dataHeads.Length; col++)
                    {
                        if(dataHeads[col].ToLower() == wantedCol)
                        {
                            return col;
                        }
                    }
                }

            } while (true);
        }
        static void ShowHeads(string[] headsArray)
        {
            for (int col = 0; col < headsArray.Length; col++)
            {
                Console.WriteLine("{0}) {1}", col, headsArray[col]);
            }
        }
        static string[] GetNewLine(string[] matrixHeads, int nextId)
        {
            string[] newLine = new string[matrixHeads.Length];
            newLine[0] = nextId.ToString();
            for (int col = 1; col < matrixHeads.Length; col++)
            {
                newLine[col] = GetUserString(insertTaskFormat, matrixHeads[col]);
            }
            return newLine;
        }
        static int GetNextId(ref string[,] dataMatrix)
        {
            int nextId = 0;
            int currentId = 0;
            for (int row = 1; row < dataMatrix.GetLength(0); row++)
            {
                currentId = Convert.ToInt32(dataMatrix[row, 0]);
                if (currentId > nextId)
                {
                    nextId = currentId;
                }
            }
            nextId++;
            return nextId;
        }
        static string GetUserString(string insertTaskFormat ,string stringName = "")
        {
            Console.Write(insertTaskFormat, stringName.ToLower());
            return Console.ReadLine();
        }
        static void ShowDataLine( string[] dataHeads, string[] dataLine)
        {
            Console.WriteLine();
            for (int col = 0; col < dataHeads.Length; col++)
            {
                ShowDataField(dataHeads[col], dataLine[col]);
            }
        }
        static void ShowDataField(string fieldName, string fieldValue)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(fieldName);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" "+fieldValue);
        }
        static void StartCommand(string greeting, string description, string[,]commands, string commandsFormat, string commandsTitle)
        {
            Console.WriteLine(greeting);
            Console.WriteLine(description);
        }
        static void ShowCommands(string[,] commandsArray, string showCommandsFormat, string showCommandsTitle)
        {
            Console.WriteLine(showCommandsTitle);
            for (int commandNumber = 0; commandNumber < commandsArray.GetLength(0); commandNumber++)
            {
                Console.WriteLine(showCommandsFormat, commandNumber, commandsArray[commandNumber, 0], commandsArray[commandNumber, 1]);
            }
            Console.WriteLine();
        }
        static void SetCommandNumber(string[,] commandsArray, ref int commandNumber, string setCommandTask, string wrongCommand)
        {
            bool rightCommandNumber = false;
            do
            {
                rightCommandNumber = FindCommand(commandsArray, GetWantedCommand(setCommandTask), out int nextCommandNumber);
                if (!rightCommandNumber)
                {
                    ShowWrongCommands(wrongCommand);
                }
                else
                {
                    commandNumber = nextCommandNumber;
                }
            }
            while (!rightCommandNumber);
        }
        static string GetWantedCommand(string setCommandTask)
        {
            Console.Write(setCommandTask);
            return Console.ReadLine();
        }
        static bool FindCommand(string[,] commandsArray, string commandWanted, out int commandNumber)
        {
            commandNumber = GetCommandNumber(commandsArray, commandWanted);
            return commandNumber < commandsArray.GetLength(0);
        }
        static int GetCommandNumber(string[,] commands, string commandWanted)
        {
            if (!int.TryParse(commandWanted, out int commandNumber))
            {
                commandNumber = MatrixFindRow(commands, 0, commandWanted);
            }
            return commandNumber;
        }
        static void ShowWrongCommands(string wrongBlock)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(wrongBlock);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void DelMatrixRow(ref string[,] matrix, int rowNumber)
        {
            string[,] newMatrix = new string[matrix.GetLength(0) - 1, matrix.GetLength(1)];
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    if (row < rowNumber)
                    {
                        newMatrix[row, col] = matrix[row, col];
                    }
                    else if (row > rowNumber)
                    {
                        newMatrix[row - 1, col] = matrix[row, col];
                    }

                }
            }
            matrix = newMatrix;
        }
        static int MatrixFindRow(string[,] matrix, int colNumber, string value)
        {
            value = value.ToLower();
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                if (matrix[row, colNumber].ToLower() == value)
                {
                    return row;
                }
            }
            return matrix.GetLength(0);
        }

        static string[] GetMatrixRow(string[,] matrix, int rowNumber)
        {
            string[] row = new string[0];
            if (rowNumber < matrix.GetLength(0))
            {
                row = new string[matrix.GetLength(1)];
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    row[col] = matrix[rowNumber, col];
                }
            }
            return row;
        }

        static void AddMatrixRow(ref string[,] matrix, string[] newRow)
        {
            if (newRow.Length >= matrix.GetLength(1))
            {
                string[,] newMatrix = new string[matrix.GetLength(0) + 1, matrix.GetLength(1)];
                for (int row = 0; row < newMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < newMatrix.GetLength(1); col++)
                    {
                        if (row >= matrix.GetLength(0))
                        {
                            newMatrix[row, col] = newRow[col];
                        }
                        else
                        {
                            newMatrix[row, col] = matrix[row, col];
                        }

                    }
                }
                matrix = newMatrix;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("New row is lower then matrix. \nNew row is not added.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        static void AddArrayArg(ref string[] array, string addArg)
        {
            string[] rezultArray = new string[array.Length + 1];
            for (int argNumber = 0; argNumber < array.Length; argNumber++)
            {
                rezultArray[argNumber] = array[argNumber];
            }
            rezultArray[rezultArray.Length - 1] = addArg;
            array = rezultArray;
        }
    }
}
