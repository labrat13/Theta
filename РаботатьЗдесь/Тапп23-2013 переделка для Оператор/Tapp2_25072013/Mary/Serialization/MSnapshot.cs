using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mary.Serialization;

namespace Mary
{
    public class MSnapshot
    {

        #region Fields

        /// <summary>
        /// ссылка на контейнер для доступа
        /// </summary>
        private MEngine m_containerRef;
        /// <summary>
        /// Snapshot file signature 8 bytes. TODO: replace to appropriate ANSI letters
        /// </summary>
        public const UInt64 SnapshotFileSignature = 0x313033324152454D;  //MERA2301
        #endregion

        /// <summary>
        /// param constructor
        /// </summary>
        /// <param name="container">Engine reference</param>
        public MSnapshot(MEngine container)
        {
            //m_step = 0;
            m_containerRef = container;

        }


        #region Properties

        /// <summary>
        /// шаг для файла, номер версии данных.
        /// </summary>
        public int Step
        {
            get
            {
                return this.m_containerRef.SolutionSettings.SolutionVersion.SolutionStepNumber;
            }
            set
            {
                this.m_containerRef.SolutionSettings.SolutionVersion.SolutionStepNumber = value;
            }
        }


        #endregion


        /// <summary>
        /// NR-Init snapshot manager
        /// </summary>
        internal void Open(MSolutionInfo projectInfo)
        {
            //this.m_step = projectInfo.SolutionVersion.SolutionStepNumber;
        }

        /// <summary>
        /// NR-Exit snapshot manager
        /// </summary>
        internal void Close()
        {

        }

        /// <summary>
        /// Create pathname for new full snapshot file, based on Step value.
        /// Step value must be incremented after snapshot saving and after loading.
        /// </summary>
        /// <returns></returns>
        private string getNewFullSnapshotName()
        {
            //filename like ..\SolutionName.001.step
            string file = this.m_containerRef.SolutionSettings.getSolutionName16();//TAG:RENEW-13112017
            //add step number and ext
            file = String.Format("{0}.{1}.step", file, this.Step);
            //add full path
            string s = Path.Combine(this.m_containerRef.SolutionManager.SnapshotFolderPath, file);
            return s;
        }

