using System;
using System.Collections.Generic;
using System.Text;
using Mary;

namespace RenewTests2017
{
    /// <summary>
    /// NT-Помощник в работе с Структурой Сущностей Солюшена.
    /// </summary>
    public class SolutionHelper
    {
        /* Для работ с Структурой Сущностей Солюшена нужно либо бродить по Структуре Сущностей, выписывая названия нужных ячеек,
         * либо распечатать схему Солюшена, чтобы по печатной модели находить названия нужных ячеек. 
         * Есть еще вариант, когда нужная ячейка находится через иерархию, но это пока не продумано.
         * Сейчас все основывается на именах ячеек, они должны быть уникальными.
         * Эта начальная структура содержит неожиданно много ячеек и связей. Надеюсь, потом будет проще.
         *  Но в целом ее уже можно считать самым сложным и запутанным делом, которым я когда-либо занимался.
         *  Предлагаю поставить для нее в приложение иконку паутины. Она будет выражать ощущения пользователя.
         * В этой версии Структуры сущностей Солюшена:
         * В системную часть входит:
         * - Коллекции - одномерные сеты - для свойств ячеек и связей: Тип ячейки, Тип данных ячейки, Состояние ячейки, Тип связи, Состояние связи.
         * - иерархии классов для этих же свойств ячеек и связей. Они начинаются с классов BaseXxx, отношения выражены связями, а не записываются в поля ячеек.
         * - иерархия сущностей: Начальная сущность - служебная сущность. Иерархия чисто для затравки.
         * - служебные значения: System_Nothing - для обозначения, которое пока нигде не применяется. Я не придумал пока.
         * В пользовательскую часть входит:
         * - коллекция аккаунтов пользователя.
         * - один аккаунт пользователя для примера. 
         *   Аккаунт пользователя сейчас это просто ячейка. В нее пользователь должен будет присоединять свои объекты и коллекции.
         *   Подобно тому, как это в системной части сделано. Или не так - я пока не знаю, как там организовать это. Я сильно занят самим движком.
         *   И там тоже наверно можно будет создавать свои типы данных и структур, и типы ячеек и связей, соответственно.
         *   Как и зачем это делать - я сам пока не знаю. Но думаю, это будет нечто такое, с чем я никогда ранее не сталкивался.
         *   У меня совершенно нет опыта в подобной работе. Будет очень сложно.
         */ 

#region *** Названия важных служебных ячеек ***
        //Доступ к ячейкам по именам - важная и неудобная часть работы с Солюшеном.
        //С этой кучей названий будет много путаницы и странных ошибок.

        /// <summary>
        /// Константа - название ячейки коллекции ячеек 
        /// </summary>
        public const string НазваниеКоллекцияЯчейкаТипаЯчейки = "System_CellTypes";

                /// <summary>
        /// Константа - название ячейки коллекции ячеек 
        /// </summary>
        public const string НазваниеКоллекцияЯчейкаТипаДанныхЯчейки = "System_CellDataTypes";

                /// <summary>
        /// Константа - название ячейки коллекции ячеек 
        /// </summary>
        public const string НазваниеКоллекцияЯчейкаСостоянияЯчейки = "System_CellStates";

                /// <summary>
        /// Константа - название ячейки коллекции ячеек 
        /// </summary>
        public const string НазваниеКоллекцияЯчейкаТипаСвязи = "System_LinkTypes";

                /// <summary>
        /// Константа - название ячейки коллекции ячеек 
        /// </summary>
        public const string НазваниеКоллекцияЯчейкаСостоянияСвязи = "System_LinkStates";

                /// <summary>
        /// Константа - название начальной ячейки Солюшена
        /// </summary>
        public const string НазваниеНачальнаяЯчейкаСолюшена = "World";

                /// <summary>
        /// Константа - название ячейки Специальная ячейка Nothing
        /// </summary>
        public const string НазваниеСпециальнаяЯчейкаNothing = "System_Nothing";

                /// <summary>
        /// Константа - название ячейки коллекции ячеек пользовательских каталогов
        /// </summary>
        public const string НазваниеКоллекцияЯчейкаАккаунтаПользователя = "UserAccountCollection";

        /// <summary>
        /// Константа - название ячейки типа ячейки аккаунта пользователя
        /// </summary>
        public const string НазваниеЯчейкаТипаЯчейкаАккаунтаПользователя = "CellType_UserAccountRootCell";

        /// <summary>
        /// Константа - название ячейки типа ячейки 
        /// </summary>
        public const string НазваниеЯчейкаТипаЯчейкаТипаСвязи = "CellType_LinkTypeCell";

        /// <summary>
        /// Константа - название ячейки типа ячейки 
        /// </summary>
        public const string НазваниеЯчейкаТипаЯчейкаТипаДанныхЯчейки = "CellType_CellDataTypeCell";

        /// <summary>
        /// Константа - название ячейки типа ячейки 
        /// </summary>
        public const string НазваниеЯчейкаТипаЯчейкаСостоянияЯчейки = "CellType_CellStateCell";

        /// <summary>
        /// Константа - название ячейки типа ячейки 
        /// </summary>
        public const string НазваниеЯчейкаТипаЯчейкаСостоянияСвязи = "CellType_LinkStateCell";

        /// <summary>
        /// Константа - название ячейки типа Ячейка типа ячейки
        /// </summary>
        public const string НазваниеЯчейкаТипаЯчейкаТипаЯчейки = "CellType_CellTypeCell";
        /// <summary>
        /// Константа - название ячейки состояния ячейки нормального
        /// </summary>
        public const string НазваниеЯчейкаСостоянияЯчейкиНормального = "CellState_Normal";

        
        /// <summary>
        /// Константа - название ячейки типа данных ячейки - без данных
        /// </summary>
        public const string НазваниеЯчейкаТипаДанныхЯчейкиБезДанных = "CellDataType_NoData";

        
        /// <summary>
        /// Константа - название ячейки типа связи - коллекция
        /// </summary>
        public const string НазваниеЯчейкаТипаСвязиКоллекция = "LinkType_Collection";

        
        /// <summary>
        /// Константа - название ячейки типа связи - агрегация
        /// </summary>
        public const string НазваниеЯчейкаТипаСвязиАгрегация = "LinkType_Aggregation";

        /// <summary>
        /// Константа - название ячейки типа связи - абстракция
        /// </summary>
        public const string НазваниеЯчейкаТипаСвязиАбстракция = "LinkType_Abstraction";
        
