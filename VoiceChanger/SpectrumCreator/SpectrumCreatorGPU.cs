using Silk.NET.OpenCL;
using System;
using System.Collections.Generic;
using System.Text;
using VoiceChanger.FormatParser;
using VoiceChanger.Properties;
using VoiceChanger.Utils;

namespace VoiceChanger.SpectrumCreator
{
    public abstract class SpectrumCreatorGPU : SpectrumCreator
    {
        private readonly CL _cl;
        private nint _platformId;
        private nint _deviceId;
        private nint _context;
        private nint _commandQueue;
        private nint _program;
        private IntPtr _kernelHandle;

        public SpectrumCreatorGPU(AudioContainer audioContainer) : base(audioContainer)
        {
            _cl = CL.GetApi();
            InitializePlatformId();
            InitializeDeviceId();
            CreateContext();
            CreateCommandQueue();
            BuildProgram();
        }

        ~SpectrumCreatorGPU()
        {
            Dispose(false);
        }

        protected virtual string KernelName => "ComputeSpectrum";

        protected abstract byte[] KernelSource { get; }

        public override unsafe SpectrumContainer CreateSpectrum(SpectrumCreatorSettings settings)
        {
            nuint signalsBufferElementsCount = (nuint)AudioContainer.SamplesCount;
            nuint signalsBufferSize = signalsBufferElementsCount * sizeof(float);
            var signalsBuffer = _cl.CreateBuffer(_context, CLEnum.MemReadOnly, signalsBufferSize, null, out var errorCode);
            CheckSuccess(errorCode);

            nuint bufferOutputElementsCount = 10;
            nuint bufferKernelOutputSize = bufferOutputElementsCount * sizeof(float);
            var bufferKernelOutput = _cl.CreateBuffer(_context, CLEnum.MemWriteOnly, bufferKernelOutputSize, null, out errorCode);
            CheckSuccess(errorCode);

            var signalsInputHostBuffer = new UnmanagedArray<float>(signalsBufferElementsCount);
            signalsInputHostBuffer[0] = 6;
            signalsInputHostBuffer[1] = 8;
            _cl.EnqueueWriteBuffer(_commandQueue, signalsBuffer, true, 0, signalsBufferSize, signalsInputHostBuffer, 0, 0, out var @event);


            var res = _cl.SetKernelArg(_kernelHandle, 0, (nuint)sizeof(nuint), signalsBuffer);
            CheckSuccess(res);
            res = _cl.SetKernelArg(_kernelHandle, 1, (nuint)sizeof(nuint), bufferKernelOutput);
            CheckSuccess(res);

            //var globalWorkSize = new UnmanagedArray<nuint>(3)
            //{
            //    bufferInputElementsCount, 0, 0
            //};
            var localWorkSize = new UnmanagedArray<nuint>(3)
            {
                1, 1, 1
            };
            res = _cl.EnqueueNdrangeKernel(_commandQueue, _kernelHandle, 1, 0, signalsBufferElementsCount, localWorkSize, 0, null, out var calculateEvent);
            CheckSuccess(res);

            _cl.Finish(_commandQueue);

            var outputHostBuffer = new UnmanagedArray<float>(bufferKernelOutputSize / sizeof(float));
            res = _cl.EnqueueReadBuffer(_commandQueue, bufferKernelOutput, true, 0, bufferKernelOutputSize, outputHostBuffer, 0, null, out @event);
            CheckSuccess(res);

            var slices = new List<SpectrumSlice>();
            SpectrumContainer = new SpectrumContainer(slices);

            var x = outputHostBuffer[0];


            _cl.ReleaseMemObject(signalsBuffer);
            _cl.ReleaseMemObject(bufferKernelOutput);
            _cl.ReleaseKernel(_kernelHandle);
            _cl.ReleaseProgram(_program);


            return SpectrumContainer;
        }

        private unsafe void BuildProgram()
        {
            int errorCode;
            fixed (byte* kernelSourcePtr = KernelSource)
            {
                _program = _cl.CreateProgramWithSource(_context, 1, kernelSourcePtr, (nuint)KernelSource.Length, out errorCode);
                CheckSuccess(errorCode);
            }

            var res = _cl.BuildProgram(_program, 1, _deviceId, 0, NotifyCallback, null);
            CheckSuccess(res);

            _kernelHandle = _cl.CreateKernel(_program, KernelName, out errorCode);
            CheckSuccess(errorCode);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _cl.Flush(_commandQueue);
            _cl.Finish(_commandQueue);
            _cl.ReleaseCommandQueue(_commandQueue);
            _cl.ReleaseContext(_context);

            _disposed = true;

            // Call the base class implementation.
            base.Dispose(disposing);
        }

