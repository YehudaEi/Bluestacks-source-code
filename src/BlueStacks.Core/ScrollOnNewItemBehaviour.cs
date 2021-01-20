// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.ScrollOnNewItemBehaviour
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace BlueStacks.Core
{
  public class ScrollOnNewItemBehaviour : Behavior<ItemsControl>
  {
    protected override void OnAttached()
    {
      this.AssociatedObject.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.AssociatedObject.Unloaded += new RoutedEventHandler(this.OnUnLoaded);
    }

    protected override void OnDetaching()
    {
      this.AssociatedObject.Loaded -= new RoutedEventHandler(this.OnLoaded);
      this.AssociatedObject.Unloaded -= new RoutedEventHandler(this.OnUnLoaded);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      if (!(this.AssociatedObject.ItemsSource is INotifyCollectionChanged itemsSource))
        return;
      itemsSource.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
    }

    private void OnUnLoaded(object sender, RoutedEventArgs e)
    {
      if (!(this.AssociatedObject.ItemsSource is INotifyCollectionChanged itemsSource))
        return;
      itemsSource.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action != NotifyCollectionChangedAction.Add)
        return;
      int count = this.AssociatedObject.Items.Count;
      if (count == 0)
        return;
      object item = this.AssociatedObject.Items[count - 1];
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (!(this.AssociatedObject.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement frameworkElement))
          return;
        frameworkElement.BringIntoView();
      }))));
    }
  }
}
