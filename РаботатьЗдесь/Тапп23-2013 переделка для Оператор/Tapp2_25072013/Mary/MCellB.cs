using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mary.Serialization;

namespace Mary
{
    public class MCellB : MCell
    {
#region Fields
        /// <summary>
        /// текстовое описание, String.Empty по умолчанию.
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
        /// Cell name
        /// String.Empty by default
        /// </summary>
        private String m_name;
        /// <summary>
        /// Cell type id
        /// </summary>
        private int m_typeid;
        /// <summary>
        /// Cell creation timestamp
        /// </summary>
        private DateTime m_creatime;
        /// <summary>
        /// Last modification timestamp
        /// </summary>
        private DateTime m_moditime;
        /// <summary>
        /// Cell is read-only flag
        /// </summary>
        private bool m_readonly;
        /// <summary>
        /// Cell data value
        /// </summary>
        private byte[] m_value;
        /// <summary>
        /// Cell data value type id
        /// </summary>
        private int m_valuetypeid;
        ///// <summary>
        ///// Container reference
        ///// </summary>
        //private static MEngine m_container;
        /// <summary>
        /// Cell link collection
        /// </summary>
        private MLinkCollection m_links;

        private int m_cellid;

        private MCellMode m_cellMode;

 #endregion
        /// <summary>
        /// Normal constructor
        /// </summary>
        public MCellB()
        {
            m_cellid = 0;
            m_creatime = DateTime.Now;
            m_description = String.Empty;
            m_isactive = true;
            m_links = new MLinkCollection();
            m_moditime = DateTime.Now;
            m_name = String.Empty;
            m_readonly = false;
            m_serviceflag = 0;
            m_state = 0;
            m_typeid = 0;
            m_value = new byte[0];
            m_valuetypeid = 0;
            m_cellMode = MCellMode.Normal;
        }
        /// <summary>
        /// Constructor with cell saving mode
        /// </summary>
        /// <param name="mode">Cell saving mode</param>
        public MCellB(MCellMode mode)
        {
            m_cellid = 0;
            m_creatime = DateTime.Now;
            m_description = String.Empty;
            m_isactive = true;
            m_links = new MLinkCollection();
            m_moditime = DateTime.Now;
            m_name = String.Empty;
            m_readonly = false;
            m_serviceflag = 0;
            m_state = 0;
            m_typeid = 0;
            m_value = new byte[0];
            m_valuetypeid = 0;
            m_cellMode = mode;
        }