        #region LoadSnapshot functions
        /// <summary>
        /// NR-Load snapshot file to current cleared container.
        /// </summary>
        /// <param name="filename"></param>
        /// <remarks>Пока только каркас</remarks>
        public void LoadFullSnapshot(string pathname)
        {
            //Int64 beginPos = 0;
            //Int64 endPos = 0;
            //Int32 sectLen = 0;
            //Int64 sectLen64 = 0;
            BinaryReader br = null;
            //open file
            br = new BinaryReader(File.Open(pathname, FileMode.Open, FileAccess.Read, FileShare.Read));

            //read header section
            //можно загрузить SnapshotFileInfo класс, получить из него все данные, и не возиться здесь
            ////read signature
            //UInt64 signature = br.ReadUInt64();
            //if (signature != MSnapshot.SnapshotFileSignature)
            //    throw new Exception("Invalid snapshot file signature");
            ////read section length
            //beginPos = br.BaseStream.Position;
            //sectLen = br.ReadInt32();
            ///
            ////get num of cells, refcells, links and compare with free memory - skip
            //br.BaseStream.Position = br.BaseStream.Position + 12;
            ////check version
            //int verId = br.ReadInt32();
            //br.BaseStream.Position = br.BaseStream.Position + 4; //int sverId = br.ReadInt32(); 
            //if(!MVersion.isCompatibleVersion(verId))
            //    throw new Exception("Несовместимая версия");
            //br.BaseStream.Position = br.BaseStream.Position + 4; //read structure version - skip
            ////проверить что снимок полный, иначе выдать исключение.
            //MSnapshotType ty = (MSnapshotType)(int)br.ReadByte();
            //if (ty != MSnapshotType.Solid) 
            //    throw new Exception("Неправильный тип снимка");
            ////description and name - skip

            MSnapshotFileInfo msfi = new MSnapshotFileInfo();
            msfi.LoadHeader(br);//file position - on next section descriptor
            //check engine version
            //if(!MVersion.isCompatibleVersion(msfi.EngineVersion))//TODO: TAGVERSIONNEW: 
            //    throw new Exception("Несовместимая версия");
            if (!this.m_containerRef.SolutionSettings.CurrentEngineVersion.isCompatibleVersion(msfi.EngineVersion))//TODO: TAGVERSIONNEW: изменено поле EngineVersion в MSnapshotFileInfo
                throw new Exception("Несовместимая версия");
            //проверить что снимок полный, иначе выдать исключение.
            if (msfi.SnapshotType != MSnapshotType.Solid)
                throw new Exception("Неправильный тип снимка");

            //секция контейнера
            //позиция чтения должна быть на дескрипторе секции контейнера
            this.m_containerRef.fromBinary(br);

            ////секция ячеек в памяти
            this.LoadSection(br, MSerialRecordType.MemoryCellSection);
            ////секция ячеек в таблице
            this.LoadSection(br, MSerialRecordType.TableCellSection);
            ////секция внешних ячеек
            ////чего с внешними ячейками делать?
            this.LoadSection(br, MSerialRecordType.RefCellSection);
            ////секция связей
            this.LoadSection(br, MSerialRecordType.LinkSection);
            //проверить дескриптор конца файла
            if (MSerialRecordType.Footer != (MSerialRecordType)(int)br.ReadByte())
                throw new Exception("Неправильный формат файла снимка");
            //не увеличивать шаг после загрузки из снимка.
            
            return;

        }
        /// <summary>
        /// NT-Load section of elements
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sectionType"></param>
        /// <remarks>Предполагается, что позиция чтения - на дескрипторе секции.</remarks>
        private void LoadSection(BinaryReader reader, MSerialRecordType sectionType)
        {
            int numOfElements = 0;
            Int64 beginPos = 0;
            Int64 endPos = 0;
            Int16 crcval = 0;
            Int64 sectionLength = 0;


            //1 check section type
            MSerialRecordType ty = (MSerialRecordType)(int)reader.ReadByte();
            if (ty != sectionType)
                throw new Exception("Invalid section descriptor");
            //2 read section length
            beginPos = reader.BaseStream.Position;
            sectionLength = reader.ReadInt64();
            //3 read num of elements
            numOfElements = reader.ReadInt32();
            //4 read crc checksum
            crcval = reader.ReadInt16();
            //calc checksum
            Int16 crc = MCrc16.CalculateCrc16FromStream(reader.BaseStream, beginPos, 12);
            if (crcval != crc)
                throw new Exception("Invalid section crc");
            //5 elements
            //Экономить память, обрабатывать по одной.
            switch (sectionType)
            {
                case MSerialRecordType.MemoryCellSection:
                    LoadMemoryCells(reader, numOfElements);
                    break;
                case MSerialRecordType.TableCellSection:
                    LoadTableCells(reader, numOfElements);
                    break;
                case MSerialRecordType.RefCellSection:
                    CheckRefCells(reader, numOfElements); //TODO: нет внешних ячеек, поэтому ничего не делаем
                    break;
                case MSerialRecordType.LinkSection:
                    LoadAllLinks(reader, numOfElements);
                    break;
                default:
                    throw new Exception("invalid section type");
            }
            //сейчас позиция чтения должна быть на дескрипторе следующей секции.
            return;
        }
        /// <summary>
        /// NT-Read links and place to container and table
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="numOfElements"></param>
        private void LoadAllLinks(BinaryReader reader, int numOfElements)
        {
            bool flag;

            for (int i = 0; i < numOfElements; i++)
            {
                MLink link = new MLink();
                link.fromBinary(reader);
                //if link is temporary, write to container
                //+if one of linked cells exists in memory, write to container and cells
                //+if link is constant, write to table and replace id

                //check link is constant, add to database and replace id
                if (link.isLinkNotTemporary)
                    this.m_containerRef.DataLayer.LinkInsertGetId(link);
                //check cells in memory
                MCell cell = this.m_containerRef.Cells.S1_getCell(link.downCellID);
                flag = false; //for any cell present in memory
                if (cell != null)
                {
                    flag = true; //add to container
                    //set cell reference in link 
                    link.downCell = cell;
                    //if cell not MCellA, add link to cell link list
                    if(cell.isLargeCell)
                        cell.Links.AddLink(link);
                }
                cell = this.m_containerRef.Cells.S1_getCell(link.upCellID);
                if (cell != null)
                {
                    flag = true; //add to container
                    //set cell reference in link 
                    link.upCell = cell;
                    //if cell not MCellA, add link to cell link list
                    if(cell.isLargeCell)
                        cell.Links.AddLink(link);
                }
                //add link to container if any cell exists
                if (flag == true)
                    this.m_containerRef.Links.AddLink(link);
            }
            return;
        }

