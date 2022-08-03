using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mary.Serialization
{
    /// <summary>
    /// Represent container statistics
    /// </summary>
    public class MStatistic
    {
        /// <summary>
        /// Number of const and temp cells in memory
        /// </summary>
        protected int m_cellsmem;
        /// <summary>
        /// Number of const cells in database
        /// </summary>
        protected int m_cellsconst;
        /// <summary>
        /// Number of temp cells in memory
        /// </summary>
        protected int m_cellstemp;
        /// <summary>
        /// Number of used external cells  in any other containers
        /// </summary>
        protected int m_cellsext;
        /// <summary>
        /// Number of const and temp links in memory
        /// </summary>
        protected int m_linksmem;
        /// <summary>
        /// Number of const links in database
        /// </summary>
        protected int m_linksconst;
        /// <summary>
        /// Number of temp links in memory
        /// </summary>
        protected int m_linkstemp;
        /// <summary>
        /// Number of links to any external cells
        /// </summary>
        protected int m_linksext;
        /// <summary>
        /// Number of resource files in project
        /// </summary>
        protected int m_resourcefiles;
        /// <summary>
        /// Size of resource files in bytes
        /// </summary>
        protected long m_resourcesize;

        /// <summary>
        /// Number of const and temp cells in memory
        /// </summary>
        [Category("Statistics"), Description("Number of temporary and constant cells in memory")]
        public int CellsInMemory
        {
            get
            {
                return m_cellsmem;
            }
            set
            {
                m_cellsmem = value;
            }
        }

        /// <summary>
        /// Number of const cells in database
        /// </summary>
        [Category("Statistics"), Description("Number of constant cells in database")]
        public int ConstantCells
        {
            get
            {
                return m_cellsconst;
            }
            set
            {
                m_cellsconst = value;
            }
        }

        /// <summary>
        /// Number of temp cells in memory
        /// </summary>
        [Category("Statistics"), Description("Number of temporary cells")]
        public int TemporaryCells
        {
            get
            {
                return m_cellstemp;
            }
            set
            {
                m_cellstemp = value;
            }
        }

        /// <summary>
        /// Number of used external cells  in any other containers
        /// </summary>
        [Category("Statistics"), Description("Number of external linked cells")]
        public int ExternalCells
        {
            get
            {
                return m_cellsext ;
            }
            set
            {
                m_cellsext = value;
            }
        }

        /// <summary>
        /// Number of const links in database
        /// </summary>
        [Category("Statistics"), Description("Number of constant links in database")]
        public int ConstantLinks
        {
            get
            {
                return m_linksconst ;
            }
            set
            {
                m_linksconst = value;
            }
        }

        /// <summary>
        /// Number of const and temp links in memory
        /// </summary>
        [Category("Statistics"), Description("Number of constant and temporary links in memory")]
        public int LinksInMemory
        {
            get
            {
                return m_linksmem ;
            }
            set
            {
                m_linksmem = value;
            }
        }

        /// <summary>
        /// Number of temp links in memory
        /// </summary>
        [Category("Statistics"), Description("Number of temporary links")]
        public int TemporaryLinks
        {
            get
            {
                return m_linkstemp ;
            }
            set
            {
                m_linkstemp = value;
            }
        }

        /// <summary>
        /// Number of links to any external cells
        /// </summary>
        [Category("Statistics"), Description("Number of links to any external cells")]
        public int ExternalLinks
        {
            get
            {
                return m_linksext ;
            }
            set
            {
                m_linksext = value;
            }
        }

        /// <summary>
        /// Number of resource files in project
        /// </summary>
        [Category("Statistics"), Description("Number of resource files in project")]
        public int ResourceFiles
        {
            get
            {
                return m_resourcefiles;
            }
            set
            {
                m_resourcefiles = value;
            }
        }

        /// <summary>
        /// Size of resource files in bytes
        /// </summary>
        [Category("Statistics"), Description("Size of resource files in bytes")]
        public long ResourceSize
        {
            get
            {
                return m_resourcesize;
            }
            set
            {
                m_resourcesize = value;
            }
        }
    }
}
