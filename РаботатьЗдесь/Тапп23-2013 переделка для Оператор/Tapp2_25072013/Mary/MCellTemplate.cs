using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    public class MCellTemplate
    {

        #region Fields
        /// <summary>
        /// Cell name
        /// </summary>
        private String m_name;
        /// <summary>
        /// Cell type id
        /// </summary>
        private MID m_typeid;
        /// <summary>
        /// Cell creation timestamp
        /// </summary>
        private System.DateTime? m_creatime;
        /// <summary>
        /// Last modification timestamp
        /// </summary>
        private System.DateTime? m_moditime;
        /// <summary>
        /// Cell is read-only flag
        /// </summary>
        private bool? m_readonly;
        /// <summary>
        /// Cell data value
        /// </summary>
        private byte[] m_value;
        /// <summary>
        /// Cell data value type id
        /// </summary>
        private MID m_valuetypeid;
        /// <summary>
        /// Container reference
        /// </summary>
        private static MEngine m_container;
        /// <summary>
        /// Cell link collection
        /// </summary>
        private MLinkCollection m_links;
        /// <summary>
        /// текстовое описание, null по умолчанию.
        /// </summary>
        private String m_description;
        /// <summary>
        /// flag is element active or deleted //default true
        /// </summary>
        private bool? m_isactive;
        /// <summary>
        /// Поле для значения, используемого в сервисных операциях (поиск в графе,  обслуживание и так далее) //default 0
        /// </summary>
        private int? m_serviceflag;
        /// <summary>
        /// Вынесен из подклассов как общее свойство. Link state id. //default 0
        /// </summary>
        private MID m_state;

        private MID m_cellid;

        private MCellMode? m_cellmode;
        #endregion

        /// <summary>
        /// Normal constructor
        /// </summary>
        public MCellTemplate()
        {
            m_cellid = null;
            m_container = null;
            m_creatime = null;
            m_links = null;
            m_moditime = null;
            m_name = null;
            m_readonly = null;
            m_cellmode = null;
            m_typeid = null;
            m_value = null;
            m_valuetypeid = null;
            m_description = null;
            m_state = null;
            m_serviceflag = null;
            m_isactive = null;
        }

        /// <summary>
        /// Create template as full copy of cell 
        /// </summary>
        /// <param name="c"></param>
        public MCellTemplate(MCell c)
        {
            this.CellID = c.CellID;
            this.CellMode = c.CellMode;
            this.CreaTime = c.CreaTime;
            this.Description = c.Description;
            this.isActive = c.isActive;
            this.ModiTime = c.ModiTime;
            this.Name = c.Name;
            this.ReadOnly = c.ReadOnly;
            this.ServiceFlag = c.ServiceFlag;
            this.State = c.State;
            this.TypeId = c.TypeId;
            this.Value = c.Value;
            this.ValueTypeId = c.ValueTypeId;
        }

        #region Properties

        /// <summary>
        /// текстовое описание, null по умолчанию.
        /// </summary>
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }

        /// <summary>
        /// flag is element active or deleted //default true
        /// </summary>
        public bool? isActive
        {
            get
            {
                return m_isactive;
            }
            set
            {
                m_isactive = value;
            }
        }

        /// <summary>
        /// Поле для значения, используемого в сервисных операциях (поиск в графе,  обслуживание и так далее) //default 0
        /// </summary>
        public int? ServiceFlag
        {
            get
            {
                return m_serviceflag;
            }
            set
            {
                m_serviceflag = value;
            }
        }

        /// <summary>
        /// Вынесен из подклассов как общее свойство. Link state id. //default 0
        /// </summary>
        public MID State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
            }
        }


        /// <summary>
        /// Cell name
        /// </summary>
        public String Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Cell type id
        /// </summary>
        public MID TypeId
        {
            get
            {
                return m_typeid;
            }
            set
            {
                m_typeid = value;
            }
        }

        /// <summary>
        /// Cell creation timestamp
        /// </summary>
        public DateTime? CreaTime
        {
            get
            {
                return m_creatime;
            }
            set
            {
                m_creatime = value;
            }
        }

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        public System.DateTime? ModiTime
        {
            get
            {
                return m_moditime;
            }
            set
            {
                m_moditime = value;
            }
        }

        /// <summary>
        /// Cell is read-only flag
        /// </summary>
        public bool? ReadOnly
        {
            get
            {
                return m_readonly;
            }
            set
            {
                m_readonly = value;
            }
        }

        /// <summary>
        /// Cell data value
        /// </summary>
        public byte[] Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        /// <summary>
        /// Cell data value type id
        /// </summary>
        public MID ValueTypeId
        {
            get
            {
                return m_valuetypeid;
            }
            set
            {
                m_valuetypeid = value;
            }
        }

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
        /// Cell link collection
        /// </summary>
        public MLinkCollection Links
        {
            get
            {
                return m_links;
            }
            set
            {  m_links = value;   }
        }

        /// <summary>
        /// Cell save mode, for memory search only
        /// </summary>
        public MCellMode? CellMode
        {
            get
            {
                return m_cellmode;
            }
            set
            {
                m_cellmode = value;
            }
        }

        /// <summary>
        /// идентификатор ячейки в контейнере.
        /// </summary>
        public MID CellID
        {
            get
            {
                return m_cellid;
            }
            set
            {
                m_cellid = value;
            }
        }

        #endregion

        #region Serialization function implements from MObject

        #endregion

        /// <summary>
        /// NT-Specified cell match to template? Test needed for cell.Value data
        /// </summary>
        /// <param name="cell">cell</param>
        /// <returns>True if cell match template, False otherwise</returns>
        internal bool CellMatch(MCell cell)
        {
            //TODO: Переделать последовательность проверок для более быстрого поиска.
            if ((m_cellid != null) && (!m_cellid.isEqual(cell.CellID))) return false;
            else if ((m_isactive.HasValue) && (m_isactive.Value != cell.isActive)) return false;
            else if ((m_typeid != null) && (!m_typeid.isEqual(cell.TypeId))) return false;
            else if ((m_valuetypeid != null) && (!m_valuetypeid.isEqual(cell.ValueTypeId))) return false;
            else if ((m_state != null) && (!m_state.isEqual(cell.State))) return false;
            else if ((m_name != null) && (!m_name.Equals(cell.Name))) return false;
            else if (m_readonly.HasValue && (m_readonly.Value != cell.ReadOnly)) return false;
            else if ((m_value != null) && (!CellValueEqual(m_value, cell.Value))) return false; //??? проверить работу
            else if (m_serviceflag.HasValue && (m_serviceflag.Value != cell.ServiceFlag)) return false;
            else if ((m_description != null) && (!m_description.Equals(cell.Description))) return false;
            else if (m_creatime.HasValue && (m_creatime.Value != cell.CreaTime)) return false;
            else if (m_moditime.HasValue && (m_moditime.Value != cell.ModiTime)) return false;
            else if (m_cellmode.HasValue && (m_cellmode.Value != cell.CellMode)) return false;
            
            else return true;

        }
        /// <summary>
        /// Compare two byte array
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool CellValueEqual(byte[] b1, byte[] b2)
        {
            if ((b1 == null) && (b2 == null)) return true;
            else if((b1 == null) ||(b2 == null)) return false;
            else if (b1.Length != b2.Length) return false;
            else
            {
                for (int i = 0; i < b1.Length; i++)
                {
                    if (b1[i] != b2[i]) return false;
                }
                return true;
            }
        }










    }
}
