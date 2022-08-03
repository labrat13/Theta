using System;
using System.Collections.Generic;
using System.Text;
using Mary.Serialization;
using System.IO;

namespace Mary
{
   /// <summary>
    /// Class represent link between two cells
    /// </summary>
    public class MLink :MElement
    {
        #region Fields

        /// <summary>
        /// текстовое описание, null по умолчанию.
        /// </summary>
        private String m_description;
        /// <summary>
        /// flag is element active or deleted //default true
        /// </summary>
        private bool m_isactive;
        /// <summary>
        /// Поле для значения, используемого в сервисных операциях (поиск в графе,  обслуживание и так далее) //default 0
        /// </summary>
        private int m_serviceflag;
        /// <summary>
        /// Вынесен из подклассов как общее свойство. Link state id. //default 0
        /// </summary>
        private int m_state;
        
        /// <summary>
        /// cell id
        /// </summary>
        private int m_upcellid;
        /// <summary>
        /// cell id
        /// </summary>
        private int m_downcellid;
        /// <summary>
        /// cell reference
        /// </summary>
        private MCell m_upcell;
        /// <summary>
        /// cell reference
        /// </summary>
        private MCell m_downcell;
        /// <summary>
        /// link axis
        /// </summary>
        private int m_axis;

        /// <summary>
        /// Test field for link id in table
        /// </summary>
        /// <remarks>default 0. if 0, search link by it's values</remarks>
        private int m_tableid;

        #endregion
        /// <summary>
        /// Normal constructor
        /// </summary>
        public MLink()
        {
            m_axis = 0;
            m_description = String.Empty;
            m_downcell = null;
            m_downcellid = 0;
            m_isactive = true;
            m_serviceflag = 0;
            m_state = 0;
            m_upcell = null;
            m_upcellid = 0;
            m_tableid = 0;//default value for all non-saved links
        }


 
        #region Properties
        /// <summary>
        /// Down cell id. Changes not saved in database
        /// </summary>
        public MID downCellID
        {
            get { return new MID(m_downcellid); }
            set { m_downcellid = value.ID; }
        }
        /// <summary>
        /// Up cell id. Changes not saved in database
        /// </summary>
        public MID upCellID
        {
            get { return new MID(m_upcellid); }
            set { m_upcellid = value.ID; }
        }
        /// <summary>
        /// Link axis. Changes immediately saved in database
        /// </summary>
        public MID Axis
        {
            get { return new MID(m_axis); }
            set
            { 
                m_axis = value.ID;
                if (m_tableid != 0) MCell.Container.DataLayer.LinkUpdate(this);
            }
        }
        /// <summary>
        /// Reference to cell or null. Changes not saved in database
        /// </summary>
        public MCell upCell
        {
            get { return m_upcell; }
            set { m_upcell = value; }
        }

