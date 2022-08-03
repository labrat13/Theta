using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    /// <summary>
    /// Class represent link between two cells
    /// </summary>
    public class MLinkTemplate
    {
        #region Fields
        /// <summary>
        /// cell id
        /// </summary>
        private MID m_upcellid;
        /// <summary>
        /// cell id
        /// </summary>
        private MID m_downcellid;
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
        private MID m_axis;
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
        /// <summary>
        /// Test field for link id in table.
        /// </summary>
        private int? m_linkpkid;
        #endregion

        /// <summary>
        /// Normal constructor
        /// </summary>
        public MLinkTemplate()
        {
            m_axis = null;
            m_downcell = null;
            m_downcellid = null;
            m_upcell = null;
            m_upcellid = null;
            m_linkpkid = null;
            m_description = null;
            m_state = null;
            m_serviceflag = null;
            m_isactive = null;
        }

        /// <summary>
        /// Construct link template as full copy of link
        /// </summary>
        /// <param name="li">Link sample</param>
        public MLinkTemplate(MLink li)
        {
            m_axis = li.Axis;
            m_description = li.Description;
            m_downcell = li.downCell;
            m_downcellid = li.downCellID;
            m_isactive = li.isActive;
            m_linkpkid = li.TableId;
            m_serviceflag = li.ServiceFlag;
            m_state = li.State;
            m_upcell = li.upCell;
            m_upcellid = li.upCellID;
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
        /// Cell id
        /// </summary>
        public MID downCellID
        {
            get { return m_downcellid; }
            set { m_downcellid = value; }
        }

        /// <summary>
        /// Cell id
        /// </summary>
        public MID upCellID
        {
            get { return m_upcellid; }
            set { m_upcellid = value; }
        }
        /// <summary>
        /// Link axis.
        /// </summary>
        public MID Axis
        {
            get { return m_axis; }
            set { m_axis = value; }
        }
        /// <summary>
        /// Reference to cell or null. Not used for template
        /// </summary>
        public MCell upCell
        {
            get { return m_upcell; }
            set { m_upcell = value; }
        }

        /// <summary>
        /// Reference to cell or null. Not used for template
        /// </summary>
        public MCell downCell
        {
            get
            {
                return m_downcell;
            }
            set
            {
                m_downcell = value;
            }
        }

        /// <summary>
        /// Link primary key in link table. 
        /// </summary>
        public int? tableId
        {
            get { return m_linkpkid; }
            set { m_linkpkid = value; }
        }

        #endregion



        ///// <summary>
        ///// Возвращает направление связи для текущей ячейки.
        ///// </summary>
        //public MAxisDirection getAxisDirection(MID cellid)
        //{
        //    throw new System.NotImplementedException();
        //}

        /// <summary>
        /// Represent link for debug view
        /// </summary>
        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// NT-Specified link match to template? 
        /// </summary>
        /// <param name="link">link</param>
        /// <returns>True if link match template, False otherwise</returns>
        public bool isLinkMatch(MLink link)
        {
            //TODO: Переделать последовательность проверок для более быстрого поиска.
            //При переделке кода на двойной идентификатор придется заменить все проверки c MID на вызовы MID.isEqual()
            //Но уж очень оно будет тормозить...
            if (this.m_linkpkid.HasValue && (m_linkpkid.Value != link.TableId)) return false;
            else if ((m_axis != null) && (m_axis.ID != link.intGetAxis())) return false;
            else if ((m_downcellid != null) && (m_downcellid.ID != link.intGetDownId())) return false;
            else if ((m_upcellid != null) && (m_upcellid.ID != link.intGetUpId())) return false;
            else if (m_isactive.HasValue && (m_isactive.Value != link.isActive)) return false; 
            else if ((m_state != null) && (m_state.ID != link.intGetState())) return false;
            else if (m_serviceflag.HasValue && (m_serviceflag.Value != link.ServiceFlag)) return false;
            else if ((m_description != null) && (!String.Equals(m_description, link.Description))) return false;

            else return true;

        }

        /// <summary>
        /// Set cell id's by axis direction. Throw exception if specified direction is invalid
        /// </summary>
        /// <param name="dir">Axis direction: Up or Down</param>
        /// <param name="curCell">current cell id </param>
        /// <param name="targCell">target cell id</param>
        public void setCellsByDirection(MAxisDirection dir, MID curCellId, MID targCellId)
        {
            switch (dir)
            {
                case MAxisDirection.Down:
                    m_downcellid.ID = targCellId.ID;
                    m_upcellid.ID = curCellId.ID;
                    break;
                case MAxisDirection.Up:
                    m_upcellid.ID = targCellId.ID;
                    m_downcellid.ID = curCellId.ID;
                    break;
                default:
                    throw new Exception("Invalid axis direction");
            }

        }

    }
}
