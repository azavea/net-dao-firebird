﻿// Copyright (c) 2004-2010 Azavea, Inc.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using Azavea.Open.DAO.SQL;

namespace Azavea.Open.DAO.Firebird
{
    /// <summary>
    /// Implements a FastDao layer customized for PostGreSQL (optionally with PostGIS installed).
    /// </summary>
    public class FirebirdDaLayer : SqlDaDdlLayer
    {
        /// <summary>
        /// Construct the layer.  Should typically be called only by the appropriate
        /// ConnectionDescriptor.
        /// </summary>
        /// <param name="connDesc">Connection to the Firebird DB we'll be using.</param>
        public FirebirdDaLayer(FirebirdDescriptor connDesc)
            : base(connDesc, true) { }

        #region Implementation of IDaDdlLayer

        /// <summary>
        /// Returns the DDL for the type of an automatically incrementing column.
        /// Some databases only store autonums in one col type so baseType may be
        /// ignored.
        /// </summary>
        /// <param name="baseType">The data type of the column (nominally).</param>
        /// <returns>The autonumber definition string.</returns>
        protected override string GetAutoType(Type baseType)
        {
            throw new NotSupportedException("Firebird does not have autonumbers, use a sequence.");
        }

        /// <summary>
        /// Returns the SQL type used to store a byte array in the DB.
        /// </summary>
        protected override string GetByteArrayType()
        {
            return "BLOB";
        }

        /// <summary>
        /// Returns the SQL type used to store a long in the DB.
        /// </summary>
        protected override string GetLongType()
        {
            return "INT64";
        }

        /// <summary>
        /// Returns the SQL type used to store a DateTime in the DB.
        /// </summary>
        protected override string GetDateTimeType()
        {
            return "TIMESTAMP";
        }

        /// <summary>
        /// Returns the SQL type used to store a boolean in the DB.
        /// </summary>
        protected override string GetBooleanType()
        {
            return "SMALLINT";
        }

        /// <summary>
        /// Returns the SQL type used to store a "normal" (unicode) string in the DB.
        /// NOTE: At the moment just uses varchar, so will rely on whatever the DB is using
        ///       for an encoding.
        /// </summary>
        protected override string GetStringType()
        {
            // TODO: Figure out how force some sort of unicode encoding.
            // This does not work:
            //return "VARCHAR(2000) CHARACTER SET UTF8";
            return "VARCHAR(2000)";
        }

        /// <summary>
        /// Returns whether a sequence with this name exists or not.
        /// Firebird doesn't appear to support the SQL standard information_schema.
        /// </summary>
        /// <param name="name">Name of the sequence to check for.</param>
        /// <returns>Whether a sequence with this name exists in the data source.</returns>
        public override bool SequenceExists(string name)
        {
            int count = SqlConnectionUtilities.XSafeIntQuery(_connDesc,
                "SELECT count(*) FROM RDB$GENERATORS WHERE RDB$GENERATOR_NAME = '" +
                name.ToUpper() + "'", null);
            return count > 0;
        }

        /// <summary>
        /// Returns true if you need to call "CreateStoreRoom" before storing any
        /// data.  This method is "Missing" not "Exists" because implementations that
        /// do not use a store room can return "false" from this method without
        /// breaking either a user's app or the spirit of the method.
        /// 
        /// Store room typically corresponds to "table".
        /// Firebird doesn't appear to support the SQL standard information_schema.
        /// </summary>
        /// <returns>Returns true if you need to call "CreateStoreRoom"
        ///          before storing any data.</returns>
        public override bool StoreRoomMissing(ClassMapping mapping)
        {
            int count = SqlConnectionUtilities.XSafeIntQuery(_connDesc,
                "select count(*) from rdb$relations where rdb$relation_name = '" +
                mapping.Table.ToUpper() + "'", null);
            return count == 0;
        }
        #endregion
    }
}