        /// <summary>
        /// NT-Read cells and place to table
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="numOfElements"></param>
        private void LoadTableCells(BinaryReader reader, int numOfElements)
        {
            //загружаем MCellB ячейки и CellInsert()
            //позиция чтения должна быть на дескрипторе секции
            //читаем в цикле по счетчику
            for (int i = 0; i < numOfElements; i++)
            {
                //предлагается читать в MCellB объект, возвращая cellmode.
                //и соответственно потом переделывать.
                //при загрузке MCellB.fromBinary() проперти не используются, поэтому проблем быть не должно.
                MCellB cell = new MCellB(); //можно создать только один раз, все равно все поля перезаписываются.
                cell.fromBinary(reader);
                this.m_containerRef.DataLayer.CellInsert(cell);
            }
        }
        /// <summary>
        /// NR-Check identity of referenced cells for snapshot.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="numOfElements"></param>
        private void CheckRefCells(BinaryReader reader, int numOfElements)
        {
            //ничего не делаем - пока нет внешних ячеек и их концепции.
            //тут надо проверять совпадение состояния существующих ячеек с прописанными в снимке, формировать список
            //несовпадающих ячеек и выдавать его пользователю на рассмотрение проблем согласования версий.
        }
        /// <summary>
        /// NT-Read cells and place to container memory appropriate to celltype.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="numOfElements">number of elements</param>
        private void LoadMemoryCells(BinaryReader reader, int numOfElements)
        {
            //позиция чтения должна быть на дескрипторе секции
            //читаем в цикле по счетчику
            for (int i = 0; i < numOfElements; i++)
            {
                //предлагается читать в MCellB объект, возвращая cellmode.
                //и соответственно потом переделывать.
                //при загрузке MCellB.fromBinary() проперти не используются, поэтому проблем быть не должно.
                MCell cell = new MCellB();
                cell.fromBinary(reader);
                if (cell.CellMode == MCellMode.Compact)
                {
                    //копируем идентификатор ячейки, уничтожаем временный объект.
                    MCellA tt = new MCellA();
                    tt.CellID = cell.CellID;
                    cell = tt;
                }
                //add to container
                this.m_containerRef.Cells.S1_AddCell(cell);
            }
            return;
        }


        #endregion

        #region SaveSnapshot functions

