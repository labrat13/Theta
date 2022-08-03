using System;
using System.Collections.Generic;
using System.Text;
using Mary.Serialization;

namespace Mary
{
    /// <summary>
    /// Представляет интерфейс для двух взаимозаменяемых вариантов ячеек.
    /// </summary>
    public abstract class MCell : MElement
    {
        /// <summary>
        /// Container reference - статическая ссылка для экономии памяти.
        /// </summary>
        private static MEngine m_container;

        /// <summary>
        /// Порог числа связей ячейки при выборе: поиск в памяти или в таблице.
        /// </summary>
        /// <remarks>Зависит от производительности сервера БД, сети и состония БД. Подобрать значение опытным путем в процессе тестов.</remarks>
        public static int LinksTreshold = 512;

#region Properties
        /// <summary>
        /// Container reference
        /// </summary>
        public static MEngine Container
        {
            get
            {
                return m_container;
            }
            set
            {
                m_container = value;
            }
        }


        /// <summary>
        /// Cell name
        /// </summary>
        public abstract String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Cell type id
        /// </summary>
        public abstract MID TypeId
        {
            get;
            set;
        }

        /// <summary>
        /// Cell creation timestamp
        /// </summary>
        public abstract DateTime CreaTime
        {
            get;
            //internal set;
        }

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        public abstract DateTime ModiTime
        {
            get;
            //internal set;
        }

        /// <summary>
        /// Cell is read-only flag (not used currently)
        /// </summary>
        public abstract bool ReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Cell data value
        /// </summary>
        public abstract byte[] Value
        {
            get;
            set;
        }

        /// <summary>
        /// Cell data value type id
        /// </summary>
        public abstract MID ValueTypeId
        {
            get;
            set;
        }

        /// <summary>
        /// Cell link collection. 
        /// Only for link reading!
        /// </summary>
        public abstract MLinkCollection Links
        {
            get;
        }

        /// <summary>
        /// Cell (saving) mode: Compact, Normal, DelaySave, Temporary
        /// </summary>
        public abstract MCellMode CellMode
        {
            get;
            //internal set;
        }

        /// <summary>
        /// Cell identifier
        /// </summary>
        public abstract MID CellID
        {
            get;
            set;
        }

        /// <summary>
        /// Current cell is MCellB cell? 
        /// (This property is read only)
        /// </summary>
        public abstract bool isLargeCell
        {
            get;
        }

#endregion


        #region Serialization function implements from MObject
        /// <summary>
        /// NT-Write cell record to serialization stream
        /// </summary>
        /// <param name="writer">serialization stream</param>
        public override void toBinary(System.IO.BinaryWriter writer)
        {

            throw new NotImplementedException();
        }

        public override void fromBinary(System.IO.BinaryReader reader)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Overrided in child classes
        /// </summary>
        /// <returns></returns>
        public override byte[] toBinaryArray()
        {
            throw new NotImplementedException();
        }

        public override string toTextString(bool withHex)
        {
            throw new NotImplementedException();
        }

        public override void toText(System.IO.TextWriter writer, bool withHex)
        {
            throw new NotImplementedException();
        }

        public override void fromText(System.IO.TextReader reader)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// NT-Сохраняет DelaySave или Temporary ячейку и ее связи в БД. Ячейка становится Normal типа.
        /// Для Normal или Compact ячеек ничего не делает.
        /// </summary>
        public abstract void S1_Save();

        /// <summary>
        /// Помечает ячейку удаленной. 
        /// Если ячейка не временная, не отложенной записи, то флаг удаления автоматически записывается в таблицу БД.
        /// </summary>
        public abstract void S1_Delete();

        /// <summary>
        /// NT-Выгружает ячейку из памяти, когда она больше не нужна. 
        /// </summary>
        public void S1_Unload()
        {
                   
            //Это странно сейчас, но, возможно, есть причины существования ячеек без загрузки в СписокЯчеекКонтейнера?
            //Тогда такие ячейки надо выявлять и игнорировать здесь.

            //Объект ячейки должен быть загружен в СписокЯчеекКонтейнера,
            //чтобы его можно было выгрузить.
            if(MCell.Container.Cells.S1_containsCell(this.CellID))
                MCell.Container.S1_intUnloadCell(this);

            return;
        }

