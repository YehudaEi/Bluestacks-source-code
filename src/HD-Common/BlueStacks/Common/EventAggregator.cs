// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.EventAggregator
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BlueStacks.Common
{
  public static class EventAggregator
  {
    private static Dictionary<Type, IList> mSubscriber = new Dictionary<Type, IList>();

    public static void Publish<TMessageType>(TMessageType message)
    {
      Type key = typeof (TMessageType);
      if (!EventAggregator.mSubscriber.ContainsKey(key))
        return;
      foreach (Subscription<TMessageType> subscription in (IEnumerable) new List<Subscription<TMessageType>>(EventAggregator.mSubscriber[key].Cast<Subscription<TMessageType>>()))
        subscription.Action(message);
    }

    public static Subscription<TMessageType> Subscribe<TMessageType>(
      System.Action<TMessageType> action)
    {
      Type key = typeof (TMessageType);
      Subscription<TMessageType> subscription = new Subscription<TMessageType>(action);
      IList list1;
      if (!EventAggregator.mSubscriber.TryGetValue(key, out list1))
      {
        IList list2 = (IList) new List<Subscription<TMessageType>>();
        list2.Add((object) subscription);
        EventAggregator.mSubscriber.Add(key, list2);
      }
      else
        list1.Add((object) subscription);
      return subscription;
    }

    public static void Unsubscribe<TMessageType>(Subscription<TMessageType> subscription)
    {
      Type key = typeof (TMessageType);
      if (!EventAggregator.mSubscriber.ContainsKey(key))
        return;
      EventAggregator.mSubscriber[key].Remove((object) subscription);
    }
  }
}
