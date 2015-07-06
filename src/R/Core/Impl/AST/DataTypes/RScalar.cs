﻿using System.Diagnostics;
using Microsoft.R.Core.AST.DataTypes.Definitions;

namespace Microsoft.R.Core.AST.DataTypes
{
    /// <summary>
    /// Represents scalar (numerical or string) value. 
    /// Scalars are one-element vectors.
    /// </summary>
    [DebuggerDisplay("[{Value}]")]
    public abstract class RScalar<T>: RObject, IRVector, IRScalar<T>
    {
        #region IRVector
        public int Length
        {
            get { return 1; }
        }

        public abstract RMode Mode { get; }
        #endregion

        #region IRScalar
        public T Value { get; set; }
        #endregion

        public RScalar(T value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