        /// <summary>
        /// Reference to cell or null. Changes not saved in database
        /// </summary>
        public MCell downCell
        {
            get  {  return m_downcell; }
            set  {  m_downcell = value;}
        }
        /// <summary>
        /// Link description. Changes immediately saved in database
        /// </summary>
        public override string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
                if (m_tableid != 0) MCell.Container.DataLayer.LinkUpdate(this);
            }
        }

        /// <summary>
        /// link active flag. Changes immediately saved in database
        /// </summary>
        public override bool isActive
        {
            get
            {
                return m_isactive;
            }
            set
            {
                m_isactive = value;
                if (m_tableid != 0) MCell.Container.DataLayer.LinkUpdate(this);
            }
        }
        /// <summary>
        /// Link service flag. Changes immediately saved in database
        /// </summary>
        public override int ServiceFlag
        {
            get
            {
                return m_serviceflag;
            }
            set
            {
                m_serviceflag = value;
                if (m_tableid != 0) MCell.Container.DataLayer.LinkUpdate(this);
            }
        }
        /// <summary>
        /// Link state. Changes immediately saved in database
        /// </summary>
        public override MID State
        {
            get
            {
                return new MID(m_state);
            }
            set
            {
                m_state = value.ID;
                if (m_tableid != 0) MCell.Container.DataLayer.LinkUpdate(this);
            }
        }

        /// <summary>
        /// Link primary key in link table. Value 0 for temporary links, prevent saving in database.
        /// </summary>
        public int TableId
        {
            get { return m_tableid; }
            set { m_tableid = value; }
        }

        /// <summary>
        /// Is link exists in database - is link have linkid?
        /// </summary>
        public bool isLinkNotTemporary
        {
            get { return (m_tableid != 0); }
        }

        #endregion



        #region Serialization function implements from MObject

        /// <summary>
        /// NFT- Serialize link to binary stream
        /// </summary>
        /// <param name="writer"></param>
        public override void toBinary(System.IO.BinaryWriter writer)
        {
            int recordLen = 0;//41bytes + description

            writer.Write((Byte)((int)MSerialRecordType.Link)); //1b length
            Int64 beginStreamPos = writer.BaseStream.Position;
            writer.Write(recordLen); //4b length
            writer.Write(this.downCellID.toU64()); //8b //8 
            writer.Write(this.upCellID.toU64()); //8b   //16
            writer.Write(this.Axis.toU64()); //8b    
            writer.Write(this.State.toU64()); //8b      //32
            writer.Write(this.isActive); //1b           //33
            writer.Write(this.ServiceFlag); //4b        //37
            writer.Write(this.TableId);   //4b          //41
            writer.Write(this.Description); //1+n or 2+n bytes //??
            writer.Write((Int16)0); //checksum must be last field
            Int64 endstreamPos = writer.BaseStream.Position;
            Int64 len = endstreamPos - beginStreamPos;
            recordLen = (Int32)len;
            //записать реальное значение длины записи.
            writer.BaseStream.Position = beginStreamPos;
            writer.Write(recordLen - 4); //record length with checksum field without recordLen field
            //crc16
            Int16 crcval = MCrc16.CalculateCrc16FromStream(writer.BaseStream, beginStreamPos, recordLen - 2);//get bytes from first(section size) to last before crc field
            writer.BaseStream.Position = endstreamPos - 2; //to crc16 field
            writer.Write(crcval);
            //restore stream position
            writer.BaseStream.Position = endstreamPos;//to end of current record
        }
        /// <summary>
        /// NT-Deserialize link from binary stream
        /// </summary>
        /// <param name="reader"></param>
        /// <remarks>
        /// предполагается, что текущая позиция чтения - на дескрипторе секции.
        /// Позиция при выходе из функции - на следующем дескрипторе секции.
        /// Модифицировать переменные, а не проперти, чтобы избежать запросов в БД итп.
        /// </remarks>
        public override void fromBinary(System.IO.BinaryReader reader)
        {
            //read section code
            MSerialRecordType rt = (MSerialRecordType)(int)reader.ReadByte();
            if (rt != MSerialRecordType.Link) throw new Exception("Invalid section type");//serialization error
            //read section length
            Int64 beginPos = reader.BaseStream.Position;
            Int32 sectLen = reader.ReadInt32();
            //checksum
            Int16 cr = MCrc16.CalculateCrc16FromStream(reader.BaseStream, beginPos, sectLen + 4 - 2);
            reader.BaseStream.Position = beginPos + sectLen + 4 - 2;
            Int16 crc = reader.ReadInt16();
            if (cr != crc) throw new Exception("Invalid crc value");
            reader.BaseStream.Position = beginPos + 4; //to after sectionLength
            //prevent writing to database - clear table id, он все равно не соответствует, потребуется новый
            this.m_tableid = 0;
            //read downcelid
            this.downCellID = MID.fromU64(reader.ReadUInt64());
            this.upCellID = MID.fromU64(reader.ReadUInt64());
            this.Axis = MID.fromU64(reader.ReadUInt64());
            this.State = MID.fromU64(reader.ReadUInt64());
            this.m_isactive = reader.ReadBoolean();
            this.m_serviceflag = reader.ReadInt32();
            //table id - писать последним, чтобы избежать записи в таблицу, если записывать данные через проперти
            this.m_tableid = reader.ReadInt32(); //а мы  через проперти пишем только MID
            this.m_description = reader.ReadString();
            ////get crc - moved up
            //Int64 endPos = reader.BaseStream.Position;
            //Int16 crc = reader.ReadInt16();
            reader.BaseStream.Position += 2;
        }

        /// <summary>
        /// NFT - Serialize link to array of bytes
        /// </summary>
        /// <returns></returns>
        public override byte[] toBinaryArray()
        {
            //create memory stream and writer
            MemoryStream ms = new MemoryStream(64);//initial size for cell data 
            BinaryWriter bw = new BinaryWriter(ms);
            //convert data
            this.toBinary(bw);
            //close memory stream and get bytes
            bw.Close();
            return ms.ToArray();
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
        /// NT-Возвращает направление связи для текущей ячейки. Если текущей ячейки нет в этой связи, или она в обеих концах, возвращается MAxisDirection.Any
        /// </summary>
        public MAxisDirection getAxisDirection(MID cellid)
        {
            if (m_upcellid == m_downcellid) return MAxisDirection.Any;
            else if (m_upcellid == cellid.ID) return MAxisDirection.Down;
            else if (m_downcellid == cellid.ID) return MAxisDirection.Up;
            else return MAxisDirection.Any;
        }

        /// <summary>
        /// NT-Set cell references and id's by axis direction. 
        /// Throw exception if specified direction is invalid or cell is null 
        /// </summary>
        /// <param name="dir">Axis direction: Up or Down</param>
        /// <param name="curCell">Current cell </param>
        /// <param name="targCell">Target cell </param>
        public void setCellsByDirection(MAxisDirection dir, MCell curCell, MCell targCell)
        {
            switch (dir)
            {
                case MAxisDirection.Down:
                    this.downCell = targCell;
                    this.downCellID = targCell.CellID;
                    this.upCell = curCell;
                    this.upCellID = curCell.CellID;
                    break;
                case MAxisDirection.Up:
                    this.downCell = curCell;
                    this.downCellID = curCell.CellID;
                    this.upCell = targCell;
                    this.upCellID = targCell.CellID;
                    break;
                default:
                    throw new Exception("Invalid axis direction");
            }
        }

        /// <summary>
        /// Set cell references and id's by axis direction. Throw exception if specified direction is invalid
        /// </summary>
        /// <param name="dir">Axis direction: Up or Down</param>
        /// <param name="curCell">current cell or null</param>
        /// <param name="targCellId">target cell id or 0</param>
        public void setCellsByDirection(MAxisDirection dir, MCell curCell, MID targCellId)
        {
            int cid = 0;
            if (curCell != null) cid = curCell.CellID.ID;
            
            switch (dir)
            {
                case MAxisDirection.Down:
                    m_downcell = null;
                    m_downcellid = targCellId.ID;
                    m_upcell = curCell;
                    m_upcellid = cid;
                    break;
                case MAxisDirection.Up:
                    m_upcell = null;
                    m_upcellid = targCellId.ID;
                    m_downcell = curCell;
                    m_downcellid = cid;
                    break;
                default:
                    throw new Exception("Invalid axis direction");
            }

        }







        /// <summary>
        /// Represent link for debug view
        /// </summary>
        public override string ToString()
        {
            return String.Format("Up={0} Dn={1} Ax={2} St={3} ID={4}", this.upCellID, this.downCellID, this.Axis, this.State, this.TableId);
        }

        /// <summary>
        /// Return true if any of cells is temporary. False if both cells is not temporary or is null.
        /// </summary>
        /// <returns></returns>
        public bool isLinkHaveTempCell()
        {
            if (m_downcell != null)
                if (m_downcell.CellMode == MCellMode.Temporary) return true;
            if (m_upcell != null)
                if (m_upcell.CellMode == MCellMode.Temporary) return true;
            return false;
        }

#region Fast access functions for link search - violate MID concept!
        //предназначены для быстрого чтения в обход типового механизма
        //Он уж очень тормозной намечается
        //Для ячеек не прокатит - MCellA ячейки не имеют внутренних переменных.

        /// <summary>
        /// Get pure link state
        /// </summary>
        /// <returns></returns>
        internal int intGetState()
        {
            return m_state;
        }
        /// <summary>
        /// Get pure link axis
        /// </summary>
        /// <returns></returns>
        internal int intGetAxis()
        {
            return m_axis;
        }
        /// <summary>
        /// Get pure link upcell id
        /// </summary>
        /// <returns></returns>
        internal int intGetUpId()
        {
            return m_upcellid;
        }
        /// <summary>
        /// Get pure link downcell id
        /// </summary>
        /// <returns></returns>
        internal int intGetDownId()
        {
            return m_downcellid;
        }
        /// <summary>
        /// Return true if link contains cellid, false otherwise
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <returns></returns>
        /// <remarks>Нарушает концепцию идентификаторов ради скорости</remarks>
        internal bool isLinkHaveCell(MID cellid)
        {
            int t = cellid.ID;
            return ((m_upcellid == t) || (m_downcellid == t));
        }
        /// <summary>
        /// NT-Set cell reference for link if cell exists in link
        /// </summary>
        /// <param name="cellid">Cell id</param>
        /// <param name="cell">Cell reference or null</param>
        internal void setCellRefsIfExists(MID cellid, MCell cell)
        {
            int t = cellid.ID;
            if (m_upcellid == t) m_upcell = cell;
            if (m_downcellid == t) m_downcell = cell;
        }

        /// <summary>
        /// NR-Return id of cell linked with specified cell.
        /// Return null if specified cell linked itself or not exists in link
        /// </summary>
        /// <param name="cellId">cell id</param>
        /// <returns></returns>
        public MID getLinkedCellId(MID cellId)
        {
            int t = cellId.ID;
            if ((m_downcellid == t) && (m_upcellid != t))
                    return this.upCellID;
            if ((m_upcellid == t) && (m_downcellid != t))
                    return this.downCellID;
            //else
            return null;

        }
        /// <summary>
        /// NR-Return ref of cell linked with specified cell
        /// Return null if specified cell linked itself or not exists in link
        /// </summary>
        /// <param name="mCell"></param>
        /// <returns></returns>
        internal MCell getLinkedCell(MCell cel)
        {
            int t = cel.CellID.ID;
            if ((m_downcellid == t) && (m_upcellid != t))
                return this.upCell;
            if ((m_upcellid == t) && (m_downcellid != t))
                return this.downCell;
            //else
            return null;
        }

        /// <summary>
        /// Replace cell id's in link
        /// </summary>
        /// <param name="oldId">old id</param>
        /// <param name="newId">new id</param>
        internal void intReplaceID(MID oldId, MID newId)
        {
            int t = oldId.ID;
            if (m_downcellid == t) m_downcellid = newId.ID;
            if (m_upcellid == t) m_upcellid = newId.ID;
        }

#endregion

        /// <summary>
        /// Return inverted axis direction
        /// </summary>
        /// <param name="axisDirection"></param>
        /// <returns></returns>
        public static MAxisDirection inverseAxisDirection(MAxisDirection axisDirection)
        {
            switch (axisDirection)
            {
                case MAxisDirection.Down:
                    return MAxisDirection.Up;
                    //break;
                case MAxisDirection.Up:
                    return MAxisDirection.Down;
                    //break;
                default:
                    return MAxisDirection.Any;
                    //break;
            }
        }


        /// <summary>
        /// NT-Return true if both links equals by id, cellid's, axis, active, state
        /// </summary>
        /// <param name="li"></param>
        /// <returns></returns>
        internal bool isEqual(MLink li)
        {
            //TODO: оптимизировать порядок проверок
            if (this.m_downcellid != li.m_downcellid) return false;
            if (this.m_axis != li.m_axis) return false;
            if (this.m_upcellid != li.m_upcellid) return false;
            if (this.m_tableid != li.m_tableid) return false;
            if (this.m_isactive != li.m_isactive) return false;
            if (this.m_state != li.m_state) return false;
            return true;
        }

        /// <summary>
        /// NT-Return true if both links equals by cellid's, axis, active
        /// </summary>
        /// <param name="li"></param>
        /// <returns></returns>
        internal bool isEqualLink(MLink li)
        {
            //TODO: оптимизировать порядок проверок
            if (this.m_downcellid != li.m_downcellid) return false;
            if (this.m_axis != li.m_axis) return false;
            if (this.m_upcellid != li.m_upcellid) return false;
            if (this.m_isactive != li.m_isactive) return false;
            return true;
        }






    }

 
}