        /// <summary>
        /// NT - universal section saving
        /// </summary>
        /// <param name="sectionType">recorded section type</param>
        /// <param name="writer"></param>
        private void SaveSection(BinaryWriter writer, MSerialRecordType sectionType)
        {
            int numOfElements = 0;
            Int64 beginPos = 0;
            Int64 endPos = 0;
            Int16 crcval = 0;
            Int64 sectionLength = 0;

            //1 section type
            writer.Write((byte)((int) sectionType));
            //2 section size 8 bytes
            beginPos = writer.BaseStream.Position;//store writing point
            writer.Write(sectionLength); //0 
            //3 num of items 4bytes
            writer.Write(numOfElements); //0
            //4 crc16 checksum
            writer.Write(crcval); //0
            //5 elements
            //Экономить память, из таблицы выводить порциями, а из памяти перебором.
            switch (sectionType)
            {
                case MSerialRecordType.MemoryCellSection:
                    numOfElements = SaveMemoryCells(writer);
                    break;
                case MSerialRecordType.TableCellSection:
                    //Экономить память, из таблицы выводить порциями, а из памяти перебором.
                    numOfElements = SaveTableCells(writer);
                    break;
                case MSerialRecordType.RefCellSection:
                    numOfElements = SaveRefCells(writer, new List<MID>()); //TODO: пока неясно как получать список внешних ячеек, передаем пустой список.
                    break;
                case MSerialRecordType.LinkSection:
                    numOfElements = SaveAllLinks(writer);
                    break;
                default:
                    throw new Exception("invalid section type");
            }
            //write section length ant num of elements
            endPos = writer.BaseStream.Position;//store current position
            sectionLength = endPos - beginPos - 8;
            writer.BaseStream.Position = beginPos;
            writer.Write(sectionLength); //store section length
            writer.Write(numOfElements); //store item counter
            //calc crc16 for section without items = sectionSize and numOfElements = 8 + 4 = 12bytes
            crcval = MCrc16.CalculateCrc16FromStream(writer.BaseStream, beginPos, 12);
            writer.Write(crcval);
            writer.BaseStream.Position = endPos; //restore current position
            return;
        }

        