        /// <summary>
        /// NT-Создать связь, если уже существует - исключение.
        /// </summary>
        /// <param name="Axis">ось связи</param>
        /// <param name="axisDirection">направление связи</param>
        /// <param name="targetCell">конечная ячейка</param>
        /// <returns></returns>
        public MLink S1_createLink(MID Axis, MAxisDirection axisDirection, MCell targetCell)
        {
            //выбрать ячейку, которая будет иметь больший приоритет при создании связи: MCellBt, MCellBds, MCellB, MCellA
            //Поскольку код создания связи зависит от типа обеих ячеек, удобно их привести к одному порядку, чтобы сократить число вариантов.
            //За счет переворачивания вместо 16 вариантов будет 10.
            if (this.CellMode >= targetCell.CellMode)
                return this.S1_intCreateLink(Axis, axisDirection, targetCell);
            else
                return targetCell.S1_intCreateLink(Axis, MLink.inverseAxisDirection(axisDirection), this);
        }

        /// <summary>
        /// NT-Удалить связь между ячейками. Связь только помечается удаленной.
        /// Помечает удаленными все связи, подходящие под заданные параметры.
        /// 23122017-доделанная функция
        /// </summary>
        /// <param name="axis">Идентификатор ячейки типа связи</param>
        /// <param name="axisDirection">Направление связи: Up, Down, Any</param>
        /// <param name="targetCell">Идентификатор конечной ячейки</param>
        /// <returns>Возвращает число измененных связей</returns>
        public int S1_deleteLink(MID axis, MAxisDirection axisDirection, MCell targetCell)
        {
            //1)получить один или несколько экземпляров связи
            //2)пометить его/их удаленным
            //3)если связь постоянная, записать ее состояние в таблицу
            MLinkCollection col = this.S1_intGetLinks(axis, axisDirection, targetCell, true);
            foreach (MLink link in col.Items)
            {
                link.isActive = false;//пометим ячейку неактивной и эти изменения запишутся в таблицу связей БД, если связь туда уже записана.
                //if (link.isLinkNotTemporary) - уже встроено в проперти MLink.isActive
                //    MCell.Container.DataLayer.LinkUpdate(link);
            }
            //возвратим число измененных связей
            return col.Items.Count; ;
        }

        /// <summary>
        /// NT-Получить связь между ячейками
        /// </summary>
        /// <param name="Axis">Идентификатор ячейки типа связи</param>
        /// <param name="axisDirection">Направление связи: Up, Down, Any</param>
        /// <param name="targetCell">Идентификатор конечной ячейки</param>
        /// <param name="activeOnly">True - возвращать только активные связи, False - возвращать активные и неактивные связи</param>
        /// <returns>Возвращает объект коллекции связей MLinkCollection, содержащий отобранные связи.</returns>
        internal MLinkCollection S1_intGetLinks(MID Axis, MAxisDirection axisDirection, MCell targetCell, bool activeOnly)
        {
            //это все закомментировано пока - это должна была быть оптимизация скорости.
            //за счет поиска только связей в памяти а не где придется
            //ее применим позже, когда отработаем основные механизмы.

            ////искать в меньшем списке связей ячеек
            ////Таблица логики:
            ////если this=MCellA и target=MCellA то: берем любую из коллекций связей, то есть, коллекцию текущей ячейки. Нельзя сравнивать размеры, так как это потребует выборки из БД связей для обоих ячеек.
            ////если this=MCellA и target!=MCellA то: берем коллекцию связей target ячейки - ее связи полностью в памяти и с ней все проще.
            ////если this!=MCellA и target=MCellA то: берем коллекцию связей this ячейки - ее связи полностью в памяти и с ней все проще.
            ////если this!=MCellA и target!=MCellA то: берем коллекцию меньшего размера.

            //MLinkCollection searchPlace = null;
            ////тут надо избегать обращения к коллекции связей ячейки MCellA, поскольку каждое ее получение запускает выборку из БД, 
            ////это медленно и незачем это делать несколько раз.
            //if (this.CellMode != MCellMode.Compact)
            //{
            //    searchPlace = this.Links;//сразу припишем, поскольку это не MCellA, для упрощения кода
            //    //если одна из ячеек MCellA, то ищем связь в другой ячейке
            //    if (targetCell.CellMode != MCellMode.Compact)
            //    {
            //        //если обе ячейки не MCellA, то ищем в меньшем списке
            //        if (this.Links.Items.Count > targetCell.Links.Items.Count)
            //            searchPlace = targetCell.Links;
            //    }
            //}
            //else
            //{
            //    //текущая ячейка MCellA типа.
            //    //если другая ячейка не MCellA типа, то ищем в ней.
            //    //а если тоже MCellA, то все равно где искать.
            //    if (targetCell.CellMode != MCellMode.Compact)
            //        searchPlace = targetCell.Links;
            //    else
            //        searchPlace = this.Links;
            //}

            //теперь ищем в выбранной коллекции нужные связи
            return this.Links.getLinks(Axis, axisDirection, this.CellID, targetCell.CellID, activeOnly);
        }


