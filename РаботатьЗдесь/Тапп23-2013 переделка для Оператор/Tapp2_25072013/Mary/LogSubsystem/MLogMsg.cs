using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    public class MLogMsg
    {
        #region Fields

        /// <summary>
        /// Класс сообщения
        /// </summary>
        private MMessageClass m_class;
        /// <summary>
        /// Код сообщения
        /// </summary>
        private MMessageCode m_code;
        /// <summary>
        /// время события
        /// </summary>
        private DateTime m_time;
        /// <summary>
        /// текстовое описание события
        /// </summary>
        private string m_description;
        /// <summary>
        /// начальное состояние объекта события в hex
        /// </summary>
        private String m_initialHex;
        /// <summary>
        /// конечное состояние объекта события в hex
        /// </summary>
        private String m_finalHex;

        #endregion

        /// <summary>
        /// Normal constructor
        /// </summary>
        public MLogMsg()
        {
            m_class = MMessageClass.Nothing;
            m_code = MMessageCode.Nothing;
            m_description = String.Empty;
            m_finalHex = String.Empty;
            m_initialHex = String.Empty;
            m_time = DateTime.Now;
        }

        #region Properties

        /// <summary>
        /// Класс сообщения
        /// </summary>
        public MMessageClass MsgClass
        {
            get
            {
                return m_class;
            }
            set
            {
                m_class = value;
            }
        }

        /// <summary>
        /// Код сообщения
        /// </summary>
        public MMessageCode MsgCode
        {
            get
            {
                return m_code;
            }
            set
            {
                m_code = value;
            }
        }

        /// <summary>
        /// время события
        /// </summary>
        public DateTime Time
        {
            get
            {
                return m_time;
            }
            set
            {
                m_time = value;
            }
        }

        /// <summary>
        /// текстовое описание события
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
        /// начальное состояние объекта события в hex
        /// </summary>
        public string InitialHex
        {
            get
            {
                return m_initialHex;
            }
            set
            {
                m_initialHex = value;
            }
        }

        /// <summary>
        /// конечное состояние объекта события в hex
        /// </summary>
        public string FinalHex
        {
            get
            {
                return m_finalHex;
            }
            set
            {
                m_finalHex = value;
            }
        }

        #endregion

    }
}
