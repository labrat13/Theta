using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;
using System.Data.SqlTypes;

namespace Mary.DatabaseAdapters
{
    /// <summary>
    /// Адаптер БД MsSql2005
    /// Транзакции пока не поддерживаются.
    /// </summary>
    public class MDbAdapterMsSql2005: MDbAdapterBase
    {
        //некоторые поля и проперти остались в базовом классе.
#region *** Fields ***
        /// <summary>
        /// Own item of connection - not from base class
        /// </summary>
        protected new SqlConnection m_connection;
        
        //объекты команд создаются и инициализируются в функциях, которые их используют.
        //все объекты команд должны сбрасываться в нуль при отключении соединения с БД
        //TODO: Новые команды внести в ClearCommands()
        SqlCommand m_cmdWithoutArguments;
        SqlCommand m_cmdGetCellExists;
        SqlCommand m_cmdInsertCellTable;
        SqlCommand m_cmdGetCell;
        SqlCommand m_cmdUpdateCellTable;
        SqlCommand m_cmdGetBlockOfCells;
        SqlCommand m_cmdGetCellLinks;
        SqlCommand m_cmdSetCellData;
        SqlCommand m_cmdGetCellData;
        SqlCommand m_cmdDeleteLinkById;
        SqlCommand m_cmdInsertLinkTable;
        SqlCommand m_cmdUpdateLinkTable;
        SqlCommand m_cmdGetBlockOfLinks;
        SqlCommand m_cmdGetLinkId;
        SqlCommand m_cmdGetCellByTitle;