        /// <summary>
        /// Конструктор для загрузки данных ячейки из БД или снимка
        /// </summary>
        /// <param name="mode">Cell saving mode</param>
        public MCellB(MCellMode mode, Int32 cellID, String name, String description, bool isActive, Int32 typeId, DateTime creaTime, DateTime modiTime, bool readOnly, Int32 state, Int32 serviceFlag, byte[] cellValue, Int32 valueTypeId)
        {
            m_cellid = cellID;
            m_creatime = creaTime;
            m_description = description;
            m_isactive = isActive;
            m_links = new MLinkCollection();
            m_moditime = modiTime;
            m_name = name;
            m_readonly = readOnly;
            m_serviceflag = serviceFlag;
            m_state = state;
            m_typeid = typeId;
            m_value = cellValue;
            m_valuetypeid = valueTypeId;
            m_cellMode = mode;

            return;
        }

#region Properties
        /// <summary>
        /// Element description string
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
                m_moditime = DateTime.Now;
                //save changes
                if (m_cellMode == MCellMode.Normal)
                    MCell.Container.DataLayer.setCellDescription(m_cellid, value);
            }
        }

        /// <summary>
        /// Element is active 
        /// </summary>
        public override bool isActive
        {
            get
            {
                return m_isactive;
            }
            set
            {
                m_isactive = value; m_moditime = DateTime.Now;
                //save changes
                if (m_cellMode == MCellMode.Normal)
                    MCell.Container.DataLayer.setCellActive(m_cellid, value);
            }
        }
        /// <summary>
        /// Value for servicing
        /// </summary>
        public override int ServiceFlag
        {
            get
            {
                return m_serviceflag;
            }
            set
            {
                m_serviceflag = value; m_moditime = DateTime.Now;
                //save changes
                if (m_cellMode == MCellMode.Normal)
                    MCell.Container.DataLayer.setCellServiceFlag(m_cellid, value);
            }
        }
        /// <summary>
        /// Element state id
        /// </summary>
        public override MID State
        {
            get
            {
                return new MID(m_state);
            }
            set
            {
                m_state = value.ID; m_moditime = DateTime.Now;
                //save changes
                if (m_cellMode == MCellMode.Normal)
                    MCell.Container.DataLayer.setCellState(m_cellid, value);
            }
        }

        /// <summary>
        /// Cell name
        /// </summary>
        public override String Name
        {
            get
            {
                return m_name;
            }
            set 
            {
                m_name = value; m_moditime = DateTime.Now;
                //save changes
                if (m_cellMode == MCellMode.Normal)
                    MCell.Container.DataLayer.setCellName(m_cellid, value);
            }
        }

        /// <summary>
        /// Cell type id
        /// </summary>
        public override MID TypeId
        {
            get
            {
                return new MID(m_typeid);
            }
            set {
                m_typeid = value.ID; m_moditime = DateTime.Now;
                //save changes
                if (m_cellMode == MCellMode.Normal)
                    MCell.Container.DataLayer.setCellTypeId(m_cellid, value);
            }
        }

        /// <summary>
        /// Cell creation timestamp
        /// </summary>
        public override DateTime CreaTime
        {
            get
            {
                return m_creatime;
            }
            //internal set {
            //    m_creatime = value; m_moditime = DateTime.Now;
            //    //save changes
            //    if (m_cellMode == MCellMode.Normal)
            //        MCell.Container.DataLayer.setCellCreationTime(m_cellid, value);
            //}
        }

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        public override DateTime ModiTime
        {
            get
            {
                return m_moditime;
            }
            //internal set
            //{
            //    m_moditime = value;
            //    //not save in db because automatic updated on every field change
            //}
        }

        /// <summary>
        /// Cell is read-only flag
        /// </summary>
        public override bool ReadOnly
        {
            get
            {
                return m_readonly;
            }
            set {
                m_readonly = value; m_moditime = DateTime.Now;
                //save changes
                if (m_cellMode == MCellMode.Normal)
                    MCell.Container.DataLayer.setCellReadOnly(m_cellid, value);
            }
        }

        /// <summary>
        /// Cell data value
        /// </summary>
        public override byte[] Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value; m_moditime = DateTime.Now;
            //save changes
                if (m_cellMode == MCellMode.Normal)
                MCell.Container.DataLayer.setCellValue(m_cellid, value);
            }
        }

        /// <summary>
        /// Cell data value type id
        /// </summary>
        public override MID ValueTypeId
        {
            get
            {
                return new MID(m_valuetypeid);
            }
            set
            {
                m_valuetypeid = value.ID; m_moditime = DateTime.Now;
            //save changes
                if (m_cellMode == MCellMode.Normal)
                    MCell.Container.DataLayer.setCellValueTypeId(m_cellid, value);
            }
        }


        /// <summary>
        /// Cell link collection
        /// </summary>
        public override MLinkCollection Links
        {
            get
            {
                return m_links;
            }
        }

        /// <summary>
        /// Cell saving mode
        /// </summary>
        public override MCellMode CellMode
        {
            get
            {
                return m_cellMode;   
            }
            //internal set
            //{
            //    if (value == MCellMode.Compact) throw new Exception("Try to set Compact cell mode for MCellB cell");
            //    //Check Temporary state restriction
            //    if (((m_cellMode == MCellMode.Temporary) && (value != MCellMode.Temporary)) || ((value == MCellMode.Temporary) && (m_cellMode != MCellMode.Temporary)))
            //    {
            //        throw new Exception("Cell save mode change fail");
            //    }
            //    else
            //        m_cellMode = value;

            //}
        }

        /// <summary>
        /// идентификатор ячейки в контейнере.
        /// </summary>
        public override MID CellID
        {
            get { return new MID(m_cellid);  }
            set { m_cellid = value.ID; }
        }

        /// <summary>
        /// Current cell is MCellB cell?
        /// </summary>
        public override bool isLargeCell
        {
            get { return true; }
        }

