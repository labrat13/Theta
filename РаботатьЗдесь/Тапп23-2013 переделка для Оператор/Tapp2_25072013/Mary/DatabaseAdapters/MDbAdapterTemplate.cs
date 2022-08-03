using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Mary.DatabaseAdapters
{
    
    /// <summary>
    /// Это шаблон, отсюда копировать заголовки функций с описаниями и организацией
    /// </summary>
    class MDbAdapterTemplate: MDbAdapterBase
    {
        //некоторые поля и проперти остались в базовом классе.
#region *** Fields ***

        //все объекты команд должны сбрасываться в нуль при отключении соединения с БД
        //TODO: Новые команды внести в ClearCommands()

 #endregion

        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public MDbAdapterTemplate(MEngine engine): base(engine)
        {
            
            //add some inits here
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~MDbAdapterTemplate()
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
                return MDatabaseType.Unknown; //TODO: переопределить значение в конкретных классах
            }
        }

        /// <summary>
        /// Is connection opened?
        /// </summary>
        public override bool isConnectionOpen
        {
            get
            {
                return ((this.m_connection != null) && (this.m_connection.State == ConnectionState.Open));
            }
        }
 #endregion



#region *** Функции сеанса и транзакции***
        /// <summary>
        /// NR-все объекты команд сбросить в нуль
        /// </summary>
        protected override void ClearCommands()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get string representation of object.
        /// </summary>
        /// <returns>Return string representation of object.</returns>
        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-SolutionOpen manager
        /// </summary>
        /// <param name="info">Объект настроек Солюшена</param>
        public override void Open(Mary.MSolutionInfo info)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Close manager
        /// </summary>
        public override void Close()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Открыть соединение с БД по текущей строке соединения
        /// </summary>
        /// <remarks>
        /// Эта пара функций позволяет закрыть и заново открыть соединение, если оно оказалось в неправильном состоянии.
        /// </remarks>
        public override void Connect()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NR-Закрыть соединение с БД
        /// </summary>
        /// <remarks>
        /// Эта пара функций позволяет закрыть и заново открыть соединение, если оно оказалось в неправильном состоянии.
        /// </remarks>
        public override void Disconnect()
        {
            throw new System.NotImplementedException();
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
        /// NR-Create connection string
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
            throw new NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Create connection string
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
        /// NR-Создать БД проекта.
        /// создать, открыть, записать и закрыть БД 
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        public override void DatabaseCreate(MSolutionInfo info)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Удалить существующую БД вместе со всем содержимым.
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        public override void DatabaseDelete(MSolutionInfo info)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Очистить БД
        /// </summary>
        public override void DatabaseClear()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Create tables and indexes on existing database
        /// </summary>
        /// <param name="connectionString">Строка соединения с создаваемой БД.</param>
        public override void CreateTablesIndexes(string connectionString)
        {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// NR-Remove all from celltable and linktable
        /// Это удаляет вся ячейки и связи Солюшена, не затрагивая данные Контейнера.
        /// </summary>
        public override void ClearCellTableAndLinkTable()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Удалить все строки из указанной таблицы
        /// </summary>
        /// <param name="table">Название таблицы</param>
        protected override void ClearTable(string table)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// NR-Return last table identity (primary key value)
        /// </summary>
        /// <param name="table">Table name</param>
        /// <returns>Returns last table identity (primary key value)</returns>
        protected override int getTableIdentity(string table)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Получить число записей в таблице
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <returns>Возвращает число записей в таблице</returns>
        protected override int getRowCount(string table, string column)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get max of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>Returns value of max of table id's</returns>
        protected override int getTableMaxId(string table, string column)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NR-Get min of table id's
        /// </summary>
        /// <param name="table">table name</param>
        /// <returns>Returns value of min of table id's</returns>
        protected override int getTableMinId(string table, string column)
        {
            throw new System.NotImplementedException();
        }

 #endregion

#region *** Функции контейнера ***

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

#endregion

#region *** Функции ячеек ***

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


#region *** Функции связей ***

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
