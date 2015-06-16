﻿using System;
using System.Data;
using System.IO;
using System.Linq;
using DotSpatial.Tests.Common;
using NUnit.Framework;

namespace DotSpatial.Data.Tests
{
    [TestFixture]
    class AttributeTableTests
    {
        [Test]
        [TestCase(1)]
        [TestCase(15)]
        [TestCase(254)]
        public void DbfTextFieldSize(byte maxLen)
        {
            var at = new AttributeTable();
            // Add Some Columns
            at.Table.Columns.Add(new DataColumn("ID", typeof(int)));
            at.Table.Columns.Add(new DataColumn("Text", typeof(string)) { MaxLength = maxLen });

            at.Table.Rows.Add(1, string.Concat(Enumerable.Repeat("t", maxLen)));

            var fileName = FileTools.GetTempFileName(".dbf");
            at.SaveAs(fileName, true);
            try
            {
                var actual = new AttributeTable(fileName);
                Assert.AreEqual(at.Table.Columns["Text"].MaxLength, actual.Table.Columns["Text"].MaxLength);
                Assert.AreEqual(at.Table.Rows[0]["Text"], actual.Table.Rows[0]["Text"]);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Test]
        public void CanReadDataRowWithZeroDates()
        {
            const string path = @"Data\Shapefiles\DateShapefile\DateShapefile.dbf";
            var at = new AttributeTable(path);
            var dt = at.SupplyPageOfData(0, 1);
            Assert.IsNotNull(dt);
            Assert.IsNotNull(dt.Rows[0]);
            Assert.AreEqual(DBNull.Value, dt.Rows[0]["datefiled"]);
        }
    }
}
