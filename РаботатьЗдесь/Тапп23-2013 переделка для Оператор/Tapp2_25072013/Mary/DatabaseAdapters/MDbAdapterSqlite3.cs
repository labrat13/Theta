using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Globalization;

namespace Mary.DatabaseAdapters
{
    
    /// <summary>
    /// Класс адаптера БД для БД Sqlite3
    /// </summary>
    /// <remarks>
    /// Для тестирования и запуска необходимо добавить сборки собственно СУБД в каталог приложения в специальном порядке.
    /// Нужно смотреть документацию по sqlite3 NET Connector.
    /// </remarks>
    public class MDbAdapterSqlite3: MDbAdapterBase
    {
        /// <summary>
        /// Имя файла базы данных для файловых СУБД
        /// </summary>
        public const string DatabaseFileName = "db.sqlite";
        //некоторые поля и проперти остались в базовом классе.

#region *** Fields ***

        //все объекты команд должны сбрасываться в нуль при отключении соединения с БД
        //TODO: Новые команды внести в ClearCommands()
        private SQLiteCommand m_cmdWithoutArguments;

 #endregion

        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public MDbAdapterSqlite3(MEngine engine): base(engine)
        {
            
            //add some inits here
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~MDbAdapterSqlite3()
        {
            this.Close();
        }

#region *** Properties ***
        /// <summary>
        /// Тип БД, поддерживаемый адаптером
        /// </summary>
        public override Mary.MDatabaseType DatabaseType
        {
            get
            {
                return MDatabaseType.Sqlite3; 
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



#region *** Функции сеанса и транзакции*** - написаны
        /// <summary>
        /// NT-все объекты команд сбросить в нуль
        /// </summary>
        protected override void ClearCommands()
        {
            this.m_cmdWithoutArguments = null;
            //throw new System.NotImplementedException();
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
        /// NT-Open manager
        /// </summary>
        /// <param name="info">Объект настроек Солюшена</param>
        public override void Open(Mary.MSolutionInfo info)
        {
            //тут надо создать строку подключения к СУБД и вписать ее в поле класса.
            this.m_connectionString = this.createConnectionString(info);
            //set up timeout value
            this.m_Timeout = info.DatabaseTimeout;
            //TODO: add more initialization here...
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
            SQLiteConnection con = new SQLiteConnection(this.m_connectionString);
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
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            //open dummy file
            builder.DataSource = DatabaseServerPath;
            builder.ReadOnly = false;
            builder.FailIfMissing = true;//чтобы выбрасывать исключение если БД не существует. Иначе она будет создаваться новая.
            //password can specify here - но я не уверен что шифрование тут поддерживается
            return builder.ConnectionString;
        }

        /// <summary>
        /// NT-Create connection string
        /// </summary>
        /// <param name="pfile">Объект ФайлСолюшена</param>
        /// <returns>Возвращает строку соединения с выбранной БД</returns>
        /// <remarks>Без лога, или проверять его существование!</remarks>>
        public override string createConnectionString(MSolutionInfo pfile)
        {
            return this.createConnectionString(pfile.DatabaseServerPath, pfile.DatabaseName, pfile.DatabasePortNumber, pfile.DatabaseTimeout, pfile.UserName, pfile.UserPassword, pfile.UseIntegratedSecurity);
        }
 #endregion

        #region *** Функции создания и изменения БД *** - осталась getTableIdentity()

        /// <summary>
        /// NT-Создать БД проекта.
        /// Создать, открыть, инициализировать содержимое и закрыть БД. 
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        public override void DatabaseCreate(MSolutionInfo info)
        {
            //0 создать путь к файлу БД и вписать в MSolutionInfo.DatabaseServerPath
            String DbFilePath = Path.Combine(info.getCurrentSolutionDirectory(), MDbAdapterSqlite3.DatabaseFileName);
            info.DatabaseServerPath = DbFilePath;
            //1 создать БД
            SQLiteConnection.CreateFile(DbFilePath);
            //2 проверить что БД доступна и наполнить ее таблицами и индексами
            string connectionString = this.createConnectionString(info);
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteTransaction t = con.BeginTransaction();
            this.CreateTablesIndexes(null);
            this.insertEngineTableInitial(info, con, 600);//add container properties values to table
            t.Commit();
            con.Close();
            con = null;

            return;
        }

        /// <summary>
        /// NT-Удалить существующую закрытую БД.
        /// </summary>
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
            this.ClearTable(MDbAdapterSqlite3.CellTableName);
            this.ClearTable(MDbAdapterSqlite3.LinkTableName);
            this.ClearTable(MDbAdapterSqlite3.ContainerTableName);

            return;
        }

        /// <summary>
        /// NT-Create tables and indexes on existing database
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        public override void CreateTablesIndexes(string connectionString)
        {

            string[] queries = new string[] { 
                "DROP TABLE IF EXISTS `CellTable`;",
                "CREATE TABLE \"CellTable\"( \"id\" Integer Primary Key Autoincrement  NOT NULL , \"name\" Text  NOT NULL , \"descr\" Text NOT NULL , \"active\" Integer NOT NULL DEFAULT (0), \"type\" Integer NOT NULL DEFAULT (0), \"creatime\" Integer NOT NULL   DEFAULT (0), \"moditime\" Integer NOT NULL DEFAULT (0), \"ronly\" Integer  NOT NULL DEFAULT (0), \"state\" Integer DEFAULT (0), \"sflag\" Integer NOT NULL DEFAULT (0), \"val\" Blob DEFAULT ('NULL'), \"valtype\" Integer NOT NULL DEFAULT (0), \"cellid\" Integer NOT NULL DEFAULT (0));",
                "DROP TABLE IF EXISTS `LinkTable`;",
                "CREATE TABLE \"LinkTable\"(\"id\" Integer Primary Key Autoincrement, \"downID\" Integer NOT NULL DEFAULT (0), \"upID\" Integer NOT NULL DEFAULT (0), \"axis\" Integer NOT NULL DEFAULT (0), \"state\" Integer NOT NULL DEFAULT (0), \"active\" Integer NOT NULL DEFAULT (0), \"sflag\" Integer NOT NULL DEFAULT (0), \"descr\" Text, \"moditime\" Integer NOT NULL DEFAULT (0));",
                "DROP TABLE IF EXISTS `EngineTable`;",
                "CREATE TABLE \"EngineTable\"( \"id\" Integer Primary Key Autoincrement, \"version\" Text NOT NULL, \"step\" Text  NOT NULL, \"lognum\" Integer NOT NULL DEFAULT (0), \"loglevel\" Integer NOT NULL DEFAULT (0), \"descr\" Text, \"name\" Text  NOT NULL, \"sflag\" Integer NOT NULL DEFAULT (0), \"state\" Integer  NOT NULL DEFAULT (0), \"cellmode\" Integer NOT NULL DEFAULT (0), \"idcon\" Integer NOT NULL DEFAULT (0));",
                "CREATE INDEX index_CellTable_Name ON CellTable(name ASC)",
                "CREATE UNIQUE INDEX index_CellTable_CellId ON CellTable(cellid ASC)",
                "CREATE INDEX index_LinkTable_upId ON LinkTable(upID ASC)",
                "CREATE INDEX index_LinkTable_downId ON LinkTable(downID ASC)" 
                };

            //execute
            foreach (string s in queries)
            {
                CmdExecuteScalar(s, 600);
            }

            return;
        }
        
        /// <summary>
        /// NT-Remove all from celltable and linktable
        /// Это удаляет вся ячейки и связи Солюшена, не затрагивая данные Контейнера.
        /// </summary>
        public override void ClearCellTableAndLinkTable()
        {
            this.ClearTable(MDbAdapterSqlite3.CellTableName);
            this.ClearTable(MDbAdapterSqlite3.LinkTableName);

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
        protected Int32 CmdExecuteScalar(string query, int timeout)
        {
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new SQLiteCommand(String.Empty, (SQLiteConnection)this.m_connection, (SQLiteTransaction)this.m_transaction);
                m_cmdWithoutArguments.CommandTimeout = timeout;
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
        /// NT-Удалить все строки из указанной таблицы
        /// </summary>
        /// <param name="table">Название таблицы</param>
        protected override void ClearTable(string table)
        {
            string query = String.Format(CultureInfo.InvariantCulture, "DELETE FROM {0};", table);
            this.CmdExecuteScalar(query, 600);//10 минут на очистку 1 таблицы

            return;
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
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT COUNT({0}) FROM {1};", column, table);
            return CmdExecuteScalar(query, this.m_Timeout);
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
            return CmdExecuteScalar(query, this.m_Timeout);
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
            return CmdExecuteScalar(query, this.m_Timeout);
        }

        /// <summary>
        /// NT-Получить строку текста из ридера таблицы или пустую строку
        /// </summary>
        /// <param name="rdr">Объект ридера таблицы бд</param>
        /// <param name="p">Номер столбца в таблице и ридере</param>
        /// <returns>Возвращает строку текста из поля или пустую строку если в поле хранится значение DbNull</returns>
        public static string getDbString(SQLiteDataReader rdr, int p)
        {
            if (rdr.IsDBNull(p))
                return String.Empty;
            else return rdr.GetString(p).Trim();
        }
 #endregion

#region *** Функции контейнера *** - неготовы

        /// <summary>
        /// NR-Load first existing container from database. Throw exception if container not found 
        /// </summary>
        /// <param name="container">Container object for loading</param>
        /// <exception cref="SqlException">locked row</exception>
        /// <exception cref="InvalidOperationException">connection is closed</exception>
        public override void ContainerLoad(Mary.MEngine cont)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Save container in database. Create new record if any not exists
        /// </summary>
        /// <param name="container"></param>
        /// <remarks>Без лога, или проверять его существование!</remarks>
        public override void ContainerSave(Mary.MEngine container)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NT-Создать команду для таблицы контейнера - для экономии кода
        /// </summary>
        /// <param name="query">Текст запроса для команды.</param>
        /// <returns></returns>
        private SQLiteCommand createContainerCmd(string query, SQLiteConnection con)
        {
            SQLiteCommand sc = new SQLiteCommand(query, con);
            sc.CommandTimeout = this.m_Timeout;
            sc.CommandType = CommandType.Text;
            sc.Parameters.Add("@version", DbType.String);
            sc.Parameters.Add("@step", DbType.String);
            sc.Parameters.Add("@lognum", DbType.Int32);
            sc.Parameters.Add("@loglevel", DbType.Int32);
            sc.Parameters.Add("@descr", DbType.String);
            sc.Parameters.Add("@name", DbType.String);
            sc.Parameters.Add("@sflag", DbType.Int32);
            sc.Parameters.Add("@state", DbType.UInt64);
            sc.Parameters.Add("@cellmode", DbType.Int32);
            sc.Parameters.Add("@idcon", DbType.Int32);

            return sc;
        }

        /// <summary>
        /// NT-Insert initial (first) row in EngineTable
        /// </summary>
        /// <param name="info">Настройки Солюшена.</param>
        /// <param name="con">Открытое соединение с БД.</param>
        /// <param name="timeout">Таймаут операции, секунд.</param>
        private void insertEngineTableInitial(MSolutionInfo info, SQLiteConnection con, int timeout)
        {
            //при переделке также изменить и insertEngineTable(..)
            string query = "INSERT INTO " + MDbAdapterSqlite3.ContainerTableName + " (version, step, lognum, loglevel, descr, name, sflag, state, cellmode, idcon) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";
            //поскольку команда используется однократно, создаем ее в стеке, экономим память.
            SQLiteCommand sc = createContainerCmd(query, con);
            sc.CommandTimeout = timeout;
            //add values
            sc.Parameters[0].Value = info.SolutionEngineVersion.toTextString();//TODO: TAGVERSIONNEW: изменить тип поля на строку версии (64символа)
            sc.Parameters[1].Value = info.SolutionVersion.toTextString();//TODO: TAGVERSIONNEW: изменить тип поля на строку версии (64символа)
            sc.Parameters[2].Value = info.LogfileNumber;
            sc.Parameters[3].Value = (int)info.LogDetailsFlags;
            sc.Parameters[4].Value = info.SolutionDescription;
            sc.Parameters[5].Value = info.SolutionName;
            sc.Parameters[6].Value = info.ContainerServiceFlag;
            sc.Parameters[7].Value = (UInt64)info.ContainerState;//TODO: TAGVERSIONNEW: изменить тип поля на ulong
            sc.Parameters[8].Value = (int)info.ContainerDefaultCellMode;
            sc.Parameters[9].Value = info.SolutionId;
            //execute command
            sc.ExecuteNonQuery();

            return;
        }

#endregion

#region *** Функции ячеек *** - неготовы

        /// <summary>
        /// NR-Проверить, что ячейка с таким идентификатором существует
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <returns>Returns true if cell exists, false otherwise</returns>
        public override bool isCellExists(Mary.MID cellid)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Проверить, что ячейка с таким названием существует
        /// </summary>
        /// <param name="cellName">Cell name</param>
        /// <returns>Returns true if cell exists, false otherwise</returns>
        public override bool isCellExists(string cellName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Return number of rows in cell table
        /// </summary>
        /// <returns>Return number of rows in cell table</returns>
        public override int getNumberOfCells()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get max of cell id's in table, return 0 if no cells
        /// </summary>
        /// <returns>Returns max of cell id's in table, return 0 if no cells</returns>
        public override int getMaxCellId()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get min of cell id's in table, return 0 if no cells
        /// </summary>
        /// <returns>Returns min of cell id's in table, return 0 if no cells</returns>
        public override int getMinCellId()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Insert cell record to table
        /// </summary>
        /// <param name="cell">Cell object</param>
        public override int CellInsert(Mary.MCellB cell)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Save cell data - update or insert row
        /// </summary>
        /// <param name="cell">Cell object</param>
        public override void CellSave(Mary.MCellB cell)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get cell by cell id
        /// </summary>
        /// <param name="cellId">Cell identificator</param>
        /// <param name="largeCell">Cell mode: False for MCellA, true for MCellB</param>
        /// <returns>Returns MCell object or null if cell not exists</returns>
        public override Mary.MCell CellSelect(Mary.MID cellId, bool largeCell)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get cell by cell title
        /// </summary>
        /// <param name="title">Cell title string</param>
        /// <param name="largeCell">Cell mode: False for MCellA, true for MCellB</param>
        /// <returns>Returns MCell object or null if cell not exists</returns>
        public override Mary.MCell CellSelect(string title, bool largeCell)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Update cell record 
        /// </summary>
        /// <param name="cell">Cell object</param>
        /// <returns>Returns number of affected rows</returns>
        public override int CellUpdate(Mary.MCellB cell)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get band of cells
        /// </summary>
        /// <param name="rowFrom">cell id from which begin select cells</param>
        /// <param name="rowTo">cell id to (but not include) end select cells</param>
        /// <returns>Returns list of cells</returns>
        public override List<Mary.MCell> getBlockOfCells(int rowFrom, int rowTo)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get cell links filtered by axis direction
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <param name="axisDir">axis direction: Any, Up, Down</param>
        /// <returns>Returns collection of links filtered by axis direction</returns>
        public override Mary.MLinkCollection getCellLinks(int cellid, Mary.MAxisDirection axisDir)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Find cells meet specified template. 
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
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override bool getCellActive(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override DateTime getCellCreationTime(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override string getCellDescription(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override System.DateTime getCellModificationTime(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override string getCellName(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override bool getCellReadOnly(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override int getCellServiceFlag(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override Mary.MID getCellState(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override Mary.MID getCellTypeId(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override byte[] getCellValue(int cellid)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override Mary.MID getCellValueTypeId(int cellid)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellActive(int cellid, bool val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellCreationTime(int cellid, System.DateTime val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellDescription(int cellid, string val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellModificationTime(int cellid, System.DateTime val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellName(int cellid, string val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellReadOnly(int cellid, bool val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellServiceFlag(int cellid, int val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellState(int cellid, Mary.MID val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellTypeId(int cellid, Mary.MID val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellValue(int cellid, byte[] val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellValueTypeId(int cellid, Mary.MID val)
        {
            throw new System.NotImplementedException();
        }

 #endregion


#region *** Функции связей *** - неготовы

        /// <summary>
        /// NR-Return last LinkTable identity (primary key value)
        /// </summary>
        /// <returns>Returns last LinkTable identity (primary key value)</returns>
        protected override int getLastIdentityLinksTable()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Delete link by link table id
        /// </summary>
        /// <param name="linkId">link id from table primary key</param>
        /// <returns>Returns number of affected rows</returns>
        public override int LinkDelete(int linkId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Insert link to table and change and return link primary key, that serve as linkId.
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Возвращает идентификатор связи</returns>
        public override int LinkInsertGetId(Mary.MLink link)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Insert Link record into LinkTable
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Returns number of affected rows</returns>
        public override int LinkInsert(Mary.MLink link)
        {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// NR-Update link. Return num of affected rows
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Returns number of affected rows</returns>
        public override int LinkUpdate(Mary.MLink link)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get band of links
        /// </summary>
        /// <param name="rowFrom">link id from which begin select links</param>
        /// <param name="rowTo">link id to (but not include) end select links</param>
        /// <returns>Returns collection of links</returns>
        public override Mary.MLinkCollection getBlockOfLinks(int rowFrom, int rowTo)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-get link id primary key for first founded link. Return 0 if link not exists
        /// TODO: неправильно работает при наличии нескольких связей между ячейками
        /// </summary>
        /// <param name="dnCellId">Идентификатор подчиненной ячейки</param>
        /// <param name="upCellId">Идентификатор главной ячейки</param>
        /// <param name="axis">Идентификатор типа связи</param>
        /// <returns>Возвращает первичный ключ записи связи в таблице</returns>
        public override int getLinkID(Mary.MID dnCellId, Mary.MID upCellId, Mary.MID axis)
        {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// NR-get link id primary key for first founded link. Return 0 if link not exists
        /// TODO: неправильно работает при наличии нескольких связей между ячейками
        /// </summary>
        /// <param name="link">Объект связи</param>
        /// <returns>Возвращает первичный ключ записи связи в таблице</returns>
        public override int getLinkID(Mary.MLink link)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get cell links filtered by axis direction
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <param name="axisDir">axis direction: Any, Up, Down</param>
        /// <returns>Returns collection of links filtered by axis direction</returns>
        public override Mary.MLinkCollection getLinks(Mary.MLinkTemplate tmp)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get max of link primary key in table. Return 0 if no links.
        /// </summary>
        /// <returns>Returns max of link primary key in table. Return 0 if no links.</returns>
        public override int getMaxLinkId()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Get min of link primary key in table. Return 0 if no links.
        /// </summary>
        /// <returns>Returns min of link primary key in table. Return 0 if no links.</returns>
        public override int getMinLinkId()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Return number of rows in link table
        /// </summary>
        /// <returns>Return number of rows in link table</returns>
        public override int getNumberOfLinks()
        {
            throw new System.NotImplementedException();
        }
 #endregion













    }
}
