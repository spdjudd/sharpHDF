﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CSharpHDF5.Enums;
using CSharpHDF5.Objects;
using CSharpHDF5.Structs;
using HDF.PInvoke;

namespace CSharpHDF5.Helpers
{
    public static class DatasetHelper
    {
        /// <summary>
        /// Assumes that dataset already open
        /// </summary>
        /// <param name="_fileId"></param>
        /// <param name="_datasetId"></param>
        /// <returns></returns>
        public static Hdf5Dataset LoadDataset(Hdf5Identifier _fileId, Hdf5Identifier _datasetId, string _fullPath)
        {
            Hdf5DataType datatype = TypeHelper.GetDataType(_datasetId);
            Hdf5Dataspace dataspace = DataspaceHelper.GetDataspace(_datasetId);

            Hdf5Dataset dataset = new Hdf5Dataset(_fileId, _fullPath)
            {
                FileId = _fileId,
                Dataspace = dataspace,
                Type = datatype
            };

            return dataset;
        }

        public static Array GetData(Hdf5Dataset _dataset)
        {
            var id = H5O.open(_dataset.FileId.Value, _dataset.Path.FullPath).ToId();

            if (id.Value > 0)
            {
                Array a = null;

                if (_dataset.Dataspace.NumberOfDimensions == 1)
                {
                    a = SingleDimension(id, _dataset);
                }
                else if (_dataset.Dataspace.NumberOfDimensions == 2)
                {
                    a = TwoDimension(id, _dataset);
                }

                H5O.close(id.Value);

                return a;
            }

            return null;
        }

        private static Array SingleDimension(Hdf5Identifier _datasetIdentifer, Hdf5Dataset _dataset)
        {
            if (_dataset.Type.Type == Hdf5DataTypes.Int8)
            {
                return Read1DArray<sbyte>(_datasetIdentifer, _dataset);
            }
            
            if (_dataset.Type.Type == Hdf5DataTypes.UInt8)
            {
                return Read1DArray<byte>(_datasetIdentifer, _dataset);
            }


            return null;
        }

        private static Array TwoDimension(Hdf5Identifier _datasetIdentifer, Hdf5Dataset _dataset)
        {
            return null;
        }

        /// <summary>
        /// Assumes dataset is already open
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_datasetIdentifer"></param>
        /// <param name="_dataset"></param>
        /// <returns></returns>
        private static T[] Read1DArray<T>(Hdf5Identifier _datasetIdentifer, Hdf5Dataset _dataset)
        {
            T[] dataArray = new T[_dataset.Dataspace.DimensionProperties[0].CurrentSize];

            GCHandle arrayHandle = GCHandle.Alloc(dataArray, GCHandleType.Pinned);

            var dataType = H5T.copy(_dataset.Type.NativeType.Value).ToId();

            int result = H5D.read(
                _datasetIdentifer.Value,
                dataType.Value,
                H5S.ALL,
                H5S.ALL,
                H5P.DEFAULT,
                arrayHandle.AddrOfPinnedObject());

            arrayHandle.Free();

            H5T.close(dataType.Value);

            return dataArray;
        }
    }
}

