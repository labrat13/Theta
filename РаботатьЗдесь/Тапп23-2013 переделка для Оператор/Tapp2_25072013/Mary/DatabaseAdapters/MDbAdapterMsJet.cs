using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Globalization;

namespace Mary.DatabaseAdapters
{
    
    /// <summary>
    /// NFT-Класс адаптера БД для БД MS Access 2003
    /// </summary>
    /// <remarks>
    /// Вся сборка должна быть скомпилирована для архитектуры x86, иначе БД не запустится на Windows 7 и выше.
    /// </remarks>
    public class MDbAdapterMsJet: MDbAdapterBase
    {
        /// <summary>
        /// Имя файла базы данных для файловых СУБД
        /// </summary>
        public const string DatabaseFileName = "db.mdb";
        //некоторые поля и проперти остались в базовом классе.

#region *** Fields ***
        //объекты команд создаются и инициализируются в функциях, которые их используют. 
        //все объекты команд должны сбрасываться в нуль при отключении соединения с БД
        //TODO: Новые команды внести в ClearCommands()
        OleDbCommand m_cmdWithoutArguments;
        OleDbCommand m_cmdGetCellExists;
        OleDbCommand m_cmdInsertCellTable;
        OleDbCommand m_cmdGetCell;
        OleDbCommand m_cmdUpdateCellTable;
        OleDbCommand m_cmdGetBlockOfCells;
        OleDbCommand m_cmdGetCellLinks;
        OleDbCommand m_cmdSetCellData;
        OleDbCommand m_cmdGetCellData;
        OleDbCommand m_cmdDeleteLinkById;
        OleDbCommand m_cmdInsertLinkTable;
        OleDbCommand m_cmdUpdateLinkTable;
        OleDbCommand m_cmdGetBlockOfLinks;
        OleDbCommand m_cmdGetLinkId;
        OleDbCommand m_cmdGetCellByTitle;
 #endregion

        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public MDbAdapterMsJet(MEngine engine): base(engine)
        {
            //add some inits here
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~MDbAdapterMsJet()
        {
            this.Close();
        }

#region *** Properties ***
        /// <summary>
        /// NT-Тип БД, поддерживаемый адаптером
        /// </summary>
        public override Mary.MDatabaseType DatabaseType
        {
            get
            {
                return MDatabaseType.MsAccess; 
            }
        }
        ///// <summary>
        ///// Is connection opened?
        ///// </summary>
        //public override bool isConnectionOpen
        //{
        //    get
        //    {
        //        return ((this.m_connection != null) && (this.m_connection.State == ConnectionState.Open));
        //    }
        //}
 #endregion

//Запросы тут немного другие:
//1. в конце запроса должна стоять ;
//2. Названия параметров запроса игнорируются.
//   Если в тексте запроса обычно приводятся имена параметров вроде @param1, 
//    то здесь они должны заменяться на ?
//    а порядок подачи параметров должен соответствовать их местам в запросе.
//Типы полей:
//Строки = OleDbType.WChar
//целое число = OleDbType.Integer
//таймштамп = OleDbType.Date
//логическое = OleDbType.Boolean
//блок данных = OleDbType.Binary

#region *** Функции сеанса и транзакции*** - реализованы
        /// <summary>
        /// NT-все объекты команд сбросить в нуль
        /// </summary>
        protected override void ClearCommands()
        {
            this.m_cmdDeleteLinkById = null;
            this.m_cmdGetBlockOfCells = null;
            this.m_cmdGetBlockOfLinks = null;
            this.m_cmdGetCell = null;
            this.m_cmdGetCellByTitle = null;
            this.m_cmdGetCellData = null;
            this.m_cmdGetCellExists = null;
            this.m_cmdGetCellLinks = null;
            this.m_cmdGetLinkId = null;
            this.m_cmdInsertCellTable = null;
            this.m_cmdInsertLinkTable = null;
            this.m_cmdSetCellData = null;
            this.m_cmdUpdateCellTable = null;
            this.m_cmdUpdateLinkTable = null;
            this.m_cmdWithoutArguments = null;
        }

        /// <summary>
        /// NT-Get string representation of object.
        /// </summary>
        /// <returns>Return string representation of object.</returns>
        public override string ToString()
        {
            return base.ToString();
        }
        /// <summary>
        /// NT-SolutionOpen manager
        /// </summary>
        /// <param name="info">Объект настроек Солюшена</param>
        public override void Open(Mary.MSolutionInfo info)
        {
            //тут надо создать строку подключения к СУБД и вписать ее в поле класса.
            this.m_connectionString = this.createConnectionString(info);
            //set up timeout value
            this.m_Timeout = info.DatabaseTimeout;
            //todo: add more initialization here...
            this.Connect();
            return;
        }
        /// <summary>
        /// NT-Close manager
        /// </summary>
        public override void Close()
        {
            //тут надо завершить менеджер, освободив все используемые ресурсы
            Disconnect();
        }
        /// <summary>
        /// NT-Открыть соединение с БД по текущей строке соединения
        /// </summary>
        /// <remarks>
        /// Эта пара функций позволяет закрыть и заново открыть соединение, если оно оказалось в неправильном состоянии.
        /// </remarks>
        public override void Connect()
        {
            //если соединение уже открыто, тихо выходим ? Или выдаем исключение?
            if (this.isConnectionOpen)
                return;
            //создаем объект соединения
            OleDbConnection con = new OleDbConnection(this.m_connectionString);
            //try open connection
            con.Open();
            this.m_connection = con;

            return;
        }
        /// <summary>
        /// NT-Закрыть соединение с БД
        /// </summary>
        /// <remarks>
        /// Эта пара функций позволяет закрыть и заново открыть соединение, если оно оказалось в неправильном состоянии.
        /// </remarks>
        public override void Disconnect()
        {
            //все объекты команд сбросить в нуль при отключении соединения с БД, чтобы ссылка на объект соединения при следующем подключении не оказалась устаревшей
            ClearCommands();

            if (m_connection != null)
            {
                if (m_connection.State == ConnectionState.Open)
                    m_connection.Close();
                m_connection = null;
            }

            return;
        }

        //Эти функции транзакций сейчас реализованы в базовом классе.
        //Если будут проблемы с ними, то их можно переопределить здесь. 
        
        ///// <summary>
        ///// NT-Начать транзакцию.
        ///// </summary>
        //public override void TransactionBegin()
        //{
        //    throw new System.NotImplementedException();
        //}
        ///// <summary>
        ///// NT-Подтвердить транзакцию Нужно закрыть соединение после этого!
        ///// </summary>
        //public override void TransactionCommit()
        //{
        //    throw new System.NotImplementedException();
        //}
        ///// <summary>
        ///// NT-Отменить транзакцию. Нужно закрыть соединение после этого!
        ///// </summary>
        //public override void TransactionRollback()
        //{
        //    throw new System.NotImplementedException();
        //}

        /// <summary>
        /// NT-Create connection string
        /// </summary>
        /// <param name="DatabaseServerPath">Путь к серверу БД или файлу БД.</param>
        /// <param name="DatabaseName">Название БД</param>
        /// <param name="dbPort">Порт сервера. Если аргумент имеет значение 0, использовать номер порта, установленный по умолчанию.</param>
        /// <param name="UserPassword">Пароль пользователя. Пустая строка допустима как отсутствие пароля.</param>
        /// <param name="UserName">Имя пользователя. Если вместо имени пользователя указана пустая строка, необходимо запросить имя и пароль пользователя через приложение.</param>
        /// <param name="Timeout">Таймаут попытки подключения в секундах</param>
        /// <param name="IntegratedSecurity">
        /// Использовать встроенную проверку подлинности для MsSqlServer. 
        /// True - Windows autentification. 
        /// False - SQL Server autentification.
        /// </param>
        /// <returns>Возвращает строку соединения с выбранной БД</returns>
        /// <remarks>Без лога, или проверять его существование!</remarks>
        public override string createConnectionString(string DatabaseServerPath, string DatabaseName, int dbPort, int Timeout, string UserName, string UserPassword, bool IntegratedSecurity)
        {
            OleDbConnectionStringBuilder csb = new OleDbConnectionStringBuilder();
            csb.Provider = "Microsoft.Jet.OLEDB.4.0";
            csb.DataSource = DatabaseServerPath;//set path to db file
            
            return csb.ConnectionString;
        }

        /// <summary>
        /// NT-Create connection string
        /// </summary>
        /// <param name="info">Объект ФайлСолюшена</param>
        /// <returns>Возвращает строку соединения с выбранной БД</returns>
        /// <remarks>Без лога, или проверять его существование!</remarks>>
        public override string createConnectionString(MSolutionInfo info)
        {
            return this.createConnectionString(info.DatabaseServerPath, info.DatabaseName, info.DatabasePortNumber, info.DatabaseTimeout, info.UserName, info.UserPassword, info.UseIntegratedSecurity);
        }

        /// <summary>
        /// NT-получить значение автоинкремента для последней измененной таблицы в текущем сеансе БД
        /// </summary>
        /// <returns></returns>
        internal int GetLastAutonumber()
        {
            return CmdExecuteScalar("SELECT @@IDENTITY;");
        }

 #endregion

#region *** Функции создания и изменения БД *** - одна не реализована, но обойдена.



        /// <summary>
        /// NT-Создать БД проекта.
        /// создать, открыть, записать и закрыть БД 
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        public override void DatabaseCreate(MSolutionInfo info)
        {
            //0 создать путь к файлу БД и вписать в MSolutionInfo.DatabaseServerPath
            String DbFilePath = Path.Combine(info.getCurrentSolutionDirectory(), MDbAdapterMsJet.DatabaseFileName);
            info.DatabaseServerPath = DbFilePath;
            //1 извлечь файл БД из ресурсов сборки в указанное место
            //путь к бд должен быть уже подготовлен 
            MDbAdapterBase.extractDbFile(DbFilePath, Mary.Properties.Resources.OleDbBaseFile);
            //2 проверить что БД доступна
            string connectionString = this.createConnectionString(info);
            OleDbConnection con = new OleDbConnection(connectionString);
            con.Open();
            this.insertEngineTableInitial(info, con);//add container properties values to table
            con.Close();

            return;
        }



        /// <summary>
        /// NT-Удалить существующую БД вместе со всем содержимым.
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        public override void DatabaseDelete(MSolutionInfo info)
        {
            string dbfilepath = info.DatabaseServerPath;
            File.Delete(dbfilepath);

            return;
        }

        /// <summary>
        /// NT-Очистить открытую БД
        /// </summary>
        public override void DatabaseClear()
        {
            //delete all tables from database
            this.ClearTable(MDbAdapterMsJet.CellTableName);
            this.ClearTable(MDbAdapterMsJet.LinkTableName);
            this.ClearTable(MDbAdapterMsJet.ContainerTableName);

            return;
        }

        ///// <summary>
        ///// NR-Create tables and indexes on existing database
        ///// </summary>
        ///// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        //public override void CreateTablesIndexes(string connectionString)
        //{
        //    throw new System.NotImplementedException();
        //}
        
        /// <summary>
        /// NT-Remove all from celltable and linktable
        /// Это удаляет вся ячейки и связи Солюшена, не затрагивая данные Контейнера.
        /// </summary>
        public override void ClearCellTableAndLinkTable()
        {
            this.ClearTable(MDbAdapterMsJet.CellTableName);
            this.ClearTable(MDbAdapterMsJet.LinkTableName);

            return;
        }

        /// <summary>
        /// NT-Удалить все строки из указанной таблицы
        /// </summary>
        /// <param name="table">Название таблицы</param>
        protected override void ClearTable(string table)
        {
            //DELETE FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, (OleDbConnection)this.m_connection, (OleDbTransaction)m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 600;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "DELETE FROM {0};", table);
            m_cmdWithoutArguments.CommandText = query;
            m_cmdWithoutArguments.ExecuteNonQuery();

            return;
        }
        /// <summary>
        /// NT-Выполнить скалярный запрос и вернуть результат запроса
        /// </summary>
        /// <param name="query">Текст запроса</param>
        /// <returns>
        /// Возвращает результат запроса как число Int32.
        /// Возвращает -1 если запрос вернул null.
        /// </returns>
        protected Int32 CmdExecuteScalar(string query)
        {
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, (OleDbConnection)this.m_connection, (OleDbTransaction)m_transaction);
                m_cmdWithoutArguments.CommandTimeout = this.m_Timeout;
            }
            //execute command
            m_cmdWithoutArguments.CommandText = query;
            Object ob = m_cmdWithoutArguments.ExecuteScalar(); //Тут могут быть исключения из-за другого типа данных
            String s = ob.ToString();
            if (String.IsNullOrEmpty(s))
                return -1;
            else return Int32.Parse(s);
        }

