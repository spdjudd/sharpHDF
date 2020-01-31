﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by Brian Nelson 2016.                                           *
 * See license in repo for more information                                  *
 * https://github.com/sharpHDF/sharpHDF                                      *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HDF.PInvoke;
using sharpHDF.Library.Enums;
using sharpHDF.Library.Exceptions;
using sharpHDF.Library.Objects;
using sharpHDF.Library.Structs;

namespace sharpHDF.Library.Helpers
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
            Hdf5DataType datatype = TypeHelper.GetDataTypeFromDataset(_datasetId);
            Hdf5Dataspace dataspace = DataspaceHelper.GetDataspace(_datasetId);

            Hdf5Dataset dataset = new Hdf5Dataset(_fileId, _datasetId, _fullPath)
            {
                
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
                return Read1DArray<UInt16>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt32)
            {
                return Read1DArray<UInt32>(_datasetIdentifer, _dataset);
            }

            if (_dataset.DataType.Type == Hdf5DataTypes.UInt64)
            {
                return Read1DArray<UInt64>(_datasetIdentifer, _dataset);
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
        public static T[] Read1DArray<T>(Hdf5Identifier _datasetIdentifer, Hdf5Dataset _dataset)
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

        public static void Write1DArray<T>(Hdf5Dataset _dataset, T[] _array)
        {
            if (_dataset.Dataspace.NumberOfDimensions != 1)
            {
                throw new Hdf5ArrayDimensionsMismatchException();
            }

            if ((ulong)_array.Length != _dataset.Dataspace.DimensionProperties[0].CurrentSize)
            {
                throw new Hdf5ArraySizeMismatchException();
            }

            var datasetId = H5O.open(_dataset.FileId.Value, _dataset.Path.FullPath).ToId();

            GCHandle arrayHandle = GCHandle.Alloc(_array, GCHandleType.Pinned);

            var typeId = H5T.copy(_dataset.DataType.NativeType.Value).ToId();

            int result = H5D.write(
                datasetId.Value,
                typeId.Value,
                H5S.ALL,
                H5S.ALL,
                H5P.DEFAULT,
                arrayHandle.AddrOfPinnedObject());

            arrayHandle.Free();

            H5T.close(typeId.Value);
            H5O.close(datasetId.Value);

            FileHelper.FlushToFile(_dataset.FileId);
        }

        public static void Write2DArray<T>(Hdf5Dataset _dataset, T[,] _array)
        {
            if (_dataset.Dataspace.NumberOfDimensions != 2)
            {
                throw new Hdf5ArrayDimensionsMismatchException();
            }

            if ((ulong)_array.GetLength(0) != _dataset.Dataspace.DimensionProperties[0].CurrentSize)
            {
                throw new Hdf5ArraySizeMismatchException();
            }

            if ((ulong)_array.GetLength(1) != _dataset.Dataspace.DimensionProperties[1].CurrentSize)
            {
                throw new Hdf5ArraySizeMismatchException();
            }

            var datasetId = H5O.open(_dataset.FileId.Value, _dataset.Path.FullPath).ToId();

            GCHandle arrayHandle = GCHandle.Alloc(_array, GCHandleType.Pinned);

            var typeId = H5T.copy(_dataset.DataType.NativeType.Value).ToId();

            int result = H5D.write(
                datasetId.Value,
                typeId.Value,
                H5S.ALL,
                H5S.ALL,
                H5P.DEFAULT,
                arrayHandle.AddrOfPinnedObject());

            arrayHandle.Free();

            H5T.close(typeId.Value);
            H5O.close(datasetId.Value);

            FileHelper.FlushToFile(_dataset.FileId);
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
            ReadonlyNamedItemList<Hdf5Dataset> _datasets,
            Hdf5Identifier _fileId,
            Hdf5Path _parentPath,
            string _name,
            Hdf5DataTypes _datatype,
            int _numberOfDimensions,
            List<Hdf5DimensionProperty> _properties,
            Hdf5CompressionProperty _compressionProperty = null)
        {
            Hdf5Dataset dataset = CreateDataset(
                _fileId, 
                _parentPath, 
                _name, 
                _datatype,
                _numberOfDimensions,
                _properties,
                _compressionProperty);

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
            List<Hdf5DimensionProperty> _properties,
            Hdf5CompressionProperty _compressionProperty = null)
        {
            Hdf5Path path = _parentPath.Append(_name);

            UInt64[] dimensionSize = new UInt64[_numberOfDimensions];
            UInt64[] maxSize = null; // new UInt64[_numberOfDimensions];

            if (_numberOfDimensions != _properties.Count ||
                (_compressionProperty != null && _numberOfDimensions != _compressionProperty.ChunkDimensions.Length))
            {
                throw new Hdf5ArrayDimensionsMismatchException();
            }

            int i = 0;
            foreach (var property in _properties)
            {
                dimensionSize[i] = property.CurrentSize;

                if (_compressionProperty != null && _compressionProperty.ChunkDimensions[i] > property.CurrentSize)
                {
                    throw new Hdf5ArraySizeMismatchException();
                }

                i++;
            }

            Hdf5Identifier dataspaceId = H5S.create_simple(_numberOfDimensions, dimensionSize, maxSize).ToId();
            
            //TODO handle string datasets
            Hdf5Identifier typeId = H5T.copy(TypeHelper.GetNativeType(_datatype).Value).ToId();
            var status = H5T.set_order(typeId.Value, H5T.order_t.LE);

            var plist_id = _compressionProperty != null ? H5P.create(H5P.DATASET_CREATE) : 0;
            if (plist_id != 0)
            {
                H5P.set_chunk(plist_id, _compressionProperty.ChunkDimensions.Length, _compressionProperty.ChunkDimensions);
                H5P.set_deflate(plist_id, _compressionProperty.CompressionLevel);
            }

            Hdf5Identifier datasetId = H5D.create(_fileId.Value, path.FullPath, typeId.Value, dataspaceId.Value, dcpl_id: plist_id).ToId();

            Hdf5Dataset dataset = null;

            if (datasetId.Value > 0)
            {
                dataset = new Hdf5Dataset(_fileId, datasetId, path.FullPath)
                {
                    DataType = TypeHelper.GetDataTypeFromDataset(datasetId),
                    Dataspace = DataspaceHelper.GetDataspace(datasetId)
                };

                H5D.close(datasetId.Value);
            }

            H5T.close(typeId.Value);
            if (plist_id != 0)
            {
                H5P.close(plist_id);
            }

            FileHelper.FlushToFile(_fileId);

            return dataset;
        }
    }
}