        /// <summary>
        /// NT-Save snapshot file from current container
        /// </summary>
        /// <param name="filename"></param>
        /// <remarks>After snapshot saving, increment step value</remarks>
        public void SaveFullSnapshot()
        {
            //увеличить версию снимка перед выгрузкой снимка. Не изменять при загрузке снимка.
            this.Step++;

            //UInt32 LinkCounter = 0;//links in snapshot
            //UInt32 CellCounter = 0;//cells in snapshot
            //UInt32 RefCellCounter = 0; //referenced cells in snapshot
            //Int64 SectionLength; //length of links/cells section's (may be big)
            //Int64 SectionBeginPos;//writing point for section length storing
            //Int16 crcval; //crc value
            
            //get filename
            string filename = this.getNewFullSnapshotName();
            //1-создать файл
            BinaryWriter bw = new BinaryWriter(File.Create(filename));
            //2-записать шапку снимка

            ////2.1 signature 8 bytes
            //bw.Write(MSnapshot.SnapshotFileSignature);
            ////get file position
            //Int64 firstPos = bw.BaseStream.Position;
            ////2.2 section size 4 bytes - include crc but not include signature and length
            //bw.Write(0);
            ////2.5 num of cells in snapshot - 4 bytes
            ////вписать после вывода ячеек - в конце файла
            //bw.Write(0);
            ////2.5.1 num of referenced cells in snapshot - 4 bytes
            ////вписать после вывода ячеек - в конце файла
            //bw.Write(0);
            ////2.6 num of links in snapshot - 4 bytes
            ////вписать после вывода связей - в конце файла
            //bw.Write(0);
            ////2.3 engine version
            //bw.Write(MVersion.VersionID);
            ////TODO: надо ли также SubVersion записывать?
            //bw.Write(MVersion.VersionSubID);//на всякий случай запишем
            ////2.4 structure version ??? чего это такое и где взять?
            //bw.Write(this.m_step);
            ////2.7 snapshot type
            //bw.Write((byte)((int)Serialization.MSnapshotType.Solid));
            ////2.8 project name string
            //bw.Write(this.m_containerRef.SolutionManager.ProjectFile.SolutionName);
            ////2.9 project description string
            //bw.Write(this.m_containerRef.SolutionManager.ProjectFile.Description);
            //bw.Write((Int16)0); //checksum
            ////...еще чего для шапки?
            ////store section length
            //Int64 lastPos = bw.BaseStream.Position; //end of header
            //Int32 headerLength = (Int32)(lastPos - firstPos); //store size of header section
            //bw.BaseStream.Position = firstPos;
            //bw.Write(headerLength - 4); //store header section size without sectionLength field
            //bw.BaseStream.Position = lastPos; //restore file position
            ////store counters and crc after all other - see end of function

            //above has been replaced by MSnapshotFileInfo
            MSnapshotFileInfo info = new MSnapshotFileInfo();
            info.getContainerState(this.m_containerRef); //load container's values
            info.SnapshotType = MSnapshotType.Solid;//set snapshot type
            info.SaveHeader(bw);//write header section

            //3-записать контейнер
            this.m_containerRef.toBinary(bw);
            
            //4-записать секцию ячеек снимка из памяти
            ////4.1 section type
            //bw.Write((byte)((int)Serialization.MSerialRecordType.MemoryCellSection));
            ////4.2 section size 8 bytes
            //SectionBeginPos = bw.BaseStream.Position;//store writing point
            //bw.Write((Int64)0);
            ////4.3 num of cells 4bytes
            //bw.Write(0);
            ////4.4 cells
            ////Экономить память, из таблицы выводить порциями, а из памяти перебором.
            //CellCounter = SaveMemoryCells(bw);
            ////4.5 checksum without cells 2 bytes
            //bw.Write((Int16)0); //Должно быть последним полем
            ////4.6 ...?
            ////write section length
            //lastPos = bw.BaseStream.Position;//store current position
            //SectionLength = lastPos - SectionBeginPos;
            //bw.BaseStream.Position = SectionBeginPos;
            //bw.Write(SectionLength - 8); //store section length
            //bw.Write(CellCounter); //store cell counter
            //bw.BaseStream.Position = lastPos; //restore current position
            ////calc crc16 for section without cells = sectionSize and numOfCells = 8 + 4 = 12bytes
            //crcval = MCrc16.CalculateCrc16FromStream(bw.BaseStream, SectionBeginPos, 12);
            //bw.BaseStream.Position = lastPos - 2;
            //bw.Write(crcval);
            //bw.BaseStream.Position = lastPos; //restore current position

            this.SaveSection(bw, MSerialRecordType.MemoryCellSection);


            //4-записать секцию ячеек снимка из таблицы
            ////4.1 section type
            //bw.Write((byte)((int)Serialization.MSerialRecordType.TableCellSection));
            ////4.2 section size 8 bytes
            //SectionBeginPos = bw.BaseStream.Position;//store writing point
            //bw.Write((Int64)0);
            ////4.3 num of cells 4bytes
            //bw.Write(0);
            ////4.4 cells
            ////Экономить память, из таблицы выводить порциями, а из памяти перебором.
            //CellCounter += SaveTableCells(bw);
            ////4.5 checksum without cells 2 bytes
            //bw.Write((Int16)0); //TODO: пока не используем
            ////4.6 ...?
            ////write section length
            //lastPos = bw.BaseStream.Position;//store current position
            //SectionLength = lastPos - SectionBeginPos;
            //bw.BaseStream.Position = SectionBeginPos;
            //bw.Write(SectionLength - 8); //store section length
            //bw.Write(CellCounter); //store cell counter
            //bw.BaseStream.Position = lastPos; //restore current position
            ////calc crc16 for section without cells = sectionSize and numOfCells = 8 + 4 = 12bytes
            //crcval = MCrc16.CalculateCrc16FromStream(bw.BaseStream, SectionBeginPos, 12);
            //bw.BaseStream.Position = lastPos - 2;
            //bw.Write(crcval);
            //bw.BaseStream.Position = lastPos; //restore current position

            this.SaveSection(bw, MSerialRecordType.TableCellSection);


            //Если есть внешние ячейки, записать также секцию связанных ячеек.
            // //как получить список идентификаторов внешних ячеек, связанных с контейнером?
            ////41.1 section type
            //bw.Write((byte)((int)Serialization.MSerialRecordType.RefCellSection));
            ////41.2 section size 8 bytes
            //SectionBeginPos = bw.BaseStream.Position;//store writing point
            //bw.Write((Int64)0);
            ////41.3 num of cells 4bytes
            //bw.Write(0);
            ////41.4 cells
            ////TODO: вывести все ячейки проекта
            ////Экономить память, из таблицы выводить порциями, а из памяти перебором.
            //RefCellCounter = SaveRefCells(bw, new List<MID>()); //TODO: пока неясно как получать список внешних ячеек, передаем пустой список.
            ////41.5 checksum without cells 2 bytes
            //bw.Write((Int16)0); //TODO: пока не используем
            ////41.6 ...?
            ////write section length
            //lastPos = bw.BaseStream.Position;//store current position
            //SectionLength = lastPos - SectionBeginPos;
            //bw.BaseStream.Position = SectionBeginPos;
            //bw.Write(SectionLength - 8); //store section length
            //bw.Write(RefCellCounter); //store cell counter
            //bw.BaseStream.Position = lastPos; //restore current position
            ////calc crc16 for section without cells = sectionSize and numOfCells = 8 + 4 = 12bytes
            //crcval = MCrc16.CalculateCrc16FromStream(bw.BaseStream, SectionBeginPos, 12);
            //bw.BaseStream.Position = lastPos - 2;
            //bw.Write(crcval);
            //bw.BaseStream.Position = lastPos; //restore current position

            this.SaveSection(bw, MSerialRecordType.RefCellSection);

            //5-записать секцию связей 
            ////5.1 section type
            //bw.Write((byte)((int)Serialization.MSerialRecordType.LinkSection));
            ////5.2 section size 8 bytes
            //SectionBeginPos = bw.BaseStream.Position;
            //bw.Write((Int64)0);
            ////5.3 num of links 4bytes
            //bw.Write(0);
            ////5.4 links
            ////TODO: вывести все связи проекта, подсчитав их количество. 
            ////Экономить память, из таблицы выводить порциями, а из памяти перебором. 
            //LinkCounter = SaveAllLinks(bw);
            ////5.5 checksum without links 2 bytes
            //bw.Write((Int16)0); //TODO: пока не используем
            ////5.6 ...?
            ////write section length
            //lastPos = bw.BaseStream.Position;//store current position
            //SectionLength = lastPos - SectionBeginPos;
            //bw.BaseStream.Position = SectionBeginPos;
            //bw.Write(SectionLength - 8); //store section length
            //bw.Write(LinkCounter); //store link counter
            //bw.BaseStream.Position = lastPos; //restore current position
            ////calc crc16 for section without cells = sectionSize and numOfLinks = 8 + 4 = 12bytes
            //crcval = MCrc16.CalculateCrc16FromStream(bw.BaseStream, SectionBeginPos, 12);
            //bw.BaseStream.Position = lastPos - 2;
            //bw.Write(crcval);
            //bw.BaseStream.Position = lastPos; //restore current position
            this.SaveSection(bw, MSerialRecordType.LinkSection);

            //6-закрыть файл
            //store cell and link counters in header
            //lastPos = bw.BaseStream.Position;
            //bw.BaseStream.Position = firstPos + 4; //after HeaderSize field
            //bw.Write(CellCounter); //4 bytes
            //bw.Write(RefCellCounter);//4 bytes
            //bw.Write(LinkCounter); //4 bytes
            ////header crc calculation
            //crcval = MCrc16.CalculateCrc16FromStream(bw.BaseStream, firstPos, headerLength - 2);//get bytes from first(section size) to last before crc field
            //bw.BaseStream.Position = firstPos + headerLength - 2; //to crc16 field
            //bw.Write(crcval);
            //bw.BaseStream.Position = lastPos; //restore current position

            //write footer section code
            bw.Write((byte)((int)Serialization.MSerialRecordType.Footer));
            bw.Close();

            return;
        }

