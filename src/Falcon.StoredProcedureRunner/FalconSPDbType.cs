namespace Falcon.StoredProcedureRunner
{

    /// <summary>
    /// 定义数据库数据类型
    /// </summary>
    public enum FalconSPDbType
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        //
        // 摘要:
        //     System.Int64. A 64-bit signed integer.
        SqlServerBigInt = 0,
        //
        // 摘要:
        //     System.Array of type System.Byte. A fixed-length stream of binary data ranging
        //     between 1 and 8,000 bytes.
        SqlServerBinary = 1,
        //
        // 摘要:
        //     System.Boolean. An unsigned numeric value that can be 0, 1, or null.
        SqlServerBit = 2,
        //
        // 摘要:
        //     System.String. A fixed-length stream of non-Unicode characters ranging between
        //     1 and 8,000 characters.
        SqlServerChar = 3,
        //
        // 摘要:
        //     System.DateTime. Date and time data ranging in value from January 1, 1753 to
        //     December 31, 9999 to an accuracy of 3.33 milliseconds.
        SqlServerDateTime = 4,
        //
        // 摘要:
        //     System.Decimal. A fixed precision and scale numeric value between -10 38 -1 and
        //     10 38 -1.
        SqlServerDecimal = 5,
        //
        // 摘要:
        //     System.Double. A floating point number within the range of -1.79E +308 through
        //     1.79E +308.
        SqlServerFloat = 6,
        //
        // 摘要:
        //     System.Array of type System.Byte. A variable-length stream of binary data ranging
        //     from 0 to 2 31 -1 (or 2,147,483,647) bytes.
        SqlServerImage = 7,
        //
        // 摘要:
        //     System.Int32. A 32-bit signed integer.
        SqlServerInt = 8,
        //
        // 摘要:
        //     System.Decimal. A currency value ranging from -2 63 (or -9,223,372,036,854,775,808)
        //     to 2 63 -1 (or +9,223,372,036,854,775,807) with an accuracy to a ten-thousandth
        //     of a currency unit.
        SqlServerMoney = 9,
        //
        // 摘要:
        //     System.String. A fixed-length stream of Unicode characters ranging between 1
        //     and 4,000 characters.
        SqlServerNChar = 10,
        //
        // 摘要:
        //     System.String. A variable-length stream of Unicode data with a maximum length
        //     of 2 30 - 1 (or 1,073,741,823) characters.
        SqlServerNText = 11,
        //
        // 摘要:
        //     System.String. A variable-length stream of Unicode characters ranging between
        //     1 and 4,000 characters. Implicit conversion fails if the string is greater than
        //     4,000 characters. Explicitly set the object when working with strings longer
        //     than 4,000 characters. Use System.Data.SqlDbType.NVarChar when the database column
        //     is nvarchar(max).
        SqlServerNVarChar = 12,
        //
        // 摘要:
        //     System.Single. A floating point number within the range of -3.40E +38 through
        //     3.40E +38.
        SqlServerReal = 13,
        //
        // 摘要:
        //     System.Guid. A globally unique identifier (or GUID).
        SqlServerUniqueIdentifier = 14,
        //
        // 摘要:
        //     System.DateTime. Date and time data ranging in value from January 1, 1900 to
        //     June 6, 2079 to an accuracy of one minute.
        SqlServerSmallDateTime = 0xF,
        //
        // 摘要:
        //     System.Int16. A 16-bit signed integer.
        SqlServerSmallInt = 0x10,
        //
        // 摘要:
        //     System.Decimal. A currency value ranging from -214,748.3648 to +214,748.3647
        //     with an accuracy to a ten-thousandth of a currency unit.
        SqlServerSmallMoney = 17,
        //
        // 摘要:
        //     System.String. A variable-length stream of non-Unicode data with a maximum length
        //     of 2 31 -1 (or 2,147,483,647) characters.
        SqlServerText = 18,
        //
        // 摘要:
        //     System.Array of type System.Byte. Automatically generated binary numbers, which
        //     are guaranteed to be unique within a database. timestamp is used typically as
        //     a mechanism for version-stamping table rows. The storage size is 8 bytes.
        SqlServerTimestamp = 19,
        //
        // 摘要:
        //     System.Byte. An 8-bit unsigned integer.
        SqlServerTinyInt = 20,
        //
        // 摘要:
        //     System.Array of type System.Byte. A variable-length stream of binary data ranging
        //     between 1 and 8,000 bytes. Implicit conversion fails if the byte array is greater
        //     than 8,000 bytes. Explicitly set the object when working with byte arrays larger
        //     than 8,000 bytes.
        SqlServerVarBinary = 21,
        //
        // 摘要:
        //     System.String. A variable-length stream of non-Unicode characters ranging between
        //     1 and 8,000 characters. Use System.Data.SqlDbType.VarChar when the database column
        //     is varchar(max).
        SqlServerVarChar = 22,
        //
        // 摘要:
        //     System.Object. A special data type that can contain numeric, string, binary,
        //     or date data as well as the SQL Server values Empty and Null, which is assumed
        //     if no other type is declared.
        SqlServerVariant = 23,
        //
        // 摘要:
        //     An XML value. Obtain the XML as a string using the System.Data.SqlClient.SqlDataReader.GetValue(System.Int32)
        //     method or System.Data.SqlTypes.SqlXml.Value property, or as an System.Xml.XmlReader
        //     by calling the System.Data.SqlTypes.SqlXml.CreateReader method.
        SqlServerXml = 25,
        //
        // 摘要:
        //     A SQL Server user-defined type (UDT).
        SqlServerUdt = 29,
        //
        // 摘要:
        //     A special data type for specifying structured data contained in table-valued
        //     parameters.
        SqlServerStructured = 30,
        //
        // 摘要:
        //     Date data ranging in value from January 1,1 AD through December 31, 9999 AD.
        SqlServerDate = 0x1F,
        //
        // 摘要:
        //     Time data based on a 24-hour clock. Time value range is 00:00:00 through 23:59:59.9999999
        //     with an accuracy of 100 nanoseconds. Corresponds to a SQL Server time value.
        SqlServerTime = 0x20,
        //
        // 摘要:
        //     Date and time data. Date value range is from January 1,1 AD through December
        //     31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy
        //     of 100 nanoseconds.
        SqlServerDateTime2 = 33,
        //
        // 摘要:
        //     Date and time data with time zone awareness. Date value range is from January
        //     1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999
        //     with an accuracy of 100 nanoseconds. Time zone value range is -14:00 through
        //     +14:00.
        SqlServerDateTimeOffset = 34,

        OracleBFile = 101,
        OracleBlob = 102,
        OracleByte = 103,
        OracleChar = 104,
        OracleClob = 105,
        OracleDate = 106,
        OracleDecimal = 107,
        OracleDouble = 108,
        OracleLong = 109,
        OracleLongRaw = 110,
        OracleInt16 = 111,
        OracleInt32 = 112,
        OracleInt64 = 113,
        OracleIntervalDS = 114,
        OracleIntervalYM = 115,
        OracleNClob = 116,
        OracleNChar = 117,
        OracleNVarchar2 = 119,
        OracleRaw = 120,
        OracleRefCursor = 121,
        OracleSingle = 122,
        OracleTimeStamp = 123,
        OracleTimeStampLTZ = 124,
        OracleTimeStampTZ = 125,
        OracleVarchar2 = 126,
        OracleXmlType = 0x7F,
        OracleArray = 0x80,
        OracleObject = 129,
        OracleRef = 130,
        OracleBinaryDouble = 132,
        OracleBinaryFloat = 133,
        OracleBoolean = 134,
        OracleJson = 135

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
}
