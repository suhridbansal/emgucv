using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Emgu.Util;

namespace Emgu.CV.Reflection
{
   /// <summary>
   /// A collection of reflection function that can be applied to IImage object
   /// </summary>
   public class ReflectIImage
   {
      /// <summary>
      /// Get all the methods that belongs to the IImage and Image class with ExposableMethodAttribute set true.
      /// </summary>
      /// <returns></returns>
      public static IEnumerable<KeyValuePair<String, MethodInfo>> GetImageMethods(IImage image)
      {
         if (image != null)
         {
            List<MethodInfo> methods = new List<MethodInfo>();
            methods.AddRange(image.GetType().GetMethods());
            //methods.AddRange(image.GetType().GetInterface("IImage").GetMethods());

            foreach (MethodInfo mi in methods)
            {
               Object[] atts = mi.GetCustomAttributes(typeof(ExposableMethodAttribute), false);
               if (atts.Length > 0)
               {
                  ExposableMethodAttribute att = (ExposableMethodAttribute)atts[0];
                  if (att.Exposable)
                     yield return new KeyValuePair<String, MethodInfo>(att.Category, mi as MethodInfo);
               }
            }
         }
      }

      /// <summary>
      /// Get the color type of the image
      /// </summary>
      /// <param name="image">the image to apply reflection on</param>
      /// <returns>the color type of the image</returns>
      public static Type GetTypeOfColor(IImage image)
      {
         Type imageType = Toolbox.GetBaseType(image.GetType(), "Image`2");
         return imageType.GetGenericArguments()[0];
      }

      /// <summary>
      /// Get the depth type of the image
      /// </summary>
      /// <param name="image">the image to apply reflection on</param>
      /// <returns>The depth type of the image</returns>
      public static Type GetTypeOfDepth(IImage image)
      {
         Type imageType = Toolbox.GetBaseType(image.GetType(), "Image`2");
         return imageType.GetGenericArguments()[1];
      }

      /// <summary>
      /// Get the color at the specific location of the image
      /// </summary>
      /// <param name="image">The image to obtain pixel value from</param>
      /// <param name="location">The location to sample a pixel</param>
      /// <returns>The color at the specific location</returns>
      public static ColorType GetPixelColor(IImage image, System.Drawing.Point location)
      {
         location.X = Math.Min(location.X, image.Width - 1);
         location.Y = Math.Min(location.Y, image.Height - 1);

         Type t = image.GetType();
         MethodInfo indexers = t.GetMethod("get_Item", new Type[2] { typeof(int), typeof(int) });
         if (indexers != null)
         {
            return indexers.Invoke(image, new object[2] { location.Y, location.X }) as ColorType;
         }
         else
         {
            return new Bgra();
         }
      }
   }
}