        /// <summary>
        /// NT-Save all links from memory and tables. Returns number of saved links
        /// </summary>
        /// <param name="bw"></param>
        /// <returns>Saved links counter</returns>
        private int SaveAllLinks(BinaryWriter bw)
        {
            int res = SaveLinksFromMemory(bw);
            res += SaveLinksFromDatabase(bw);
            return res;
        }
        /// <summary>
        /// NR-Save all const links from database
        /// </summary>
        /// <param name="bw"></param>
        /// <returns></returns>
        private int SaveLinksFromDatabase(BinaryWriter bw)
        {
            //PRJNODB: if project not have database, return
            if (!this.m_containerRef.SolutionManager.UsesDatabase)
                return 0; 
            //получать из БД связи порциями по ХЗ штук и выводить
            //несколько ступеней размера: 8192-256-1
            //сначала надо определить диапазон чисел, потом цикл по 8192 связи.
            //функция dblayer принимает числа от и до и возвращает список связей.
            Int32 linkcounter = 0;
            Int32 minLinkId = this.m_containerRef.DataLayer.getMinLinkId();
            Int32 maxLinkId = this.m_containerRef.DataLayer.getMaxLinkId();
            if (maxLinkId == 0) return 0; //return if no any links in table
            //get number of steps for 8192 rows per step
            Int32 rows = 1 + maxLinkId - minLinkId;
            Int32 steps = rows / 8192; //целое без дробной части
            if ((rows % 8192) != 0) steps++;//учитываем остаток деления
            //циклично обрабатываем шаги
            Int32 rowFrom = minLinkId;
            Int32 rowTo;
            List<MLink> linkList = null;
            for (int i = 0; i < steps; i++)
            {
                //get and process 8192 rows per query
                rowTo = rowFrom + 8192;//не включительно!
                if (GetLinkRows(rowFrom, rowTo, ref linkList) == false)
                {
                    //невозможно получить 8192 записи за раз - пробуем по 256.
                    //try to 256 rows per query - 32 rounds
                    for (int j = 0; j < 32; j++)
                    {
                        rowTo = rowFrom + 256; //не включительно!
                        if (GetLinkRows(rowFrom, rowTo, ref linkList) == false)
                        {
                            //невозможно получить 256 записи за раз - пробуем по одной.
                            //try to 1 row per query
                            for (int k = 0; k < 256; k++)
                            {
                                rowTo = rowFrom + 1; //не включительно!
                                if (GetLinkRows(rowFrom, rowTo, ref linkList) == false)
                                {
                                    throw new Exception("Unable get link from table");  //невозможно получить даже одну запись, прекращаем работу.
                                }
                                else
                                {
                                    SerializeLinks(linkList, bw);
                                    linkcounter += linkList.Count;
                                }
                                rowFrom = rowTo;
                            }
                        }
                        else
                        {
                            SerializeLinks(linkList, bw);
                            linkcounter += linkList.Count;
                        }
                        rowFrom = rowTo;
                    }
                }
                else
                {
                    SerializeLinks(linkList, bw);
                    linkcounter += linkList.Count;
                }
                rowFrom = rowTo;
            }
            //end for
            return linkcounter;
        }
        /// <summary>
        /// Serialize list of links
        /// </summary>
        /// <param name="linkList"></param>
        /// <param name="bw"></param>
        private void SerializeLinks(List<MLink> linkList, BinaryWriter bw)
        {
            foreach (MLink li in linkList)
                li.toBinary(bw);
        }
        /// <summary>
        /// NT-Get band of links. Return true if success, false if any errors.
        /// </summary>
        private bool GetLinkRows(int rowFrom, int rowTo, ref List<MLink> linkList)
        {
            bool res = false;
            try
            {
                //get cells
                linkList = this.m_containerRef.DataLayer.getBlockOfLinks(rowFrom, rowTo).Items;
                res = true;
            }
            catch (Exception)
            {
            }
            finally
            {

            }
            return res;

        }