#endregion



        #region Serialization function implements from MObject

        /// <summary>
        /// NT-Write cell record to serialization stream
        /// </summary>
        /// <param name="writer">serialization stream</param>
        public override void toBinary(System.IO.BinaryWriter writer)
        {
            toBinarySub(writer, this.CellMode);
        }

        /// <summary>
        /// NT-Write cell record to serialization stream
        /// </summary>
        /// <param name="writer">serialization stream</param>
        internal void toBinarySub(System.IO.BinaryWriter writer, MCellMode cellmode)
        {
            int recordLen = 0;

            writer.Write((Byte)((int)MSerialRecordType.Cell)); //1b length
            Int64 beginStreamPos = writer.BaseStream.Position;
            writer.Write(recordLen); //4b length
            //write cell fields
            //TODO: переупорядочить по важности для десериализации
            writer.Write(this.CellID.toU64());//8b // ID serializer
            writer.Write((Byte)((int)cellmode)); //1b //replace cellmode for MCellA serializing           
            writer.Write(this.Name);      //1b byteslength + chars     
            writer.Write(this.State.toU64()); //8b // ID serializer
            writer.Write(this.TypeId.toU64());//8b // ID serializer
            writer.Write(this.ValueTypeId.toU64());//8b // ID serializer
            writer.Write(this.Value.Length); //4b value array length as int 
            writer.Write(this.Value); //value bytes
            writer.Write(this.Description);//1b byteslength + chars
            writer.Write(this.isActive);//1b bool
            writer.Write(this.CreaTime.ToBinary());//8b //TODO: check
            writer.Write(this.ModiTime.ToBinary());//8b //TODO: check
            writer.Write(this.ReadOnly);//1b bool
            writer.Write(this.ServiceFlag);//4b
            writer.Write((UInt16)0); //checksum CRC16 must be last field
            //получаем длину записи без идентификатора секции 
            Int64 endstreamPos = writer.BaseStream.Position; 
            Int64 len = endstreamPos - beginStreamPos;
            recordLen = (Int32)len;//length < 2gb
            //записать значение длины записи.
            writer.BaseStream.Position = beginStreamPos; //to record size
            writer.Write(recordLen - 4); //record length - recordLen field length
            //crc16
            Int16 crcval = MCrc16.CalculateCrc16FromStream(writer.BaseStream, beginStreamPos, recordLen - 2);//get bytes from first(section size) to last before crc field
            writer.BaseStream.Position = endstreamPos - 2; //to crc16 field
            writer.Write(crcval);
            //restore stream position
            writer.BaseStream.Position = endstreamPos;
            return;
        }

        /// <summary>
        /// NR-Deserialize cell from binary stream
        /// </summary>
        /// <param name="reader"></param>
        /// <remarks>
        /// предполагается, что текущая позиция чтения - на дескрипторе секции.
        /// Позиция при выходе из функции - на следующем дескрипторе секции.
        /// Модифицировать переменные, а не проперти, чтобы избежать запросов в БД итп.
        /// При записи через проперти moditime должно быть последним, поскольку оно модифицируется при записи в проперти.
        /// </remarks>
        public override void fromBinary(System.IO.BinaryReader reader)
        {
            //check section descriptor
            MSerialRecordType rt = (MSerialRecordType)(int)reader.ReadByte();
            if (rt != MSerialRecordType.Cell) throw new Exception("Invalid section type");//serialization error
            //read section length 4bytes
            Int64 beginPos = reader.BaseStream.Position;//for crc check
            reader.BaseStream.Position = reader.BaseStream.Position + 4; //skip section len
            this.CellID = MID.fromU64(reader.ReadUInt64()); //cell id 8bytes
            this.m_cellMode = (MCellMode)(int)reader.ReadByte(); //1b
            this.m_name = reader.ReadString();
            this.m_state  = MID.fromU64(reader.ReadUInt64()).ID; //8bytes TODO: нарушение концепции идентификаторов
            this.m_typeid = MID.fromU64(reader.ReadUInt64()).ID; //8bytes TODO: нарушение концепции идентификаторов
            this.m_valuetypeid = MID.fromU64(reader.ReadUInt64()).ID; //8bytes TODO: нарушение концепции идентификаторов
            int vlen = reader.ReadInt32(); //value length
            this.m_value = reader.ReadBytes(vlen);
            this.m_description = reader.ReadString();
            this.m_isactive = reader.ReadBoolean();
            this.m_creatime = DateTime.FromBinary(reader.ReadInt64());
            this.m_moditime = DateTime.FromBinary(reader.ReadInt64());
            this.m_readonly = reader.ReadBoolean();
            this.m_serviceflag = reader.ReadInt32();
            //get crc
            Int64 endPos = reader.BaseStream.Position;
            Int16 crc = reader.ReadInt16();
            //checksum
            Int16 cr = MCrc16.CalculateCrc16FromStream(reader.BaseStream, beginPos, (int)(endPos - beginPos));
            if (cr != crc) throw new Exception("Invalid crc value");
        }

        /// <summary>
        /// NT- serialize cell to binary array
        /// </summary>
        /// <returns></returns>
        public override byte[] toBinaryArray()
        {
            //create memory stream and writer
            MemoryStream ms = new MemoryStream(64);//initial size for cell data 
            BinaryWriter bw = new BinaryWriter(ms);
            //convert data
            toBinarySub(bw, this.CellMode);
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
        /// NT-Сохраняет DelaySave или Temporary ячейку и ее связи в БД. Ячейка становится Normal типа.
        /// Для Normal ячеек ничего не делает.
        /// </summary>
        public override void S1_Save()
        {
            switch (this.m_cellMode)
            {
                case MCellMode.Normal:
                    //сохранение не требуется
                    break;
                case MCellMode.DelaySave:
                    //cell already have const cellid
                    //save cell
                    MCell.Container.DataLayer.CellUpdate((MCellB)this);
                    //save (insert) temp links if MCellA/MCellB target cell
                    //save (update) const links - необязательно.
                    this.saveLinks();
                    //set saved cell mode
                    this.m_cellMode = MCellMode.Normal;
                    break;
                case MCellMode.Temporary:
                    //get new const cellId
                    MID newId = MCell.Container.getNewCellId(false);
                    MID oldId = this.CellID;
                    //save cell
                    this.CellID = newId; //set const id for cell
                    MCell.Container.DataLayer.CellInsert((MCellB)this);
                    //удалить текущую ячейку из списка ячеек контейнера, 
                    //где она лежит под идентификатором временной ячейки 
                    //и заново вставить под новым, постоянным идентификатором.
                    MCell.Container.Cells.S1_RemoveCell(oldId);
                    MCell.Container.Cells.S1_AddCell(this);
                    //update cellid cash - remove temp id, store const id
                    MCell.Container.changeIdCashOnCreateCell(newId);
                    MCell.Container.changeIdCashOnRemoveTempCell(oldId);
                    //update cellid in links
                    this.Links.replaceCellId(oldId, newId);
                    //save (insert) temp links if MCellA/MCellB target cell
                    //save (update) const links - необязательно.
                    this.saveLinks();
                    //set saved cell mode - заменить режим ячейки с Temporary на Normal без проверок.
                    this.m_cellMode = MCellMode.Normal;

                    break;
                default:
                    throw new Exception("Invalid cell mode");
            }
        }

        /// <summary>
        /// помечает ячейку удаленной. Если ячейка не временная, не отложенной записи, то записывается в таблицу.
        /// </summary>
        public override void S1_Delete()
        {
            this.isActive = false; //записывается в таблицу из кода проперти
        }

        ///// <summary>
        ///// выгружает ячейку из памяти, когда она больше не нужна. При выгрузке надо просмотреть список связей ячейки, сбросить все ссылки на ячейку, выгрузить из памяти также все связи ячейки с незагруженными в память ячейками.
        ///// </summary>
        //public override void S1_Unload()
        //{
        //    ////выполнить чего-то ...

        //    ////удалить ячейку из списка ячеек контейнера
        //    //MCell.Container.UnloadCell(new MID(m_cellid));
        //    ////надо бы заменить на UnloadCell(this) - не надо, там словарь, а не список. Поиск и удаление по ИД 
        //    throw new NotImplementedException();
        //}



        /// <summary>
        /// Save (insert) temp links if MCellA/MCellB target cell
        /// Save (update) const links - необязательно.
        /// </summary>
        private void saveLinks()
        {
            foreach (MLink li in this.Links.Items)
            {
                if (li.isLinkNotTemporary)
                {
                    //update const links - необязательно.
                }
                else
                {
                    //insert temp links if MCellA/MCellB target cell
                    //TODO: переделать код здесь
                    //get target cell or cellId
                    MCell ta = li.getLinkedCell(this);
                    if (ta == null)
                    {
                        //get id and find in cell list (if cell is loaded)
                        MID id = li.getLinkedCellId(this.CellID);
                        ta = MCell.Container.Cells.S1_getCell(id);
                    }
                    //if cell is loaded, get cellmode
                    //TODO: сохранять ли временные связи если конечная ячейка отсутствует в памяти?
                    //пока будем сохранять если конечная ячейка не временная
                    if ((ta != null) && (ta.CellMode != MCellMode.Temporary) && (ta.CellMode != MCellMode.DelaySave))
                        S1_InsertLinkToTableAndAssignID(li);
                    if ((ta == null) && (!li.getLinkedCellId(this.CellID).isTemporaryID()))
                        S1_InsertLinkToTableAndAssignID(li);
                }
            }
            return;
        }



        /// <summary>
        /// NT-Получить текстовое представление ячейки
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String s = null;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("ID={0} Name={1} Mode={2} ", this.CellID.ToString(), this.Name, this.CellMode.ToString());
                sb.AppendFormat("Type={0} Data", this.TypeId.ToString());
                sb.Append("("); sb.Append(this.ValueTypeId.ToString()); sb.Append(")");
                sb.Append("["); sb.Append(this.Value.Length.ToString()); sb.Append("]");
                sb.AppendFormat(" Links={0}", this.Links.Items.Count);
                s = sb.ToString();
            }
            catch (Exception ex)
            {
                s = ex.GetType().Name;
            }
            return s;
        }

        /// <summary>
        /// Set new link collection for cell
        /// </summary>
        /// <param name="col"></param>
        internal void setLinkCollection(MLinkCollection col)
        {
            this.m_links = col;
        }

 
    }
}
