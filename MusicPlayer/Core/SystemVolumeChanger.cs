using System;
using System.Runtime.InteropServices;

namespace MusicPlayer.Core
{
    class SystemVolumeChanger
    {
        // CoInitialize function
        [DllImport("ole32.dll")]
        static extern int CoInitialize(IntPtr pvReserved);

        // CoCreateInstance function
        [DllImport("ole32.dll")]
        static extern int CoCreateInstance(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
            IntPtr pUnkOuter,
            uint dwClsContext,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            out IMMDeviceEnumerator ppv
        );

        [ComImport]
        [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IMMDeviceCollection
        {
            int GetCount(out int pcDevices);
            int Item(int nDevice, out IMMDevice ppDevice);
        }

        // Definitions of MMDeviceAPI.h interfaces and structures
        [ComImport]
        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IMMDeviceEnumerator
        {
            int EnumAudioEndpoints(int dataFlow, int dwStateMask, out IMMDeviceCollection ppDevices);
            int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppEndpoint);
        }

        [ComImport]
        [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IMMDevice
        {
            int Activate([MarshalAs(UnmanagedType.LPStruct)] Guid iid, uint dwClsCtx, IntPtr pActivationParams, out IAudioEndpointVolume ppInterface);
        }

        [ComImport]
        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IAudioEndpointVolume
        {
            int RegisterControlChangeNotify(IntPtr pNotify);
            int UnregisterControlChangeNotify(IntPtr pNotify);
            int GetChannelCount(out uint pnChannelCount);
            int SetMasterVolumeLevel(float fLevelDB, ref Guid pguidEventContext);
            int SetMasterVolumeLevelScalar(float fLevel, ref Guid pguidEventContext);
            int GetMasterVolumeLevel(out float pfLevelDB);
            int GetMasterVolumeLevelScalar(out float pfLevel);
            int SetChannelVolumeLevel(uint nChannel, float fLevelDB, ref Guid pguidEventContext);
            int SetChannelVolumeLevelScalar(uint nChannel, float fLevel, ref Guid pguidEventContext);
            int GetChannelVolumeLevel(uint nChannel, out float pfLevelDB);
            int GetChannelVolumeLevelScalar(uint nChannel, out float pfLevel);
            int SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, ref Guid pguidEventContext);
            int GetMute(out bool pbMute);
            int GetVolumeStepInfo(out uint pnStep, out uint pnStepCount);
            int VolumeStepUp(ref Guid pguidEventContext);
            int VolumeStepDown(ref Guid pguidEventContext);
            int QueryHardwareSupport(out uint pdwHardwareSupportMask);
            int GetVolumeRange(out float pflVolumeMindB, out float pflVolumeMaxdB, out float pflVolumeIncrementdB);
        }

        private static bool IsActive { get; set; }

        private IMMDeviceEnumerator DeviceEnumerator { get; set; }
        private IMMDevice Device { get; set; }
        private IAudioEndpointVolume AudioEndpointVolume { get; set; }
        private int Hr { get; set; }

        public SystemVolumeChanger()
        {
            // Well
            if (IsActive)
            {
                return;
            }
            IsActive = true;

            //
            CoInitialize(IntPtr.Zero);

            IMMDeviceEnumerator deviceEnumerator = null;
            IMMDevice device = null;
            IAudioEndpointVolume audioEndpointVolume = null;

            // Get enumerator for audio endpoint devices.
            Hr = CoCreateInstance(
                 new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E"),
                 IntPtr.Zero,
                 1,
                 new Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"),
                 out deviceEnumerator
             );

            // Get default audio-rendering device.
            int dataFlow = 0; // eRender
            int role = 0; // eConsole
            Hr = deviceEnumerator.GetDefaultAudioEndpoint(dataFlow, role, out device);

            Guid IID_IAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");
            IntPtr pActivationParams = IntPtr.Zero;

            Hr = device.Activate(IID_IAudioEndpointVolume, 23, pActivationParams, out audioEndpointVolume);

            //
            DeviceEnumerator = deviceEnumerator;
            Device = device;
            AudioEndpointVolume = audioEndpointVolume;
        }

        public void SetSystemVolume(float volume)
        {
            volume = Math.Clamp(volume, 0.0f, 100.0f);
            Guid eventContext = Guid.Empty;
            Hr = AudioEndpointVolume.SetMasterVolumeLevel(volume, eventContext);
        }

        public float GetSystemVolume()
        {
            float volume = 0.0f;
            Hr = AudioEndpointVolume.GetMasterVolumeLevel(out volume);
            return volume;
        }

        public void IncreaseSystemVolume(float volume)
        {
            SetSystemVolume(GetSystemVolume() + volume);
        }

        public void DecreaseSystemVolume(float volume)
        {
            SetSystemVolume(GetSystemVolume() - volume);
        }
    }
}
