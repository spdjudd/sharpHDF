﻿using System.Collections.Generic;
using System.Text;
using CSharpHDF5.Helpers;
using CSharpHDF5.Interfaces;

namespace CSharpHDF5.Objects
{
    public class Hdf5Group : AbstractHdf5Object, IHasGroups, IHasAttributes, IHasDatasets
    {
        private int m_FileId;

        public Hdf5Group()
        {
            Groups = new List<Hdf5Group>();
            Datasets = new List<Hdf5Dataset>();
        }

        internal Hdf5Group(int _fileId, int _groupId, string _path)
        {
            m_FileId = _fileId;
            Id = _groupId;

            Path = new Hdf5Path(_path);
            Name = Path.Name;

            Groups = new List<Hdf5Group>();                       
            Datasets = new List<Hdf5Dataset>();
        }

        public string Name { get; set; }

        public List<Hdf5Group> Groups { get; set; }

        public List<Hdf5Dataset> Datasets { get; set; } 

        public List<Hdf5Attribute> Attributes
        {
            get { return AttributeHelper.GetAttributes(this); }
        }

        internal void LoadChildObjects()
        {
            GroupHelper.PopulateChildrenObjects(m_FileId, this);
        }
    }
}