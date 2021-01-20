// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.AnimationCache
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace BlueStacks.Common
{
  internal static class AnimationCache
  {
    private static readonly Dictionary<AnimationCache.CacheKey, ObjectAnimationUsingKeyFrames> _animationCache = new Dictionary<AnimationCache.CacheKey, ObjectAnimationUsingKeyFrames>();
    private static readonly Dictionary<AnimationCache.CacheKey, int> _referenceCount = new Dictionary<AnimationCache.CacheKey, int>();

    public static void IncrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
    {
      AnimationCache.CacheKey key = new AnimationCache.CacheKey(source, repeatBehavior);
      int num1;
      AnimationCache._referenceCount.TryGetValue(key, out num1);
      int num2 = num1 + 1;
      AnimationCache._referenceCount[key] = num2;
    }

    public static void DecrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
    {
      AnimationCache.CacheKey key = new AnimationCache.CacheKey(source, repeatBehavior);
      int num;
      AnimationCache._referenceCount.TryGetValue(key, out num);
      if (num > 0)
      {
        --num;
        AnimationCache._referenceCount[key] = num;
      }
      if (num != 0)
        return;
      AnimationCache._animationCache.Remove(key);
      AnimationCache._referenceCount.Remove(key);
    }

    public static void AddAnimation(
      ImageSource source,
      RepeatBehavior repeatBehavior,
      ObjectAnimationUsingKeyFrames animation)
    {
      AnimationCache.CacheKey index = new AnimationCache.CacheKey(source, repeatBehavior);
      AnimationCache._animationCache[index] = animation;
    }

    public static void RemoveAnimation(
      ImageSource source,
      RepeatBehavior repeatBehavior,
      ObjectAnimationUsingKeyFrames _)
    {
      AnimationCache.CacheKey key = new AnimationCache.CacheKey(source, repeatBehavior);
      AnimationCache._animationCache.Remove(key);
    }

    public static ObjectAnimationUsingKeyFrames GetAnimation(
      ImageSource source,
      RepeatBehavior repeatBehavior)
    {
      AnimationCache.CacheKey key = new AnimationCache.CacheKey(source, repeatBehavior);
      ObjectAnimationUsingKeyFrames animationUsingKeyFrames;
      AnimationCache._animationCache.TryGetValue(key, out animationUsingKeyFrames);
      return animationUsingKeyFrames;
    }

    private class CacheKey
    {
      private readonly ImageSource _source;
      private readonly RepeatBehavior _repeatBehavior;

      public CacheKey(ImageSource source, RepeatBehavior repeatBehavior)
      {
        this._source = source;
        this._repeatBehavior = repeatBehavior;
      }

      private bool Equals(AnimationCache.CacheKey other)
      {
        return AnimationCache.CacheKey.ImageEquals(this._source, other._source) && object.Equals((object) this._repeatBehavior, (object) other._repeatBehavior);
      }

      public override bool Equals(object obj)
      {
        if (obj == null)
          return false;
        if (this == obj)
          return true;
        return obj.GetType() == this.GetType() && this.Equals((AnimationCache.CacheKey) obj);
      }

      public override int GetHashCode()
      {
        return AnimationCache.CacheKey.ImageGetHashCode(this._source) * 397 ^ this._repeatBehavior.GetHashCode();
      }

      private static int ImageGetHashCode(ImageSource image)
      {
        if (image != null)
        {
          Uri uri = AnimationCache.CacheKey.GetUri(image);
          if (uri != (Uri) null)
            return uri.GetHashCode();
        }
        return 0;
      }

      private static bool ImageEquals(ImageSource x, ImageSource y)
      {
        if (object.Equals((object) x, (object) y))
          return true;
        if (x == null != (y == null) || x.GetType() != y.GetType())
          return false;
        Uri uri1 = AnimationCache.CacheKey.GetUri(x);
        Uri uri2 = AnimationCache.CacheKey.GetUri(y);
        return uri1 != (Uri) null && uri1 == uri2;
      }

      private static Uri GetUri(ImageSource image)
      {
        if (image is BitmapImage bitmapImage && bitmapImage.UriSource != (Uri) null)
        {
          if (bitmapImage.UriSource.IsAbsoluteUri)
            return bitmapImage.UriSource;
          if (bitmapImage.BaseUri != (Uri) null)
            return new Uri(bitmapImage.BaseUri, bitmapImage.UriSource);
        }
        if (image is BitmapFrame bitmapFrame)
        {
          string uriString = bitmapFrame.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          Uri result;
          if (uriString != bitmapFrame.GetType().FullName && Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out result))
          {
            if (result.IsAbsoluteUri)
              return result;
            if (bitmapFrame.BaseUri != (Uri) null)
              return new Uri(bitmapFrame.BaseUri, result);
          }
        }
        return (Uri) null;
      }
    }
  }
}