        /// <summary>
        /// NT-Save all temp links from memory. Return number of saved elements
        /// </summary>
        /// <param name="bw"></param>
        /// <returns></returns>
        private int SaveLinksFromMemory(BinaryWriter bw)
        {
            Int32 cnt = 0;
            //проходом по списку связей контейнера сохранять только временные связи
            foreach (MLink li in this.m_containerRef.Links.Items)
            {
                if (!li.isLinkNotTemporary)
                {
                    li.toBinary(bw);
                    cnt++;
                }
            }
            return cnt;
        }

        /// <summary>
        /// NT-Save all (constant) cells from database. If project not use database, return 0, else return number of saved elements
        /// </summary>
        /// <param name="bw"></param>
        /// <returns>Saved cells counter</returns>
        private int SaveTableCells(BinaryWriter bw)
        {
            //PRJNODB: if project not have database, return
            if (!this.m_containerRef.SolutionManager.UsesDatabase)
                return 0; 
            //получать из БД ячейки порциями по ХЗ штук и выводить
            //несколько ступеней размера: 1024-32-1
            //сначала надо определить диапазон чисел, потом цикл по 1024 ячейки.
            //функция dblayer принимает числа от и до и возвращает список ячеек.
            Int32 cellcounter = 0;
            Int32 minCellId = this.m_containerRef.DataLayer.getMinCellId();
            Int32 maxCellId = this.m_containerRef.DataLayer.getMaxCellId();
            if (maxCellId == 0) return 0; //return if no any cells in table
            //get number of steps for 1024 rows per step
            Int32 rows = 1 + maxCellId - minCellId;
            Int32 steps = rows / 1024; //целое без дробной части
            if ((rows % 1024) != 0) steps++;//учитываем остаток деления
            //циклично обрабатываем шаги
            Int32 rowFrom = minCellId;
            Int32 rowTo;
            List<MCell> cellList = null;
            for (int i = 0; i < steps; i++)
            {
                //get and process 1024 rows per query
                rowTo = rowFrom + 1024;//не включительно!
                if (GetCellRows(rowFrom, rowTo, ref cellList) == false)
                {
                    //невозможно получить 1024 записи за раз - пробуем по 32.
                    //try to 32 rows per query
                    for (int j = 0; j < 32; j++)
                    {
                        rowTo = rowFrom + 32; //не включительно!
                        if (GetCellRows(rowFrom, rowTo, ref cellList) == false)
                        {
                            //невозможно получить 32 записи за раз - пробуем по одной.
                            //try to 1 row per query
                            for (int k = 0; k < 32; k++)
                            {
                                rowTo = rowFrom + 1; //не включительно!
                                if (GetCellRows(rowFrom, rowTo, ref cellList) == false)
                                {
                                    throw new Exception("Unable get cell from table");  //невозможно получить даже одну запись, прекращаем работу.
                                }
                                else
                                {
                                    SerializeCells(cellList, bw);
                                    cellcounter += cellList.Count;
                                }
                                rowFrom = rowTo;
                            }
                        }
                        else
                        {
                            SerializeCells(cellList, bw);
                            cellcounter += cellList.Count;
                        }
                        rowFrom = rowTo;
                    }
                }
                else
                {
                    SerializeCells(cellList, bw);
                    cellcounter += cellList.Count;
                }
                rowFrom = rowTo;
            }
            //end for
            return cellcounter;
        }

