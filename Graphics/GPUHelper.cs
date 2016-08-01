using UnityEngine;

namespace Demonixis.Toolbox.Graphics
{
    public static class GPUHelper
    {
        private static readonly string[] GoodNVGPUs = new string[]
        {
            "1080", "1070",
            "980", "970",
            "780"
        };

        private static readonly string[] MiddleNVGPUs = new string[]
        {
            "1060", "1050",
            "960", "950",
            "770", "760",
            "680", "670"
        };

        private static readonly string[] GoodAMDGPUs = new string[]
        {
            "480", "470",
            "390", "290"
        };

        private static readonly string[] MiddleAMDGPUs = new string[]
        {
           "460",
            "380", "280", "270"
        };

        public enum GPUQuality
        {
            Good, Middle, Low
        }

        public enum GPUVendor
        {
            AMD = 0, Intel, Nvidia, Unknow
        }

        public struct GPUInfo
        {
            public GPUQuality Quality;
            public GPUVendor Vendor;
        }

        public static GPUInfo GetGPUInfo()
        {
            var gpuInfo = SystemInfo.graphicsDeviceName.ToLower();
            var isNvidiaGPU = gpuInfo.IndexOf("nvidia") > -1;
            var isAMDGPU = gpuInfo.IndexOf("amd") > -1;
            var isIntelGPU = gpuInfo.IndexOf("intel") > -1;

            if (isAMDGPU)
                return CheckVendor(GPUVendor.AMD, gpuInfo, GoodAMDGPUs, MiddleAMDGPUs);
            else if (isNvidiaGPU)
                return CheckVendor(GPUVendor.Nvidia, gpuInfo, GoodNVGPUs, MiddleNVGPUs);
            else if (isIntelGPU)
                return CheckVendor(GPUVendor.Intel, gpuInfo, null, null);
            else
                return CheckVendor(GPUVendor.Unknow, gpuInfo, null, null);
        }

        private static GPUInfo CheckVendor(GPUVendor vendor, string search, string[] goodArray, string[] middleArray)
        {
            var gpuQuality = GPUQuality.Low;

            if (InArray(search, goodArray))
                gpuQuality = GPUQuality.Good;
            else if (InArray(search, middleArray))
                gpuQuality = GPUQuality.Middle;

            return new GPUInfo()
            {
                Quality = gpuQuality,
                Vendor = vendor
            };
        }

        private static bool InArray(string search, string[] array)
        {
            if (array != null)
            {
                for (int i = 0, l = array.Length; i < l; i++)
                    if (search.IndexOf(array[i]) > -1)
                        return true;
            }

            return false;
        }
    }
}
