﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by Brian Nelson 2016.                                           *
 * See license in repo for more information                                  *
 * https://github.com/sharpHDF/sharpHDF                                      *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System.IO;
using NUnit.Framework;
using sharpHDF.Library.Objects;

namespace sharpHDF.Library.Tests.Objects
{
    [TestFixture]
    public class Hdf5GroupTests : BaseTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            DirectoryName = Path.Join("testdata", "grouptests");

            CleanDirectory();
        }

        [Test]
        public void CreateGroup()
        {
            string fileName = GetFilename("creategroup.h5");

            Hdf5File file = Hdf5File.Create(fileName);
            Hdf5Group group = file.Groups.Add("group1");

            Assert.IsNotNull(group);
            Assert.AreEqual(1, file.Groups.Count);

            file.Close();
        }

        [Test]
        public void OpenGroup()
        {
            string fileName = GetFilename("opengroup.h5");

            Hdf5File file = Hdf5File.Create(fileName);
            Hdf5Group group = file.Groups.Add("group1");

            Assert.IsNotNull(group);
            Assert.AreEqual(1, file.Groups.Count);

            file.Close();

            file = new Hdf5File(fileName);
            group = file.Groups[0];

            Assert.IsNotNull(group);
        }

        [Test]
        public void CreateGroupInGroup()
        {
            string fileName = GetFilename("creategroupingroup.h5");

            Hdf5File file = Hdf5File.Create(fileName);
            Hdf5Group group1 = file.Groups.Add("group1");

            Assert.IsNotNull(group1);
            Assert.AreEqual(1, file.Groups.Count);

            Hdf5Group group2 = group1.Groups.Add("group2");
            Assert.IsNotNull(group2);
            Assert.AreEqual(1, file.Groups.Count);
            Assert.AreEqual(1, group1.Groups.Count);

            file.Close();
        }

        [Test]
        public void OpeningGroupInGroup()
        {
            string fileName = GetFilename("opengroupingroup.h5");

            Hdf5File file = Hdf5File.Create(fileName);
            Hdf5Group group1 = file.Groups.Add("group1");

            Assert.IsNotNull(group1);
            Assert.AreEqual(1, file.Groups.Count);

            Hdf5Group group2 = group1.Groups.Add("group2");
            Assert.AreEqual(1, file.Groups.Count);
            Assert.AreEqual(1, group1.Groups.Count);

            file.Close();

            file = new Hdf5File(fileName);
            group1 = file.Groups[0];
            Assert.IsNotNull(group1);
            Assert.AreEqual("group1", group1.Name);

            group2 = group1.Groups[0];
            Assert.IsNotNull(group2);
            Assert.AreEqual("group2", group2.Name);
        }

        [Test]
        public void DeleteGroup()
        {
            string fileName = GetFilename("deletegroup.h5");

            Hdf5File file = Hdf5File.Create(fileName);
            Hdf5Group group1 = file.Groups.Add("group1");
            Hdf5Group group2 = file.Groups.Add("group2");
            Hdf5Group group3 = file.Groups.Add("group3");
            Hdf5Group group4 = file.Groups.Add("group4");
            Hdf5Group group5 = file.Groups.Add("group5");

            Assert.AreEqual(5, file.Groups.Count);

            //TODO - delete
        }

        [Test]
        public void LoopThrougGroups()
        {
            string fileName = GetFilename("loopthroughgroups.h5");

            Hdf5File file = Hdf5File.Create(fileName);
            Hdf5Group group1 = file.Groups.Add("group1");
            Hdf5Group group2 = file.Groups.Add("group2");
            Hdf5Group group3 = file.Groups.Add("group3");
            Hdf5Group group4 = file.Groups.Add("group4");
            Hdf5Group group5 = file.Groups.Add("group5");

            Assert.AreEqual(5, file.Groups.Count);

            foreach (Hdf5Group hdf5Group in file.Groups)
            {
                Assert.IsNotNull(hdf5Group);
            }

            for (int i = 0; i < 5; i++)
            {
                var group = file.Groups[i];
            }
            
            file.Close();
        }

    }
}