        /// <summary>
        /// Константа - название ячейки 
        /// </summary>
        public const string НазваниеЯчейкаСостоянияСвязиНормальное = "LinkState_Normal";



#endregion
        /// <summary>
        /// NT-Создать начальную структуру Солюшена.
        /// Обычно вызывается для пустого Солюшена.
        /// </summary>
        /// <param name="cellMode">Режим создаваемых ячеек</param>
        /// <remarks>
        /// Функция создает начальный набор ячеек и связей, включая коллекции ячеек типов и связи между ними.
        /// Ячейки различаются по названиям, поэтому их названия должны быть уникальными для Солюшена.
        /// Функция обычно вызывается для пустого Солюшена, чтобы подготовить его для хранения данных.
        /// </remarks>
        public static void СоздатьНачальнуюСтруктуруСолюшена(MEngine engine, MCellMode cellMode)
        {
            //TODO: очень сложно - возможно, есть ошибки в назначении свойств
            #region *** Создание ячеек ***

            MCell world = engine.CellCreate(cellMode, SolutionHelper.НазваниеНачальнаяЯчейкаСолюшена, "Начальный класс структуры сущностей");
            MCell sys = engine.CellCreate(cellMode, "System", "Служебная часть структуры сущностей");
            MCell ct = engine.CellCreate(cellMode, SolutionHelper.НазваниеКоллекцияЯчейкаТипаЯчейки, "Коллекция ячеек типов ячеек");
            MCell bct = engine.CellCreate(cellMode, "BaseCellType", "Базовая ячейка типа ячейки");
            MCell cdt = engine.CellCreate(cellMode, SolutionHelper.НазваниеКоллекцияЯчейкаТипаДанныхЯчейки, "Коллекция ячеек типов данных ячеек");
            MCell bcdt = engine.CellCreate(cellMode, "BaseCellDataType", "Базовая ячейка типа данных ячейки");
            MCell cs = engine.CellCreate(cellMode, SolutionHelper.НазваниеКоллекцияЯчейкаСостоянияЯчейки, "Коллекция ячеек состояния ячеек");
            MCell bcs = engine.CellCreate(cellMode, "BaseCellState", "Базовая ячейка состояния ячейки");
            MCell la = engine.CellCreate(cellMode, SolutionHelper.НазваниеКоллекцияЯчейкаТипаСвязи, "Коллекция ячеек типов связи");
            MCell bla = engine.CellCreate(cellMode, "BaseLinkType", "Базовая ячейка типа связи");
            MCell ls = engine.CellCreate(cellMode, SolutionHelper.НазваниеКоллекцияЯчейкаСостоянияСвязи, "Коллекция ячеек состояния связи");
            MCell bls = engine.CellCreate(cellMode, "BaseLinkState", "Базовая ячейка состояния связи");
            MCell n = engine.CellCreate(cellMode, SolutionHelper.НазваниеСпециальнаяЯчейкаNothing, "Специальная ячейка Nothing. Представляет идентификатор для использования в элементах, где нельзя назначить идентификатор, но он необходим");
            
            
            //вот вроде бы простая работа - а нет, сложная. 
            //Теперь надо создать связи ячеек. Но для них нужны типы связей.
            //А чтобы создать типы связей, нужно создать ячейки типов связей.
            //А не эти болванки-заготовки, что я выше наклепал.
            //А для этих ячеек тоже нужны связи, и для них тоже нужны ячейки типов.
            //Вот какая кручень получается. Итеративная ручная работа.
            //Хорошо еще, что эти типы не требуются сразу при создании, их можно позже дописывать.

            //методика получается такая: сначала описываем свойства ячеек и связей, которые надо вписать в уже созданные ячейки и связи.
            //Затем создаем требуемые для этого ячейки.
            //Затем описываем требуемые связи между этими новыми ячейками, типы ячеек, связи итд.
            //Затем создаем требуемые для этого ячейки.
            //... повторяем, пока не окажется, что создавать ячейки более не нужно.
            //Теперь все созданные ячейки выстраиваем в один ряд и вписываем в их свойства идентификаторы соответствующих ячеек.
            //А потом создаем связи между ячейками, описывая их в комментариях. 
            //Эти связи сейчас древовидные, поэтому группируем их по корню дерева.

                        //1.2) типы ячеек - тут я уже увяз в классах и их отношениях, тут надо много прорабатывать эту тему. Сейчас просто напишу что придумается.
            //тут хорошо, что типы представлены полями ячейки, а не связями. Иначе я бы тут завис надолго.
            //World имеет тип Сущность-НачальнаяСущность
            //System имеет тип Сущность-СлужебнаяСущность
            //Nothing имеет тип Сущность-СлужебнаяСущность
            //CellTypes имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей. Коллекция чего именно - это вроде бы тоже класс, но потом разберемся, как с ним быть.
            //CellDataTypes  имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей. 
            //CellStates  имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей.
            //LinkAxises  имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей. 
            //LinkStates  имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей.
            //BaseCellType имеет тип ЯчейкаТипаЯчейки
            //BaseCellDataType имеет тип ЯчейкаТипаДанныхЯчейки
            //CellDataTypeNoData  имеет тип ЯчейкаТипаДанныхЯчейки
            //BaseCellState имеет тип ЯчейкаСостоянияЯчейки
            //BaseLinkAxis имеет тип ЯчейкаТипаСвязи
            //BaseLinkState имеет тип ЯчейкаСостоянияСвязи
            
            //создаем ячейки типов ячеек
            MCell cellTypeEntity = engine.CellCreate(cellMode, "CellType_Entity", "Ячейка типа ячейки - Сущность");
            MCell cellTypeEntityInitial = engine.CellCreate(cellMode, "CellType_InitialEntity", "Ячейка типа ячейки - Начальная Сущность");
            MCell cellTypeEntityService = engine.CellCreate(cellMode, "CellType_ServiceEntity", "Ячейка типа ячейки - Служебная Сущность");
            MCell cellTypeEntityCollection = engine.CellCreate(cellMode, "CellType_EntityCollection", "Ячейка типа ячейки - Коллекция однотипных Сущностей");
            MCell cellTypeEntityCollectionTyped = engine.CellCreate(cellMode, "CellType_EntityCollectionTyped", "Ячейка типа ячейки - Коллекция однотипных Сущностей");
            MCell cellTypeEntityCollectionCellTypeCell = engine.CellCreate(cellMode, "CellType_EntityCollection_CellTypeCell", "Ячейка типа ячейки - Коллекция ЯчейкаТипаЯчейки");
            MCell cellTypeEntityCollectionCellDataTypeCell = engine.CellCreate(cellMode, "CellType_EntityCollection_CellDataTypeCell", "Ячейка типа ячейки - Коллекция ЯчейкаТипаДанныхЯчейки");
            MCell cellTypeEntityCollectionCellStateCell = engine.CellCreate(cellMode, "CellType_EntityCollection_CellStateCell", "Ячейка типа ячейки - Коллекция ЯчейкаСостоянияЯчейки");
            MCell cellTypeEntityCollectionLinkTypeCell = engine.CellCreate(cellMode, "CellType_EntityCollection_LinkTypeCell", "Ячейка типа ячейки - Коллекция ЯчейкаТипаСвязи");
            MCell cellTypeEntityCollectionLinkStateCell = engine.CellCreate(cellMode, "CellType_EntityCollection_LinkStateCell", "Ячейка типа ячейки - Коллекция ЯчейкаСостоянияСвязи");

            MCell cellType_CellTypeCell = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаЯчейкаТипаЯчейки, "Ячейка типа ячейки - ЯчейкаТипаЯчейки");//сам себя описывает
            MCell cellType_CellDataTypeCell = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаЯчейкаТипаДанныхЯчейки, "Ячейка типа ячейки - ЯчейкаТипаДанныхЯчейки");
            MCell cellType_CellStateCell = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаЯчейкаСостоянияЯчейки, "Ячейка типа ячейки - ЯчейкаСостоянияЯчейки");
            MCell cellType_LinkTypeCell = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаЯчейкаТипаСвязи, "Ячейка типа ячейки - ЯчейкаТипаСвязи");
            MCell cellType_LinkStateCell = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаЯчейкаСостоянияСвязи, "Ячейка типа ячейки - ЯчейкаСостоянияСвязи");
            
            //1.3) тип данных ячеек
            //все ячейки здесь не хранят данные, поэтому только один тип данных - без данных.
            MCell cellDataTypeNodata = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных, "Ячейка типа данных ячейки - Ячейка не должна содержать данные");

            //1.4) состояние ячейки
            //все ячейки в нормальном состоянии, поэтому только одно состояние - нормальное.
            MCell cellStateNormal = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального, "Ячейка состояния ячейки - Ячейка в обычном состоянии");
            
            
            //1.5) типы связей
            //объектов тут пока нет вроде бы
            //связанность ячеек
            //1.5.1) Агрегация: Входит в состав или Является частью 
            //System входит в состав World
            //Nothing входит в состав System
            //CellTypes входит в состав System 
            //CellDataTypes входит в состав System 
            //CellStates входит в состав System 
            //LinkAxises входит в состав System 
            //LinkStates входит в состав System
            //1.5.2 Элемент коллекции
            //BaseCellType элемент коллекции CellTypes
            //BaseCellDataType элемент коллекции CellDataTypes
            //BaseCellState элемент коллекции CellStates
            //BaseLinkAxis элемент коллекции LinkAxises
            //BaseLinkState элемент коллекции LinkStates
            //1.5.3) Абстракция: Является подклассом или Является надклассом
            MCell cellLinkType_Aggregation = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаСвязиАгрегация, "Ячейка типа связи - Агрегация: Входит в состав или Является частью");
            MCell cellLinkType_Abstraction = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция, "Ячейка типа связи - Абстракция: Является подклассом или Является надклассом");
            MCell cellLinkType_Collection = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция, "Ячейка типа связи - Коллекция: Является частью набора элементов или Является коллекцией элементов");

            //1.6) состояние связей
            //все связи в нормальном состоянии, поэтому только одно состояние - нормальное.
            MCell cellLinkStateNormal = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное, "Ячейка состояния связи - Связь в обычном состоянии");
            #endregion

            #region *** Заполнение свойств ячеек ***
            //1.7) создаем общий список созданных ячеек

            //world 
            //sys 
            //ct 
            //bct 
            //cdt 
            //bcdt 
            //cs 
            //bcs 
            //la 
            //bla 
            //ls 
            //bls 
            //n 
            ////ячейки типов ячеек
            //cellTypeEntity 
            //cellTypeEntityInitial 
            //cellTypeEntityService 
            //cellTypeEntityCollection 
            //cellTypeEntityCollectionTyped 
            //cellTypeEntityCollectionCellTypeCell 
            //cellTypeEntityCollectionCellDataTypeCell 
            //cellTypeEntityCollectionCellStateCell 
            //cellTypeEntityCollectionLinkTypeCell 
            //cellTypeEntityCollectionLinkStateCell 
            //cellType_CellTypeCell 
            //cellType_CellDataTypeCell 
            //cellType_CellStateCell 
            //cellType_LinkTypeCell 
            //cellType_LinkStateCell 
            ////ячейки тип данных ячеек
            //cellDataTypeNodata 
            ////ячейки состояния ячейки
            //cellStateNormal
            ////ячейки типа связей
            //cellLinkType_Aggregation 
            //cellLinkType_Abstraction 
            //cellLinkType_Collection 
            ////ячейки состояния связей
            //cellLinkStateNormal 

            //1.8) пропишем типы данных всех ячеек здесь
            //вписываем копии идентификаторов, чтобы изменение поля в одном из них не повлияло на все использующие его ячейки
            world.ValueTypeId = cellDataTypeNodata.CellID; 
            sys.ValueTypeId = cellDataTypeNodata.CellID; 
            ct.ValueTypeId = cellDataTypeNodata.CellID; 
            bct.ValueTypeId = cellDataTypeNodata.CellID; 
            cdt.ValueTypeId = cellDataTypeNodata.CellID; 
            bcdt.ValueTypeId = cellDataTypeNodata.CellID; 
            cs.ValueTypeId = cellDataTypeNodata.CellID; 
            bcs.ValueTypeId = cellDataTypeNodata.CellID; 
            la.ValueTypeId = cellDataTypeNodata.CellID; 
            bla.ValueTypeId = cellDataTypeNodata.CellID; 
            ls.ValueTypeId = cellDataTypeNodata.CellID; 
            bls.ValueTypeId = cellDataTypeNodata.CellID; 
            n.ValueTypeId = cellDataTypeNodata.CellID; 
            //ячейки типов ячеек
            cellTypeEntity.ValueTypeId = cellDataTypeNodata.CellID; 
            cellTypeEntityInitial.ValueTypeId = cellDataTypeNodata.CellID; 
            cellTypeEntityService.ValueTypeId = cellDataTypeNodata.CellID; 
            cellTypeEntityCollection.ValueTypeId = cellDataTypeNodata.CellID; 
            cellTypeEntityCollectionTyped.ValueTypeId = cellDataTypeNodata.CellID; 
            cellTypeEntityCollectionCellTypeCell.ValueTypeId = cellDataTypeNodata.CellID; 
            cellTypeEntityCollectionCellDataTypeCell.ValueTypeId = cellDataTypeNodata.CellID; 
            cellTypeEntityCollectionCellStateCell.ValueTypeId = cellDataTypeNodata.CellID; 
            cellTypeEntityCollectionLinkTypeCell.ValueTypeId = cellDataTypeNodata.CellID; 
            cellTypeEntityCollectionLinkStateCell.ValueTypeId = cellDataTypeNodata.CellID; 
            cellType_CellTypeCell.ValueTypeId = cellDataTypeNodata.CellID; 
            cellType_CellDataTypeCell.ValueTypeId = cellDataTypeNodata.CellID; 
            cellType_CellStateCell.ValueTypeId = cellDataTypeNodata.CellID; 
            cellType_LinkTypeCell.ValueTypeId = cellDataTypeNodata.CellID; 
            cellType_LinkStateCell.ValueTypeId = cellDataTypeNodata.CellID; 
            //ячейки тип данных ячеек
            cellDataTypeNodata.ValueTypeId = cellDataTypeNodata.CellID; 
            //ячейки состояния ячейки
            cellStateNormal.ValueTypeId = cellDataTypeNodata.CellID;
            //ячейки типа связей
            cellLinkType_Aggregation.ValueTypeId = cellDataTypeNodata.CellID; 
            cellLinkType_Abstraction.ValueTypeId = cellDataTypeNodata.CellID; 
            cellLinkType_Collection.ValueTypeId = cellDataTypeNodata.CellID; 
            //ячейки состояния связей
            cellLinkStateNormal.ValueTypeId = cellDataTypeNodata.CellID; 

            //1.9) пропишем состояние ячеек здесь - оно у всех одинаковое = cellStateNormal.CellID;
            world.State = cellStateNormal.CellID; 
            sys.State = cellStateNormal.CellID; 
            ct.State = cellStateNormal.CellID; 
            bct.State = cellStateNormal.CellID; 
            cdt.State = cellStateNormal.CellID; 
            bcdt.State = cellStateNormal.CellID; 
            cs.State = cellStateNormal.CellID; 
            bcs.State = cellStateNormal.CellID; 
            la.State = cellStateNormal.CellID; 
            bla.State = cellStateNormal.CellID; 
            ls.State = cellStateNormal.CellID; 
            bls.State = cellStateNormal.CellID; 
            n.State = cellStateNormal.CellID; 
            //ячейки типов ячеек
            cellTypeEntity.State = cellStateNormal.CellID; 
            cellTypeEntityInitial.State = cellStateNormal.CellID; 
            cellTypeEntityService.State = cellStateNormal.CellID; 
            cellTypeEntityCollection.State = cellStateNormal.CellID; 
            cellTypeEntityCollectionTyped.State = cellStateNormal.CellID; 
            cellTypeEntityCollectionCellTypeCell.State = cellStateNormal.CellID; 
            cellTypeEntityCollectionCellDataTypeCell.State = cellStateNormal.CellID; 
            cellTypeEntityCollectionCellStateCell.State = cellStateNormal.CellID; 
            cellTypeEntityCollectionLinkTypeCell.State = cellStateNormal.CellID; 
            cellTypeEntityCollectionLinkStateCell.State = cellStateNormal.CellID; 
            cellType_CellTypeCell.State = cellStateNormal.CellID; 
            cellType_CellDataTypeCell.State = cellStateNormal.CellID; 
            cellType_CellStateCell.State = cellStateNormal.CellID; 
            cellType_LinkTypeCell.State = cellStateNormal.CellID; 
            cellType_LinkStateCell.State = cellStateNormal.CellID; 
            //ячейки тип данных ячеек
            cellDataTypeNodata.State = cellStateNormal.CellID; 
            //ячейки состояния ячейки
            cellStateNormal.State = cellStateNormal.CellID;
            //ячейки типа связей.State = cellStateNormal.CellID;
            cellLinkType_Aggregation.State = cellStateNormal.CellID; 
            cellLinkType_Abstraction.State = cellStateNormal.CellID; 
            cellLinkType_Collection.State = cellStateNormal.CellID; 
            //ячейки состояния связей
            cellLinkStateNormal.State = cellStateNormal.CellID; 

            //1.9) пропишем типы ячеек здесь - они разные!
            world.TypeId = cellTypeEntityInitial.CellID; //World имеет тип Сущность-НачальнаяСущность
            sys.TypeId = cellTypeEntityService.CellID;// System имеет тип Сущность-СлужебнаяСущность
            ct.TypeId = cellTypeEntityCollectionCellTypeCell.CellID;// CellTypes имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей.
            bct.TypeId = cellType_CellTypeCell.CellID;//BaseCellType имеет тип ЯчейкаТипаЯчейки 
            cdt.TypeId = cellTypeEntityCollectionCellDataTypeCell.CellID;//CellDataTypes  имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей.
            bcdt.TypeId = cellType_CellDataTypeCell.CellID; //BaseCellDataType имеет тип ЯчейкаТипаДанныхЯчейки 
            cs.TypeId = cellTypeEntityCollectionCellStateCell.CellID;//CellStates  имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей. 
            bcs.TypeId = cellType_CellStateCell.CellID;//BaseCellState имеет тип ЯчейкаСостоянияЯчейки 
            la.TypeId = cellTypeEntityCollectionLinkTypeCell.CellID;//LinkAxises  имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей.
            bla.TypeId = cellType_LinkTypeCell.CellID;//BaseLinkAxis имеет тип ЯчейкаТипаСвязи 
            ls.TypeId = cellTypeEntityCollectionLinkStateCell.CellID;//LinkStates  имеет тип Коллекция Сущностей - Коллекция Однотипных Сущностей. 
            bls.TypeId = cellType_LinkStateCell.CellID;//BaseLinkState имеет тип ЯчейкаСостоянияСвязи 
            n.TypeId = cellTypeEntityService.CellID;//Nothing имеет тип Сущность-СлужебнаяСущность 
            //ячейки типов ячеек - имеют тип ЯчейкаТипаЯчейки
            cellTypeEntity.TypeId = cellType_CellTypeCell.CellID; 
            cellTypeEntityInitial.TypeId = cellType_CellTypeCell.CellID; 
            cellTypeEntityService.TypeId = cellType_CellTypeCell.CellID; 
            cellTypeEntityCollection.TypeId = cellType_CellTypeCell.CellID; 
            cellTypeEntityCollectionTyped.TypeId = cellType_CellTypeCell.CellID; 
            cellTypeEntityCollectionCellTypeCell.TypeId = cellType_CellTypeCell.CellID; 
            cellTypeEntityCollectionCellDataTypeCell.TypeId = cellType_CellTypeCell.CellID; 
            cellTypeEntityCollectionCellStateCell.TypeId = cellType_CellTypeCell.CellID; 
            cellTypeEntityCollectionLinkTypeCell.TypeId = cellType_CellTypeCell.CellID; 
            cellTypeEntityCollectionLinkStateCell.TypeId = cellType_CellTypeCell.CellID; 
            cellType_CellTypeCell.TypeId = cellType_CellTypeCell.CellID; 
            cellType_CellDataTypeCell.TypeId = cellType_CellTypeCell.CellID; 
            cellType_CellStateCell.TypeId = cellType_CellTypeCell.CellID; 
            cellType_LinkTypeCell.TypeId = cellType_CellTypeCell.CellID; 
            cellType_LinkStateCell.TypeId = cellType_CellTypeCell.CellID; 
            //ячейки тип данных ячеек
            cellDataTypeNodata.TypeId = cellType_CellDataTypeCell.CellID; 
            //ячейки состояния ячейки
            cellStateNormal.TypeId = cellType_CellStateCell.CellID;
            //ячейки типа связей
            cellLinkType_Aggregation.TypeId = cellType_LinkTypeCell.CellID;
            cellLinkType_Abstraction.TypeId = cellType_LinkTypeCell.CellID; 
            cellLinkType_Collection.TypeId = cellType_LinkTypeCell.CellID; 
            //ячейки состояния связей
            cellLinkStateNormal.TypeId = cellType_LinkStateCell.CellID; 
            
            #endregion
            
            #region *** Создание связей ***

            //2) теперь надо создать связи между ячейками.
            //2.1) главное дерево World
            MLink link1 = world.S1_createLink(cellLinkType_Aggregation.CellID, MAxisDirection.Down, sys);
            MLink link2 = sys.S1_createLink(cellLinkType_Aggregation.CellID, MAxisDirection.Down, n);
            MLink link3 = sys.S1_createLink(cellLinkType_Aggregation.CellID, MAxisDirection.Down, ct);
            MLink link4 = sys.S1_createLink(cellLinkType_Aggregation.CellID, MAxisDirection.Down, cdt);
            MLink link5 = sys.S1_createLink(cellLinkType_Aggregation.CellID, MAxisDirection.Down, cs);
            MLink link6 = sys.S1_createLink(cellLinkType_Aggregation.CellID, MAxisDirection.Down, la);
            MLink link7 = sys.S1_createLink(cellLinkType_Aggregation.CellID, MAxisDirection.Down, ls);

            //2.2) дерево коллекции типов ячеек
            //- собираем коллекцию всех классов в один ряд
            MLink link8 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, bct);
            MLink link9 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntity);
            MLink link10 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntityInitial);
            MLink link11 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntityService);
            MLink link12 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntityCollection);
            MLink link13 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntityCollectionTyped);
            MLink link14 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntityCollectionCellTypeCell);
            MLink link15 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntityCollectionCellDataTypeCell);
            MLink link16 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntityCollectionCellStateCell);
            MLink link17 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntityCollectionLinkTypeCell);
            MLink link18 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellTypeEntityCollectionLinkStateCell);
            MLink link19 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellType_CellTypeCell);
            MLink link20 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellType_CellDataTypeCell);
            MLink link21 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellType_CellStateCell);
            MLink link22 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellType_LinkTypeCell);
            MLink link23 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellType_LinkStateCell);

            // - строим дерево классов если возможно. Главным классом будет базовая ячейка, хотя она ничего не делает 
            //-- Сущность - Начальная сущность и Служебная сущность
            MLink link24 = bct.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntity);
            MLink link25 = cellTypeEntity.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntityInitial);
            MLink link26 = cellTypeEntity.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntityService);
            //-- Коллекция сущностей - однотипная коллекция сущностей - коллекции ячеек типов итд
            MLink link27 = bct.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntityCollection);
            MLink link28 = cellTypeEntityCollection.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntityCollectionTyped);
            MLink link29 = cellTypeEntityCollectionTyped.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntityCollectionCellTypeCell);
            MLink link30 = cellTypeEntityCollectionTyped.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntityCollectionCellDataTypeCell);
            MLink link31 = cellTypeEntityCollectionTyped.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntityCollectionCellStateCell);
            MLink link32 = cellTypeEntityCollectionTyped.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntityCollectionLinkTypeCell);
            MLink link33 = cellTypeEntityCollectionTyped.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellTypeEntityCollectionLinkStateCell);
            //TODO: тут я забыл про иерархию классов типов ячеек? Для коллекций написал, а для простых типов - нет.
            //они должны же быть соединены с ячейкой bct все. Или нет? Я тут запутался уже в этих отношениях ячеек.
            MLink link50 = bct.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellType_CellTypeCell);
            MLink link51 = bct.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellType_CellDataTypeCell);
            MLink link52 = bct.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellType_CellStateCell);
            MLink link53 = bct.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellType_LinkTypeCell);
            MLink link54 = bct.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellType_LinkStateCell);

            //2.3) дерево коллекции типов данных ячеек
            //-собираем коллекцию всех классов в один ряд
            MLink link34 = cdt.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, bcdt);
            MLink link35 = cdt.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellDataTypeNodata);
            //-строим дерево классов если возможно. Главным классом будет базовая ячейка, хотя она ничего не делает 
            MLink link36 = bcdt.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellDataTypeNodata);


            //2.4) дерево коллекции состояний ячейки
            //-собираем коллекцию всех классов в один ряд
            MLink link37 = cs.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, bcs);
            MLink link38 = cs.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellStateNormal);
            //-строим дерево классов если возможно. Главным классом будет базовая ячейка, хотя она ничего не делает 
            MLink link39 = bcs.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellStateNormal);

            //2.5) дерево коллекции типов связи
            //-собираем коллекцию всех классов в один ряд
            MLink link40 = la.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, bla);
            MLink link41 = la.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellLinkType_Aggregation);
            MLink link42 = la.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellLinkType_Abstraction);
            MLink link48 = la.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellLinkType_Collection);
            //-строим дерево классов если возможно. Главным классом будет базовая ячейка, хотя она ничего не делает 
            MLink link43 = bla.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellLinkType_Aggregation);
            MLink link44 = bla.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellLinkType_Abstraction);
            MLink link49 = bla.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellLinkType_Collection);

            //2.6) дерево коллекции состояний связи
            //-собираем коллекцию всех классов в один ряд
            MLink link45 = ls.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, bls);
            MLink link46 = ls.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellLinkStateNormal);
            //-строим дерево классов если возможно. Главным классом будет базовая ячейка, хотя она ничего не делает 
            MLink link47 = bls.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellLinkStateNormal);

            #endregion
            #region *** Заполнение свойств связей ***
            link1.State = cellLinkStateNormal.CellID;
            link2.State = cellLinkStateNormal.CellID;
            link3.State = cellLinkStateNormal.CellID;
            link4.State = cellLinkStateNormal.CellID;
            link5.State = cellLinkStateNormal.CellID;
            link6.State = cellLinkStateNormal.CellID;
            link7.State = cellLinkStateNormal.CellID;
            link8.State = cellLinkStateNormal.CellID;
            link9.State = cellLinkStateNormal.CellID;
            link10.State = cellLinkStateNormal.CellID;
            link11.State = cellLinkStateNormal.CellID;
            link12.State = cellLinkStateNormal.CellID;
            link13.State = cellLinkStateNormal.CellID;
            link14.State = cellLinkStateNormal.CellID;
            link15.State = cellLinkStateNormal.CellID;
            link16.State = cellLinkStateNormal.CellID;
            link17.State = cellLinkStateNormal.CellID;
            link18.State = cellLinkStateNormal.CellID;
            link19.State = cellLinkStateNormal.CellID;
            link20.State = cellLinkStateNormal.CellID;
            link21.State = cellLinkStateNormal.CellID;
            link22.State = cellLinkStateNormal.CellID;
            link23.State = cellLinkStateNormal.CellID;
            link24.State = cellLinkStateNormal.CellID;
            link25.State = cellLinkStateNormal.CellID;
            link26.State = cellLinkStateNormal.CellID;
            link27.State = cellLinkStateNormal.CellID;
            link28.State = cellLinkStateNormal.CellID;
            link29.State = cellLinkStateNormal.CellID;
            link30.State = cellLinkStateNormal.CellID;
            link31.State = cellLinkStateNormal.CellID;
            link32.State = cellLinkStateNormal.CellID;
            link33.State = cellLinkStateNormal.CellID;
            link34.State = cellLinkStateNormal.CellID;
            link35.State = cellLinkStateNormal.CellID;
            link36.State = cellLinkStateNormal.CellID;
            link37.State = cellLinkStateNormal.CellID;
            link38.State = cellLinkStateNormal.CellID;
            link39.State = cellLinkStateNormal.CellID;
            link40.State = cellLinkStateNormal.CellID;
            link41.State = cellLinkStateNormal.CellID;
            link42.State = cellLinkStateNormal.CellID;
            link43.State = cellLinkStateNormal.CellID;
            link44.State = cellLinkStateNormal.CellID;
            link45.State = cellLinkStateNormal.CellID;
            link46.State = cellLinkStateNormal.CellID;
            link47.State = cellLinkStateNormal.CellID;
            link48.State = cellLinkStateNormal.CellID;
            link49.State = cellLinkStateNormal.CellID;
            link50.State = cellLinkStateNormal.CellID;
            link51.State = cellLinkStateNormal.CellID;
            link52.State = cellLinkStateNormal.CellID;
            link53.State = cellLinkStateNormal.CellID;
            link54.State = cellLinkStateNormal.CellID;
            #endregion


            #region *** Создание пользовательского каталога ***
            //ячейки каталога пользователей
            //КоллекцияАккаунтовПользователей-АккаунтПользователя
            MCell cellUserAccountCollection = engine.CellCreate(cellMode, SolutionHelper.НазваниеКоллекцияЯчейкаАккаунтаПользователя, "Ячейка КоллекцияАккаунтовПользователей");
            MCell cellUserAccountRoot = engine.CellCreate(cellMode, "ОбщийАккаунтПользователя", "Ячейка АккаунтПользователя");
            //ячейки типов ячеек
            MCell cellCellTypeUserAccountRootCell = engine.CellCreate(cellMode, SolutionHelper.НазваниеЯчейкаТипаЯчейкаАккаунтаПользователя, "Ячейка типа ячейки - АккаунтПользователя");
            MCell cellCellTypeEntityCollectionUserAccountRootCell = engine.CellCreate(cellMode, "CellType_EntityCollection_UserAccountRootCell", "Ячейка типа ячейки - КоллекцияАккаунтовПользователей");
            //свойства ячеек
            cellUserAccountCollection.State = cellStateNormal.CellID;
            cellUserAccountRoot.State = cellStateNormal.CellID;
            cellCellTypeUserAccountRootCell.State = cellStateNormal.CellID;
            cellCellTypeEntityCollectionUserAccountRootCell.State = cellStateNormal.CellID;
            //
            cellUserAccountCollection.TypeId = cellCellTypeEntityCollectionUserAccountRootCell.CellID;
            cellUserAccountRoot.TypeId = cellCellTypeUserAccountRootCell.CellID; 
            cellCellTypeUserAccountRootCell.TypeId = cellType_CellTypeCell.CellID;
            cellCellTypeEntityCollectionUserAccountRootCell.TypeId = cellType_CellTypeCell.CellID;
            //
            cellUserAccountCollection.ValueTypeId = cellDataTypeNodata.CellID;
            cellUserAccountRoot.ValueTypeId = cellDataTypeNodata.CellID;
            cellCellTypeUserAccountRootCell.ValueTypeId = cellDataTypeNodata.CellID;
            cellCellTypeEntityCollectionUserAccountRootCell.ValueTypeId = cellDataTypeNodata.CellID;
            //создание связей
            //основные связи
            MLink link100 = world.S1_createLink(cellLinkType_Aggregation.CellID, MAxisDirection.Down, cellUserAccountCollection);
            MLink link101 = cellUserAccountCollection.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellUserAccountRoot);
            //связи ячеек типов ячеек с их коллекциями
            MLink link102 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellCellTypeUserAccountRootCell);
            MLink link103 = ct.S1_createLink(cellLinkType_Collection.CellID, MAxisDirection.Down, cellCellTypeEntityCollectionUserAccountRootCell);
            //и в иерархии типов
            MLink link104 = cellTypeEntityCollectionTyped.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellCellTypeEntityCollectionUserAccountRootCell);
            MLink link105 = bct.S1_createLink(cellLinkType_Abstraction.CellID, MAxisDirection.Down, cellCellTypeUserAccountRootCell);
            //установить свойства связей
            link100.State = cellLinkStateNormal.CellID;
            link101.State = cellLinkStateNormal.CellID;
            link102.State = cellLinkStateNormal.CellID;
            link103.State = cellLinkStateNormal.CellID;
            link104.State = cellLinkStateNormal.CellID;
            link105.State = cellLinkStateNormal.CellID;

 #endregion

            //У этого проекта должна быть иконка паутины - я уже запутался в описании ячеек и связей.
            //Как можно быстро и просто строить тут структуры данных, 
            //если добавление первых 13 сущностей потребовало 33 ячейки и более 50 связей.
            //А добавление еще двух сущностей здесь - пользовательский аккаунт - 4 ячеек и 6 связей.
            //Это просто какой-то кошмар.
            //Что тут можно сделать?
            //- эти ячейки и связи описывают базовую инфраструктуру солюшена - и реализацию и структуру типов. 
            //Поэтому их так много получилось сейчас.
            //Если в будущем и дальше будет много новых типов, то и ячеек и связей будет много.
            //Это будет сложная работа, хотя ее можно автоматизировать - написать функции создания классов:
            //сразу и создавать ячейки типов ячеек и помещать их в коллекцию, и связывать с иерархией ячеек типов ячеек.
            //А если работа будет в основном с объектами, то сначала немного новых классов приплести, а потом просто объекты к ним прицеплять, даже без связей.

            //Но все равно, я вижу, что тут нужна теория и навыки построения таких структур.
            //Ее разработка займет время, конечно, но нужен опыт использования.

            //Выводы:
            //- нужен опыт использования, нужно создать методику создания и строения структуры сущностей, типовые решения.
            //- нужно способ автоматизировать операции создания структуры сущностей. Хотя они зависят от строения Структуры сущностей.
            //   Это уже конфигурация получается, как в 1С.
            //- Концентрировать это устройство системы на работу с объектами. Так меньше ячеек надо создавать и можно использовать уже существующие.
            //- Развивать Структуру сущностей - чем больше в ней применимых классов, тем меньше их придется создавать каждый раз.

            return;
        }

        /// <summary>
        /// NT- Создать структуру аккаунта пользователя
        /// </summary>
        /// <param name="engine">Объект движка</param>
        /// <param name="cellMode">Режим создаваемых ячеек</param>
        /// <param name="accountName">Название аккаунта пользователя</param>
        /// <param name="accountDescription">Описание аккаунта пользователя</param>
        /// <returns></returns>
        public static MCell СоздатьАккаунтПользователя(MEngine engine, MCellMode cellMode, String accountName, String accountDescription)
        {
            //сейчас структура аккаунта пользователя состоит из одной ячейки аккаунта пользователя.
            //в нее должны сводиться все пользовательские сущности.
            //пока нет пользовательских коллекций и т.п. структур.
            //они должны тоже тут определяться.

            //получить ячейку коллекции аккаунтов пользователя
            MCell accol = engine.CellGet(SolutionHelper.НазваниеКоллекцияЯчейкаАккаунтаПользователя);
            //- если ее нет, выбросить исключение
            if(accol == null) throw new Exception("Не найдена ячейка: " + SolutionHelper.НазваниеКоллекцияЯчейкаАккаунтаПользователя);
            //получить ячейку типа ячейки аккаунта пользователя
            MCell tau = engine.CellGet(SolutionHelper.НазваниеЯчейкаТипаЯчейкаАккаунтаПользователя);
            //- если ее нет, выбросить исключение
            if(tau == null) throw new Exception("Не найдена ячейка: " + SolutionHelper.НазваниеЯчейкаТипаЯчейкаАккаунтаПользователя);
            //получить ячейку состояния ячейки нормального
            MCell csn = engine.CellGet(SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального);
            //- если ее нет, выбросить исключение
            if(csn == null) throw new Exception("Не найдена ячейка: " + SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального);
            //получить ячейку типа данных ячейки - без данных
            MCell cdtnd = engine.CellGet(SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных);
            //- если ее нет, выбросить исключение
            if(cdtnd == null) throw new Exception("Не найдена ячейка: " + SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных);
            //Получить ячейку типа связи - коллекция
            MCell clt = engine.CellGet(SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция);
            //- если ее нет, выбросить исключение
            if(clt == null) throw new Exception("Не найдена ячейка: " + SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция);
            //Получить ячейку состояния связи - нормально
            MCell cls = engine.CellGet(SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное);

            //- если ее нет, выбросить исключение
            if(cls == null) throw new Exception("Не найдена ячейка: " + SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное);
            //создать ячейку аккаунта пользователя
            MCell cell = engine.CellCreate(cellMode, accountName, accountDescription);
            cell.State = csn.CellID;
            cell.TypeId = tau.CellID;
            cell.ValueTypeId = cdtnd.CellID;
            
            //создать связи ячейки аккаунта пользователя и ячейки коллекции аккаунтов пользователей
            MLink link1 = accol.S1_createLink(clt.CellID, MAxisDirection.Down, cell);
            link1.State = cls.CellID;//set link state as normal
            
            //вернуть ячейку аккаунта пользователя
            return cell;
        }

        /// <summary>
        /// NT-Получить из солюшена ячейки по их названиям.
        /// Названия не должны повторяться.
        /// </summary>
        /// <param name="engine">Объект движка</param>
        /// <param name="cellNames">Набор уникальных названий ячеек</param>
        /// <returns>Возвращает словарь ячеек с выборкой по названию ячейки</returns>
        public static Dictionary<String, MCell> getCells(MEngine engine, params String[] cellNames)
        {
            Dictionary<String, MCell> dict = new Dictionary<string,MCell>();
            foreach(String par in cellNames)
            {
                MCell cell = engine.CellGet(par);
                if(cell == null)
                    throw new Exception("Не найдена ячейка: " + par);
                else
                    dict.Add(par, cell);
            }

            return dict;
        }

        /// <summary>
        /// NT-Создать ячейку ЯчейкаТипаЯчейки, производную от указанной ЯчейкаТипаЯчейки.
        /// Добавляет его в коллекцию ячеек типов ячеек служебной части Структуры Сущностей Солюшена.
        /// </summary>
        /// <param name="engine">Объект движка</param>
        /// <param name="baseCell">Родительская ЯчейкаТипаЯчейки</param>
        /// <param name="cellMode">Режим создаваемой ячейки</param>
        /// <param name="cellTitle">Название создаваемой ячейки: CellType_xxx</param>
        /// <param name="cellDescription">Описание создаваемой ячейки.</param>
        /// <returns>Возвращает объект созданной ЯчейкаТипаЯчейки</returns>
        public static MCell СоздатьЯчейкуТипаЯчейки(MEngine engine, MCell baseCell, MCellMode cellMode, String cellTitle, String cellDescription )
        {
            //1. получить все нужные ячейки свойств по именам ячеек
            //вообще, нужны только их идентификаторы, но тут получаем объекты ячеек
            //а если затребованной ячейки не найдено, тут же выбрасывается исключение, и ничего не надо откатывать в Солюшене.
            Dictionary<string, MCell> refs = getCells(engine,
                SolutionHelper.НазваниеКоллекцияЯчейкаТипаЯчейки,
                SolutionHelper.НазваниеЯчейкаТипаЯчейкаТипаЯчейки,
                SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального,
                SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных,
                SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция,
                SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция,
                SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное);

            //2. получить ячейку родительского типа baseCell - передана как аргумент
            //3. Создать ячейку типа ячейки
            MCell cell = engine.CellCreate(cellMode, cellTitle, cellDescription);
            cell.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального].CellID;
            cell.TypeId = refs[SolutionHelper.НазваниеЯчейкаТипаЯчейкаТипаЯчейки].CellID;
            cell.ValueTypeId = refs[SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных].CellID;

            //4. связать ячейку с коллекцией
            MCell col = refs[SolutionHelper.НазваниеКоллекцияЯчейкаТипаЯчейки];
            MLink link1 = cell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция].CellID, MAxisDirection.Up, col);
            link1.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;
            
            //5. связать ячейку с ячейкой суперкласса
            MLink link2 = baseCell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция].CellID, MAxisDirection.Down, cell);
            link2.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;
            //6. Вернуть объект созданной ячейки
            refs.Clear();
            return cell;
        }
        /// <summary>
        /// NT-Создать ячейку ЯчейкаТипаСвязи, производную от указанной ЯчейкаТипаСвязи.
        /// Добавляет его в коллекцию ячеек типов связей служебной части Структуры Сущностей Солюшена.
        /// </summary>
        /// <param name="engine">Объект движка</param>
        /// <param name="baseCell">Родительская ЯчейкаТипаСвязи</param>
        /// <param name="cellMode">Режим создаваемой ячейки</param>
        /// <param name="cellTitle">Название создаваемой ячейки: LinkType_xxx</param>
        /// <param name="cellDescription">Описание создаваемой ячейки.</param>
        /// <returns>Возвращает объект созданной ЯчейкаТипаСвязи</returns>
        public static MCell СоздатьЯчейкуТипаСвязи(MEngine engine, MCell baseCell, MCellMode cellMode, String cellTitle, String cellDescription  )
        {
            //1. получить все нужные ячейки свойств по именам ячеек
            //вообще, нужны только их идентификаторы, но тут получаем объекты ячеек
            //а если затребованной ячейки не найдено, тут же выбрасывается исключение, и ничего не надо откатывать в Солюшене.
            Dictionary<string, MCell> refs = getCells(engine,
                SolutionHelper.НазваниеКоллекцияЯчейкаТипаСвязи,
                SolutionHelper.НазваниеЯчейкаТипаЯчейкаТипаСвязи,
                SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального,
                SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных,
                SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция,
                SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция,
                SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное);

            //2. получить ячейку родительского типа baseCell - передана как аргумент
            //3. Создать ячейку типа ячейки
            MCell cell = engine.CellCreate(cellMode, cellTitle, cellDescription);
            cell.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального].CellID;
            cell.TypeId = refs[SolutionHelper.НазваниеЯчейкаТипаЯчейкаТипаСвязи].CellID;
            cell.ValueTypeId = refs[SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных].CellID;

            //4. связать ячейку с коллекцией
            MCell col = refs[SolutionHelper.НазваниеКоллекцияЯчейкаТипаСвязи];
            MLink link1 = cell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция].CellID, MAxisDirection.Up, col);
            link1.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;

            //5. связать ячейку с ячейкой суперкласса
            MLink link2 = baseCell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция].CellID, MAxisDirection.Down, cell);
            link2.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;
            //6. Вернуть объект созданной ячейки
            refs.Clear();
            return cell;
        }
        /// <summary>
        /// NT-Создать ячейку ЯчейкаТипаДанныхЯчейки, производную от указанной ЯчейкаТипаДанныхЯчейки.
        /// Добавляет его в коллекцию ячеек типов данных ячейки служебной части Структуры Сущностей Солюшена.
        /// </summary>
        /// <param name="engine">Объект движка</param>
        /// <param name="baseCell">Родительская ЯчейкаТипаДанныхЯчейки</param>
        /// <param name="cellMode">Режим создаваемой ячейки</param>
        /// <param name="cellTitle">Название создаваемой ячейки: CellDataType_xxx</param>
        /// <param name="cellDescription">Описание создаваемой ячейки.</param>
        /// <returns>Возвращает объект созданной ЯчейкаТипаДанныхЯчейки</returns>
        public static MCell СоздатьЯчейкуТипаДанныхЯчейки(MEngine engine, MCell baseCell, MCellMode cellMode, String cellTitle, String cellDescription  )
        {
            //1. получить все нужные ячейки свойств по именам ячеек
            //вообще, нужны только их идентификаторы, но тут получаем объекты ячеек
            //а если затребованной ячейки не найдено, тут же выбрасывается исключение, и ничего не надо откатывать в Солюшене.
            Dictionary<string, MCell> refs = getCells(engine,
                SolutionHelper.НазваниеКоллекцияЯчейкаТипаДанныхЯчейки,
                SolutionHelper.НазваниеЯчейкаТипаЯчейкаТипаДанныхЯчейки,
                SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального,
                SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных,
                SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция,
                SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция,
                SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное);

            //2. получить ячейку родительского типа baseCell - передана как аргумент
            //3. Создать ячейку типа ячейки
            MCell cell = engine.CellCreate(cellMode, cellTitle, cellDescription);
            cell.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального].CellID;
            cell.TypeId = refs[SolutionHelper.НазваниеЯчейкаТипаЯчейкаТипаДанныхЯчейки].CellID;
            cell.ValueTypeId = refs[SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных].CellID;

            //4. связать ячейку с коллекцией
            MCell col = refs[SolutionHelper.НазваниеКоллекцияЯчейкаТипаДанныхЯчейки];
            MLink link1 = cell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция].CellID, MAxisDirection.Up, col);
            link1.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;

            //5. связать ячейку с ячейкой суперкласса
            MLink link2 = baseCell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция].CellID, MAxisDirection.Down, cell);
            link2.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;
            //6. Вернуть объект созданной ячейки
            refs.Clear();
            return cell;
        }
        /// <summary>
        /// NT-Создать ячейку ЯчейкаСостоянияЯчейки, производную от указанной ЯчейкаСостоянияЯчейки.
        /// Добавляет его в коллекцию ячеек Состояния Ячейки служебной части Структуры Сущностей Солюшена.
        /// </summary>
        /// <param name="engine">Объект движка</param>
        /// <param name="baseCell">Родительская ЯчейкаСостоянияЯчейки</param>
        /// <param name="cellMode">Режим создаваемой ячейки</param>
        /// <param name="cellTitle">Название создаваемой ячейки: CellState_xxx</param>
        /// <param name="cellDescription">Описание создаваемой ячейки.</param>
        /// <returns>Возвращает объект созданной ЯчейкаСостоянияЯчейки</returns>
        public static MCell СоздатьЯчейкуСостоянияЯчейки(MEngine engine, MCell baseCell, MCellMode cellMode, String cellTitle, String cellDescription  )
        {
            //1. получить все нужные ячейки свойств по именам ячеек
            //вообще, нужны только их идентификаторы, но тут получаем объекты ячеек
            //а если затребованной ячейки не найдено, тут же выбрасывается исключение, и ничего не надо откатывать в Солюшене.
            Dictionary<string, MCell> refs = getCells(engine,
                SolutionHelper.НазваниеКоллекцияЯчейкаСостоянияЯчейки,
                SolutionHelper.НазваниеЯчейкаТипаЯчейкаСостоянияЯчейки,
                SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального,
                SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных,
                SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция,
                SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция,
                SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное);

            //2. получить ячейку родительского типа baseCell - передана как аргумент
            //3. Создать ячейку типа ячейки
            MCell cell = engine.CellCreate(cellMode, cellTitle, cellDescription);
            cell.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального].CellID;
            cell.TypeId = refs[SolutionHelper.НазваниеЯчейкаТипаЯчейкаСостоянияЯчейки].CellID;
            cell.ValueTypeId = refs[SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных].CellID;

            //4. связать ячейку с коллекцией
            MCell col = refs[SolutionHelper.НазваниеКоллекцияЯчейкаСостоянияЯчейки];
            MLink link1 = cell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция].CellID, MAxisDirection.Up, col);
            link1.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;

            //5. связать ячейку с ячейкой суперкласса
            MLink link2 = baseCell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция].CellID, MAxisDirection.Down, cell);
            link2.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;
            //6. Вернуть объект созданной ячейки
            refs.Clear();
            return cell;
        }
        /// <summary>
        /// NT-Создать ячейку ЯчейкаСостоянияСвязи, производную от указанной ЯчейкаСостоянияСвязи.
        /// Добавляет его в коллекцию ячеек Состояния Связислужебной части Структуры Сущностей Солюшена.
        /// </summary>
        /// <param name="engine">Объект движка</param>
        /// <param name="baseCell">Родительская ЯчейкаСостоянияСвязи</param>
        /// <param name="cellMode">Режим создаваемой ячейки</param>
        /// <param name="cellTitle">Название создаваемой ячейки: LinkState_xxx</param>
        /// <param name="cellDescription">Описание создаваемой ячейки.</param>
        /// <returns>Возвращает объект созданной ЯчейкаСостоянияСвязи</returns>
        public static MCell СоздатьЯчейкуСостоянияСвязи(MEngine engine, MCell baseCell, MCellMode cellMode, String cellTitle, String cellDescription)
        {
            //1. получить все нужные ячейки свойств по именам ячеек
            //вообще, нужны только их идентификаторы, но тут получаем объекты ячеек
            //а если затребованной ячейки не найдено, тут же выбрасывается исключение, и ничего не надо откатывать в Солюшене.
            Dictionary<string, MCell> refs = getCells(engine,
                SolutionHelper.НазваниеКоллекцияЯчейкаСостоянияСвязи,
                SolutionHelper.НазваниеЯчейкаТипаЯчейкаСостоянияСвязи,
                SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального,
                SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных,
                SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция,
                SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция,
                SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное);

            //2. получить ячейку родительского типа baseCell - передана как аргумент
            //3. Создать ячейку типа ячейки
            MCell cell = engine.CellCreate(cellMode, cellTitle, cellDescription);
            cell.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияЯчейкиНормального].CellID;
            cell.TypeId = refs[SolutionHelper.НазваниеЯчейкаТипаЯчейкаСостоянияСвязи].CellID;
            cell.ValueTypeId = refs[SolutionHelper.НазваниеЯчейкаТипаДанныхЯчейкиБезДанных].CellID;

            //4. связать ячейку с коллекцией
            MCell col = refs[SolutionHelper.НазваниеКоллекцияЯчейкаСостоянияСвязи];
            MLink link1 = cell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиКоллекция].CellID, MAxisDirection.Up, col);
            link1.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;

            //5. связать ячейку с ячейкой суперкласса
            MLink link2 = baseCell.S1_createLink(refs[SolutionHelper.НазваниеЯчейкаТипаСвязиАбстракция].CellID, MAxisDirection.Down, cell);
            link2.State = refs[SolutionHelper.НазваниеЯчейкаСостоянияСвязиНормальное].CellID;
            //6. Вернуть объект созданной ячейки
            refs.Clear();
            return cell;
        }





    }
}
