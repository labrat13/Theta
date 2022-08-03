using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    /// <summary>
    /// Class represent argument for method
    /// </summary>
    public class MArg
    {
        /// <summary>
        /// Argument description
        /// </summary>
        private String m_Description;
        /// <summary>
        /// Argument name
        /// </summary>
        private String m_Name;
        /// <summary>
        /// Argument value or reference
        /// </summary>
        private Object m_Value;
        /// <summary>
        /// Argument type 
        /// </summary>
        private Type m_Type;
        ///// <summary>
        ///// ID of argument if argument is a cell
        ///// </summary>
        //private MID m_Id;

        #region Properties
        /// <summary>
        /// Argument C#-type
        /// </summary>
        public Type argType
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        ///// <summary>
        ///// ID of argument if argument is a semantic object.
        ///// </summary>
        //public MID Id
        //{
        //    get
        //    {
        //        return m_Id;
        //    }
        //    set
        //    {
        //        m_Id = new MID(value);//make copy
        //    }
        //}

        /// <summary>
        /// Argument description. 
        /// </summary>
        public String Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
            }
        }

        /// <summary>
        /// Argument name. 
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// Get argument typename
        /// </summary>
        public string TypeName
        {
            get
            {
                return m_Type.FullName;
            }
        }
        /// <summary>
        /// Argument value or reference. 
        /// </summary>
        public Object Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public MArg()
        {
            m_Description = "";
            m_Name = "";
            m_Value = null;
            m_Type = typeof(Object);
            //_ClassId = new MID();
        }

        /// <summary>
        /// params constructor without value
        /// </summary>
        /// <param name="name">Argument name</param>
        /// <param name="description">Argument description</param>
        /// <param name="valueType">Argument value type</param>
        /// <param name="ID">ID of Argument semantic object or null</param>
        public MArg(String name, String description, Type valueType) // MID ID)
        {
            m_Name = name;
            m_Type = valueType;
            m_Description = description;
            m_Value = null;   //don't use value of argument now
            //if (ID == null) m_Id = new MID();
            //else m_Id = new MID(ID);
        }
        /// <summary>
        /// NT-params constructor
        /// </summary>
        /// <param name="name">Argument name</param>
        /// <param name="description">Argument description</param>
        /// <param name="valueType">Argument value type</param>
        /// <param name="val">Argument value</param>
        /// <param name="ID">ID of Argument semantic object or null</param>
        public MArg(String name, String description, Type valueType, Object val) //, MID ID)
        {
            m_Name = name;
            m_Type = valueType;
            m_Description = description;
            m_Value = val;
            //if (ID == null) m_Id = new MID();
            //else m_Id = new MID(ID);
        }
        /// <summary>
        /// NT-params constructor
        /// </summary>
        /// <param name="name">Argument name</param>
        /// <param name="description">Argument description</param>
        /// <param name="val">Argument value</param>
        public MArg(String name, String description, Object val)
        {
            m_Name = name;
            m_Description = description;
            m_Value = val;
            m_Type = val.GetType();
            //если это ячейка, надо привести ее к типу MCell
            if (MCell.IsCellType(val))
            {
                m_Type = typeof(MCell);

                //        m_Id = ((MCell)val).CellID;
                //    }
                //    else
                //        m_Id = new MID();
            }
        }
    }
}