        /// <summary>
        /// NT-Create link between two cells
        /// </summary>
        /// <param name="Axis"></param>
        /// <param name="axisDirection"></param>
        /// <param name="targetCell"></param>
        /// <returns></returns>
        /// <remarks>
        /// Предполагается, что текущая ячейка обладает приоритетом при создании связи.
        /// См. S1_createLink(). Если это не так, связи могут быть созданы неправильно.
        /// TODO: Хотя есть возможность упростить свичи, пока оставим их развернутыми, для наглядности.
        /// Надо написать по ним описание процесса создания связи, для полного представления.
        /// </remarks>
        private MLink S1_intCreateLink(MID Axis, MAxisDirection axisDirection, MCell targetCell)
        {
            //create link object
            MLink link = new MLink();
            link.setCellsByDirection(axisDirection, this, targetCell);//set cell id and references
            link.Axis = Axis;

            //Заполнять остальные свойства будет caller, после выполнения функции.
            //Ход:
            //1)Проверить отсутствие связи в памяти или таблице.
            //2)Добавить связь в таблицу, записать идентификатор из таблицы (если надо)
            //3)Добавить связь в списки ячеек и список контейнера (если надо)
            //4)Вернуть связь вызывающему коду 
            switch (this.CellMode)
            {
                case MCellMode.Temporary:
                    //допустимые варианты:
                    //MCellBt - MCellBt
                    //MCellBt - MCellBds
                    //MCellBt - MCellB
                    //MCellBt - MCellA
                    //Проверка отсутствия связи: в памяти
                    //Создание связи: в памяти

                    switch (targetCell.CellMode)
                    {
                        case MCellMode.Temporary:
                            //Проверка отсутствия связи: в меньшем списке связей ячеек 
                            //Создание связи: в списках ячеек и контейнере

                            //Проверка
                            if (this.containsLinkMinCollection(link, this, targetCell, false))
                                throw new Exception("Link already exists");
                            //Создание 
                            this.Links.AddLink(link);
                            targetCell.Links.AddLink(link);
                            MCell.Container.Links.AddLink(link);
                            break;
                        case MCellMode.DelaySave:
                            //Проверка отсутствия связи: в меньшем списке связей ячеек
                            //Создание связи: в списках ячеек и контейнере

                            //Проверка
                            if (this.containsLinkMinCollection(link, this, targetCell, false))
                                throw new Exception("Link already exists");
                            //Создание 
                            this.Links.AddLink(link);
                            targetCell.Links.AddLink(link);
                            MCell.Container.Links.AddLink(link);
                            break;
                        case MCellMode.Normal:
                            //Проверка отсутствия связи: в меньшем списке связей ячеек
                            //Создание связи: в списках ячеек и контейнере

                            //Проверка
                            if (this.containsLinkMinCollection(link, this, targetCell, false))
                                throw new Exception("Link already exists");
                            //Создание 
                            this.Links.AddLink(link);
                            targetCell.Links.AddLink(link);
                            MCell.Container.Links.AddLink(link);
                            break;
                        case MCellMode.Compact:
                            //Проверка отсутствия связи: в списке связей текущей ячейки
                            //Создание связи: в списке текущей ячейки и контейнере (у MCellA нет постоянного списка связей)

                            //Проверка
                            if(this.Links.containsLink(link)) 
                                throw new Exception("Link already exists");
                            //Создание 
                            this.Links.AddLink(link);
                            MCell.Container.Links.AddLink(link);
                            break;
                        default:
                            throw new Exception("Invalid CellMode value");
                    }
                    break;
                case MCellMode.DelaySave:
                    //допустимые варианты:
                    //MCellBds - MCellBds
                    //MCellBds - MCellB
                    //MCellBds - MCellA
                    //Проверка отсутствия связи: в памяти
                    //Создание связи: в памяти
                    //Можно объединить этот случай с предыдущим - код одинаковый, кажется.
                    switch (targetCell.CellMode)
                    {
                        case MCellMode.DelaySave:
                            //Проверка отсутствия связи: в меньшем списке связей ячеек 
                            //Создание связи: в списках ячеек и контейнере

                            //Проверка
                            if (this.containsLinkMinCollection(link, this, targetCell, false))
                                throw new Exception("Link already exists");
                            //Создание 
                            this.Links.AddLink(link);
                            targetCell.Links.AddLink(link);
                            MCell.Container.Links.AddLink(link);
                            break;
                        case MCellMode.Normal:
                            //Проверка отсутствия связи: в меньшем списке связей ячеек 
                            //Создание связи: в списках ячеек и контейнере

                            //Проверка
                            if (this.containsLinkMinCollection(link, this, targetCell, false))
                                throw new Exception("Link already exists");
                            //Создание 
                            this.Links.AddLink(link);
                            targetCell.Links.AddLink(link);
                            MCell.Container.Links.AddLink(link);
                            break;
                        case MCellMode.Compact:
                            //Проверка отсутствия связи: в списке связей текущей ячейки
                            //Создание связи: в списке текущей ячейки и контейнере (у MCellA нет постоянного списка связей)

                            //Проверка
                            if (this.Links.containsLink(link))
                                throw new Exception("Link already exists");
                            //Создание 
                            this.Links.AddLink(link);
                            MCell.Container.Links.AddLink(link);
                            break;
                        default:
                            throw new Exception("Invalid CellMode value");
                    }
                    break;
                case MCellMode.Normal:
                    //допустимые варианты:
                    //MCellB - MCellB
                    //MCellB - MCellA
                    //Проверка отсутствия связи: в памяти или таблице
                    //Создание связи: в памяти и таблице
                    switch (targetCell.CellMode)
                    {
                        case MCellMode.Normal:
                            //Проверка отсутствия связи: в меньшем списке связей ячеек или в таблице
                            //Создание связи: в списках ячеек и контейнере, в таблице

                            //Проверка
                            if(this.containsLinkMinCollection(link, this, targetCell, true))
                                throw new Exception("Link already exists");
                            //Создание - сначала в таблицу, чтобы получить ИД связи
                            S1_InsertLinkToTableAndAssignID(link); //Теперь все изменения в связи будут сразу записываться в таблицу!
                            this.Links.AddLink(link);
                            targetCell.Links.AddLink(link);
                            MCell.Container.Links.AddLink(link);
                            break;
                        case MCellMode.Compact:
                            //Проверка отсутствия связи: в списке связей текущей ячейки или в таблице
                            //Создание связи: в списке текущей ячейки и контейнере (у MCellA нет постоянного списка связей), в таблице

                            //Проверка
                            if(this.containsLinkMorT(link, true))
                                throw new Exception("Link already exists");
                            //Создание - сначала в таблицу, чтобы получить ИД связи
                            S1_InsertLinkToTableAndAssignID(link); //Теперь все изменения в связи будут сразу записываться в таблицу!
                            this.Links.AddLink(link);
                            MCell.Container.Links.AddLink(link);
                            break;
                        default:
                            throw new Exception("Invalid CellMode value");
                    }
                    break;
                case MCellMode.Compact:
                    //допустимые варианты:
                    //MCellA - MCellA
                    //Проверка отсутствия связи: в таблице
                    //Создание связи: в таблице
                    if (targetCell.CellMode != MCellMode.Compact) throw new Exception("Invalid CellMode value!");
                    //проверка
                    if (this.containsLinkInTable(link)) throw new Exception("Link already exists");
                    //сохранение
                    S1_InsertLinkToTableAndAssignID(link); //Теперь все изменения в связи будут сразу записываться в таблицу!
                    break;
                default:
                    throw new Exception("Invalid CellMode value");
            }
            return link;
        }

