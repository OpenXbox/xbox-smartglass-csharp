using System;
using System.Collections.Generic;
using System.IO;

namespace SmartGlass.Nano.Consumer
{
    public enum NalUnitType
    {
        UNSPECIFIED1,                    // 0: Unspecified (non-VCL)
        P_FRAME,                         // 1: Coded slice of a non-IDR picture (P-Frame) (VCL)
        PARTITION_A,                     // 2: Coded slice data partition A (VCL)
        PARTITION_B,                     // 3: Coded slice data partition B (VCL)
        PARTITION_C,                     // 4: Coded slice data partition C (VCL)
        I_FRAME,                         // 5: Coded slice of an IDR picture (I-Frame) (VCL)
        SUPPLEMENTAL_ENHANCEMENT_INFO,   // 6: Supplemental enhancement information (SEI) (non-VCL)
        SEQUENCE_PARAMETER_SET,          // 7: Sequence parameter set (SPS) (non-VCL)
        PICTURE_PARAMETER_SET,           // 8: Picture parameter set (PPS) (non-VCL)
        ACCESS_UNIT_DELIMITER,           // 9: Access unit delimiter (non-VCL)
        END_OF_SEQUENCE,                 // 10: End of sequence (non-VCL)
        END_OF_STREAM,                   // 11: End of stream (non-VCL)
        FILLER_DATA,                     // 12: Filler data (non-VCL)
        SEQUENCE_PARAMETER_SET_EXTENSION,// 13: Sequence parameter set extension (non-VCL)
        PREFIX_NAL_UNIT,                 // 14: Prefix NAL unit (non-VCL)
        SUBSET_SEQUENCE_PARAMETER_SET,   // 15: Subset sequence parameter set (non-VCL)
        RESERVED1,                       // 16: Reserved (non-VCL)
        RESERVED2,                       // 17: Reserved (non-VCL)
        RESERVED3,                       // 18: Reserved (non-VCL)
        SLICE_AUXILIARY_CODED_PICTURE,   // 19: Coded slice of an auxiliary coded picture without partitioning (non-VCL)
        CODED_SLICE_EXTENSION,           // 20: Coded slice extension (non-VCL)
        CODED_SLICE_EXT_FOR_DVC,         // 21: Coded slice extension for depth view components (non-VCL)
        RESERVED4,                       // 22: Reserved (non-VCL)
        RESERVED5,                       // 23: Reserved (non-VCL)
        STAP_A,                          // 24: STAP-A Single-time aggregation packet (non-VCL)
        STAP_B,                          // 25: STAP-B Single-time aggregation packet (non-VCL)
        MTAP16,                          // 26: MTAP16 Multi-time aggregation packet (non-VCL)
        MTAP24,                          // 27: MTAP24 Multi-time aggregation packet (non-VCL)
        FU_A,                            // 28: FU-A Fragmentation unit (non-VCL)
        FU_B,                            // 29: FU-B Fragmentation unit (non-VCL)
        UNSPECIFIED2,                    // 30: Unspecified (non-VCL)
        UNSPECIFIED3                     // 31: Unspecified (non-VCL)
    };

    public class H264Frame
    {
        private static readonly byte[] NAL_PREFIX = { 0x00, 0x00, 0x00, 0x01 };

        private bool _isParsed;

        private byte[] _ppsData;
        private byte[] _spsData;
        private byte[] _frameData;

        public byte[] RawData { get; private set; }
        public long TimeStamp { get; private set; }
        public uint FrameId { get; private set; }
        public List<NalUnitType> NalUnitTypes { get; private set; }

        public bool ContainsPPS
        {
            get
            {
                if (!_isParsed)
                    ParseData();
                return NalUnitTypes.Contains(NalUnitType.PICTURE_PARAMETER_SET);
            }
        }

        public bool ContainsSPS
        {
            get
            {
                if (!_isParsed)
                    ParseData();
                return NalUnitTypes.Contains(NalUnitType.SEQUENCE_PARAMETER_SET);
            }
        }

        public bool ContainsIFrame
        {
            get
            {
                if (!_isParsed)
                    ParseData();
                return NalUnitTypes.Contains(NalUnitType.I_FRAME);
            }
        }

        public bool ContainsPFrame
        {
            get
            {
                if (!_isParsed)
                    ParseData();
                return NalUnitTypes.Contains(NalUnitType.P_FRAME);
            }
        }

        public NalUnitType PrimaryType
        {
            get
            {
                return (NalUnitType)(RawData[4] & 0x1F);
            }
        }

        // Parsing is not done on initialization to save computation time,
        // in case of dropped frame
        public H264Frame(byte[] data, uint frameId, long timeStamp)
        {
            _isParsed = false;
            RawData = data;
            TimeStamp = timeStamp;
            FrameId = frameId;
            NalUnitTypes = new List<NalUnitType>();
        }

        public byte[] GetPpsData()
        {
            if (!_isParsed)
                ParseData();
            return _ppsData;
        }

        public byte[] GetPpsDataPrefixed()
        {
            if (!_isParsed)
                ParseData();
            if (_ppsData == null)
                return null;

            byte[] result = new byte[NAL_PREFIX.Length + _ppsData.Length];
            using (var stream = new MemoryStream(result))
            {
                stream.Write(NAL_PREFIX, 0, NAL_PREFIX.Length);
                stream.Write(_ppsData, 0, _ppsData.Length);
            }
            return result;
        }

        public byte[] GetSpsData()
        {
            if (!_isParsed)
                ParseData();
            return _spsData;
        }

