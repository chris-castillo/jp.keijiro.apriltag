using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Color32 = UnityEngine.Color32;
using ImageU8 = AprilTag.Interop.ImageU8;

namespace AprilTag {

//
// Burst-accelerated image convertion functions
//
[BurstCompile]
static class ImageConverter
{
    public unsafe static void
      Convert(ReadOnlySpan<Color32> data, ImageU8 image)
    {
        fixed (Color32* src = &data.GetPinnableReference())
            fixed (byte* dst = &image.Buffer.GetPinnableReference())
                BurstConvert(src, dst, image.Width, image.Height, image.Stride);
    }

    public unsafe static void
      Convert(NativeArray<byte> data, ImageU8 image)
    {
        var src = (byte*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(data);
        fixed (byte* dst = &image.Buffer.GetPinnableReference())
            BurstCopy(src, dst, image.Width, image.Height, image.Stride);
    }

    [BurstCompile]
    unsafe static void BurstConvert
      (Color32* src, byte* dst, int width, int height, int stride)
    {
        var offs_src = 0;
        var offs_dst = stride * (height - 1);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
                dst[offs_dst + x] = src[offs_src + x].g;

            offs_src += width;
            offs_dst -= stride;
        }
    }

    [BurstCompile]
    unsafe static void BurstCopy
      (byte* src, byte* dst, int width, int height, int stride)
    {
        var offs_src = 0;
        var offs_dst = stride * (height - 1);

        for (var y = 0; y < height; y++)
        {
            UnsafeUtility.MemCpy(dst + offs_dst, src + offs_src, (uint)width);

            offs_src += width;
            offs_dst -= stride;
        }
    }
}

} // namespace AprilTag