        /// <summary>
        /// NT-Добавить связь в таблицу и записать в связь идентификатор строки.
        /// Связь становится постоянной и сама обновляется в таблицу при изменениях проперти.
        /// </summary>
        /// <param name="link">Связь</param>
        protected void S1_InsertLinkToTableAndAssignID(MLink link)
        {
            MCell.Container.DataLayer.LinkInsertGetId(link);
            //link.TableId = id; - уже сделано в LinkInsertGetId(link)
        }

        /// <summary>
        /// NT-Get minimal collection of links from two (linked) cells.
        /// This function work with MCellA cells too (slow)
        /// </summary>
        /// <param name="cellA">Cell</param>
        /// <param name="cellB">Cell</param>
        /// <returns></returns>
        protected static MLinkCollection getMinLinkCollection(MCell cellA, MCell cellB)
        {
            MLinkCollection outCol = cellA.Links;
            if (cellB.Links.Items.Count < outCol.Items.Count) outCol = cellB.Links;
            return outCol; 
        }

        /// <summary>
        /// NT-Returns true if one or more links exists in table
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private bool containsLinkInTable(MLink link)
        {
            //create template for search
            MLinkTemplate temp = new MLinkTemplate();
            temp.Axis = link.Axis;
            temp.downCellID = link.downCellID;
            temp.isActive = link.isActive;
            temp.upCellID = link.upCellID;
            //get search
            MLinkCollection col = MCell.Container.DataLayer.getLinks(temp);
            return (col.Items.Count > 0);
        }