 #endregion

        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public MDbAdapterMsSql2005(MEngine engine):base(engine)
        {
            
            //add some inits here
            return;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~MDbAdapterMsSql2005()
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
                return MDatabaseType.MicrosoftSqlServer2005;
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



#region *** Функции сеанса и транзакции***
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
        /// NR-Get string representation of object.
        /// </summary>
        /// <returns>Return string representation of object.</returns>
        public override string ToString()
        {
            return base.ToString();
        }
        /// <summary>
        /// NT-Initialize manager
        /// </summary>
        /// <param name="info">Объект настроек Солюшена</param>
        public override void Open(MSolutionInfo info)
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
        /// NT-Finalize manager
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
            if ((this.m_connection != null) && (this.m_connection.State != ConnectionState.Closed))
                return;
            //создаем объект соединения
            SqlConnection con = new SqlConnection(this.m_connectionString);
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
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.ConnectTimeout = Timeout;
            csb.DataSource = DatabaseServerPath;
            csb.InitialCatalog = DatabaseName;
            csb.IntegratedSecurity = IntegratedSecurity;
            csb.Password = UserPassword;
            csb.UserID = UserName;
            //dbPort тут не применим - нет поля для номера порта

            return csb.ConnectionString;
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

#region *** Функции создания и изменения БД ***

        /// <summary>
        /// NT-Создать БД проекта и заполнить таблицами. Вписать данные контейнера в таблицу контейнера.
        /// создать, открыть, записать и закрыть БД 
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        /// <remarks>User must have "dbcreator" role of SqlServer. In created database user is "dbo".</remarks>
        public override void DatabaseCreate(MSolutionInfo info)
        {
            string connectionString = this.createConnectionString(info);
            //replace dbname to "master"
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
            String dbName = csb.InitialCatalog;
            csb.InitialCatalog = "master";
            //create database
            SqlConnection con = new SqlConnection(csb.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(String.Format("CREATE DATABASE {0}", dbName), con);
            cmd.ExecuteNonQuery();
            con.Close();

            //add tables to database
            con = new SqlConnection(connectionString);
            con.Open();
            this.createTablesIndexes(con);//а тут можно бы и переделать на использование обычного соединения как при обычной работе.
            this.insertEngineTableInitial(info, con);//add container properties values to table
            con.Close();

            return;
        }

        /// <summary>
        /// NT-Удалить существующую БД вместе со всем содержимым.
        /// Пользователь должен иметь соответствующие права в СУБД.
        /// </summary>
        /// <param name="connectionString">Строка соединения с БД. </param>
        public override void DatabaseDelete(MSolutionInfo info)
        {
            string connectionString = this.createConnectionString(info);
            //replace dbname to "master"
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
            String dbName = csb.InitialCatalog;
            csb.InitialCatalog = "master";
            String constr = csb.ConnectionString;
            //delete database
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            //disconnect any users and remove database from server
            SqlCommand cmd = new SqlCommand(String.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {0};", dbName), con);
            cmd.ExecuteNonQuery();
            con.Close();

            return;
        }

        /// <summary>
        /// NT-Очистить БД
        /// </summary>
        /// <returns>Returns True if Success, False otherwise</returns>
        public override void DatabaseClear()
        {
            //delete all tables from database
            this.TableDelete(MDbAdapterMsSql2005.CellTableName);
            this.TableDelete(MDbAdapterMsSql2005.LinkTableName);
            this.TableDelete(MDbAdapterMsSql2005.ContainerTableName);

            return;
        }

        /// <summary>
        /// NT-Create tables and indexes on existing database
        /// Создать все таблицы и индексы, если они не существуют
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        protected void createTablesIndexes(SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand("", con);

            string tt0 = "SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON; IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CellTable]') AND type in (N'U')) BEGIN CREATE TABLE [dbo].[CellTable](	[id] [int] IDENTITY(1,1) NOT NULL, 	[name] [nvarchar](440) NOT NULL, [descr] [ntext] NULL,	[active] [bit] NOT NULL, [type] [int] NOT NULL, [creatime] [datetime] NOT NULL, [moditime] [datetime] NOT NULL, [ronly] [bit] NOT NULL, [state] [int] NOT NULL, [sflag] [int] NOT NULL, [val] [varbinary](max) NULL, [valtype] [int] NOT NULL, [cellid] [int] NOT NULL, CONSTRAINT [PK_CellTable] PRIMARY KEY CLUSTERED ( [id] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] END;";
            cmd.CommandText = tt0;
            cmd.ExecuteNonQuery();

            /****** Объект:  Index [IX_CellTable_cellid]    Дата сценария: 06/01/2012 20:33:25 ******/
            string tt1 = "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CellTable]') AND name = N'IX_CellTable_cellid') CREATE UNIQUE NONCLUSTERED INDEX [IX_CellTable_cellid] ON [dbo].[CellTable] ([cellid] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            cmd.CommandText = tt1;
            cmd.ExecuteNonQuery();

            /****** Объект:  Index [IX_CellTable_name]    Дата сценария: 06/01/2012 20:33:25 ******/
            string tt2 = "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CellTable]') AND name = N'IX_CellTable_name') CREATE NONCLUSTERED INDEX [IX_CellTable_name] ON [dbo].[CellTable] ([name] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            cmd.CommandText = tt2;
            cmd.ExecuteNonQuery();

            //linktable
            string tt3 = "SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON; IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LinkTable]') AND type in (N'U')) BEGIN CREATE TABLE [dbo].[LinkTable](	[id] [int] IDENTITY(1,1) NOT NULL,	[downID] [int] NOT NULL,	[upID] [int] NOT NULL,	[axis] [int] NOT NULL,	[state] [int] NOT NULL,	[active] [bit] NOT NULL, [sflag] [int] NOT NULL, [descr] [ntext] NULL, [moditime] [datetime] NOT NULL, CONSTRAINT [PK_LinkTable] PRIMARY KEY CLUSTERED ([id] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] END";
            cmd.CommandText = tt3;
            cmd.ExecuteNonQuery();

            /****** Объект:  Index [IX_LinkTable_downID]    Дата сценария: 06/01/2012 20:33:25 ******/
            string tt4 = "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[LinkTable]') AND name = N'IX_LinkTable_downID') CREATE NONCLUSTERED INDEX [IX_LinkTable_downID] ON [dbo].[LinkTable] ([downID] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            cmd.CommandText = tt4;
            cmd.ExecuteNonQuery();

            /****** Объект:  Index [IX_LinkTable_updownID]    Дата сценария: 06/01/2012 20:33:25 ******/
            string tt5 = "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[LinkTable]') AND name = N'IX_LinkTable_updownID') CREATE NONCLUSTERED INDEX [IX_LinkTable_updownID] ON [dbo].[LinkTable] ([upID] ASC, [downID] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            cmd.CommandText = tt5;
            cmd.ExecuteNonQuery();

            /****** Объект:  Index [IX_LinkTable_upID]    Дата сценария: 06/01/2012 20:33:25 ******/
            string tt6 = "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[LinkTable]') AND name = N'IX_LinkTable_upID') CREATE NONCLUSTERED INDEX [IX_LinkTable_upID] ON [dbo].[LinkTable] ([upID] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            cmd.CommandText = tt6;
            cmd.ExecuteNonQuery();

            /****** Объект:  Table [dbo].[EngineTable]    Дата сценария: 06/01/2012 20:33:25 ******/
            //старая версия: string tt7 = "SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON; IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EngineTable]') AND type in (N'U')) BEGIN CREATE TABLE [dbo].[EngineTable]([id] [int] IDENTITY(1,1) NOT NULL, [version] [int] NOT NULL CONSTRAINT [DF_EngineTable_version]  DEFAULT ((23000)), [step] [int] NOT NULL CONSTRAINT [DF_EngineTable_step]  DEFAULT ((0)), [lognum] [int] NOT NULL CONSTRAINT [DF_EngineTable_lognum]  DEFAULT ((0)), [loglevel] [int] NOT NULL CONSTRAINT [DF_EngineTable_loglevel]  DEFAULT ((0)), [descr] [ntext] NULL, [name] [nvarchar](max) NULL, [sflag] [int] NOT NULL, [state] [int] NOT NULL, [cellmode] [int] NOT NULL CONSTRAINT [DF_EngineTable_cellmode]  DEFAULT ((0)), [idcon] [int] NOT NULL CONSTRAINT [DF_EngineTable_idcon]  DEFAULT ((0)), CONSTRAINT [PK_EngineTable] PRIMARY KEY CLUSTERED ([id] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] END";
            string tt7 = "SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON; IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EngineTable]') AND type in (N'U')) BEGIN CREATE TABLE [dbo].[EngineTable]([id] [int] IDENTITY(1,1) NOT NULL, [version] [nvarchar](64) NOT NULL, [step] [nvarchar](64) NOT NULL, [lognum] [int] NOT NULL CONSTRAINT [DF_EngineTable_lognum]  DEFAULT ((0)), [loglevel] [int] NOT NULL CONSTRAINT [DF_EngineTable_loglevel]  DEFAULT ((0)), [descr] [ntext] NULL, [name] [nvarchar](max) NULL, [sflag] [int] NOT NULL, [state] [bigint] NOT NULL, [cellmode] [int] NOT NULL CONSTRAINT [DF_EngineTable_cellmode]  DEFAULT ((0)), [idcon] [int] NOT NULL CONSTRAINT [DF_EngineTable_idcon]  DEFAULT ((0)), CONSTRAINT [PK_EngineTable] PRIMARY KEY CLUSTERED ([id] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] END";
            cmd.CommandText = tt7;
            cmd.ExecuteNonQuery();

            return;
        }
        
        /// <summary>
        /// NT-Remove all from celltable and linktable
        /// Это удаляет вся ячейки и связи Солюшена, не затрагивая данные Контейнера.
        /// </summary>
        public override void ClearCellTableAndLinkTable()
        {
            //delete celltable and linktable from database
            //DeleteTablesCellLink();
            this.TableDelete(MDbAdapterMsSql2005.CellTableName);
            this.TableDelete(MDbAdapterMsSql2005.LinkTableName);

            //create tables and indexes if not exists
            createTablesIndexes(this.m_connection);
            return;
        }

        /// <summary>
        /// NT-Удалить таблицу
        /// </summary>
        /// <param name="table">Назваие таблицы</param>
        protected void TableDelete(string table)
        {
            string query = "DROP TABLE " + table;
            SqlCommand t = new SqlCommand(query, m_connection);
            t.CommandTimeout = 60;
            t.ExecuteNonQuery();

            return;
        }

        /// <summary>
        /// NT-Удалить все строки из указанной таблицы
        /// </summary>
        /// <param name="table">Название таблицы</param>
        protected override void ClearTable(string table)
        {
            string query = "DELETE * FROM " + table;
            SqlCommand t = new SqlCommand(query, m_connection);
            t.CommandTimeout = 600;
            t.ExecuteNonQuery();

            return;
        }


        /// <summary>
        /// NT-Return last table identity (primary key value)
        /// </summary>
        /// <param name="table">Table name</param>
        /// <returns>
        /// Returns last table identity (primary key value)
        /// Возвращает -1 если запрос вернул null.
        /// </returns>
        protected override int getTableIdentity(string table)
        {
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT IDENT_CURRENT('{0}') AS Expr1", table);  //Поведение после отката транзакции неясно.
            return CmdExecuteScalar(query);
        }
        /// <summary>
        /// NT-Выполнить скалярный запрос и вернуть результат запроса
        /// </summary>
        /// <param name="query">Текст запроса</param>
        /// <returns>
        /// Возвращает результат запроса как число Int32.
        /// Возвращает -1 если запрос вернул null.
        /// </returns>
        protected int CmdExecuteScalar(string query)
        {
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new SqlCommand(query, m_connection);
                m_cmdWithoutArguments.CommandTimeout = m_Timeout;
                m_cmdWithoutArguments.CommandType = System.Data.CommandType.Text;
            }
            Object ob = m_cmdWithoutArguments.ExecuteScalar();
            //int result = -1;
            //if (ob != null) result = Convert.ToInt32(ob);
            //Если не пойдет так, то попробовать через строку:
            String s = ob.ToString();
            if (String.IsNullOrEmpty(s))
                return -1;
            else return Int32.Parse(s);

            //return result;
        }

        /// <summary>
        /// NT-Получить число записей в таблице
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <returns>
        /// Возвращает число записей в таблице.
        /// Возвращает -1 если запрос вернул null.
        /// </returns>
        protected override int getRowCount(string table, string column)
        {
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT COUNT(id) FROM {0};", table);
            return CmdExecuteScalar(query);
        }

        /// <summary>
        /// NT-Get max of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>
        /// Returns value of max of table id's
        /// Возвращает -1 если запрос вернул null.
        /// </returns>
        protected override int getTableMaxId(string table, string column)
        {
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT MAX(id) FROM {0};", table);
            return CmdExecuteScalar(query);
        }

        /// <summary>
        /// NT-Get min of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>
        /// Returns value of min of table id's
        /// Возвращает -1 если запрос вернул null.
        /// </returns>
        protected override int getTableMinId(string table, string column)
        {
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT MIN(id) FROM {0};", table);
            return CmdExecuteScalar(query);
        }

 #endregion

#region *** Функции контейнера ***

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
            SqlCommand sc = new SqlCommand("SELECT EngineTable.* FROM EngineTable", m_connection);
            sc.CommandType = CommandType.Text;
            sc.CommandTimeout = m_Timeout;
            SqlDataReader sdr = sc.ExecuteReader();
            if (sdr.HasRows == true)
            {
                while (sdr.Read())
                {
                    
                    version = new MEngineVersionInfo(sdr.GetString(1));//TODO: TAGVERSIONNEW:-сделано 
                    //check engine version
                    if(cont.SolutionSettings.CurrentEngineVersion.isCompatibleVersion(version) == false)
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
            string query = "UPDATE EngineTable SET version = @ver, step = @step, lognum = @lname, loglevel = @llevel, descr = @descr, name = @name, sflag = @sflag, state = @state, cellmode = @cellmode, idcon = @idcon";
            //UPDATE EngineTable SET version = @ver, step = @step, dirpath = @path, logname = @lname, loglevel = @llevel, stepname = @sname, descr = @descr, name = @name, sflag = @sflag, state = @state, limiter = @limiter
            //поскольку команда используется сравнительно редко, создаем ее в стеке, экономим память.
            SqlCommand sc = new SqlCommand(query, this.m_connection);
            sc.CommandTimeout = this.m_Timeout;
            sc.CommandType = CommandType.Text;
            sc.Parameters.Add("@ver", SqlDbType.NVarChar);
            sc.Parameters.Add("@step", SqlDbType.NVarChar);
            sc.Parameters.Add("@lname", SqlDbType.Int);
            sc.Parameters.Add("@llevel", SqlDbType.Int);
            sc.Parameters.Add("@descr", SqlDbType.NText);
            sc.Parameters.Add("@name", SqlDbType.NVarChar);
            sc.Parameters.Add("@sflag", SqlDbType.Int);
            sc.Parameters.Add("@state", SqlDbType.BigInt);
            sc.Parameters.Add("@cellmode", SqlDbType.Int);
            sc.Parameters.Add("@idcon", SqlDbType.Int);

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
        /// NT-Insert row in EngineTable
        /// </summary>
        /// <param name="cont"></param>
        private void insertEngineTable(MEngine cont)
        {
            //при переделке также изменить и insertEngineTableInitial(..)
            string query = "INSERT INTO EngineTable (version, step, lognum, loglevel, descr, name, sflag, state, cellmode, idcon) VALUES (@ver, @step, @lname, @llevel, @descr, @name, @sflag, @state, @cellmode, @idcon)";
            //INSERT INTO EngineTable (version, step, dirpath, logname, loglevel, stepname, descr, name, sflag, state, limiter) VALUES (23000, 0, N'c:\\dir', N'logname', 0, N'stepname', N'descr', N'namme', 0, 0, '`')
            //поскольку команда используется однократно, создаем ее в стеке, экономим память.
            SqlCommand sc = new SqlCommand(query, this.m_connection);
            sc.CommandTimeout = this.m_Timeout;
            sc.CommandType = CommandType.Text;
            sc.Parameters.Add("@ver", SqlDbType.NVarChar);
            sc.Parameters.Add("@step", SqlDbType.NVarChar);
            sc.Parameters.Add("@lname", SqlDbType.Int);
            sc.Parameters.Add("@llevel", SqlDbType.Int);
            sc.Parameters.Add("@descr", SqlDbType.NText);
            sc.Parameters.Add("@name", SqlDbType.NVarChar);
            sc.Parameters.Add("@sflag", SqlDbType.Int);
            sc.Parameters.Add("@state", SqlDbType.BigInt);
            sc.Parameters.Add("@cellmode", SqlDbType.Int);
            sc.Parameters.Add("@idcon", SqlDbType.Int);
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
        private void insertEngineTableInitial(MSolutionInfo info, SqlConnection con)
        {
            //при переделке также изменить и insertEngineTable(..)
            string query = "INSERT INTO EngineTable (version, step, lognum, loglevel, descr, name, sflag, state, cellmode, idcon) VALUES (@ver, @step, @lname, @llevel, @descr, @name, @sflag, @state, @cellmode, @idcon)";
            //INSERT INTO EngineTable (version, step, dirpath, logname, loglevel, stepname, descr, name, sflag, state, limiter) VALUES (23000, 0, N'c:\\dir', N'logname', 0, N'stepname', N'descr', N'namme', 0, 0, '`')
            //поскольку команда используется однократно, создаем ее в стеке, экономим память.
            SqlCommand sc = new SqlCommand(query, con);
            sc.CommandTimeout = this.m_Timeout;
            sc.CommandType = CommandType.Text;
            sc.Parameters.Add("@ver", SqlDbType.NVarChar);
            sc.Parameters.Add("@step", SqlDbType.NVarChar);
            sc.Parameters.Add("@lname", SqlDbType.Int);
            sc.Parameters.Add("@llevel", SqlDbType.Int);
            sc.Parameters.Add("@descr", SqlDbType.NText);
            sc.Parameters.Add("@name", SqlDbType.NVarChar);
            sc.Parameters.Add("@sflag", SqlDbType.Int);
            sc.Parameters.Add("@state", SqlDbType.BigInt);
            sc.Parameters.Add("@cellmode", SqlDbType.Int);
            sc.Parameters.Add("@idcon", SqlDbType.Int);
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

#region *** Функции ячеек ***

        /// <summary>
        /// NT-Проверить, что ячейка с таким идентификатором существует
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <returns>Returns true if cell exists, false otherwise</returns>
        public override bool isCellExists(MID cellid)
        {
            //SELECT CellTable.* FROM CellTable WHERE (cellid = @Param1)
            if (m_cmdGetCellExists == null)
            {
                m_cmdGetCellExists = new SqlCommand("SELECT CellTable.cellid FROM CellTable WHERE (cellid = @cid)", m_connection);
                m_cmdGetCellExists.CommandTimeout = m_Timeout;
                m_cmdGetCellExists.Parameters.Add("@cid", SqlDbType.Int);
            }
            //execute
            m_cmdGetCellExists.Parameters[0].Value = cellid.ID;
            SqlDataReader rdr = m_cmdGetCellExists.ExecuteReader();//TODO: коряво, надо переделать бы на COUNT
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
            String query = String.Format("SELECT COUNT(id) AS Expr1 FROM CellTable WHERE (name = {0})", cellName);
            int result = CmdExecuteScalar(query);
            //if result = -1, должно быть выброшено исключение - так как запрос должен вернуть 0 или 1.
            if (result < 0) 
                throw new Exception("Invalid query results");
            //если result = 0, ячейка не найдена
            if (result > 0) return true;
            else return false;

            ////если способ не будет работать, вот старый код:
            ////create temp command
            //SqlCommand cmd = new SqlCommand("SELECT COUNT(id) AS Expr1 FROM CellTable WHERE (name = @name)", m_connection);
            //cmd.CommandTimeout = m_Timeout;
            //cmd.Parameters.Add("@name", SqlDbType.NVarChar);
            //cmd.Parameters[0].Value = cellName;
            ////get result
            //SqlDataReader rdr = cmd.ExecuteReader();
            //rdr.Read();
            //SqlInt32 res = rdr.GetSqlInt32(0);
            //rdr.Close();
            //if (res.IsNull) return false;
            //else return (bool)(res.Value != 0);

        }

        /// <summary>
        /// NT-Return number of rows in cell table
        /// </summary>
        /// <returns>
        /// Return number of rows in cell table.
        /// Возвращает -1 если запрос вернул null.
        /// </returns>
        public override int getNumberOfCells()
        {
            return this.getRowCount( MDbAdapterMsSql2005.CellTableName, "id");
            
            ////Если этот код не будет работать, вот старый:
            //SqlCommand cmd = new SqlCommand("SELECT COUNT(id) AS Expr1 FROM CellTable", m_connection);
            //cmd.CommandTimeout = m_Timeout;
            ////get result
            //SqlDataReader rdr = cmd.ExecuteReader();
            //rdr.Read();
            //SqlInt32 res = rdr.GetSqlInt32(0);
            //rdr.Close();
            //if (res.IsNull) return 0;
            //else return res.Value;
        }
        /// <summary>
        /// NT-Get max of cell id's in table, return 0 if no cells
        /// Возвращает -1 если запрос вернул null.
        /// </summary>
        /// <returns>Returns max of cell id's in table, return 0 if no cells</returns>
        public override int getMaxCellId()
        {
            String query = "SELECT MAX(cellid) AS Expr1 FROM CellTable";
            int result =  CmdExecuteScalar(query);
            if (result < 0) result = 0;
            return result;
            ////Если этот код не будет работать, вот старый:
            ////SELECT MAX(cellid) AS Expr1 FROM CellTable
            //if (Cmd_getMaxCellID == null) 
            //{
            //    Cmd_getMaxCellID = new SqlCommand("SELECT MAX(cellid) AS Expr1 FROM CellTable", m_connection);
            //    Cmd_getMaxCellID.CommandTimeout = m_Timeout;
            //}
            ////execute
            //SqlDataReader rdr = Cmd_getMaxCellID.ExecuteReader();
            //rdr.Read();
            //SqlInt32 res =  rdr.GetSqlInt32(0);
            //rdr.Close();
            //if (res.IsNull) return 0;
            //else return res.Value;
        }
        /// <summary>
        /// NT-Get min of cell id's in table, return 0 if no cells
        /// </summary>
        /// <returns>Returns min of cell id's in table, return 0 if no cells</returns>
        public override int getMinCellId()
        {
            String query = "SELECT MIN(cellid) AS Expr1 FROM CellTable";
            int result = CmdExecuteScalar(query);
            if (result < 0) result = 0;
            return result;

            ////Если этот код не будет работать, вот старый:
            ////create temp command
            //SqlCommand cmd = new SqlCommand("SELECT MIN(cellid) AS Expr1 FROM CellTable", m_connection);
            //cmd.CommandTimeout = m_Timeout;
            ////get result
            //SqlDataReader rdr = cmd.ExecuteReader();
            //rdr.Read();
            //SqlInt32 res = rdr.GetSqlInt32(0);
            //rdr.Close();
            //if (res.IsNull) return 0;
            //else return res.Value;
        }

        /// <summary>
        /// NT-Insert cell record to table
        /// </summary>
        /// <param name="cell">Cell object</param>
        public override int CellInsert(Mary.MCellB cell)
        {
            string query = "INSERT INTO CellTable (name, descr, active, type, creatime, moditime, ronly, state, sflag, val, valtype, cellid) VALUES (@name,@descr,@active,@type,@creat,@modit,@ronly,@state,@sflag,@val,@valtype,@cid)";
            if (m_cmdInsertCellTable == null)
            {
                m_cmdInsertCellTable = new SqlCommand(query, m_connection);
                m_cmdInsertCellTable.CommandTimeout = m_Timeout;
                m_cmdInsertCellTable.Parameters.Add("@name", SqlDbType.NVarChar);
                m_cmdInsertCellTable.Parameters.Add("@descr", SqlDbType.NText);
                m_cmdInsertCellTable.Parameters.Add("@active", SqlDbType.Bit);
                m_cmdInsertCellTable.Parameters.Add("@type", SqlDbType.Int);
                m_cmdInsertCellTable.Parameters.Add("@creat", SqlDbType.DateTime);
                m_cmdInsertCellTable.Parameters.Add("@modit", SqlDbType.DateTime);
                m_cmdInsertCellTable.Parameters.Add("@ronly", SqlDbType.Bit);
                m_cmdInsertCellTable.Parameters.Add("@state", SqlDbType.Int);
                m_cmdInsertCellTable.Parameters.Add("@sflag", SqlDbType.Int);
                m_cmdInsertCellTable.Parameters.Add("@val", SqlDbType.VarBinary);
                m_cmdInsertCellTable.Parameters.Add("@valtype", SqlDbType.Int);
                m_cmdInsertCellTable.Parameters.Add("@cid", SqlDbType.Int);
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
            //TODO: проверить, какой вариант кода быстрее работает

            //этот код будет дважды проводить поиск для ячейки, возможно, кэширование на стороне сервера ускорит работу.
            if (isCellExists(cell.CellID))
                CellUpdate(cell);
            else
                CellInsert(cell);

            //этот код будет медленным для новых ячеек
            ////try update record
            //if (CellUpdate(cell) < 1)
            //    //record not found, create new
            //    CellInsert(cell);
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
                m_cmdGetCell = new SqlCommand("SELECT CellTable.* FROM CellTable WHERE (cellid = @cid)", m_connection);
                m_cmdGetCell.CommandTimeout = m_Timeout;
                m_cmdGetCell.Parameters.Add("@cid", SqlDbType.Int);
            }
            //execute
            m_cmdGetCell.Parameters[0].Value = cellId.ID;
            SqlDataReader rdr = m_cmdGetCell.ExecuteReader();

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
                m_cmdGetCellByTitle = new SqlCommand("SELECT CellTable.* FROM CellTable WHERE (cellid = @title)", m_connection);
                m_cmdGetCellByTitle.CommandTimeout = m_Timeout;
                m_cmdGetCellByTitle.Parameters.Add("@title", SqlDbType.VarChar);
            }
            //execute
            m_cmdGetCellByTitle.Parameters[0].Value = title;
            SqlDataReader rdr = m_cmdGetCellByTitle.ExecuteReader();

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
        /// NT-Read cell row
        /// </summary>
        /// <param name="rdr">Command result set reader</param>
        /// <param name="cell">Cell object for out</param>
        private MCellB ReadLargeCellRow(SqlDataReader rdr)
        {
            //read one row from result
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
            byte[] cellValue = rdr.GetSqlBinary(10).Value;
            Int32 valueTypeId = rdr.GetInt32(11);
            Int32 cellID = rdr.GetInt32(12);
            MCellMode cellMode = MCellMode.Normal; //enable property saving
            //call constructor
            MCellB cell = new MCellB(cellMode, cellID, name, description, isActive, typeId, creaTime, modiTime, readOnly, state, serviceFlag, cellValue, valueTypeId);

            return cell;
        }


        /// <summary>
        /// NT-Update cell record 
        /// </summary>
        /// <param name="cell">Cell object</param>
        /// <returns>Returns number of affected rows</returns>
        public override int CellUpdate(Mary.MCellB cell)
        {
            //UPDATE CellTable SET name = @name, descr = @descr, active = @active, type = @type, creatime = @creat, moditime = @modit, ronly = @ronly, state = @state, sflag = @sflag, val = @val, valtype = @valtype WHERE (cellid = @cid)
            string query = "UPDATE CellTable SET name = @name, descr = @descr, active = @active, type = @type, creatime = @creat, moditime = @modit, ronly = @ronly, state = @state, sflag = @sflag, val = @val, valtype = @valtype WHERE (cellid = @cid)";
            if (m_cmdUpdateCellTable == null)
            {
                m_cmdUpdateCellTable = new SqlCommand(query, m_connection);
                m_cmdUpdateCellTable.CommandTimeout = m_Timeout;
                m_cmdUpdateCellTable.Parameters.Add("@name", SqlDbType.NVarChar);
                m_cmdUpdateCellTable.Parameters.Add("@descr", SqlDbType.NText);
                m_cmdUpdateCellTable.Parameters.Add("@active", SqlDbType.Bit);
                m_cmdUpdateCellTable.Parameters.Add("@type", SqlDbType.Int);
                m_cmdUpdateCellTable.Parameters.Add("@creat", SqlDbType.DateTime);
                m_cmdUpdateCellTable.Parameters.Add("@modit", SqlDbType.DateTime);
                m_cmdUpdateCellTable.Parameters.Add("@ronly", SqlDbType.Bit);
                m_cmdUpdateCellTable.Parameters.Add("@state", SqlDbType.Int);
                m_cmdUpdateCellTable.Parameters.Add("@sflag", SqlDbType.Int);
                m_cmdUpdateCellTable.Parameters.Add("@val", SqlDbType.VarBinary);
                m_cmdUpdateCellTable.Parameters.Add("@valtype", SqlDbType.Int);
                m_cmdUpdateCellTable.Parameters.Add("@cid", SqlDbType.Int);
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
        public override List<Mary.MCell> getBlockOfCells(int idFrom, int idTo)
        {
            if (m_cmdGetBlockOfCells == null)
            {
                m_cmdGetBlockOfCells = new SqlCommand("SELECT CellTable.* FROM CellTable WHERE ((cellid >= @idfrom) AND (cellid < @idto))", m_connection);
                m_cmdGetBlockOfCells.CommandTimeout = m_Timeout;
                m_cmdGetBlockOfCells.Parameters.Add("@idfrom", SqlDbType.Int);
                m_cmdGetBlockOfCells.Parameters.Add("@idto", SqlDbType.Int);
            }
            //execute
            m_cmdGetBlockOfCells.Parameters[0].Value = idFrom;
            m_cmdGetBlockOfCells.Parameters[1].Value = idTo;
            SqlDataReader rdr = m_cmdGetBlockOfCells.ExecuteReader();

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
                m_cmdGetCellLinks = new SqlCommand("", m_connection);
                m_cmdGetCellLinks.CommandTimeout = m_Timeout;
                m_cmdGetCellLinks.Parameters.Add("@cid", SqlDbType.Int);
            }
            m_cmdGetCellLinks.Parameters[0].Value = cellid;
            //set query string
            String query = "SELECT LinkTable.* FROM LinkTable WHERE ";
            String wherepart = "";
            switch (axisDir)
            {
                case MAxisDirection.Down:
                    wherepart ="(upID = @cid)";
                    break;
                case MAxisDirection.Up:
                    wherepart = "(downID = @cid)";
                    break;
                case MAxisDirection.Any:
                    wherepart = "((downID = @cid) OR (upID = @cid))";
                    break;
                default:
                    throw new Exception("Invalid axis direction");
            }
            m_cmdGetCellLinks.CommandText = query + wherepart;
            //execute command
            SqlDataReader rdr = m_cmdGetCellLinks.ExecuteReader();
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
            SqlParameter sp;

            //повторное использование команды не предполагается пока
            SqlCommand alias = new SqlCommand("", m_connection);
            //Создаем текст запроса и параметры
            sb.Append("SELECT CellTable.");
            if (largeCells) sb.Append("*"); else sb.Append("cellid");
            sb.Append(" FROM CellTable WHERE ");

            #region Creating query and parameters
            if (tmp.CellID != null)
            {
                sb.Append("(cellid = @p1)");
                sp = new SqlParameter("@p1", SqlDbType.Int);
                sp.Value = tmp.CellID.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.CreaTime.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(creatime = @p2)");
                sp = new SqlParameter("@p2", SqlDbType.DateTime);
                sp.Value = tmp.CreaTime.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ModiTime.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(moditime = @p3)");
                sp = new SqlParameter("@p3", SqlDbType.DateTime);
                sp.Value = tmp.ModiTime.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.State != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(state = @p4)");
                sp = new SqlParameter("@p4", SqlDbType.Int);
                sp.Value = tmp.State.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.TypeId != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(type = @p5)");
                sp = new SqlParameter("@p5", SqlDbType.Int);
                sp.Value = tmp.TypeId.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ValueTypeId != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(valtype = @p6)");
                sp = new SqlParameter("@p6", SqlDbType.Int);
                sp.Value = tmp.ValueTypeId.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ServiceFlag.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(sflag = @p7)");
                sp = new SqlParameter("@p7", SqlDbType.Int);
                sp.Value = tmp.ServiceFlag.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Name != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(name = @p8)");
                sp = new SqlParameter("@p8", SqlDbType.NVarChar);
                sp.Value = tmp.Name;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Description != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(descr LIKE @p9)");
                sp = new SqlParameter("@p9", SqlDbType.NText);
                sp.Value = tmp.Description;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.isActive.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(active = @p10)");
                sp = new SqlParameter("@p10", SqlDbType.Bit);
                sp.Value = tmp.isActive.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ReadOnly.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(ronly = @p11)");
                sp = new SqlParameter("@p11", SqlDbType.Bit);
                sp.Value = tmp.ReadOnly.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Value != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(val = @p12)");
                sp = new SqlParameter("@p12", SqlDbType.VarBinary);
                sp.Value = tmp.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            #endregion

            alias.CommandTimeout = m_Timeout; // *5 ?
            alias.CommandText = sb.ToString();
            //execute command
            SqlDataReader rdr = alias.ExecuteReader();
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

        #region *** Get cell properties functions ***
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override bool getCellActive(int cellid)
        {
            SqlDataReader sdr = this.getCellColumnDataReader("active", cellid);
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
            SqlDataReader sdr = this.getCellColumnDataReader("creatime", cellid);
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
            SqlDataReader sdr = this.getCellColumnDataReader("descr", cellid);
            string res = sdr.GetString(0);
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
            SqlDataReader sdr = this.getCellColumnDataReader("moditime", cellid);
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
            SqlDataReader sdr = this.getCellColumnDataReader("name", cellid);
            string res = sdr.GetString(0);
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
            SqlDataReader sdr = this.getCellColumnDataReader("ronly", cellid);
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
            SqlDataReader sdr = this.getCellColumnDataReader("sflag", cellid);
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
            SqlDataReader sdr = this.getCellColumnDataReader("state", cellid);
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
            SqlDataReader sdr = this.getCellColumnDataReader("type", cellid);
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
            SqlDataReader sdr = this.getCellColumnDataReader("val", cellid);
            SqlBinary res = sdr.GetSqlBinary(0);
            sdr.Close();
            return res.Value;
        }
        /// <summary>
        /// NT-Get cell property value 
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <returns></returns>
        public override Mary.MID getCellValueTypeId(int cellid)
        {
            SqlDataReader sdr = this.getCellColumnDataReader("valtype", cellid);
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
        private SqlDataReader getCellColumnDataReader(string columnName, int cellid)
        {
            string query = String.Format("SELECT CellTable.{0} FROM CellTable WHERE (cellid = @cid)", columnName);
            //create command
            if (m_cmdGetCellData == null)
            {
                m_cmdGetCellData = new SqlCommand(query, m_connection);
                m_cmdGetCellData.CommandTimeout = m_Timeout;
                m_cmdGetCellData.Parameters.Add("@cid", SqlDbType.Int);
            }
            //execute command
            m_cmdGetCellData.CommandText = query;
            m_cmdGetCellData.Parameters[0].Value = cellid;
            SqlDataReader rdr = m_cmdGetCellData.ExecuteReader();
            if (rdr.HasRows == false)
            {
                rdr.Close();
                throw new Exception("Cell record not found in table");
            }
            else
            {
                rdr.Read();
                //rdr.Close(); - вынесено в вызывающий код
                return rdr;
            }
        }

 #endregion
        #region *** Set cell properties functions***
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellActive(int cellid, bool val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@active", SqlDbType.Bit);
            sp.Value = val;
            //write parameter
            this.setCellColumnData("active", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellCreationTime(int cellid, System.DateTime val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@creatime", SqlDbType.DateTime);
            sp.Value = val;
            //write parameter
            this.setCellColumnData("creatime", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellDescription(int cellid, string val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@descr", SqlDbType.NText);
            sp.Value = val;
            //write parameter
            this.setCellColumnData("descr", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellModificationTime(int cellid, System.DateTime val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@moditime", SqlDbType.DateTime);
            sp.Value = val;
            //write parameter
            this.setCellColumnData("moditime", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellName(int cellid, string val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@name", SqlDbType.NVarChar);
            sp.Value = val;
            //write parameter
            this.setCellColumnData("name", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellReadOnly(int cellid, bool val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@ronly", SqlDbType.Bit);
            sp.Value = val;
            //write parameter
            this.setCellColumnData("ronly", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellServiceFlag(int cellid, int val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@sflag", SqlDbType.Int);
            sp.Value = val;
            //write parameter
            this.setCellColumnData("sflag", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellState(int cellid, Mary.MID val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@state", SqlDbType.Int);
            sp.Value = val.ID;
            //write parameter
            this.setCellColumnData("state", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellTypeId(int cellid, Mary.MID val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@type", SqlDbType.Int);
            sp.Value = val.ID;
            //write parameter
            this.setCellColumnData("type", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellValue(int cellid, byte[] val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@val", SqlDbType.VarBinary);
            sp.Value = val;
            //write parameter
            this.setCellColumnData("val", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }
        /// <summary>
        /// NT-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public override void setCellValueTypeId(int cellid, Mary.MID val)
        {
            //create parameter
            SqlParameter sp = new SqlParameter("@valtype", SqlDbType.Int);
            sp.Value = val.ID;
            //write parameter
            this.setCellColumnData("valtype", sp, cellid);
            //if returns 0, cell not exists. throw exception?
        }

        /// <summary>
        /// NT-Update column in cell record
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="param">Sql parameter with filled sqldbtype and value fields. </param>
        /// <param name="cellid">midified cell id</param>
        /// <returns>Number of affected rows</returns>
        private int setCellColumnData(string columnName, SqlParameter param, int cellid)
        {
            //UPDATE CellTable SET name = @Param2, moditime = @Param3 WHERE (cellid = @Param1)
            string query = String.Format("UPDATE CellTable SET {0} = @{0}, moditime = @mody WHERE (cellid = @cid)", columnName);
            string paramName = String.Format("@{0}", columnName);
            param.ParameterName = paramName;//для страховки

            //create command
            if (m_cmdSetCellData == null)
            {
                m_cmdSetCellData = new SqlCommand(query, m_connection);
                m_cmdSetCellData.CommandTimeout = m_Timeout;
            }
            //execute command
            m_cmdSetCellData.CommandText = query;
            m_cmdSetCellData.Parameters.Clear();
            m_cmdSetCellData.Parameters.Add(param);
            m_cmdSetCellData.Parameters.Add("@mody", SqlDbType.DateTime);
            m_cmdSetCellData.Parameters.Add("@cid", SqlDbType.Int);
            m_cmdSetCellData.Parameters[1].Value = DateTime.Now;
            m_cmdSetCellData.Parameters[2].Value = cellid;
            return m_cmdSetCellData.ExecuteNonQuery();
        }
        #endregion



 #endregion


#region *** Функции связей ***

        ///// <summary>
        ///// NT-Return last LinkTable identity (primary key value)
        ///// </summary>
        ///// <returns>Returns last LinkTable identity (primary key value)</returns>
        //protected override int getLastIdentityLinksTable()
        //{
        //    return this.getTableIdentity(MDbAdapterMsSql2005.LinkTableName);
        //}

        /// <summary>
        /// NT-Delete link by link table id
        /// </summary>
        /// <param name="linkId">link id from table primary key</param>
        /// <returns>Returns number of affected rows</returns>
        public override int LinkDelete(int linkId)
        {
            if (m_cmdDeleteLinkById == null)
            {
                m_cmdDeleteLinkById = new SqlCommand("DELETE FROM LinkTable WHERE (id = @lid)", m_connection);
                m_cmdDeleteLinkById.CommandTimeout = m_Timeout;
                m_cmdDeleteLinkById.Parameters.Add("@lid", SqlDbType.Int);
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
            LinkInsert(link);
            //вернуть ид связи 
            int id = this.getTableIdentity(MDbAdapterMsSql2005.LinkTableName);
            link.TableId = id;

            return id;//а он не требуется нигде
        }

        /// <summary>
        /// NT-Insert Link record into LinkTable
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Returns number of affected rows</returns>
        public override int LinkInsert(Mary.MLink link)
        {
            string query = "INSERT INTO LinkTable (downID, upID, axis, state, active, sflag, descr, moditime) VALUES  (@down,@up,@axis,@state,@active,@sflag,@descr,@modit)";
            if (m_cmdInsertLinkTable == null)
            {
                m_cmdInsertLinkTable = new SqlCommand(query, m_connection);
                m_cmdInsertLinkTable.CommandTimeout = m_Timeout;
                m_cmdInsertLinkTable.Parameters.Add("@down", SqlDbType.Int);
                m_cmdInsertLinkTable.Parameters.Add("@up", SqlDbType.Int);
                m_cmdInsertLinkTable.Parameters.Add("@axis", SqlDbType.Int);
                m_cmdInsertLinkTable.Parameters.Add("@state", SqlDbType.Int);
                m_cmdInsertLinkTable.Parameters.Add("@active", SqlDbType.Bit);
                m_cmdInsertLinkTable.Parameters.Add("@sflag", SqlDbType.Int);
                m_cmdInsertLinkTable.Parameters.Add("@descr", SqlDbType.NText);
                m_cmdInsertLinkTable.Parameters.Add("@modit", SqlDbType.DateTime);
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
            string query = "UPDATE LinkTable SET downID = @down, upID = @up, axis = @axis, state = @state, active = @active, sflag = @sflag, descr = @descr, moditime = @modit WHERE (id = @lid)";
            if (m_cmdUpdateLinkTable == null)
            {
                m_cmdUpdateLinkTable = new SqlCommand(query, m_connection);
                m_cmdUpdateLinkTable.CommandTimeout = m_Timeout;
                m_cmdUpdateLinkTable.Parameters.Add("@down", SqlDbType.Int);
                m_cmdUpdateLinkTable.Parameters.Add("@up", SqlDbType.Int);
                m_cmdUpdateLinkTable.Parameters.Add("@axis", SqlDbType.Int);
                m_cmdUpdateLinkTable.Parameters.Add("@state", SqlDbType.Int);
                m_cmdUpdateLinkTable.Parameters.Add("@active", SqlDbType.Bit);
                m_cmdUpdateLinkTable.Parameters.Add("@sflag", SqlDbType.Int);
                m_cmdUpdateLinkTable.Parameters.Add("@descr", SqlDbType.NText);
                m_cmdUpdateLinkTable.Parameters.Add("@modit", SqlDbType.DateTime);
                m_cmdUpdateLinkTable.Parameters.Add("@lid", SqlDbType.Int);
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
        /// <param name="idFrom">link id from which begin select links</param>
        /// <param name="idTo">link id to (but not include) end select links</param>
        /// <returns>Returns collection of links</returns>
        public override Mary.MLinkCollection getBlockOfLinks(int idFrom, int idTo)
        {
            if (m_cmdGetBlockOfLinks == null)
            {
                m_cmdGetBlockOfLinks = new SqlCommand("SELECT LinkTable.* FROM LinkTable WHERE ((id >= @idfrom) AND (id < @idto))", m_connection);
                m_cmdGetBlockOfLinks.CommandTimeout = m_Timeout;
                m_cmdGetBlockOfLinks.Parameters.Add("@idfrom", SqlDbType.Int);
                m_cmdGetBlockOfLinks.Parameters.Add("@idto", SqlDbType.Int);
            }
            //execute
            m_cmdGetBlockOfLinks.Parameters[0].Value = idFrom;
            m_cmdGetBlockOfLinks.Parameters[1].Value = idTo;
            SqlDataReader rdr = m_cmdGetBlockOfLinks.ExecuteReader();

            return readLinkResultSet(rdr);
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
                m_cmdGetLinkId = new SqlCommand("SELECT LinkTable.id FROM LinkTable WHERE (downID = @cidn) AND (upID = @cidu) AND (axis = @axis)", m_connection);
                m_cmdGetLinkId.CommandTimeout = m_Timeout;
                m_cmdGetLinkId.Parameters.Add("@cidn", SqlDbType.Int);
                m_cmdGetLinkId.Parameters.Add("@cidu", SqlDbType.Int);
                m_cmdGetLinkId.Parameters.Add("@axis", SqlDbType.Int);
            }
            m_cmdGetLinkId.Parameters[0].Value = dnCellId.ID;
            m_cmdGetLinkId.Parameters[1].Value = upCellId.ID;
            m_cmdGetLinkId.Parameters[2].Value = axis.ID;
            SqlDataReader rdr = m_cmdGetLinkId.ExecuteReader();
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
            SqlParameter sp;

            //повторное использование команды не предполагается пока
            SqlCommand alias = new SqlCommand("", m_connection);
            //Создаем текст запроса и параметры
            sb.Append("SELECT LinkTable.* FROM LinkTable WHERE ");

            #region Creating query and parameters
            if (tmp.tableId.HasValue)
            {
                sb.Append("(id = @p1)");
                sp = new SqlParameter("@p1", SqlDbType.Int);
                sp.Value = tmp.tableId.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.downCellID != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(downID = @p2)");
                sp = new SqlParameter("@p2", SqlDbType.Int);
                sp.Value = tmp.downCellID.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.upCellID != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(upID = @p3)");
                sp = new SqlParameter("@p3", SqlDbType.Int);
                sp.Value = tmp.upCellID.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Axis != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(axis = @p4)");
                sp = new SqlParameter("@p4", SqlDbType.Int);
                sp.Value = tmp.Axis.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.State != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(state = @p5)");
                sp = new SqlParameter("@p5", SqlDbType.Int);
                sp.Value = tmp.State.ID;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.isActive.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(active = @p6)");
                sp = new SqlParameter("@p6", SqlDbType.Bit);
                sp.Value = tmp.isActive.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.ServiceFlag.HasValue)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(sflag = @p7)");
                sp = new SqlParameter("@p7", SqlDbType.Int);
                sp.Value = tmp.ServiceFlag.Value;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            if (tmp.Description != null)
            {
                if (AndFlag == true) sb.Append(" AND ");
                sb.Append("(descr LIKE @p8)");
                sp = new SqlParameter("@p8", SqlDbType.NText);
                sp.Value = tmp.Description;
                alias.Parameters.Add(sp);
                AndFlag = true;
            }
            #endregion

            alias.CommandTimeout = m_Timeout; // *5 ?
            alias.CommandText = sb.ToString();
            //execute command
            SqlDataReader rdr = alias.ExecuteReader();

            return readLinkResultSet(rdr);
        }

        /// <summary>
        /// NT-Get max of link primary key in table. Return 0 if no links.
        /// </summary>
        /// <returns>Returns max of link primary key in table. Return 0 if no links.</returns>
        public override int getMaxLinkId()
        {
            string query = "SELECT MAX(id) AS Expr1 FROM LinkTable";
            return CmdExecuteScalar(query);
            ////Если этот код не будет работать, вот старый:
            //SqlCommand cmd = new SqlCommand("SELECT MAX(id) AS Expr1 FROM LinkTable", m_connection);
            //cmd.CommandTimeout = m_Timeout;
            ////get result
            //SqlDataReader rdr = cmd.ExecuteReader();
            //rdr.Read();
            //SqlInt32 res = rdr.GetSqlInt32(0);
            //rdr.Close();
            //if (res.IsNull) return 0;
            //else return res.Value;
        }
        /// <summary>
        /// NT-Get min of link primary key in table. Return 0 if no links.
        /// </summary>
        /// <returns>Returns min of link primary key in table. Return 0 if no links.</returns>
        public override int getMinLinkId()
        {
            string query = "SELECT MIN(id) AS Expr1 FROM LinkTable";
            return CmdExecuteScalar(query);

            ////Если этот код не будет работать, вот старый:
            //SqlCommand cmd = new SqlCommand("SELECT MIN(id) AS Expr1 FROM LinkTable", m_connection);
            //cmd.CommandTimeout = m_Timeout;
            ////get result
            //SqlDataReader rdr = cmd.ExecuteReader();
            //rdr.Read();
            //SqlInt32 res = rdr.GetSqlInt32(0);
            //rdr.Close();
            //if (res.IsNull) return 0;
            //else return res.Value;
        }
        /// <summary>
        /// NT-Return number of rows in link table
        /// </summary>
        /// <returns>Return number of rows in link table</returns>
        public override int getNumberOfLinks()
        {
            return this.getRowCount(MDbAdapterMsSql2005.LinkTableName, "id");

            ////Если этот код не будет работать, вот старый:
            //SqlCommand cmd = new SqlCommand("SELECT COUNT(id) AS Expr1 FROM LinkTable", m_connection);
            //cmd.CommandTimeout = m_Timeout;
            ////get result
            //SqlDataReader rdr = cmd.ExecuteReader();
            //rdr.Read();
            //SqlInt32 res = rdr.GetSqlInt32(0);
            //rdr.Close();
            //if (res.IsNull) return 0;
            //else return res.Value;
        }

        /// <summary>
        /// NT-Read all rows from sql result set, close reader.
        /// </summary>
        /// <param name="rdr"></param>
        /// <returns></returns>
        private MLinkCollection readLinkResultSet(SqlDataReader rdr)
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

 #endregion













    }
}
