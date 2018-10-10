using System;
using System.Collections.Generic;
using System.Diagnostics;
using AVFoundation;
using CoreAnimation;
using CoreMedia;
using DarkId.SmartGlass.Nano.Consumer;
using DarkId.SmartGlass.Nano.Packets;
using Foundation;

namespace DarkId.SmartGlass.Nano.AVFoundation
{
    public class VideoEngineManager : IDisposable
    {
        private CoreGraphics.CGSize _videoDimensions;
        private CMVideoFormatDescription _videoFormatDescription;
        private AVSampleBufferDisplayLayer _displayLayer;

        public VideoEngineManager(VideoFormat format)
        {
            if (format.Codec != VideoCodec.H264)
            {
                throw new InvalidOperationException("VideoEngineManager only supports H264");
            }

            InitDisplayLayer();
        }

        private void InitDisplayLayer()
        {
            _displayLayer = new AVSampleBufferDisplayLayer();
            //TODO: ViewController.setDisplayLayer()
        }

        public int EnqueueH264Nalu(CMBlockBuffer naluData)
        {
            if (_displayLayer.Status == AVQueuedSampleBufferRenderingStatus.Failed)
            {
                InitDisplayLayer();
                return 1;
            }

            CMSampleBufferError sampleErr;
            CMSampleBuffer sampleBuf = CMSampleBuffer.CreateReady(
                dataBuffer: naluData,
                formatDescription: _videoFormatDescription,
                samplesCount: 1,
                sampleTimingArray: null,
                sampleSizeArray: null,
                error: out sampleErr);

            if (sampleErr != CMSampleBufferError.None)
            {
                Debug.WriteLine($"CMSampleBuffer.CreateReady failed, err: {sampleErr}");
                return 2;
            }

            _displayLayer.Enqueue(sampleBuf);

            return 0;
        }

        public int ConsumeVideoData(long timestamp, byte[] frameData)
        {
            // Source: https://mobisoftinfotech.com/resources/mguide/h264-encode-decode-using-videotoolbox/

            int frameSize = frameData.Length;

            // I know how my H.264 data source's NALUs looks like so I know start code index is always 0.
            // if you don't know where it starts, you can use a for loop similar to how I find the 2nd and 3rd start codes
            int startCodeIndex = 0;
            int secondStartCodeIndex = 0;
            int thirdStartCodeIndex = 0;

            int _spsSize = 0;
            int _ppsSize = 0;

            long blockLength = 0;

            byte nalu_type = (byte)(frameData[startCodeIndex + 4] & 0x1F);

            // if we havent already set up our format description with our SPS PPS parameters, we
            // can't process any frames except type 7 that has our parameters
            if (nalu_type != 7 && _videoFormatDescription == null)
            {
                Debug.WriteLine($"Video error: Frame is not an I Frame and format description is null");
                return 1;
            }

            // NALU type 7 is the SPS parameter NALU
            if (nalu_type == 7)
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
                nalu_type = (byte)(frameData[secondStartCodeIndex + 4] & 0x1F);
                Debug.WriteLine($"Received NALU Type {nalu_type}");
            }


            // type 8 is the PPS parameter NALU
            if (nalu_type == 8)
            {

                // find where the NALU after this one starts so we know how long the PPS parameter is
                for (int i = _spsSize + 12; i < _spsSize + 50; i++)
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

                // allocate enough data to fit the SPS and PPS parameters into our data objects.
                // VTD doesn't want you to include the start code header (4 bytes long) so we add the - 4 here
                /*
                    uint8_t* data = NULL;
                    uint8_t* pps = NULL;
                    uint8_t* sps = NULL;
                    */
                byte[] pps = new byte[_spsSize - 4];
                byte[] sps = new byte[_ppsSize - 4];

                // copy in the actual sps and pps values, again ignoring the 4 byte header
                /*
                memcpy(sps, &frame[4], _spsSize - 4);
                memcpy(pps, &frame[_spsSize + 4], _ppsSize - 4);
                */
                Array.Copy(frameData, 4, sps, 0, _spsSize - 4);
                Array.Copy(frameData, _spsSize + 4, pps, 0, _ppsSize - 4);

                // now we set our H264 parameters
                List<byte[]> parameterSetPointers = new List<byte[]> { sps, pps };
                parameterSetPointers.Add(sps);

                CMFormatDescriptionError formatDescError;
                _videoFormatDescription = CMVideoFormatDescription.FromH264ParameterSets(
                    parameterSets: null,
                    nalUnitHeaderLength: 0,
                    error: out formatDescError);

                if (formatDescError != CMFormatDescriptionError.None)
                {
                    Debug.WriteLine($"CMVideoFormatDescription.FromH264ParameterSets failed, err: {formatDescError}");
                    return 2;
                }

                _videoDimensions = _videoFormatDescription.GetPresentationDimensions(false, false);

                // now lets handle the IDR frame that (should) come after the parameter sets
                // I say "should" because that's how I expect my H264 stream to work, YMMV
                nalu_type = (byte)(frameData[thirdStartCodeIndex + 4] & 0x1F);
                Debug.WriteLine($"Received NALU Type {nalu_type}");
            }

            // NALU type 1 is non-IDR (or PFrame) picture
            // NALU type 5 is an IDR frame NALU.  The SPS and PPS NALUs should always be followed by an IDR (or IFrame) NALU, as far as I know
            if (nalu_type == 1 || nalu_type == 5)
            {
                // P-Frame (1): non-IDR frames do not have an offset due to SPS and PSS
                // I-Frame (5): find the offset, or where the SPS and PPS NALUs end and the IDR frame NALU begins

                CMBlockBufferError blockBufErr;
                CMBlockBuffer blockBuf;

                int offset = nalu_type == 5 ? _spsSize + _ppsSize : 0;
                blockLength = frameSize - offset;

                byte[] naluBuf = new byte[blockLength];
                Array.Copy(frameData, offset, naluBuf, 0, blockLength);

                // replace the start code header on this NALU with its size.
                // AVCC format requires that you do this.
                // htonl converts the unsigned int from host to network byte order
                UInt32 dataLength32 = (UInt32)(blockLength - 4);

                naluBuf[0] = (byte)((dataLength32 & 0xFF000000) >> 24);
                naluBuf[1] = (byte)((dataLength32 & 0x00FF0000) >> 16);
                naluBuf[2] = (byte)((dataLength32 & 0x0000FF00) >> 8);
                naluBuf[3] = (byte)((dataLength32 & 0x000000FF));

                blockBuf = CMBlockBuffer.FromMemoryBlock(
                    naluBuf, 0, 0, out blockBufErr);

                if (blockBufErr != CMBlockBufferError.None)
                {
                    Debug.WriteLine($"BlockBufferCreation IFrame failed, err: {blockBufErr}");
                    return 3;
                }
            }
            return 0;
        }

        public void Dispose()
        {
            _displayLayer.Dispose();
        }
    }
}
