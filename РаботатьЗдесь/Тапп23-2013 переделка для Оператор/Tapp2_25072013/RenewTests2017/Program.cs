using System;
using System.Collections.Generic;
using System.Text;
using Mary;
using System.Globalization;

namespace RenewTests2017
{
    /// <summary>
    /// Тесты движка в процессе переделки движка в 2017 году.
    /// Без тестов дело идет медленно.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //создать солюшен на БД Аксцесс - сработает только на х86 виндовс!
            СоздатьТестовыйСолюшенJet("C:\\Temp");// - успешно
            //СоздатьТестовыйСолюшенSqlServer2005("C:\\Temp"); - нет сервера БД для тестов
            ОткрытьСолюшен("C:\\Temp\\TestSolution\\TestSolution.tapj"); //- success
            TestContainerProperties("C:\\Temp\\TestSolution\\TestSolution.tapj"); //- success
            TestSolutionOperations("C:\\Temp\\TestSolution\\TestSolution.tapj");
            TestCellsAndLinks("C:\\Temp\\TestSolution\\TestSolution.tapj");

            //создать в солюшене начальную структуру данных и получить статистику солюшена
            //для оценки работы движка
            OpenSolutionForTryWork("C:\\Temp\\TestSolution\\TestSolution.tapj");//Успешно


