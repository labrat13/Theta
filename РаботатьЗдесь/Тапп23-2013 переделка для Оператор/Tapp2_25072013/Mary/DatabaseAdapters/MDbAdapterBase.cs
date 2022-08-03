using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace Mary.DatabaseAdapters
{

    /// <summary>
    /// Represent adapter layer for database
    /// </summary>
    public class MDbAdapterBase
    {
        ///// <summary>
        ///// Имя файла базы данных для файловых СУБД
        ///// </summary>
        //public const string DatabaseFileName = "db";
        /// <summary>
        /// Название таблицы документов
        /// </summary>
        protected const string ContainerTableName = "EngineTable";
        /// <summary>
        /// Название таблицы изображений
        /// </summary>
        protected const string CellTableName = "CellTable";
        /// <summary>
        /// Название таблицы сущностей
        /// </summary>
        protected const string LinkTableName = "LinkTable";
        ///// <summary>
        ///// Название таблицы описания - если есть
        ///// </summary>
        //protected const string AboutTableName = "about";

        #region ***** Class member's *****
        /// <summary>
        /// Ссылка на контейнер
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        protected MEngine m_container;
        /// <summary>
        /// Connection string
        /// </summary>
        protected string m_connectionString;
        /// <summary>
        /// Timeout value for DB command, in seconds
        /// </summary>
        protected int m_Timeout;


        //TODO: создать правильные переменные здесь
        /// <summary>
        /// DB connection object - universal type
        /// </summary>
        protected IDbConnection m_connection;
        /// <summary>
        /// Transaction for current connection - universal type
        /// </summary>
        protected IDbTransaction m_transaction;

        //все объекты команд сбрасываются в нуль при отключении соединения с БД
        //TODO: Новые команды внести в ClearCommands()

        ///// <summary>
        ///// SQLCommand for tests
        ///// </summary>
        //private SqlCommand Cmd_Test;
        ///// <summary>
        ///// SQLCommand for tests
        ///// </summary>
        //private SqlCommand Cmd_TestR;

        //Add more commands here...



        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="engine">Ссылка на контейнер</param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public MDbAdapterBase(MEngine engine)
        {
            m_container = engine;
            m_Timeout = 120; //2 minutes
        }


        /// <summary>
        /// Close and dispose connection
        /// </summary>
        ~MDbAdapterBase()
        {
            this.Close();
        }

        #region  ***** Properties *****
        /// <summary>
        /// Получить тип адаптера БД
        /// //TAG:RENEW-13112017
        /// </summary>
        public virtual MDatabaseType DatabaseType
        {
            get { return MDatabaseType.Unknown; } //TODO: переопределить значение в производных классах
        }

        /// <summary>
        /// Get or Set connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return m_connectionString;
            }
            set
            {
                m_connectionString = value;
                //this.m_connection = new SqlConnection(m_ConnString);
            }
        }

        /// <summary>
        /// Get Set timeout value for all  new execute command
        /// </summary>
        public int Timeout
        {
            get
            {
                return m_Timeout;
            }
            set
            {
                m_Timeout = value;
            }
        }
        /// <summary>
        /// Is connection opened?
        /// </summary>
        public virtual bool isConnectionOpen
        {
            get
            {
                return ((this.m_connection != null) && (this.m_connection.State == ConnectionState.Open));
            }
        }

        #endregion

        /// <summary>
        /// NT-все объекты команд сбросить в нуль
        /// </summary>
        protected virtual void ClearCommands()
        {
            //m_cmdWithoutArguments = null;
            //m_cmdUpdateUsings = null;
            //m_cmdGetDublicates = null;
            //m_cmdInsertFileRecord = null;
            //m_cmdInsertEntityRecord = null;
            //m_cmdGetEntityByName = null;
            //m_cmdGetEntityByLike = null;
            return;
        }

        /// <summary>
        /// NR-SolutionOpen manager
        /// </summary>
        /// <param name="info"></param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public virtual void Open(MSolutionInfo info)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Close manager
        /// </summary>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public virtual void Close()
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NT-Get string representation of object.
        /// </summary>
        /// <returns>Return string representation of object.</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public override string ToString()
        {
            return base.ToString();//TODO: Add code here...
        }

        #region ***Database management functions***
        /// <summary>
        /// NT-Открыть соединение с БД по текущей строке соединения
        /// </summary>
        /// <remarks>
        /// Эта пара функций позволяет закрыть и заново открыть соединение, если оно оказалось в неправильном состоянии.
        /// </remarks>
        public virtual void Connect()
        {
            throw new NotImplementedException();//TODO: add code here
        }


        /// <summary>
        /// NT-Закрыть соединение с БД
        /// </summary>
        /// <remarks>
        /// Эта пара функций позволяет закрыть и заново открыть соединение, если оно оказалось в неправильном состоянии.
        /// </remarks>
        public virtual void Disconnect()
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NT-Начать транзакцию. 
        /// </summary>
        public virtual void TransactionBegin()
        {
            m_transaction = m_connection.BeginTransaction();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands();
        }
        /// <summary>
        /// NT-Подтвердить транзакцию Нужно закрыть соединение после этого!
        /// </summary>
        public virtual void TransactionCommit()
        {
            m_transaction.Commit();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands(); 
            m_transaction = null;

        }
        /// <summary>
        /// NT-Отменить транзакцию. Нужно закрыть соединение после этого!
        /// </summary>
        public virtual void TransactionRollback()
        {
            m_transaction.Rollback();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands();
            m_transaction = null;
        }


        /// <summary>
        /// NR-Create connection string
        /// </summary>
        /// <param name="DatabaseServerPath">Путь к серверу БД или файлу БД.</param>
        /// <param name="DatabaseName">Имя БД</param>
        /// <param name="dbPort">Порт сервера. Если аргумент имеет значение 0, использовать номер порта, установленный по умолчанию.</param>
        /// <param name="UserPassword">Пароль пользователя. Пустая строка допустима как отсутствие пароля.</param>
        /// <param name="UserName">Имя пользователя. Если вместо имени пользователя указана пустая строка, необходимо запросить имя и пароль пользователя через приложение.</param>
        /// <param name="Timeout"></param>
        /// <param name="IntegratedSecurity">
        /// Использовать встроенную проверку подлинности для MsSqlServer. 
        /// True - Windows autentification. 
        /// False - SQL Server autentification.
        /// </param>
        /// <returns>Возвращает строку соединения с выбранной БД</returns>
        /// <remarks>Без лога, или проверять его существование!</remarks>
        public virtual string createConnectionString(string DatabaseServerPath, string DatabaseName, int dbPort, int Timeout, string UserName, string UserPassword, bool IntegratedSecurity)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Create connection string
        /// </summary>
        /// <param name="pfile">Объект ФайлСолюшена</param>
        /// <returns>Возвращает строку соединения с выбранной БД</returns>
        /// <remarks>Без лога, или проверять его существование!</remarks>
        public virtual string createConnectionString(MSolutionInfo pfile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// NT-Remove all from celltable and linktable
        /// </summary>
        public virtual void ClearCellTableAndLinkTable()
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Создать БД проекта.
        /// создать, открыть, записать и закрыть БД 
        /// </summary>
        public virtual void DatabaseCreate(MSolutionInfo info)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Create tables and indexes on existing database
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        public virtual void CreateTablesIndexes(string connectionString)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Удалить существующую БД вместе со всем содержимым.
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        public virtual void DatabaseDelete(MSolutionInfo info)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NT-Извлечь файл шаблона базы данных из ресурсов сборки
        /// </summary>
        /// <param name="filepath">Путь к итоговому файлу</param>
        /// <param name="resource">Файл БД из ресурсов текущей сборки</param>
        protected static void extractDbFile(string filepath, Byte[] resource)
        {
            FileStream fs = new FileStream(filepath, FileMode.Create);
            fs.Write(resource, 0, resource.Length);
            fs.Close();

            return;
        }

        /// <summary>
        /// NT-Создать объект класса БД в зависимости от типа БД в проекте
        /// </summary>
        /// <param name="engine">Контейнер для проекта</param>
        /// <param name="databaseType">Тип БД для создания</param>
        /// <returns>Возвращает созданный объект класса, производного от MDbLayer и соответствующего типу БД проекта.</returns>
        /// <remarks>
        /// TODO: Эту функцию не переопределять в производных классах!
        /// </remarks>
        /// <seealso cref=""/>
        public static MDbAdapterBase DbSelector(MEngine engine, MDatabaseType databaseType)
        {
            switch (databaseType)
            {
                case MDatabaseType.NoDatabase:
                    return new MDbAdapterNoDb(engine);
                //break;
                case MDatabaseType.MySql5:
                    throw new Exception("Ошибка - неподдерживаемый тип БД: " + databaseType.ToString());
                //break;
                case MDatabaseType.MicrosoftSqlServer2005:
                    return new MDbAdapterMsSql2005(engine);
                //break;
                case MDatabaseType.MsAccess:
                    return new MDbAdapterMsJet(engine);
                case MDatabaseType.Sqlite3:
                    return new MDbAdapterSqlite3(engine); 
                    //throw new Exception("Ошибка - неподдерживаемый тип БД: " + databaseType.ToString());
                default:
                    throw new Exception("Ошибка - неподдерживаемый тип БД: " + databaseType.ToString());
                //break;
            }
        }

        #endregion

        #region *** Для всех таблиц ***
        /// <summary>
        /// NR-Return last table identity (primary key value)
        /// </summary>
        /// <param name="table">Table name</param>
        /// <returns>Returns last table identity (primary key value)</returns>
        protected virtual int getTableIdentity(string table)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NT- get max of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>Value</returns>
        protected virtual int getTableMaxId(string table, string column)
        {
            //SELECT MAX(id) FROM table;
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-get min of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>Value</returns>
        protected virtual int getTableMinId(string table, string column)
        {
            //SELECT MIN(id) FROM table;
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NT-Получить число записей в таблице
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <returns></returns>
        protected virtual int getRowCount(string table, string column)
        {
            //SELECT COUNT(id) FROM table;
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Очистить БД
        /// </summary>
        public virtual void DatabaseClear()
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// RT-Удалить все строки из указанной таблицы
        /// </summary>
        /// <param name="table">Название таблицы</param>
        protected virtual void ClearTable(string table)
        {
            //DELETE FROM table;
            throw new NotImplementedException();//TODO: add code here
        }


        #endregion

        #region Engine table functions

        /// <summary>
        /// Load first existing container from database. Throw exception if container not found 
        /// </summary>
        /// <param name="container">Container object for loading</param>
        /// <exception cref="SqlException">locked row</exception>
        /// <exception cref="InvalidOperationException">connection is closed</exception>
        public virtual void ContainerLoad(MEngine cont)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// Save container in database. Create new record if any not exists
        /// </summary>
        /// <param name="container"></param>
        /// <remarks>Без лога, или проверять его существование!</remarks>
        public virtual void ContainerSave(MEngine container)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        #endregion

        #region Cell table functions

        /// <summary>
        /// NR-Проверить, что ячейка с таким идентификатором существует
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <returns>Returns true if cell exists, false otherwise</returns>
        public virtual bool isCellExists(MID cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NT-Get cell by id
        /// </summary>
        /// <param name="cellId">Cell identificator</param>
        /// <param name="largeCell">Cell mode: False for MCellA, true for MCellB</param>
        /// <returns>MCell object or null if cell not exists</returns>
        public virtual MCell CellSelect(MID cellId, bool largeCell)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NT-Get cell by title
        /// //TAG:RENEW-13112017
        /// </summary>
        /// <param name="title">Cell title string</param>
        /// <param name="largeCell">Cell mode: False for MCellA, true for MCellB</param>
        /// <returns>MCell object or null if cell not exists</returns>
        public virtual MCell CellSelect(string title, bool largeCell)
        {
            throw new NotImplementedException();//TODO: add code here
        }


        /// <summary>
        /// NT-Get max of cell id's in table, return 0 if no cells
        /// </summary>
        /// <returns></returns>
        public virtual int getMaxCellId()
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NFT-Get min of cell id's in table,  return 0 if no cells
        /// </summary>
        /// <returns></returns>
        public virtual int getMinCellId()
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Get band of cells
        /// </summary>
        /// <param name="rowFrom">cell id from which begin select cells</param>
        /// <param name="rowTo">cell id to (but not include) end select cells</param>
        /// <returns>Returns list of cells</returns>
        public virtual List<MCell> getBlockOfCells(int rowFrom, int rowTo)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// insert cell record in table
        /// </summary>
        /// <param name="cell"></param>
        public virtual int CellInsert(MCellB cell)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Update cell record 
        /// </summary>
        /// <param name="cell">Cell object</param>
        /// <returns>Returns number of affected rows</returns>
        public virtual int CellUpdate(MCellB cell)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// Save cell data - update or insert row
        /// </summary>
        /// <param name="cell"></param>
        public virtual void CellSave(MCellB cell)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        #region get cell functions
        /// <summary>
        /// NT-Get cell description
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual string getCellDescription(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual string getCellName(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual bool getCellActive(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual int getCellServiceFlag(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual MID getCellState(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual MID getCellTypeId(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual DateTime getCellCreationTime(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual DateTime getCellModificationTime(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual bool getCellReadOnly(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual byte[] getCellValue(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <returns></returns>
        public virtual MID getCellValueTypeId(int cellid)
        {
            throw new NotImplementedException();//TODO: add code here
        }

#endregion


        #region set cell functions
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <param name="val"></param>
        public virtual void setCellDescription(int cellid, string val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="cellid"></param>
        /// <param name="val"></param>
        public virtual void setCellName(int cellid, string val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public virtual void setCellActive(int cellid, bool val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public virtual void setCellServiceFlag(int cellid, int val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public virtual void setCellState(int cellid, MID val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public virtual void setCellTypeId(int cellid, MID val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public virtual void setCellCreationTime(int cellid, DateTime val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public virtual void setCellReadOnly(int cellid, bool val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public virtual void setCellValue(int cellid, byte[] val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public virtual void setCellValueTypeId(int cellid, MID val)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Set cell property value
        /// </summary>
        /// <param name="cellid">Cell identifier</param>
        /// <param name="val">New property value</param>
        public virtual void setCellModificationTime(int cellid, DateTime val)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        #endregion



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
        public virtual MCellCollection getCellsByTemplate(MCellTemplate tmp, bool largeCells)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NT-Return True if cell table contains cell with specified name 
        /// </summary>
        /// <param name="cellName">Cell name</param>
        /// <returns>Returns true if cell exists, false otherwise</returns>
        public virtual bool isCellExists(string cellName)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        #endregion


        #region Link table functions
        /// <summary>
        /// NT-Получить список связей, в соответствии с шаблоном поиска
        /// </summary>
        /// <param name="template">Шаблон поиска</param>
        /// <returns></returns>
        public virtual MLinkCollection getLinks(MLinkTemplate tmp)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Get cell links filtered by axis direction
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <param name="axisDir">axis direction: Any, Up, Down</param>
        /// <returns>Returns collection of links filtered by axis direction</returns>
        public virtual MLinkCollection getCellLinks(int cellid, MAxisDirection axisDir)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        
        /// <summary>
        /// NR-Insert Link record into LinkTable
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Returns number of affected rows</returns>
        public virtual int LinkInsert(MLink link)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Insert link to table and change and return link primary key, that serve as linkId.
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Возвращает идентификатор связи</returns>
        public virtual int LinkInsertGetId(MLink link)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Delete link by link table id
        /// </summary>
        /// <param name="linkId">link id from table primary key</param>
        /// <returns>Returns number of affected rows</returns>
        public virtual int LinkDelete(int linkId)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-get link id primary key for first founded link. Return 0 if link not exists
        /// TODO: неправильно работает при наличии нескольких связей между ячейками
        /// </summary>
        /// <param name="dnCellId">Идентификатор подчиненной ячейки</param>
        /// <param name="upCellId">Идентификатор главной ячейки</param>
        /// <param name="axis">Идентификатор типа связи</param>
        /// <returns>Возвращает первичный ключ записи связи в таблице</returns>
        public virtual int getLinkID(MID dnCellId, MID upCellId, MID axis)
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-get link id primary key for first founded link. Return 0 if link not exists
        /// TODO: неправильно работает при наличии нескольких связей между ячейками
        /// </summary>
        /// <param name="link">Объект связи</param>
        /// <returns>Возвращает первичный ключ записи связи в таблице</returns>
        public virtual int getLinkID(MLink link)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NT-Return last LinkTable identity (primary key value)
        /// </summary>
        /// <returns>Returns last LinkTable identity (primary key value)</returns>
        protected virtual int getLastIdentityLinksTable()
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Update link. Return num of affected rows
        /// </summary>
        /// <param name="link">Link object</param>
        /// <returns>Returns number of affected rows</returns>
        public virtual int LinkUpdate(MLink link)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NT-Get min of link primary key in table. Return 0 if no links.
        /// </summary>
        /// <returns></returns>
        public virtual int getMinLinkId()
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Get max of link primary key in table. Return 0 if no links.
        /// </summary>
        /// <returns>Returns max of link primary key in table. Return 0 if no links.</returns>
        public virtual int getMaxLinkId()
        {
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Get band of links
        /// </summary>
        /// <param name="rowFrom">link id from which begin select links</param>
        /// <param name="rowTo">link id to (but not include) end select links</param>
        /// <returns>Returns collection of links</returns>
        public virtual MLinkCollection getBlockOfLinks(int rowFrom, int rowTo)
        {
            throw new NotImplementedException();//TODO: add code here
        }

        #endregion


        /// <summary>
        /// NR-Return number of rows in cell table
        /// </summary>
        /// <returns></returns>
        public virtual int getNumberOfCells()
        {
            throw new NotImplementedException();//TODO: add code here
        }
        /// <summary>
        /// NR-Return number of rows in link table
        /// </summary>
        /// <returns>Return number of rows in link table</returns>
        public virtual int getNumberOfLinks()
        {
            throw new NotImplementedException();//TODO: add code here
        }














    }
}
