using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    public class MMethod
    {
        /// <summary>
        /// Backreference to container
        /// </summary>
        private MEngine m_container;

        //Constructor
        public MMethod(MEngine backref)
        {
            m_container = backref;

        }

        /// <summary>
        /// NT-Init method manager
        /// </summary>
        internal void Open(MSolutionInfo projectInfo)
        {
            //сейчас нечего инициализировать, подсистема методов не реализована.
        }

        /// <summary>
        /// NT-Exit method manager
        /// </summary>
        internal void Close()
        {
            
        }



    }
}