        /// <summary>
        /// NT-Проверить наличие связи между ячейками в памяти, выбрать минимальную коллекцию для поиска.
        /// Если разрешено, искать в таблице если в памяти слишком много связей.
        /// </summary>
        /// <param name="li">Link with cellId, Axis, Active values</param>
        /// <param name="cell1">Cell</param>
        /// <param name="cell2">Cell</param>
        /// <param name="orTable">Use links table if too many links in memory</param>
        /// <returns></returns>
        private bool containsLinkMinCollection(MLink li, MCell cell1, MCell cell2, bool orTable)
        {
            //get minimal link collection
            MLinkCollection col = MCell.getMinLinkCollection(cell1, cell2);
            //использовать таблицу?
            if (orTable && (col.Items.Count > MCell.LinksTreshold))//search in table
                return this.containsLinkInTable(li);
            else
                //check link exists
                return col.containsLink(li);
        }

        /// <summary>
        /// NT-Проверить наличие связи между ячейками в списке связей текущей ячейки 
        /// Если разрешено, искать в таблице если в памяти слишком много связей.
        /// </summary>
        /// <param name="li">Link with cellId, Axis, Active values</param>
        /// <param name="orTable">Use links table if too many links in memory</param>
        /// <returns></returns>
        private bool containsLinkMorT(MLink li, bool orTable)
        {
            if((orTable) && (this.Links.Items.Count > MCell.LinksTreshold))
                return this.containsLinkInTable(li);
            else
                return this.Links.containsLink(li);
        }

        /// <summary>
        /// Return True if specified object is MCell or MCellA or MCellB type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsCellType(Object obj)
        {
            Type t = obj.GetType();
            return ((t.Equals(typeof(MCell))) || ((t.Equals(typeof(MCellA)))) || ((t.Equals(typeof(MCellB)))));
        }











    }

 


 
}
