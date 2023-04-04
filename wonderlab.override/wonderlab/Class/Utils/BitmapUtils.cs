﻿using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class BitmapUtils {
        public static IImage GetAssetBitmap(string uri) {
            var al = AvaloniaLocator.Current.GetService<IAssetLoader>();
            if (al != null)
            {
                using (var s = al.Open(new Uri($"resm:wonderlab.Assets.{uri}")))
                {
                    return new Bitmap(s);
                }
            }
            throw new Exception();
        }

        public static IImage GetIconBitmap(string uri) {       
            var al = AvaloniaLocator.Current.GetService<IAssetLoader>();
            if (al != null)
            {
                using (var s = al.Open(new Uri($"avares://wonderlab/Assets/Icons/{uri}")))
                {
                    return new Bitmap(s);
                }
            }
            throw new Exception();
        }

        /// <summary>
        /// 裁剪皮肤图片头像
        /// </summary>
        /// <returns>裁剪后图片</returns>
        public static async ValueTask<Image<Rgba32>> CropSkinHeadBitmap(byte[] stream) {       
            Image<Rgba32> head = (Image<Rgba32>)Image.Load(stream);
            head.Mutate(x => x.Crop(Rectangle.FromLTRB(8, 8, 16, 16)));

            Image<Rgba32> hat = (Image<Rgba32>)Image.Load(stream);
            hat.Mutate(x => x.Crop(Rectangle.FromLTRB(40, 8, 48, 16)));

            Image<Rgba32> endImage = new Image<Rgba32>(8, 8);
            for (int i = 0; i < 8; i++) {           
                for (int j = 0; j < 8; j++) {               
                    endImage[i, j] = head[i, j];
                    if (hat[i, j].A == 255) {                   
                        endImage[i, j] = hat[i, j];
                    }
                }
            }

            return await Task.FromResult(ResizeImage(endImage, 60, 60));
        }

        /// <summary>
        /// 裁剪皮肤图片身体
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="skin"></param>
        /// <returns></returns>
        public static Image<TPixel> CropSkinBodyBitmap<TPixel>(Image<TPixel> skin) where TPixel : unmanaged, IPixel<TPixel> {       
            Image<TPixel> Body = CopyBitmap(skin);
            Body.Mutate(x => x.Crop(Rectangle.FromLTRB(20, 20, 28, 32)));
            return ResizeImage(Body, 60, 90);
        }

        /// <summary>
        /// 裁剪皮肤图片右手
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="skin"></param>
        /// <returns></returns>
        public static Image<TPixel> CropRightHandBitmap<TPixel>(Image<TPixel> skin) where TPixel : unmanaged, IPixel<TPixel> {       
            Image<TPixel> Arm = CopyBitmap(skin);
            Arm.Mutate(x => x.Crop(Rectangle.FromLTRB(35, 52, 39, 64)));
            return ResizeImage(Arm, 30, 90);
        }

        /// <summary>
        /// 裁剪皮肤图片左手
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="skin"></param>
        /// <returns></returns>
        public static Image<TPixel> CropLeftHandBitmap<TPixel>(Image<TPixel> skin) where TPixel : unmanaged, IPixel<TPixel> {       
            Image<TPixel> Arm = CopyBitmap(skin);
            Arm.Mutate(x => x.Crop(Rectangle.FromLTRB(44, 20, 48, 32)));
            return ResizeImage(Arm, 30, 90);
        }

        /// <summary>
        /// 裁剪皮肤图片右腿
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="skin"></param>
        /// <returns></returns>
        public static Image<TPixel> CropRightLegBitmap<TPixel>(Image<TPixel> skin) where TPixel : unmanaged, IPixel<TPixel>
        {
            Image<TPixel> Leg = CopyBitmap(skin);
            Leg.Mutate(x => x.Crop(Rectangle.FromLTRB(20, 52, 24, 64)));
            return ResizeImage(Leg, 30, 90);
        }

        /// <summary>
        /// 裁剪皮肤图片左腿
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="skin"></param>
        /// <returns></returns>
        public static Image<TPixel> CropLeftLegBitmap<TPixel>(Image<TPixel> skin) where TPixel : unmanaged, IPixel<TPixel>
        {
            Image<TPixel> Leg = CopyBitmap(skin);
            Leg.Mutate(x => x.Crop(Rectangle.FromLTRB(4, 20, 8, 32)));
            return ResizeImage(Leg, 30, 90);
        }

        /// <summary>
        /// 基础裁剪方法
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Image<TPixel> CopyBitmap<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel> {       
            Image<TPixel> tmp = new(image.Width, image.Height);
            for (int i = 0; i < image.Width; i++) {           
                for (int j = 0; j < image.Height; j++) {               
                    tmp[i, j] = image[i, j];
                }
            }
            return tmp;
        }

        /// <summary>
        /// 重置图片长宽
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="image"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Image<TPixel> ResizeImage<TPixel>(Image<TPixel> image, int w, int h) where TPixel : unmanaged, IPixel<TPixel> {       
            Image<TPixel> image2 = new(w, h);
            for (int i = 0; i < w; i++) {           
                for (int j = 0; j < h; j++) {               
                    double tmp;
                    tmp = (double)image.Width / (double)w;
                    double realW = tmp * (i);
                    tmp = (double)image.Height / (double)h;
                    double realH = (tmp) * (j);
                    image2[i, j] = image[(int)realW, (int)realH];
                }
            }

            return image2;
        }

    }
}