        /// <summary>
        /// Serialize list of cells
        /// </summary>
        /// <param name="cellList"></param>
        /// <param name="bw"></param>
        private void SerializeCells(List<MCell> cellList, BinaryWriter bw)
        {
            foreach (MCell cell in cellList)
                cell.toBinary(bw);
            return;
        }

        /// <summary>
        /// NT-Get band of cells. Return true if success, false if any errors.
        /// </summary>
        /// <param name="rowFrom">min row cellid number</param>
        /// <param name="rowTo">max row cellid number, not included in resultset</param>
        /// <returns></returns>
        private bool GetCellRows(int rowFrom, int rowTo, ref List<MCell> cellList)
        {
            bool res = false;
            try
            {
                //get cells
                cellList = this.m_containerRef.DataLayer.getBlockOfCells(rowFrom, rowTo);
                res = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                
            }
            return res;
        }

        /// <summary>
        /// NT-Save all cells (temporary and other) from memory. Returns number of saved cells.
        /// </summary>
        /// <param name="bw"></param>
        /// <returns>Saved cells counter</returns>
        private int SaveMemoryCells(BinaryWriter bw)
        {
            Int32 cnt = 0;
            //проходом по списку ячеек контейнера сохранять ячейки
            foreach (MCell ce in this.m_containerRef.Cells.Items)
            {
                    ce.toBinary(bw);
                    cnt++;
            }
            return cnt;
        }

        /// <summary>
        /// NT-Save all external cells. Now returns 0 - no external cells in project
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="lids">List of referenced cell id's</param>
        /// <returns>Saved cells counter</returns>
        private int SaveRefCells(BinaryWriter bw, List<MID> lids)
        {
            //Пока нечего выводить.
            return 0;
        }

        #endregion




    }
}