        /// <summary>
        /// NR-Return last table identity (primary key value)
        /// </summary>
        /// <param name="table">Table name</param>
        /// <returns>Returns last table identity (primary key value)</returns>
        protected override int getTableIdentity(string table)
        {
            //TODO: get table autoincrement value
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NT-Получить число записей в таблице
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <returns>Возвращает число записей в таблице</returns>
        protected override int getRowCount(string table, string column)
        {
            //SELECT COUNT(id) FROM table;
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT COUNT({0}) FROM {1};", column, table);
            return CmdExecuteScalar(query);
        }

        /// <summary>
        /// NT-Get max of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>Returns value of max of table id's</returns>
        protected override int getTableMaxId(string table, string column)
        {
            //SELECT MAX(id) FROM table;
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT MAX({0}) FROM {1};", column, table);
            return CmdExecuteScalar(query);
        }

        /// <summary>
        /// NT-Get min of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>Returns value of min of table id's</returns>
        protected override int getTableMinId(string table, string column)
        {
            //SELECT MIN(id) FROM table;
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT MIN({0}) FROM {1};", column, table);
            return CmdExecuteScalar(query);
        }

 #endregion

#region *** Функции контейнера *** - реализованы

        /// <summary>
        /// NT-Load first existing container from database. Throw exception if container not found 
        /// </summary>
        /// <param name="container">Container object for loading</param>
        /// <exception cref="SqlException">locked row</exception>
        /// <exception cref="InvalidOperationException">connection is closed</exception>
        public override void ContainerLoad(Mary.MEngine cont)
        {
            MEngineVersionInfo version;
            //SELECT EngineTable.* FROM EngineTable
            string query = String.Format("SELECT {0}.* FROM {0};", MDbAdapterMsJet.ContainerTableName);
            OleDbCommand sc = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
            sc.CommandType = CommandType.Text;
            sc.CommandTimeout = this.m_Timeout;
            OleDbDataReader sdr = sc.ExecuteReader();
            if (sdr.HasRows == true)
            {
                while (sdr.Read())
                {

                    version = new MEngineVersionInfo(sdr.GetString(1));//TODO: TAGVERSIONNEW:-сделано 
                    //check engine version
                    if (cont.SolutionSettings.CurrentEngineVersion.isCompatibleVersion(version) == false)
                        throw new Exception("Container version mismatch");//TODO: TAGVERSIONNEW: - сделано
                    //load snapshot number
                    MSolutionVersionInfo sver = new MSolutionVersionInfo(sdr.GetString(2));//TODO: TAGVERSIONNEW: -сделано
                    cont.SnapshotManager.Step = sver.SolutionStepNumber;//todo: проверить, что это нужно вписывать тут - можно ли пропустить?
                    cont.Log.LogFileNumber = sdr.GetInt32(3); //lognum, ex logname
                    cont.Log.logDetail = (MMessageClass)((uint)sdr.GetInt32(4));
                    cont.Description = sdr.GetString(5);
                    cont.Name = sdr.GetString(6);
                    cont.ServiceFlag = sdr.GetInt32(7);
                    cont.State = MID.fromU64((ulong)(sdr.GetInt64(8)));//TODO: TAGVERSIONNEW: -сделано, но надо проверить возможные искажения значений
                    cont.DefaultCellMode = (MCellMode)sdr.GetInt32(9);
                    cont.ContainerID = sdr.GetInt32(10);
                    break; //only first record used
                }
                sdr.Close();
            }
            else
            {
                sdr.Close();
                throw new Exception("Container not found");
            }

            return;
        }

        /// <summary>
        /// NT-Save container in database. Create new record if any not exists
        /// </summary>
        /// <param name="container"></param>
        /// <remarks>Без лога, или проверять его существование!</remarks>
        public override void ContainerSave(Mary.MEngine container)
        {
            //try update records
            if (updateEngineTable(container) < 1)
                //no any container records in table
                insertEngineTable(container);

            return;
        }

        /// <summary>
        /// NT-Update container record
        /// </summary>
        /// <returns> Number of updated rows</returns>
        private int updateEngineTable(MEngine cont)
        {
            string query = "UPDATE " + MDbAdapterMsJet.ContainerTableName + " SET version = ?, step = ?, lognum = ?, loglevel = ?, descr = ?, name = ?, sflag = ?, state = ?, cellmode = ?, idcon = ? ;";
            //UPDATE EngineTable SET version = @ver, step = @step, dirpath = @path, logname = @lname, loglevel = @llevel, stepname = @sname, descr = @descr, name = @name, sflag = @sflag, state = @state, limiter = @limiter
            //поскольку команда используется сравнительно редко, создаем ее в стеке, экономим память.
            OleDbCommand sc = createContainerCmd(query, (OleDbConnection)this.m_connection);

            //add values
            sc.Parameters[0].Value = cont.SolutionSettings.SolutionEngineVersion.toTextString();//TODO: TAGVERSIONNEW: изменить EngineTable
            sc.Parameters[1].Value = cont.SolutionSettings.SolutionVersion.toTextString();//TODO: TAGVERSIONNEW: 
            sc.Parameters[2].Value = cont.Log.LogFileNumber; //ex logname
            sc.Parameters[3].Value = (int)cont.Log.logDetail;
            sc.Parameters[4].Value = cont.Description;
            sc.Parameters[5].Value = cont.Name;
            sc.Parameters[6].Value = cont.ServiceFlag;
            sc.Parameters[7].Value = (long)(cont.State.toU64());//TODO: TAGVERSIONNEW: - проверить правильность конверсии типов
            sc.Parameters[8].Value = (int)cont.DefaultCellMode;
            sc.Parameters[9].Value = cont.ContainerID;
            //execute command
            return sc.ExecuteNonQuery();
        }
        /// <summary>
        /// NT-Создать команду для таблицы контейнера - для экономии кода
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private OleDbCommand createContainerCmd(string query, OleDbConnection con)
        {
            OleDbCommand sc = new OleDbCommand(query, con);
            sc.CommandTimeout = this.m_Timeout;
            sc.CommandType = CommandType.Text;
            sc.Parameters.Add("@ver", OleDbType.WChar);
            sc.Parameters.Add("@step", OleDbType.WChar);
            sc.Parameters.Add("@lname", OleDbType.Integer);
            sc.Parameters.Add("@llevel", OleDbType.Integer);
            sc.Parameters.Add("@descr", OleDbType.WChar);
            sc.Parameters.Add("@name", OleDbType.WChar);
            sc.Parameters.Add("@sflag", OleDbType.Integer);
            sc.Parameters.Add("@state", OleDbType.BigInt);
            sc.Parameters.Add("@cellmode", OleDbType.Integer);
            sc.Parameters.Add("@idcon", OleDbType.Integer);

            return sc;
        }

        /// <summary>
        /// NT-Insert row in EngineTable
        /// </summary>
        /// <param name="cont"></param>
        private void insertEngineTable(MEngine cont)
        {
            //при переделке также изменить и insertEngineTableInitial(..)
            string query = "INSERT INTO "+ MDbAdapterMsJet.ContainerTableName + " (version, step, lognum, loglevel, descr, name, sflag, state, cellmode, idcon) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";
            //поскольку команда используется однократно, создаем ее в стеке, экономим память.
            OleDbCommand sc = createContainerCmd(query, (OleDbConnection)this.m_connection);
            //add values
            sc.Parameters[0].Value = cont.SolutionSettings.SolutionEngineVersion.toTextString();//TODO: TAGVERSIONNEW: изменить EngineTable
            sc.Parameters[1].Value = cont.SolutionSettings.SolutionVersion.toTextString();//TODO: TAGVERSIONNEW: 
            sc.Parameters[2].Value = cont.Log.LogFileNumber;  //ex logname
            sc.Parameters[3].Value = (int)cont.Log.logDetail;
            sc.Parameters[4].Value = cont.Description;
            sc.Parameters[5].Value = cont.Name;
            sc.Parameters[6].Value = cont.ServiceFlag;
            sc.Parameters[7].Value = (long)(cont.State.toU64());//TODO: TAGVERSIONNEW: - проверить правильность конверсии типов
            sc.Parameters[8].Value = (int)cont.DefaultCellMode;
            sc.Parameters[9].Value = cont.ContainerID;
            //execute command
            sc.ExecuteNonQuery();

            return;
        }

        /// <summary>
        /// NT-Insert initial (first) row in EngineTable
        /// </summary>
        private void insertEngineTableInitial(MSolutionInfo info, OleDbConnection con)
        {
            //при переделке также изменить и insertEngineTable(..)
            string query = "INSERT INTO " + MDbAdapterMsJet.ContainerTableName + " (version, step, lognum, loglevel, descr, name, sflag, state, cellmode, idcon) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";
            //поскольку команда используется однократно, создаем ее в стеке, экономим память.
            OleDbCommand sc = createContainerCmd(query, con);
            //add values
            sc.Parameters[0].Value = info.SolutionEngineVersion.toTextString();//TODO: TAGVERSIONNEW: изменить тип поля на строку версии (64символа)
            sc.Parameters[1].Value = info.SolutionVersion.toTextString();//TODO: TAGVERSIONNEW: изменить тип поля на строку версии (64символа)
            sc.Parameters[2].Value = info.LogfileNumber;
            sc.Parameters[3].Value = (int)info.LogDetailsFlags;
            sc.Parameters[4].Value = info.SolutionDescription;
            sc.Parameters[5].Value = info.SolutionName;
            sc.Parameters[6].Value = info.ContainerServiceFlag;
            sc.Parameters[7].Value = (long)info.ContainerState;//TODO: TAGVERSIONNEW: изменить тип поля на ulong
            sc.Parameters[8].Value = (int)info.ContainerDefaultCellMode;
            sc.Parameters[9].Value = info.SolutionId;
            //execute command
            sc.ExecuteNonQuery();

            return;
        }

#endregion

#region *** Функции ячеек *** - реализованы

        /// <summary>
        /// NT-Проверить, что ячейка с таким идентификатором существует
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <returns>Returns true if cell exists, false otherwise</returns>
        public override bool isCellExists(Mary.MID cellid)
        {
            //SELECT CellTable.* FROM CellTable WHERE (cellid = @Param1)
            if (m_cmdGetCellExists == null)
            {
                string query = String.Format("SELECT {0}.cellid FROM {0} WHERE (cellid = ?);", MDbAdapterMsJet.CellTableName);
                m_cmdGetCellExists = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdGetCellExists.CommandTimeout = m_Timeout;
                m_cmdGetCellExists.Parameters.Add("@cid", OleDbType.Integer);
            }
            //execute
            m_cmdGetCellExists.Parameters[0].Value = cellid.ID;
            OleDbDataReader rdr = m_cmdGetCellExists.ExecuteReader();//TODO: коряво, надо переделать бы на COUNT
            bool res = rdr.HasRows;
            rdr.Close();
            return res;
        }

        /// <summary>
        /// NT-Проверить, что ячейка с таким названием существует
        /// </summary>
        /// <param name="cellName">Cell name</param>
        /// <returns>Returns true if cell exists, false otherwise</returns>
        public override bool isCellExists(string cellName)
        {
            String query = String.Format("SELECT COUNT(id) FROM {0} WHERE (name = {1})", MDbAdapterMsJet.CellTableName, cellName);
            int result = CmdExecuteScalar(query);
            //if result = -1, должно быть выброшено исключение - так как запрос должен вернуть 0 или 1.
            if (result < 0)
                throw new Exception("Invalid query results");
            //если result = 0, ячейка не найдена
            if (result > 0) return true;
            else return false;
        }

        /// <summary>
        /// NT-Return number of rows in cell table
        /// </summary>
        /// <returns>Return number of rows in cell table</returns>
        public override int getNumberOfCells()
        {
            return this.getRowCount(MDbAdapterMsJet.CellTableName, "id");
        }
        /// <summary>
        /// NT-Get max of cell id's in table, return 0 if no cells
        /// </summary>
        /// <returns>Returns max of cell id's in table, return 0 if no cells</returns>
        public override int getMaxCellId()
        {
            int id = this.getTableMaxId(MDbAdapterMsJet.CellTableName, "cellid");
            if (id < 0) id = 0;
            return id;
        }
        /// <summary>
        /// NT-Get min of cell id's in table, return 0 if no cells
        /// </summary>
        /// <returns>Returns min of cell id's in table, return 0 if no cells</returns>
        public override int getMinCellId()
        {
            int id = this.getTableMinId(MDbAdapterMsJet.CellTableName, "cellid");
            if (id < 0) id = 0;
            return id;
        }

        /// <summary>
        /// NT-Insert cell record to table
        /// </summary>
        /// <param name="cell">Cell object</param>
        public override int CellInsert(Mary.MCellB cell)
        {
            if (m_cmdInsertCellTable == null)
            {
                string query = String.Format("INSERT INTO {0} (name, descr, active, type, creatime, moditime, ronly, state, sflag, val, valtype, cellid) VALUES (?,?,?,?,?,?,?,?,?,?,?,?);", MDbAdapterMsJet.CellTableName);
                m_cmdInsertCellTable = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdInsertCellTable.CommandTimeout = m_Timeout;
                m_cmdInsertCellTable.Parameters.Add("@name", OleDbType.WChar);
                m_cmdInsertCellTable.Parameters.Add("@descr", OleDbType.WChar);
                m_cmdInsertCellTable.Parameters.Add("@active", OleDbType.Boolean);
                m_cmdInsertCellTable.Parameters.Add("@type", OleDbType.Integer);
                m_cmdInsertCellTable.Parameters.Add("@creat", OleDbType.Date);
                m_cmdInsertCellTable.Parameters.Add("@modit", OleDbType.Date);
                m_cmdInsertCellTable.Parameters.Add("@ronly", OleDbType.Boolean);
                m_cmdInsertCellTable.Parameters.Add("@state", OleDbType.Integer);
                m_cmdInsertCellTable.Parameters.Add("@sflag", OleDbType.Integer);
                m_cmdInsertCellTable.Parameters.Add("@val", OleDbType.Binary);
                m_cmdInsertCellTable.Parameters.Add("@valtype", OleDbType.Integer);
                m_cmdInsertCellTable.Parameters.Add("@cid", OleDbType.Integer);
            }
            //execute
            m_cmdInsertCellTable.Parameters[0].Value = cell.Name;
            m_cmdInsertCellTable.Parameters[1].Value = cell.Description;
            m_cmdInsertCellTable.Parameters[2].Value = cell.isActive;
            m_cmdInsertCellTable.Parameters[3].Value = cell.TypeId.ID;
            m_cmdInsertCellTable.Parameters[4].Value = cell.CreaTime;
            m_cmdInsertCellTable.Parameters[5].Value = cell.ModiTime;
            m_cmdInsertCellTable.Parameters[6].Value = cell.ReadOnly;
            m_cmdInsertCellTable.Parameters[7].Value = cell.State.ID;
            m_cmdInsertCellTable.Parameters[8].Value = cell.ServiceFlag;
            m_cmdInsertCellTable.Parameters[9].Value = cell.Value;
            m_cmdInsertCellTable.Parameters[10].Value = cell.ValueTypeId.ID;
            m_cmdInsertCellTable.Parameters[11].Value = cell.CellID.ID;
            //return num afected rows
            return m_cmdInsertCellTable.ExecuteNonQuery();
       
        }
        /// <summary>
        /// NT-Save cell data - update or insert row
        /// </summary>
        /// <param name="cell">Cell object</param>
        public override void CellSave(Mary.MCellB cell)
        {
            //этот код будет дважды проводить поиск для ячейки, возможно, кэширование на стороне сервера ускорит работу.
            if (isCellExists(cell.CellID))
                CellUpdate(cell);
            else
                CellInsert(cell);
        }

        /// <summary>
        /// NT-Get cell by cell id
        /// </summary>
        /// <param name="cellId">Cell identificator</param>
        /// <param name="largeCell">Cell mode: False for MCellA, true for MCellB</param>
        /// <returns>Returns MCell object or null if cell not exists</returns>
        public override Mary.MCell CellSelect(Mary.MID cellId, bool largeCell)
        {
            //если MEngine.DefaultCellMode=false требует MCellA, то зачем загружать ячейку из бд?
            //можно просто создать ее и записать ИД. Однако надо проверить, что она существует в таблице.
            //Поэтому переделать код, выделив вариант для MCellA как более простой и быстрый.

            //SELECT CellTable.* FROM CellTable WHERE (cellid = @Param1)
            if (m_cmdGetCell == null)
            {
                string query = String.Format("SELECT {0}.* FROM {0} WHERE (cellid = ?);", MDbAdapterMsJet.CellTableName); 
                m_cmdGetCell = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdGetCell.CommandTimeout = m_Timeout;
                m_cmdGetCell.Parameters.Add("@cid", OleDbType.Integer);
            }
            //execute
            m_cmdGetCell.Parameters[0].Value = cellId.ID;
            OleDbDataReader rdr = m_cmdGetCell.ExecuteReader();

            MCell r = null;
            if (rdr.HasRows == true)
            {
                if (largeCell == false)
                {
                    r = new MCellA();//create small cell
                    r.CellID = cellId;
                }
                else
                {
                    rdr.Read();
                    r = ReadLargeCellRow(rdr);
                }
            }
            rdr.Close();
            return r;
        }

        /// <summary>
        /// NT-Get cell by cell title
        /// </summary>
        /// <param name="title">Cell title string</param>
        /// <param name="largeCell">Cell mode: False for MCellA, true for MCellB</param>
        /// <returns>Returns MCell object or null if cell not exists</returns>
        public override Mary.MCell CellSelect(string title, bool largeCell)
        {
            //если MEngine.DefaultCellMode=false требует MCellA, то зачем загружать ячейку из бд?
            //можно просто создать ее и записать ИД. Однако надо проверить, что она существует в таблице.
            //Поэтому переделать код, выделив вариант для MCellA как более простой и быстрый.

            //SELECT CellTable.* FROM CellTable WHERE (cellid = @Param1)
            if (m_cmdGetCellByTitle == null)
            {
                String query = String.Format("SELECT {0}.* FROM {0} WHERE (cellid = ?);", MDbAdapterMsJet.CellTableName); 
                m_cmdGetCellByTitle = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdGetCellByTitle.CommandTimeout = m_Timeout;
                m_cmdGetCellByTitle.Parameters.Add("@title", OleDbType.WChar);
            }
            //execute
            m_cmdGetCellByTitle.Parameters[0].Value = title;
            OleDbDataReader rdr = m_cmdGetCellByTitle.ExecuteReader();

            MCell r = null;
            if (rdr.HasRows == true)
            {
                if (largeCell == false)
                {
                    r = new MCellA();//create small cell
                    rdr.Read();//read one row from result
                    r.CellID = new MID(rdr.GetInt32(12));//TAG:RENEW-13112017 - переделано
                }
                else
                {
                    rdr.Read();
                    r = ReadLargeCellRow(rdr);
                }
            }
            rdr.Close();

            return r;
        }

        /// <summary>
        /// NT-Update cell record 
        /// </summary>
        /// <param name="cell">Cell object</param>
        /// <returns>Returns number of affected rows</returns>
        public override int CellUpdate(Mary.MCellB cell)
        {
            //UPDATE CellTable SET name = @name, descr = @descr, active = @active, type = @type, creatime = @creat, moditime = @modit, ronly = @ronly, state = @state, sflag = @sflag, val = @val, valtype = @valtype WHERE (cellid = @cid)
            
            if (m_cmdUpdateCellTable == null)
            {
                string query = String.Format("UPDATE {0} SET name = ?, descr = ?, active = ?, type = ?, creatime = ?, moditime = ?, ronly = ?, state = ?, sflag = ?, val = ?, valtype = ? WHERE (cellid = ?);", MDbAdapterMsJet.CellTableName);
                m_cmdUpdateCellTable = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdUpdateCellTable.CommandTimeout = m_Timeout;
                m_cmdUpdateCellTable.Parameters.Add("@name", OleDbType.WChar);
                m_cmdUpdateCellTable.Parameters.Add("@descr", OleDbType.WChar);
                m_cmdUpdateCellTable.Parameters.Add("@active", OleDbType.Boolean);
                m_cmdUpdateCellTable.Parameters.Add("@type", OleDbType.Integer);
                m_cmdUpdateCellTable.Parameters.Add("@creat", OleDbType.Date);
                m_cmdUpdateCellTable.Parameters.Add("@modit", OleDbType.Date);
                m_cmdUpdateCellTable.Parameters.Add("@ronly", OleDbType.Boolean);
                m_cmdUpdateCellTable.Parameters.Add("@state", OleDbType.Integer);
                m_cmdUpdateCellTable.Parameters.Add("@sflag", OleDbType.Integer);
                m_cmdUpdateCellTable.Parameters.Add("@val", OleDbType.Binary);
                m_cmdUpdateCellTable.Parameters.Add("@valtype", OleDbType.Integer);
                m_cmdUpdateCellTable.Parameters.Add("@cid", OleDbType.Integer);
            }
            //execute
            m_cmdUpdateCellTable.Parameters[0].Value = cell.Name;
            m_cmdUpdateCellTable.Parameters[1].Value = cell.Description;
            m_cmdUpdateCellTable.Parameters[2].Value = cell.isActive;
            m_cmdUpdateCellTable.Parameters[3].Value = cell.TypeId.ID;
            m_cmdUpdateCellTable.Parameters[4].Value = cell.CreaTime;
            m_cmdUpdateCellTable.Parameters[5].Value = cell.ModiTime;
            m_cmdUpdateCellTable.Parameters[6].Value = cell.ReadOnly;
            m_cmdUpdateCellTable.Parameters[7].Value = cell.State.ID;
            m_cmdUpdateCellTable.Parameters[8].Value = cell.ServiceFlag;
            m_cmdUpdateCellTable.Parameters[9].Value = cell.Value;
            m_cmdUpdateCellTable.Parameters[10].Value = cell.ValueTypeId.ID;
            m_cmdUpdateCellTable.Parameters[11].Value = cell.CellID.ID;
            //return num afected rows
            return m_cmdUpdateCellTable.ExecuteNonQuery();
        }

        /// <summary>
        /// NT-Get band of cells
        /// </summary>
        /// <param name="rowFrom">cell id from which begin select cells</param>
        /// <param name="rowTo">cell id to (but not include) end select cells</param>
        /// <returns>Returns list of cells</returns>
        public override List<Mary.MCell> getBlockOfCells(int rowFrom, int rowTo)
        {
            if (m_cmdGetBlockOfCells == null)
            {
                string query = String.Format("SELECT {0}.* FROM {0} WHERE ((cellid >= ?) AND (cellid < ?))", MDbAdapterMsJet.LinkTableName);
                m_cmdGetBlockOfCells = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdGetBlockOfCells.CommandTimeout = m_Timeout;
                m_cmdGetBlockOfCells.Parameters.Add("@idfrom", OleDbType.Integer);
                m_cmdGetBlockOfCells.Parameters.Add("@idto", OleDbType.Integer);
            }
            //execute
            m_cmdGetBlockOfCells.Parameters[0].Value = rowFrom;
            m_cmdGetBlockOfCells.Parameters[1].Value = rowTo;
            OleDbDataReader rdr = m_cmdGetBlockOfCells.ExecuteReader();

            MCell r = null;
            List<MCell> lic = new List<MCell>();
            if (rdr.HasRows == true)
            {
                while (rdr.Read())
                {
                    r = this.ReadLargeCellRow(rdr);
                    lic.Add(r);
                }
            }
            rdr.Close();
            return lic;
        }

        /// <summary>
        /// NT-Read cell row
        /// </summary>
        /// <param name="rdr">Command result set reader</param>
        /// <param name="cell">Cell object for out</param>
        private MCellB ReadLargeCellRow(OleDbDataReader rdr)
        {
            ////read one row from result
            //read values to local variables
            String name = rdr.GetString(1);
            String description = rdr.GetString(2);
            bool isActive = rdr.GetBoolean(3);
            Int32 typeId = rdr.GetInt32(4);
            DateTime creaTime = rdr.GetDateTime(5);
            DateTime modiTime = rdr.GetDateTime(6);
            bool readOnly = rdr.GetBoolean(7);
            Int32 state = rdr.GetInt32(8);
            Int32 serviceFlag = rdr.GetInt32(9);
            byte[] cellValue = getByteArray(rdr, 10);
            Int32 valueTypeId = rdr.GetInt32(11);
            Int32 cellID = rdr.GetInt32(12);
            MCellMode cellMode = MCellMode.Normal; //enable property saving
            //call constructor
            MCellB cell = new MCellB(cellMode, cellID, name, description, isActive, typeId, creaTime, modiTime, readOnly, state, serviceFlag, cellValue, valueTypeId);

            return cell;
        }
        /// <summary>
        /// NT-Прочитать блок данных из записи БД
        /// </summary>
        /// <param name="rdr">ридер</param>
        /// <param name="column">номер столбца в ридере</param>
        /// <returns>Вовращает массив данных, прочитанный из записи БД.</returns>
        private byte[] getByteArray(OleDbDataReader rdr, int column)
        {
            //1 получить размер необходмого массива
            long len = rdr.GetBytes(column, 0, null, 0, 0); 
            //2 создать требуемый массив
            byte[] result = new Byte[(int)len];
            //3 прочитать данные
            if (len > 0)
                len = rdr.GetBytes(column, 0, result, 0, result.Length);
            //4 вернуть массив
            return result;
        }

        /// <summary>
        /// NT-Get cell links filtered by axis direction
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <param name="axisDir">axis direction: Any, Up, Down</param>
        /// <returns>Returns collection of links filtered by axis direction</returns>
        public override Mary.MLinkCollection getCellLinks(int cellid, Mary.MAxisDirection axisDir)
        {
            //SELECT LinkTable.* FROM LinkTable WHERE (downID = @cid) OR (upID = @cid)
            if (m_cmdGetCellLinks == null)
            {
                m_cmdGetCellLinks = new OleDbCommand("", (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdGetCellLinks.CommandTimeout = m_Timeout;
                m_cmdGetCellLinks.Parameters.Add("@cid", OleDbType.Integer);
                m_cmdGetCellLinks.Parameters.Add("@cid", OleDbType.Integer);
            }
            m_cmdGetCellLinks.Parameters[0].Value = cellid;
            m_cmdGetCellLinks.Parameters[1].Value = cellid;
            //set query string
            String query = String.Format("SELECT {0}.* FROM {0} WHERE ", MDbAdapterMsJet.LinkTableName);
            String wherepart = "";
            switch (axisDir)
            {
                case MAxisDirection.Down:
                    wherepart = "(upID = ?)";
                    break;
                case MAxisDirection.Up:
                    wherepart = "(downID = ?)";
                    break;
                case MAxisDirection.Any:
                    wherepart = "((downID = ?) OR (upID = ?))";
                    break;
                default:
                    throw new Exception("Invalid axis direction");
            }
            m_cmdGetCellLinks.CommandText = query + wherepart;
            //execute command
            OleDbDataReader rdr = m_cmdGetCellLinks.ExecuteReader();
            return readLinkResultSet(rdr);
        }

        /// <summary>
        /// NT-Find cells meet specified template. 
        /// Returned cells is not a part of container and not have links!!! 
        /// </summary>
        /// <param name="tmp">Cell template object</param>
        /// <param name="largeCells">True - MCellB cells, False - MCellA cells</param>
        /// <returns>
        /// Возвращает коллекцию ячеек, соответствующих шаблону. 
        /// Временная коллекция, ячейки не загружены в контейнер!
        /// </returns>
        public override Mary.MCellCollection getCellsByTemplate(Mary.MCellTemplate tmp, bool largeCells)
        {
            StringBuilder sb = new StringBuilder();
            //create command, parameters and query
            //SELECT CellTable.* FROM CellTable WHERE (creatime = @Param1) AND (type = @Param2) AND (active = @Param3) AND (descr LIKE @Param4) AND (name = @Param5)
            bool AndFlag = false;//flag for AND word
            OleDbParameter sp;

            //повторное использование команды не предполагается пока
            OleDbCommand alias = new OleDbCommand("", (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
            //Создаем текст запроса и параметры
            string ss = null;
            if (largeCells) ss = "*"; else ss = "cellid";
            sb.AppendFormat("SELECT {0}.{1} FROM {0} WHERE ", MDbAdapterMsJet.CellTableName, ss);

            #region Creating query and parameters
            if (tmp.CellID != null)
            {
                sb.Append("(cellid = ?)");
                sp = new OleDbParameter("@p1", OleDbType.Integer);
                sp.Value = tmp.CellID.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.CreaTime.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(creatime = ?)");
                sp = new OleDbParameter("@p2", OleDbType.Date);
                sp.Value = tmp.CreaTime.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ModiTime.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(moditime = ?)");
                sp = new OleDbParameter("@p3", OleDbType.Date);
                sp.Value = tmp.ModiTime.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.State != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(state = ?)");
                sp = new OleDbParameter("@p4", OleDbType.Integer);
                sp.Value = tmp.State.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.TypeId != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(type = ?)");
                sp = new OleDbParameter("@p5", OleDbType.Integer);
                sp.Value = tmp.TypeId.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ValueTypeId != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(valtype = ?)");
                sp = new OleDbParameter("@p6", OleDbType.Integer);
                sp.Value = tmp.ValueTypeId.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ServiceFlag.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(sflag = ?)");
                sp = new OleDbParameter("@p7", OleDbType.Integer);
                sp.Value = tmp.ServiceFlag.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Name != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(name = ?)");
                sp = new OleDbParameter("@p8", OleDbType.WChar);
                sp.Value = tmp.Name;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Description != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(descr LIKE ?)");
                sp = new OleDbParameter("@p9", OleDbType.WChar);
                sp.Value = tmp.Description;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.isActive.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(active = ?)");
                sp = new OleDbParameter("@p10", OleDbType.Boolean);
                sp.Value = tmp.isActive.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ReadOnly.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(ronly = ?)");
                sp = new OleDbParameter("@p11", OleDbType.Boolean);
                sp.Value = tmp.ReadOnly.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Value != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(val = ?)");
                sp = new OleDbParameter("@p12", OleDbType.Binary);
                sp.Value = tmp.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            #endregion

            alias.CommandTimeout = m_Timeout; // *5 ?
            alias.CommandText = sb.ToString();
            //execute command
            OleDbDataReader rdr = alias.ExecuteReader();
            MCellCollection col = new MCellCollection();
            MCell t;
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    //create cell and fill values
                    if (largeCells)
                    {
                        t = ReadLargeCellRow(rdr);
                    }
                    else
                    {
                        t = new MCellA();
                        t.CellID = new MID(rdr.GetInt32(12));//only cellid need, other come from table
                    }
                    col.S1_AddCell(t);
                }
            }
            rdr.Close();
            return col;
        }

        #region *** Get cell properties functions *** - реализованы
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override bool getCellActive(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("active", cellid);
            bool res = sdr.GetBoolean(0);
            sdr.Close();
            return res;
        }

        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override DateTime getCellCreationTime(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("creatime", cellid);
            DateTime res = sdr.GetDateTime(0);
            sdr.Close();
            return res;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override string getCellDescription(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("descr", cellid);
            String res = sdr.GetString(0);
            sdr.Close();
            return res;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override System.DateTime getCellModificationTime(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("moditime", cellid);
            DateTime res = sdr.GetDateTime(0);
            sdr.Close();
            return res;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override string getCellName(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("name", cellid);
            String res = sdr.GetString(0);
            sdr.Close();
            return res;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override bool getCellReadOnly(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("ronly", cellid);
            bool res = sdr.GetBoolean(0);
            sdr.Close();
            return res;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override int getCellServiceFlag(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("sflag", cellid);
            int res = sdr.GetInt32(0);
            sdr.Close();
            return res;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override Mary.MID getCellState(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("state", cellid);
            MID res = new MID(sdr.GetInt32(0));
            sdr.Close();
            return res;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override Mary.MID getCellTypeId(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("type", cellid);
            MID res = new MID(sdr.GetInt32(0));
            sdr.Close();
            return res;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override byte[] getCellValue(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("val", cellid);
            Byte[] res = this.getByteArray(sdr, 0);
            sdr.Close();
            return res;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override Mary.MID getCellValueTypeId(int cellid)
        {
            OleDbDataReader sdr = this.getCellColumnDataReader("valtype", cellid);
            MID res = new MID(sdr.GetInt32(0));
            sdr.Close();
            return res;
        }
        
        /// <summary>
        /// NT-Create data reader for specified column, as part of column access algoritm
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="cellid">Cell id</param>
        /// <returns>SqlDataReader, ready to get data, and need to be closed after use</returns>
        private OleDbDataReader getCellColumnDataReader(string columnName, int cellid)
        {
            
            //create command
            if (m_cmdGetCellData == null)
            {
                m_cmdGetCellData = new OleDbCommand(String.Empty, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdGetCellData.CommandTimeout = m_Timeout;
                m_cmdGetCellData.Parameters.Add("@cid", OleDbType.Integer);
            }
            //execute command
            string query = String.Format("SELECT {0}.{1} FROM {0} WHERE (cellid = ?);", MDbAdapterMsJet.CellTableName, columnName);
            m_cmdGetCellData.CommandText = query;
            m_cmdGetCellData.Parameters[0].Value = cellid;
            OleDbDataReader rdr = m_cmdGetCellData.ExecuteReader();
            if (rdr.HasRows == false)
            {
                rdr.Close();
                throw new Exception(String.Format("Cell record not found in table: id={1}", cellid));
            }
            else
            {
                rdr.Read();
                //rdr.Close(); - вынесено в вызывающий код
                return rdr;
            }
        }
        #endregion

        #region *** Set cell properties functions*** - реализованы

        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellActive(int cellid, bool val)
        {
            this.setCellColumnData("active", cellid, OleDbType.Boolean, val); 
        }
        
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellCreationTime(int cellid, System.DateTime val)
        {
            this.setCellColumnData("creatime", cellid, OleDbType.Date, val);
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellDescription(int cellid, string val)
        {
            this.setCellColumnData("descr", cellid, OleDbType.WChar, val);
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellModificationTime(int cellid, System.DateTime val)
        {
            this.setCellColumnData("moditime", cellid, OleDbType.Date, val);
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellName(int cellid, string val)
        {
            this.setCellColumnData("name", cellid, OleDbType.WChar, val);
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellReadOnly(int cellid, bool val)
        {
            this.setCellColumnData("ronly", cellid, OleDbType.Boolean, val);
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellServiceFlag(int cellid, int val)
        {
            this.setCellColumnData("sflag", cellid, OleDbType.Integer, val);
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellState(int cellid, Mary.MID val)
        {
            this.setCellColumnData("state", cellid, OleDbType.Integer, val.ID);  
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellTypeId(int cellid, Mary.MID val)
        {
            this.setCellColumnData("type", cellid, OleDbType.Integer, val.ID);

        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellValue(int cellid, byte[] val)
        {
            this.setCellColumnData("val", cellid, OleDbType.Binary, val);
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellValueTypeId(int cellid, Mary.MID val)
        {
            this.setCellColumnData("valtype", cellid, OleDbType.Integer, val.ID);
        }

        /// <summary>
        /// NT-Update column in cell record
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="cellid">modified cell id</param>
        /// <param name="dbtype">column value type</param>
        /// <param name="val">column value</param>
        /// <returns>Number of affected rows</returns>
        private void setCellColumnData(string columnName, int cellid, OleDbType dbtype, Object val)
        {
            //UPDATE CellTable SET name = @Param2, moditime = @Param3 WHERE (cellid = @Param1)

            //create command
            if (m_cmdSetCellData == null)
            {
                m_cmdSetCellData = new OleDbCommand(String.Empty, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdSetCellData.CommandTimeout = m_Timeout;
            }
            //execute command
            string query = String.Format("UPDATE {0} SET {1} = ?, moditime = ? WHERE (cellid = ?);", MDbAdapterMsJet.CellTableName, columnName);
            string paramName = "@" + columnName;
            OleDbParameter param = new OleDbParameter(paramName, dbtype);
            param.Value = val;
            m_cmdSetCellData.CommandText = query;
            m_cmdSetCellData.Parameters.Clear();
            m_cmdSetCellData.Parameters.Add(param);
            m_cmdSetCellData.Parameters.Add("@mody", OleDbType.Date);
            m_cmdSetCellData.Parameters.Add("@cid", OleDbType.Integer);
            m_cmdSetCellData.Parameters[1].Value = DateTime.Now;
            m_cmdSetCellData.Parameters[2].Value = cellid;
            int result = m_cmdSetCellData.ExecuteNonQuery();
            //if returns 0, cell not exists. throw exception?
            if (result != 1)
                throw new Exception("Cell not found in database: id=" + cellid);

            return;
        }

        #endregion
#endregion


#region *** Функции связей *** - одна не реализована, но обойдена.

        /// <summary>
        /// NR-Return last LinkTable identity (primary key value)
        /// </summary>
        /// <returns>Returns last LinkTable identity (primary key value)</returns>
        protected override int getLastIdentityLinksTable()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NT-Delete link by link table id
        /// </summary>
        /// <param name="linkId">link id from table primary key</param>
        /// <returns>Returns number of affected rows</returns>
        public override int LinkDelete(int linkId)
        {
            if (m_cmdDeleteLinkById == null)
            {
                string query = "DELETE FROM " + MDbAdapterMsJet.LinkTableName + " WHERE (id = ?)"; 
                m_cmdDeleteLinkById = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction) m_transaction);
                m_cmdDeleteLinkById.CommandTimeout = m_Timeout;
                m_cmdDeleteLinkById.Parameters.Add("@lid", OleDbType.Integer);
            }
            m_cmdDeleteLinkById.Parameters[0].Value = linkId;
            return m_cmdDeleteLinkById.ExecuteNonQuery();
        }

        /// <summary>
        /// NT-Insert link to table and change and return link primary key, that serve as linkId.
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Возвращает идентификатор связи</returns>
        public override int LinkInsertGetId(Mary.MLink link)
        {
            //тут надо как-то получить ид добавленной связи
            LinkInsert(link);
            //тут надо как-то получить ид записи добавленной связи
            int id = this.GetLastAutonumber(); //this.getLinkID(link);//this.getTableIdentity(MDbAdapterMsSql2005.LinkTableName);
            link.TableId = id;

            return id;
        }

        /// <summary>
        /// NT-Insert Link record into LinkTable
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Returns number of affected rows</returns>
        public override int LinkInsert(Mary.MLink link)
        {
            string query = "INSERT INTO " + MDbAdapterMsJet.LinkTableName + " (downID, upID, axis, state, active, sflag, descr, moditime) VALUES  (?,?,?,?,?,?,?,?);";
            if (m_cmdInsertLinkTable == null)
            {
                m_cmdInsertLinkTable = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdInsertLinkTable.CommandTimeout = m_Timeout;
                m_cmdInsertLinkTable.Parameters.Add("@down", OleDbType.Integer);
                m_cmdInsertLinkTable.Parameters.Add("@up", OleDbType.Integer);
                m_cmdInsertLinkTable.Parameters.Add("@axis", OleDbType.Integer);
                m_cmdInsertLinkTable.Parameters.Add("@state", OleDbType.Integer);
                m_cmdInsertLinkTable.Parameters.Add("@active", OleDbType.Boolean);
                m_cmdInsertLinkTable.Parameters.Add("@sflag", OleDbType.Integer);
                m_cmdInsertLinkTable.Parameters.Add("@descr", OleDbType.WChar);
                m_cmdInsertLinkTable.Parameters.Add("@modit", OleDbType.Date);
            }
            //execute
            m_cmdInsertLinkTable.Parameters[0].Value = link.downCellID.ID;
            m_cmdInsertLinkTable.Parameters[1].Value = link.upCellID.ID;
            m_cmdInsertLinkTable.Parameters[2].Value = link.Axis.ID;
            m_cmdInsertLinkTable.Parameters[3].Value = link.State.ID;
            m_cmdInsertLinkTable.Parameters[4].Value = link.isActive;
            m_cmdInsertLinkTable.Parameters[5].Value = link.ServiceFlag;
            m_cmdInsertLinkTable.Parameters[6].Value = link.Description;
            m_cmdInsertLinkTable.Parameters[7].Value = DateTime.Now;
            //return num afected rows
            return m_cmdInsertLinkTable.ExecuteNonQuery();

        }
        
        /// <summary>
        /// NT-Update link. Return num of affected rows
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Returns number of affected rows</returns>
        public override int LinkUpdate(Mary.MLink link)
        {
            string query = "UPDATE " + MDbAdapterMsJet.LinkTableName + " SET downID = ?, upID = ?, axis = ?, state = ?, active = ?, sflag = ?, descr = ?, moditime = ? WHERE (id = ?)";
            if (m_cmdUpdateLinkTable == null)
            {
                m_cmdUpdateLinkTable = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdUpdateLinkTable.CommandTimeout = m_Timeout;
                m_cmdUpdateLinkTable.Parameters.Add("@down", OleDbType.Integer);
                m_cmdUpdateLinkTable.Parameters.Add("@up", OleDbType.Integer);
                m_cmdUpdateLinkTable.Parameters.Add("@axis", OleDbType.Integer);
                m_cmdUpdateLinkTable.Parameters.Add("@state", OleDbType.Integer);
                m_cmdUpdateLinkTable.Parameters.Add("@active", OleDbType.Boolean);
                m_cmdUpdateLinkTable.Parameters.Add("@sflag", OleDbType.Integer);
                m_cmdUpdateLinkTable.Parameters.Add("@descr", OleDbType.WChar);
                m_cmdUpdateLinkTable.Parameters.Add("@modit", OleDbType.Date);
                m_cmdUpdateLinkTable.Parameters.Add("@lid", OleDbType.Integer);
            }
            //execute
            m_cmdUpdateLinkTable.Parameters[0].Value = link.downCellID.ID;
            m_cmdUpdateLinkTable.Parameters[1].Value = link.upCellID.ID;
            m_cmdUpdateLinkTable.Parameters[2].Value = link.Axis.ID;
            m_cmdUpdateLinkTable.Parameters[3].Value = link.State.ID;
            m_cmdUpdateLinkTable.Parameters[4].Value = link.isActive;
            m_cmdUpdateLinkTable.Parameters[5].Value = link.ServiceFlag;
            m_cmdUpdateLinkTable.Parameters[6].Value = link.Description;
            m_cmdUpdateLinkTable.Parameters[7].Value = DateTime.Now;
            m_cmdUpdateLinkTable.Parameters[8].Value = link.TableId;
            //return num afected rows
            return m_cmdUpdateLinkTable.ExecuteNonQuery();

        }

        /// <summary>
        /// NT-Get band of links
        /// </summary>
        /// <param name="rowFrom">link id from which begin select links</param>
        /// <param name="rowTo">link id to (but not include) end select links</param>
        /// <returns>Returns collection of links</returns>
        public override Mary.MLinkCollection getBlockOfLinks(int rowFrom, int rowTo)
        {
            if (m_cmdGetBlockOfLinks == null)
            {
                string query = String.Format("SELECT {0}.* FROM {0} WHERE ((id >= ?) AND (id < ?));", MDbAdapterMsJet.LinkTableName);
                m_cmdGetBlockOfLinks = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdGetBlockOfLinks.CommandTimeout = m_Timeout;
                m_cmdGetBlockOfLinks.Parameters.Add("@idfrom", OleDbType.Integer);
                m_cmdGetBlockOfLinks.Parameters.Add("@idto", OleDbType.Integer);
            }
            //execute
            m_cmdGetBlockOfLinks.Parameters[0].Value = rowFrom;
            m_cmdGetBlockOfLinks.Parameters[1].Value = rowTo;
            OleDbDataReader rdr = m_cmdGetBlockOfLinks.ExecuteReader();

            return readLinkResultSet(rdr);
        }

        /// <summary>
        /// NT-Read all rows from sql result set, close reader.
        /// </summary>
        /// <param name="rdr"></param>
        /// <returns></returns>
        private MLinkCollection readLinkResultSet(OleDbDataReader rdr)
        {
            MLinkCollection col = new MLinkCollection();
            MLink t;
            int tid;
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    t = new MLink();
                    tid = rdr.GetInt32(0); //вписывать в последнюю очередь, чтобы избежать записи в связь внутри. 
                    
                    t.downCellID = new MID(rdr.GetInt32(1));
                    t.upCellID = new MID(rdr.GetInt32(2));
                    t.Axis = new MID(rdr.GetInt32(3));
                    t.State = new MID(rdr.GetInt32(4));
                    t.isActive = rdr.GetBoolean(5);
                    t.ServiceFlag = rdr.GetInt32(6);
                    t.Description = rdr.GetString(7);
                    t.TableId = tid;
                    col.AddLink(t);
                }
            }
            rdr.Close();
            return col;
        }

        /// <summary>
        /// NT-get link id primary key for first founded link. Return 0 if link not exists
        /// TODO: неправильно работает при наличии нескольких связей между ячейками
        /// </summary>
        /// <param name="dnCellId">Идентификатор подчиненной ячейки</param>
        /// <param name="upCellId">Идентификатор главной ячейки</param>
        /// <param name="axis">Идентификатор типа связи</param>
        /// <returns>Возвращает первичный ключ записи связи в таблице</returns>
        public override int getLinkID(Mary.MID dnCellId, Mary.MID upCellId, Mary.MID axis)
        {
            //SELECT LinkTable.id FROM LinkTable WHERE (downID = @cidn) AND (upID = @cidu) AND (axis = @axis)
            if (m_cmdGetLinkId == null)
            {
                string query = String.Format("SELECT {0}.id FROM {0} WHERE (downID = ?) AND (upID = ?) AND (axis = ?)", MDbAdapterMsJet.LinkTableName);
                m_cmdGetLinkId = new OleDbCommand(query, (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
                m_cmdGetLinkId.CommandTimeout = m_Timeout;
                m_cmdGetLinkId.Parameters.Add("@cidn", OleDbType.Integer);
                m_cmdGetLinkId.Parameters.Add("@cidu", OleDbType.Integer);
                m_cmdGetLinkId.Parameters.Add("@axis", OleDbType.Integer);
            }
            m_cmdGetLinkId.Parameters[0].Value = dnCellId.ID;
            m_cmdGetLinkId.Parameters[1].Value = upCellId.ID;
            m_cmdGetLinkId.Parameters[2].Value = axis.ID;
            OleDbDataReader rdr = m_cmdGetLinkId.ExecuteReader();
            int result = 0;
            if (rdr.HasRows)
            {
                rdr.Read();
                result = rdr.GetInt32(0);
            }
            rdr.Close();
            return result;
        }
        
        /// <summary>
        /// NT-get link id primary key for first founded link. Return 0 if link not exists
        /// TODO: неправильно работает при наличии нескольких связей между ячейками
        /// </summary>
        /// <param name="link">Объект связи</param>
        /// <returns>Возвращает первичный ключ записи связи в таблице</returns>
        public override int getLinkID(Mary.MLink link)
        {
            return getLinkID(link.downCellID, link.upCellID, link.Axis);
        }
        /// <summary>
        /// NT-Get cell links filtered by axis direction
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <param name="axisDir">axis direction: Any, Up, Down</param>
        /// <returns>Returns collection of links filtered by axis direction</returns>
        public override Mary.MLinkCollection getLinks(Mary.MLinkTemplate tmp)
        {
            //SELECT LinkTable.* FROM LinkTable WHERE (downID = @Param1) AND (upID = @Param2) AND (axis = @Param3) AND (state = @Param4) AND (active = @Param5) AND (sflag = @Param6) AND (descr LIKE @Param7)
            StringBuilder sb = new StringBuilder();
            //create command, parameters and query
            bool AndFlag = false;//flag for AND word
            OleDbParameter sp;

            //повторное использование команды не предполагается пока
            OleDbCommand alias = new OleDbCommand("", (OleDbConnection)m_connection, (OleDbTransaction)m_transaction);
            //Создаем текст запроса и параметры
            sb.AppendFormat("SELECT {0}.* FROM {0} WHERE ", MDbAdapterMsJet.LinkTableName);

            #region Creating query and parameters
            if (tmp.tableId.HasValue)
            {
                sb.Append("(id = ?)");
                sp = new OleDbParameter("@p1", OleDbType.Integer);
                sp.Value = tmp.tableId.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.downCellID != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(downID = ?)");
                sp = new OleDbParameter("@p2", OleDbType.Integer);
                sp.Value = tmp.downCellID.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.upCellID != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(upID = ?)");
                sp = new OleDbParameter("@p3", OleDbType.Integer);
                sp.Value = tmp.upCellID.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Axis != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(axis = ?)");
                sp = new OleDbParameter("@p4", OleDbType.Integer);
                sp.Value = tmp.Axis.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.State != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(state = ?)");
                sp = new OleDbParameter("@p5", OleDbType.Integer);
                sp.Value = tmp.State.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.isActive.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(active = ?)");
                sp = new OleDbParameter("@p6", OleDbType.Boolean);
                sp.Value = tmp.isActive.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ServiceFlag.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(sflag = ?)");
                sp = new OleDbParameter("@p7", OleDbType.Integer);
                sp.Value = tmp.ServiceFlag.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Description != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(descr LIKE ?)");
                sp = new OleDbParameter("@p8", OleDbType.WChar);
                sp.Value = tmp.Description;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            #endregion

            alias.CommandTimeout = m_Timeout; // *5 ?
            alias.CommandText = sb.ToString();
            //execute command
            OleDbDataReader rdr = alias.ExecuteReader();

            return readLinkResultSet(rdr);
        }

        /// <summary>
        /// NT-Get max of link primary key in table. Return 0 if no links.
        /// </summary>
        /// <returns>Returns max of link primary key in table. Return 0 if no links.</returns>
        public override int getMaxLinkId()
        {
            return this.getTableMaxId(MDbAdapterMsJet.LinkTableName, "id");
        }
        /// <summary>
        /// NT-Get min of link primary key in table. Return 0 if no links.
        /// </summary>
        /// <returns>Returns min of link primary key in table. Return 0 if no links.</returns>
        public override int getMinLinkId()
        {
            return this.getTableMinId(MDbAdapterMsJet.LinkTableName, "id");
        }
        /// <summary>
        /// NT-Return number of rows in link table
        /// </summary>
        /// <returns>Return number of rows in link table</returns>
        public override int getNumberOfLinks()
        {
            return this.getRowCount(MDbAdapterMsJet.LinkTableName, "id");
        }
 #endregion




    }
}
