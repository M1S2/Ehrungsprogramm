using System;
using System.Collections.Generic;
using System.Text;

namespace Ehrungsprogramm.Core.Models
{
    /// <summary>
    /// Available function types.
    /// </summary>
    public enum FunctionType
    {
        /// <summary>
        /// Job as board member
        /// </summary>
        BOARD_MEMBER,

        /// <summary>
        /// Role as head of departement
        /// </summary>
        HEAD_OF_DEPARTEMENT,

        /// <summary>
        /// Any other function like trainer or press officer.
        /// </summary>
        OTHER_FUNCTION,

        /// <summary>
        /// Unknown function. This should only be used as placeholder.
        /// </summary>
        UNKNOWN
    }
}