        public byte[] GetSpsDataPrefixed()
        {
            if (!_isParsed)
                ParseData();
            if (_spsData == null)
                return null;

            byte[] result = new byte[NAL_PREFIX.Length + _spsData.Length];
            using (var stream = new MemoryStream(result))
            {
                stream.Write(NAL_PREFIX, 0, NAL_PREFIX.Length);
                stream.Write(_spsData, 0, _spsData.Length);
            }
            return result;
        }

        public byte[] GetFrameData()
        {
            if (!_isParsed)
                ParseData();
            return _frameData;
        }

        public byte[] GetFrameDataPrefixed()
        {
            if (!_isParsed)
                ParseData();
            if (_frameData == null)
                return null;

            byte[] result = new byte[NAL_PREFIX.Length + _frameData.Length];
            using (var stream = new MemoryStream(result))
            {
                stream.Write(NAL_PREFIX, 0, NAL_PREFIX.Length);
                stream.Write(_frameData, 0, _frameData.Length);
            }
            return result;
        }

        public byte[] GetFrameDataAvcc()
        {
            byte[] prefixedData = GetFrameDataPrefixed();
            if (prefixedData == null)
                return null;

            UInt32 dataLength = (UInt32)(prefixedData.Length - 4);
            prefixedData[0] = (byte)((dataLength & 0xFF000000) >> 24);
            prefixedData[1] = (byte)((dataLength & 0x00FF0000) >> 16);
            prefixedData[2] = (byte)((dataLength & 0x0000FF00) >> 8);
            prefixedData[3] = (byte)((dataLength & 0x000000FF));

            return prefixedData;
        }

        private void ParseData()
        {
            byte[] frameData = RawData;
            int frameSize = frameData.Length;

            // I know how my H.264 data source's NALUs looks like so I know start code index is always 0.
            // if you don't know where it starts, you can use a for loop similar to how I find the 2nd and 3rd start codes
            int startCodeIndex = 0;
            int secondStartCodeIndex = 0;
            int thirdStartCodeIndex = 0;

            int _spsSize = 0;
            int _ppsSize = 0;

            long blockLength = 0;

            _isParsed = true;

            NalUnitType type = (NalUnitType)(frameData[startCodeIndex + 4] & 0x1F);
            NalUnitTypes.Add(type);

            // NALU type 7 is the SPS parameter NALU
            if (type == NalUnitType.SEQUENCE_PARAMETER_SET)
            {
                // find where the second PPS start code begins, (the 0x00 00 00 01 code)
                // from which we also get the length of the first SPS code
                for (int i = startCodeIndex + 4; i < startCodeIndex + 40; i++)
                {
                    if (frameData[i] == 0x00 &&
                        frameData[i + 1] == 0x00 &&
                        frameData[i + 2] == 0x00 &&
                        frameData[i + 3] == 0x01)
                    {
                        secondStartCodeIndex = i;
                        _spsSize = secondStartCodeIndex;   // includes the header in the size
                        break;
                    }
                }

                // find what the second NALU type is
                type = (NalUnitType)(frameData[secondStartCodeIndex + 4] & 0x1F);
                NalUnitTypes.Add(type);
            }


            // type 8 is the PPS parameter NALU
            if (type == NalUnitType.PICTURE_PARAMETER_SET)
            {

                // find where the NALU after this one starts so we know how long the PPS parameter is
                for (int i = _spsSize + 8; i < _spsSize + 50; i++)
                {
                    if (frameData[i] == 0x00 &&
                        frameData[i + 1] == 0x00 &&
                        frameData[i + 2] == 0x00 &&
                        frameData[i + 3] == 0x01)
                    {
                        thirdStartCodeIndex = i;
                        _ppsSize = thirdStartCodeIndex - _spsSize;
                        break;
                    }
                }

                if (_ppsSize < 4 || _spsSize < 4)
                {
                    throw new InvalidDataException("Couldnt determine PPS/SPS size");
                }

                // allocate enough data to fit the SPS and PPS parameters into our data objects.
                // NOTE: buffers won't contain NAL start-code (00 00 00 01)
                _ppsData = new byte[_ppsSize - 4];
                _spsData = new byte[_spsSize - 4];

                // copy in the actual sps and pps values, again ignoring the 4 byte header
                Array.Copy(frameData,            4, _spsData, 0, _spsSize - 4);
                Array.Copy(frameData, _spsSize + 4, _ppsData, 0, _ppsSize - 4);

                // now lets handle the IDR frame that (should) come after the parameter sets
                // I say "should" because that's how I expect my H264 stream to work, YMMV
                type = (NalUnitType)(frameData[thirdStartCodeIndex + 4] & 0x1F);
                NalUnitTypes.Add(type);
            }

            // NALU type 1 is non-IDR (or PFrame) picture
            // NALU type 5 is an IDR frame NALU.  The SPS and PPS NALUs should always be followed by an IDR (or IFrame) NALU, as far as I know
            if (type == NalUnitType.P_FRAME || type == NalUnitType.I_FRAME)
            {
                // P-Frame (1): non-IDR frames do not have an offset due to SPS and PSS
                // I-Frame (5): find the offset, or where the SPS and PPS NALUs end and the IDR frame NALU begins
                int offset = type == NalUnitType.I_FRAME ? _spsSize + _ppsSize : 0;
                blockLength = frameSize - offset;

                // copy in the actual frame data, again ignoring the 4 byte header
                _frameData = new byte[blockLength - 4];
                Array.Copy(frameData, offset + 4, _frameData, 0, blockLength - 4);
            }
        }
    }
}