        private unsafe void CreateCommandQueue()
        {
            _commandQueue = _cl.CreateCommandQueue(_context, _deviceId, 0, out var errorCode);
            CheckSuccess(errorCode);
        }

        private unsafe void CreateContext()
        {
            _context = _cl.CreateContext(null, 1, _deviceId, NotifyCallback, null, out var errorCode);
            CheckSuccess(errorCode);
        }

        private unsafe void InitializeDeviceId()
        {
            var res = _cl.GetDeviceIDs(_platformId, CLEnum.DeviceTypeAll, 0, null, out var devicesCount);
            CheckSuccess(res);
            if (devicesCount == 0)
            {
                throw new Exception("OpenCL Devices not found.");
            }
            using var deviceIDs = new UnmanagedArray<nint>(devicesCount);
            res = _cl.GetDeviceIDs(_platformId, CLEnum.DeviceTypeAll, devicesCount, deviceIDs, out _);
            CheckSuccess(res);
            PrintDeviceInfos(deviceIDs);
            _deviceId = deviceIDs[0];
        }

        private unsafe void InitializePlatformId()
        {
            var res = _cl.GetPlatformIDs(0, null, out var platformsCount);
            CheckSuccess(res);
            if (platformsCount == 0)
            {
                throw new Exception("OpenCL Platforms not found.");
            }
            using var platformIDs = new UnmanagedArray<nint>(platformsCount);
            res = _cl.GetPlatformIDs(platformsCount, platformIDs, out _);
            CheckSuccess(res);
            PrintPlatformInfos(platformIDs);
            _platformId = platformIDs[0];
        }

        private unsafe void PrintDeviceInfos(UnmanagedArray<IntPtr> deviceIDs)
        {
            foreach (var deviceId in deviceIDs)
            {
                _cl.GetDeviceInfo(deviceId, (uint)CLEnum.DeviceAddressBits, 0, null, out var size1);
                _cl.GetDeviceInfo(deviceId, (uint)CLEnum.DeviceAvailable, 0, null, out var size2);
                _cl.GetDeviceInfo(deviceId, (uint)CLEnum.DeviceCompilerAvailable, 0, null, out var size3);
                _cl.GetDeviceInfo(deviceId, (uint)CLEnum.DeviceDoubleFPConfig, 0, null, out var size4);
                _cl.GetDeviceInfo(deviceId, (uint)CLEnum.DeviceEndianLittle, 0, null, out var size5);
                _cl.GetDeviceInfo(deviceId, (uint)CLEnum.DeviceErrorCorrectionSupport, 0, null, out var size6);
                _cl.GetDeviceInfo(deviceId, (uint)CLEnum.DeviceExecutionCapabilities, 0, null, out var size7);
                _cl.GetDeviceInfo(deviceId, (uint)CLEnum.DeviceExtensions, 0, null, out var size8);
            }
        }

        private unsafe void PrintPlatformInfos(UnmanagedArray<IntPtr> platformIDs)
        {
            foreach (var platformId in platformIDs)
            {
                _cl.GetPlatformInfo(platformId, (uint)CLEnum.PlatformProfile, 0, null, out var size1);
                _cl.GetPlatformInfo(platformId, (uint)CLEnum.PlatformVersion, 0, null, out var size2);
                _cl.GetPlatformInfo(platformId, (uint)CLEnum.PlatformName, 0, null, out var size3);
                _cl.GetPlatformInfo(platformId, (uint)CLEnum.PlatformVendor, 0, null, out var size4);
                _cl.GetPlatformInfo(platformId, (uint)CLEnum.PlatformExtensions, 0, null, out var size5);
                _cl.GetPlatformInfo(platformId, (uint)CLEnum.PlatformHostTimerResolution, 0, null, out var size6);
            }
        }

        private unsafe void NotifyCallback(byte* errinfo, void* privateInfo, nuint cb, void* userData)
        {
            var errorString = Encoding.ASCII.GetString(errinfo, 20);
            Console.WriteLine(errorString);
        }

        private void CheckSuccess(int result)
        {
            if (result != (int)CLEnum.Success)
            {
                throw new Exception("Operation result was not CL_SUCCESS. Error name: " + ((CLEnum)result).ToString());
            }
        }
    }
}
