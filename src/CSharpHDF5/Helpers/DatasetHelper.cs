﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using CSharpHDF5.Enums;
using CSharpHDF5.Objects;
using CSharpHDF5.Structs;
using HDF.PInvoke;

namespace CSharpHDF5.Helpers
{
    internal static class DatasetHelper
    {
        /// <summary>
        /// Assumes that dataset already open
        /// </summary>
        /// <param name="_fileId"></param>
        /// <param name="_datasetId"></param>
        /// <param name="_fullPath"></param>
        /// <returns></returns>
        public static Hdf5Dataset LoadDataset(Hdf5Identifier _fileId, Hdf5Identifier _datasetId, string _fullPath)
        {
            Hdf5DataType datatype = TypeHelper.GetDataType(_datasetId);
            Hdf5Dataspace dataspace = DataspaceHelper.GetDataspace(_datasetId);

            Hdf5Dataset dataset = new Hdf5Dataset(_fileId, _fullPath)
            {
                FileId = _fileId,
                Dataspace = dataspace,
                DataType = datatype
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

        /// <summary>
        /// This can be used when you know what type you want.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_dataset"></param>
        /// <returns></returns>
        public static Array GetSingleDimensionArray<T>(Hdf5Dataset _dataset)
        {
            var id = H5O.open(_dataset.FileId.Value, _dataset.Path.FullPath).ToId();

            if (id.Value > 0)
            {
                Array array = Read1DArray<T>(id, _dataset);
                H5O.close(id.Value);
                return array;
            }

            return null;
        }

        private static Array SingleDimension(Hdf5Identifier _datasetIdentifer, Hdf5Dataset _dataset)
        {
            if (_dataset.DataType.Type == Hdf5DataTypes.Int8)
            {
                return Read1DArray<sbyte>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Int16)
            {
                return Read1DArray<Int16>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Int32)
            {
                return Read1DArray<Int32>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Int64)
            {
                return Read1DArray<Int64>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt8)
            {
                return Read1DArray<byte>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt16)
            {
                return Read1DArray<byte>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt32)
            {
                return Read1DArray<byte>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt64)
            {
                return Read1DArray<byte>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Single)
            {
                return Read1DArray<Single>(_datasetIdentifer, _dataset); 
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Double)
            {
                return Read1DArray<Double>(_datasetIdentifer, _dataset);
            }

            return null;
        }

        private static Array TwoDimension(Hdf5Identifier _datasetIdentifer, Hdf5Dataset _dataset)
        {
            if (_dataset.DataType.Type == Hdf5DataTypes.Int8)
            {
                return Read2DArray<sbyte>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Int16)
            {
                return Read2DArray<Int16>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Int32)
            {
                return Read2DArray<Int32>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Int64)
            {
                return Read2DArray<Int64>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt8)
            {
                return Read2DArray<byte>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt16)
            {
                return Read2DArray<UInt16>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt32)
            {
                return Read2DArray<UInt32>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt64)
            {
                return Read2DArray<UInt64>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Single)
            {
                return Read2DArray<Single>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.Double)
            {
                return Read2DArray<Double>(_datasetIdentifer, _dataset);
            }

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

            var dataType = H5T.copy(_dataset.DataType.NativeType.Value).ToId();

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

        public static T[,] Read2DArray<T>(Hdf5Identifier _datasetIdentifer, Hdf5Dataset _dataset)
        {
            T[,] dataArray = new T[_dataset.Dataspace.DimensionProperties[0].CurrentSize,
                _dataset.Dataspace.DimensionProperties[1].CurrentSize];

            GCHandle arrayHandle = GCHandle.Alloc(dataArray, GCHandleType.Pinned);

            var dataType = H5T.copy(_dataset.DataType.NativeType.Value).ToId();

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

        public static Hdf5Dataset CreateDatasetAddToDatasets(
            ReadonlyList<Hdf5Dataset> _datasets,
            Hdf5Identifier _fileId,
            Hdf5Path _parentPath,
            string _name,
            Hdf5DataTypes _datatype,
            int _numberOfDimensions,
            List<Hdf5DimensionProperty> _properties)
        {
            Hdf5Dataset dataset = DatasetHelper.CreateDataset(
                _fileId, 
                _parentPath, 
                _name, 
                _datatype,
                _numberOfDimensions,
                _properties);

            if (dataset != null)
            {
                _datasets.Add(dataset);
            }

            return dataset;
        }

        public static Hdf5Dataset CreateDataset(
            Hdf5Identifier _fileId, 
            Hdf5Path _parentPath, 
            string _name, 
            Hdf5DataTypes _datatype, 
            int _numberOfDimensions, 
            List<Hdf5DimensionProperty> _properties)
        {
            Hdf5Path path = _parentPath.Append(_name);

            ulong[] dimensionSize = new ulong[_numberOfDimensions];
            ulong[] maxSize = new ulong[_numberOfDimensions];

            int i = 0;
            foreach (var property in _properties)
            {
                dimensionSize[i] = property.CurrentSize;
                maxSize[i] = property.MaximumSize;

                i++;
            }

            Hdf5Identifier dataspaceId = H5S.create_simple(_numberOfDimensions, dimensionSize, maxSize).ToId();
            
            Hdf5Identifier typeId = H5T.copy(TypeHelper.GetNativeType(_datatype).Value).ToId();
            var status = H5T.set_order(typeId.Value, H5T.order_t.LE);

            Hdf5Identifier datasetId = H5D.create(_fileId.Value, path.FullPath, typeId.Value, dataspaceId.Value).ToId();

            Hdf5Dataset dataset = null;

            if (datasetId.Value > 0)
            {
                dataset = new Hdf5Dataset(datasetId, path.FullPath)
                {
                    DataType = TypeHelper.GetDataType(datasetId),
                    Dataspace = DataspaceHelper.GetDataspace(datasetId)
                };
            }            

            H5D.close(datasetId.Value);

            return dataset;
        }
    }
}