            return;
        }
        /// <summary>
        /// RT-создать в солюшене начальную структуру данных и получить статистику солюшена
        /// </summary>
        /// <param name="solutionFilePath">Путь к существующему солюшену</param>
        private static void OpenSolutionForTryWork(string solutionFilePath)
        {
            //создаем объект движка. Тут просто инициализация движка без солюшена.
            MEngine d = new MEngine();
            //открываем солюшен и инициализируем его данными движок.
            d.SolutionOpen(solutionFilePath);
            d.SolutionClear();
            //get statistics - необязательно, просто посмотреть, что в солюшене есть
            Mary.Serialization.MStatistic stat = d.SolutionGetStatistics();
            PrintSolutionStatistics(stat);
            //create initial structure
            //она не совсем правильная, в части типов ячеек и связей, но для начала сойдет.
            SolutionHelper.СоздатьНачальнуюСтруктуруСолюшена(d, MCellMode.Normal);
            //get statistics - необязательно, просто посмотреть, что в солюшене есть
            Mary.Serialization.MStatistic stat2 = d.SolutionGetStatistics();
            PrintSolutionStatistics(stat2);
            //создать собственный тестовый аккаунт пользователя.
            MCell userCell = SolutionHelper.СоздатьАккаунтПользователя(d, MCellMode.Normal, "testUser", "Test User account cell");
            //а далее к этой ячейке следует цеплять новые создаваемые структуры данных,
            // чтобы не потерять их в общей этой паутине. Вроде бы так было описано в концепции? Я уже и забыл...

            //TODO: do some work here ...
            //тут дальше можно создавать любые сложные структуры, но без браузера просматривать сеть неудобно.
            //А браузер для этого я не написал еще.
            //Посмотрите исходный код SolutionHelper.СоздатьНачальнуюСтруктуруСолюшена(..), чтобы получить представление о создании структур с помощью кода.

            //И этот релиз движка не ведет логи, они не реализованы тут.
            //тут только код ячеек и связей реализован и тестирован, остальное - в разной степени готовности.
            
            //сохранить изменения и закрыть солюшен 
            d.SolutionClose(true);
            d = null;
            return;
        }

        /// <summary>
        /// RT
        /// </summary>
        /// <param name="rootfolder"></param>
        private static void СоздатьТестовыйСолюшенJet(string rootfolder)
        {
            //создаем объект движка. Тут просто инициализация движка без солюшена.
            MEngine d = new MEngine();
            //заполняем свойства нового солюшена
            //здесь закомментированы поля, использующие значения по умолчанию
            MSolutionInfo info = new MSolutionInfo();
            info.DatabaseType = MDatabaseType.MsAccess;//остальные поля бд заполняются движком.
            //info.DatabaseName = "";//ignored
            //info.DatabasePortNumber = 0;//ignored
            //info.DatabaseServerPath = "";//ignored
            //info.DatabaseTimeout = 60;//by default
            //info.UseIntegratedSecurity = false;//by default
            //info.UserName = "";//ignored
            //info.UserPassword = "";//ignored

            info.SolutionName = "TestSolution";
            info.SolutionDescription = "Test project in 2019";
            //info.SolutionId = 1;//by default
            //info.SolutionVersion = new MSolutionVersionInfo("1.0.0.0");//by default

            info.ContainerDefaultCellMode = MCellMode.Compact;
            //info.SolutionEngineVersion - устанавливается автоматически 
            //info.ContainerIsActiveFlag = true;//by default
            //info.ContainerServiceFlag = 0;//by default
            //info.ContainerState = 0;//state by default
            //info.CreationDate = DateTime.Now; //by default
            //info.LogDetailsFlags = 0;//unknown
            //info.LogfileNumber = 0;//for first time - unknown
            //info.SolutionFilePath = "";//by default
            //info.SnapshotNumber = 0;//by default
            //info.SolutionEngineVersion = info.CurrentEngineVersion;//by default

            //создаем солюшен и инициализируем его данными движок.
            d.SolutionCreate(rootfolder, info);//создан, но не открыт

            //some work here

            //d.SolutionClose(true); - солюшен не открыт - нечего и закрывать
            d = null;
            return;
        }

        private static void TestSolutionOperations(string p)
        {
            try
            {
                ConsoleWriteLine("Solution operations test started");
                //создаем объект движка.
                MEngine d = new MEngine();
                //открываем солюшен и инициализируем его данными движок.
                d.SolutionOpen(p);
                ConsoleWriteLine("Solution opened");
                                
                Mary.Serialization.MStatistic stat = d.SolutionGetStatistics();
                PrintSolutionStatistics(stat);
                ConsoleWriteLine("Optimizing solution..");
                d.SolutionOptimize();
                stat = d.SolutionGetStatistics();
                PrintSolutionStatistics(stat);
                ConsoleWriteLine("Saving solution..");
                d.SolutionSave();
                ConsoleWriteLine("Clearing solution..");
                d.SolutionClear();
                stat = d.SolutionGetStatistics();
                PrintSolutionStatistics(stat);
                ConsoleWriteLine("Closing solution..");
                d.SolutionClose(false);
                d = null;
            }
            catch (Exception ex)
            {
                ConsoleWriteLine(ex.ToString());

            }
            ConsoleWriteLine("Solution operations test finished");
            return;
        }



        private static void TestCellsAndLinks(string p)
        {
            try
            {
                ConsoleWriteLine("Cells and links operations test started");
                //создаем объект движка.
                MEngine d = new MEngine();
                //открываем солюшен и инициализируем его данными движок.
                d.SolutionOpen(p);
                ConsoleWriteLine("Solution opened");

                MID state1 = new MID(1);
                MID typeid1 = new MID(3);
                MID valuetype1 = new MID(5);
                MID state2 = new MID(2);
                MID typeid2 = new MID(4);
                MID valuetype2 = new MID(6);

                MCell c1 = d.CellCreate(MCellMode.Compact);
                //c1.CellID - уже должен быть назначен движком. нельзя изменять.
                //c1.CellMode - уже должен быть назначен движком. нельзя изменять, кроме некоторых специальных случаев. Надо сделать его internal, чтобы не давать пользователю шансов испортить.
                //c1.CreaTime - незачем изменять специально
                //c1.isActive - флаг удаления, незачем изменять специально
                //c1.isLargeCell - флаг только для чтения
                //c1.ModiTime - изменяется автоматически, незачем ее изменять специально.
                //c1.ReadOnly - при создании можно сразу указать как неизменяемую, но только после заполнения всех полей свойств
                c1.Name = "cell";
                c1.Description = "Cell description text";
                c1.ServiceFlag = 777;
                c1.State = state1;
                c1.TypeId = typeid1;
                c1.Value = Encoding.UTF8.GetBytes("CellsValue");
                c1.ValueTypeId = valuetype1;

                MCell c2 = d.CellCreate(MCellMode.Normal);
                c2.Name = "cell";
                c2.Description = "Cell description text";
                c2.ServiceFlag = 777;
                c2.State = state1;
                c2.TypeId = typeid1;
                c2.Value = Encoding.UTF8.GetBytes("CellsValue");
                c2.ValueTypeId = valuetype1;

                MCell c3 = d.CellCreate(MCellMode.DelaySave);
                c3.Name = "cell";
                c3.Description = "Cell description text";
                c3.ServiceFlag = 777;
                c3.State = state1;
                c3.TypeId = typeid1;
                c3.Value = Encoding.UTF8.GetBytes("CellsValue");
                c3.ValueTypeId = valuetype1;

                MCell c4 = d.CellCreate(MCellMode.Temporary);
                c4.Name = "cell";
                c4.Description = "Cell description text";
                c4.ServiceFlag = 777;
                c4.State = state1;
                c4.TypeId = typeid1;
                c4.Value = Encoding.UTF8.GetBytes("CellsValue");
                c4.ValueTypeId = valuetype1;

                //check cell properties changing
                MCell[] cells = new MCell[4];
                cells[0] = c1;
                cells[1] = c2;
                cells[2] = c3;
                cells[3] = c4;

                foreach (MCell cn in cells)
                {
                    ConsoleWriteLine("CellId = " + cn.CellID.ToString());
                    ConsoleWriteLine("Cellmode is " + cn.CellMode.ToString());
                    ConsoleWriteLine("CreaTime is " + cn.CreaTime.ToString());
                    ConsoleWriteLine("isActive is " + cn.isActive.ToString());
                    ConsoleWriteLine("ModiTime is " + cn.ModiTime.ToString());
                    ConsoleWriteLine("ReadOnly is " + cn.ReadOnly.ToString());
                    if (cn.Name != "cell")
                        ConsoleWriteLine("Cell name mismatch!");
                    if(cn.Description != "Cell description text")
                        ConsoleWriteLine("Cell description mismatch!");
                    if(cn.ServiceFlag != 777)
                        ConsoleWriteLine("Cell service flag mismatch!");
                    if(!cn.State.isEqual(state1))
                        ConsoleWriteLine("Cell state MID mismatch!");
                    if(!cn.TypeId.isEqual(typeid1))
                        ConsoleWriteLine("Cell type MID mismatch!");
                    if(!cn.ValueTypeId.isEqual(valuetype1))
                        ConsoleWriteLine("Cell valuetype MID mismatch!");
                    string text = Encoding.UTF8.GetString(cn.Value); 
                    if(text != "CellsValue")
                        ConsoleWriteLine("Cell value content mismatch!");
                    ConsoleWriteLine("");
                }
                //тестировать функции загрузки ячеек согласно типу 
                CheckCellLoadFunction(d, c1, c2, c3, c4);

                //try cell saving
                c1.S1_Save();//ничего не делает - и так все сохранено в БД.
                c2.S1_Save();//ничего не делает - и так все сохранено в БД.
                c3.S1_Save();//сохраняет в БД и превращает ячейку в Normal
                c4.S1_Save();//сохраняет в БД и превращает ячейку в Normal
                //check saving DelaySave cell - ok
                //check saving Temporary cell - ok
                
                

                //try cell unloading - отключено пока, работает только вызов MEngine.CellUnload(cellid)
                //почему же отключено? в MCell был закомментирован вызов функции, которая все выгружает.
                //надо проверить, работает ли она и почему отключена. Может быть, чтобы объект ячейки уничтожать было легче?
                c1.S1_Unload();
                c2.S1_Unload();
                c3.S1_Unload();
                c4.S1_Unload();
                d.CellUnload(c1.CellID);
                d.CellUnload(c2.CellID);
                d.CellUnload(c3.CellID);
                d.CellUnload(c4.CellID);

                //try cell loading
                MCell c5 = d.CellGet(c1.CellID);
                MCell c6 = d.CellGet(c2.CellID);
                MCell c7 = d.CellGet(c3.CellID);
                MCell c8 = d.CellGet(c4.CellID);//это бывшая временная ячейка, сохраненная и ставшая после сохранения Normal.
                


                ConsoleWriteLine("Solution closing..");
                d.SolutionClose(false);
                d = null;
            }
            catch (Exception ex)
            {
                ConsoleWriteLine(ex.ToString());

            }
            ConsoleWriteLine("Cells and links operations test finished");
            return;
        }

        private static void CheckCellLoadFunction(MEngine d, MCell c1, MCell c2, MCell c3, MCell c4)
        {
            //check CellLoad() functions

            //Сейчас это выборка из СписокЯчеекКонтейнера
            //Только один из 4 вызовов должен вернуть объект ячейки

            MCell t1 = d.CellLoad(c1.CellID, MCellMode.Compact);//только эта должна вернуть не нуль
            MCell t2 = d.CellLoad(c2.CellID, MCellMode.Compact);
            MCell t3 = d.CellLoad(c3.CellID, MCellMode.Compact);
            MCell t4 = d.CellLoad(c4.CellID, MCellMode.Compact);

            MCell t5 = d.CellLoad(c1.CellID, MCellMode.DelaySave);
            MCell t6 = d.CellLoad(c2.CellID, MCellMode.DelaySave);
            MCell t7 = d.CellLoad(c3.CellID, MCellMode.DelaySave);//только эта должна вернуть не нуль
            MCell t8 = d.CellLoad(c4.CellID, MCellMode.DelaySave);

            MCell t9 = d.CellLoad(c1.CellID, MCellMode.Normal);
            MCell t10 = d.CellLoad(c2.CellID, MCellMode.Normal);//только эта должна вернуть не нуль
            MCell t11 = d.CellLoad(c3.CellID, MCellMode.Normal);
            MCell t12 = d.CellLoad(c4.CellID, MCellMode.Normal);

            MCell t13 = d.CellLoad(c1.CellID, MCellMode.Temporary);
            MCell t14 = d.CellLoad(c2.CellID, MCellMode.Temporary);
            MCell t15 = d.CellLoad(c3.CellID, MCellMode.Temporary);
            MCell t16 = d.CellLoad(c4.CellID, MCellMode.Temporary);//только эта должна вернуть не нуль

            //а сейчас это выборка из таблицы ячеек БД
            MCell t17 = d.CellLoad(new MID(1), MCellMode.Compact);//должна вернуть  ячейку типа Compact
            MCell t18 = d.CellLoad(new MID(2), MCellMode.Normal);//должна вернуть ячейку типа Normal
            MCell t19 = d.CellLoad(new MID(3), MCellMode.DelaySave);//должна вернуть ячейку типа DelaySave
            MCell t20 = d.CellLoad(new MID(4), MCellMode.Temporary);//должна вернуть null, поскольку нельзя загрузить из БД временную ячейку.


            
        }

        private static void TestContainerProperties(string p)
        {
            try
            {
                ConsoleWriteLine("Container properties test started");
                //создаем объект движка.
                MEngine d = new MEngine();
                //открываем солюшен и инициализируем его данными движок.
                d.SolutionOpen(p);
                ConsoleWriteLine("Solution opened");
                //тест - изменить свойства солюшена, сохранить и снова загрузить солюшен - проверить что новые свойства сохранились.
                d.ContainerID = 777;
                d.DefaultCellMode = MCellMode.DelaySave;
                d.Description = "Новое описание контейнера";
                d.isActive = false;
                d.Name = "НовоеНазвание";
                d.ServiceFlag = 777;
                d.State = new MID(777);
                d.SolutionSave();
                d.SolutionClose(true);
                d = null;

                MEngine d2 = new MEngine();
                //открываем солюшен и инициализируем его данными движок.
                d2.SolutionOpen(p);
                ConsoleWriteLine("Solution  opened for second time");
                if(d2.ContainerID != 777)
                    ConsoleWriteLine("Error: ContainerID mismatch!");
                if (d2.DefaultCellMode != MCellMode.DelaySave)
                    ConsoleWriteLine("Error: DefaultCellMode mismatch!");
                if (d2.Description != "Новое описание контейнера")
                    ConsoleWriteLine("Error: Description mismatch!");
                if (d2.isActive != false)
                    ConsoleWriteLine("Error: isActive mismatch!");
                if (d2.Name != "НовоеНазвание")
                    ConsoleWriteLine("Error: Name mismatch!");
                if (d2.ServiceFlag != 777)
                    ConsoleWriteLine("Error: ServiceFlag mismatch!");
                if (d2.State.ID != 777)
                    ConsoleWriteLine("Error: State mismatch!");
                d2.SolutionClose(true);
                d2 = null;
            }
            catch (Exception ex)
            {
                ConsoleWriteLine(ex.ToString());

            }
            ConsoleWriteLine("Container properties test finished");
            return;
        }


        private static void PrintSolutionStatistics(Mary.Serialization.MStatistic stat)
        {
            ConsoleWriteLine("");
            ConsoleWriteLine("Solution statistics:");
            ConsoleWriteLine(String.Format("CellsInMemory: {0}", stat.CellsInMemory));
            ConsoleWriteLine(String.Format("ConstantCells: {0}", stat.ConstantCells));
            ConsoleWriteLine(String.Format("TemporaryCells: {0}", stat.TemporaryCells));
            ConsoleWriteLine(String.Format("ExternalCells: {0}", stat.ExternalCells));
            ConsoleWriteLine(String.Format("LinksInMemory: {0}", stat.LinksInMemory));
            ConsoleWriteLine(String.Format("ConstantLinks: {0}", stat.ConstantLinks));
            ConsoleWriteLine(String.Format("TemporaryLinks: {0}", stat.TemporaryLinks));
            ConsoleWriteLine(String.Format("ExternalLinks: {0}", stat.ExternalLinks));
            ConsoleWriteLine(String.Format("ResourceFiles: {0}", stat.ResourceFiles));
            ConsoleWriteLine(String.Format("ResourceSize: {0}", stat.ResourceSize));
            ConsoleWriteLine("");

            return;
        }
        
        /// <summary>
        /// Print text to console for user
        /// </summary>
        /// <param name="p"></param>
        private static void ConsoleWriteLine(string p)
        {
            Console.WriteLine(p);
        }

        private static void ОткрытьСолюшен(string solutionFilePath)
        {
            //создаем объект движка. Тут просто инициализация движка без солюшена.
            MEngine d = new MEngine();
            //открываем солюшен и инициализируем его данными движок.
            d.SolutionOpen(solutionFilePath);
            
            //some work here


            d.SolutionClose(true);
            d = null;
            return;
        }

        private static void СоздатьТестовыйСолюшенSqlServer2005(string rootfolder)
        {
            //создаем объект движка. Тут просто инициализация движка без солюшена.
            MEngine d = new MEngine();
            //заполняем свойства нового солюшена
            //здесь закомментированы поля, использующие значения по умолчанию
            MSolutionInfo info = new MSolutionInfo();
            info.DatabaseType = MDatabaseType.MicrosoftSqlServer2005;
            info.DatabaseName = "TestSolution";
            //info.DatabasePortNumber = 0;//by default
            info.DatabaseServerPath = ".";
            //info.DatabaseTimeout = 60;//by default
            //info.UseIntegratedSecurity = false;//by default
            info.UserName = "TappUser";
            info.UserPassword = "qwerty";

            info.SolutionName = "TestSolution";
            info.SolutionDescription = "Test project in 2017";
            //info.SolutionId = 1;//by default
            //info.SolutionVersion = new MSolutionVersionInfo("1.0.0.0");//by default

            info.ContainerDefaultCellMode = MCellMode.Compact;
            //info.ContainerIsActiveFlag = true;//by default
            //info.ContainerServiceFlag = 0;//by default
            //info.ContainerState = 0;//state by default
            //info.CreationDate = DateTime.Now; //by default
            //info.LogDetailsFlags = 0;//unknown
            //info.LogfileNumber = 0;//for first time - unknown
            //info.SolutionFilePath = "";//by default
            //info.SnapshotNumber = 0;//by default
            //info.SolutionEngineVersion = info.CurrentEngineVersion;//by default

            //создаем солюшен и инициализируем его данными движок.
            d.SolutionCreate(rootfolder, info);//создан, но не открыт

            //some work here

            //d.SolutionClose(true); - солюшен не открыт - нечего и закрывать
            d = null;
            return;
        }
    }
}
