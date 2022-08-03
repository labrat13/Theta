using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    public enum MAxisDirection
    {
        /// <summary>
        /// направление не указано или любое. Имя неудачное, надо переделать.
        /// </summary>
        Any = 0,
        /// <summary>
        /// связь снизу вверх
        /// </summary>
        Up = 1,
        /// <summary>
        /// связь сверху вниз
        /// </summary>
        Down = 2,
    }
}
