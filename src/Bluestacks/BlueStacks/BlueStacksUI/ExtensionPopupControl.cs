// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ExtensionPopupControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class ExtensionPopupControl : UserControl, IComponentConnector
  {
    private StackPanel mFeaturesStack;
    private StackPanel mDetailsStack;
    private TextBlock mDetailsText;
    private TextBlock mTagLine;
    private TextBlock mDescription;
    private TextBlock mFeaturesText;
    internal Label mTitle;
    internal Label mSubTitle;
    internal CustomButton mDownloadButton;
    internal CustomPictureBox mCloseBtn;
    internal SlideShowControl slideShow;
    private bool _contentLoaded;

    public ExtensionPopupControl()
    {
      this.InitializeComponent();
    }

    public event EventHandler DownloadClicked;

    internal void LoadExtensionPopupFromFolder(string folderPath)
    {
      if (!Path.IsPathRooted(folderPath))
        folderPath = Path.Combine(CustomPictureBox.AssetsDir, folderPath);
      if (!Directory.Exists(folderPath))
        return;
      try
      {
        string path = Path.Combine(folderPath, "extensionPopup.json");
        if (!File.Exists(path))
          return;
        ExtensionPopupControl.ExtensionPopupContext context = ExtensionPopupControl.ExtensionPopupContext.ReadJson(JObject.Parse(File.ReadAllText(path)));
        this.slideShow.ImagesFolderPath = folderPath;
        this.ApplyContext(context);
      }
      catch (Exception ex)
      {
        Logger.Error("Error while trying to read extensionpopup.json from " + folderPath + "." + ex.ToString());
      }
    }

    private void ApplyContext(
      ExtensionPopupControl.ExtensionPopupContext context)
    {
      BlueStacksUIBinding.Bind(this.mTitle, context.Title);
      BlueStacksUIBinding.Bind(this.mSubTitle, context.SubTitle);
      BlueStacksUIBinding.Bind(this.mTagLine, context.TagLine, "");
      BlueStacksUIBinding.Bind(this.mDescription, context.Description, "");
      if (context.features != null && context.features.Any<string>())
      {
        BlueStacksUIBinding.Bind(this.mFeaturesText, context.FeaturesText, "");
        foreach (string feature in context.features)
        {
          TextBlock tb = new TextBlock()
          {
            FontSize = 13.0
          };
          BlueStacksUIBinding.BindColor((DependencyObject) tb, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
          tb.Margin = new Thickness(7.0, 0.0, 0.0, 5.0);
          tb.TextWrapping = TextWrapping.Wrap;
          BlueStacksUIBinding.Bind(tb, feature, "");
          TextBlock textBlock = new TextBlock()
          {
            Text = "•"
          };
          BlueStacksUIBinding.BindColor((DependencyObject) textBlock, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
          textBlock.FontSize = 13.0;
          textBlock.FontWeight = FontWeights.Bold;
          textBlock.Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
          BulletDecorator bulletDecorator = new BulletDecorator();
          bulletDecorator.Bullet = (UIElement) textBlock;
          bulletDecorator.Child = (UIElement) tb;
          this.mFeaturesStack.Children.Add((UIElement) bulletDecorator);
        }
      }
      else
        this.mFeaturesText.Text = "";
      if (context.ExtensionDetails != null && context.ExtensionDetails.Any<KeyValuePair<string, string>>())
      {
        BlueStacksUIBinding.Bind(this.mDetailsText, context.ExtensionDetailText, "");
        foreach (KeyValuePair<string, string> extensionDetail in context.ExtensionDetails)
        {
          Grid grid = new Grid();
          grid.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Star)
          });
          grid.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.6, GridUnitType.Star)
          });
          TextBlock tb1 = new TextBlock()
          {
            FontSize = 13.0
          };
          BlueStacksUIBinding.BindColor((DependencyObject) tb1, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
          tb1.Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
          tb1.TextWrapping = TextWrapping.Wrap;
          BlueStacksUIBinding.Bind(tb1, extensionDetail.Key, "");
          grid.Children.Add((UIElement) tb1);
          Grid.SetColumn((UIElement) tb1, 0);
          TextBlock tb2 = new TextBlock()
          {
            FontSize = 13.0
          };
          BlueStacksUIBinding.BindColor((DependencyObject) tb2, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
          tb2.Margin = new Thickness(7.0, 0.0, 0.0, 5.0);
          tb2.TextWrapping = TextWrapping.Wrap;
          BlueStacksUIBinding.Bind(tb2, extensionDetail.Value, "");
          grid.Children.Add((UIElement) tb2);
          Grid.SetColumn((UIElement) tb2, 1);
          this.mDetailsStack.Children.Add((UIElement) grid);
        }
      }
      else
        this.mDetailsText.Text = "";
    }

    private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
    }

    private void mDownloadButton_Click(object sender, RoutedEventArgs e)
    {
      Logger.Info("Clicked DownloadNow Button");
      EventHandler downloadClicked = this.DownloadClicked;
      if (downloadClicked == null)
        return;
      downloadClicked(sender, (EventArgs) e);
    }

    private void DetailsStack_Initialized(object sender, EventArgs e)
    {
      this.mDetailsStack = sender as StackPanel;
    }

    private void DetailsText_Initialized(object sender, EventArgs e)
    {
      this.mDetailsText = sender as TextBlock;
    }

    private void TagLine_Initialized(object sender, EventArgs e)
    {
      this.mTagLine = sender as TextBlock;
    }

    private void Description_Initialized(object sender, EventArgs e)
    {
      this.mDescription = sender as TextBlock;
    }

    private void FeaturesText_Initialized(object sender, EventArgs e)
    {
      this.mFeaturesText = sender as TextBlock;
    }

    private void FeaturesStack_Initialized(object sender, EventArgs e)
    {
      this.mFeaturesStack = sender as StackPanel;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/extensionpopupcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mTitle = (Label) target;
          break;
        case 2:
          this.mSubTitle = (Label) target;
          break;
        case 3:
          this.mDownloadButton = (CustomButton) target;
          this.mDownloadButton.Click += new RoutedEventHandler(this.mDownloadButton_Click);
          break;
        case 4:
          this.mCloseBtn = (CustomPictureBox) target;
          this.mCloseBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseBtn_MouseLeftButtonUp);
          break;
        case 5:
          ((FrameworkElement) target).Initialized += new EventHandler(this.TagLine_Initialized);
          break;
        case 6:
          ((FrameworkElement) target).Initialized += new EventHandler(this.Description_Initialized);
          break;
        case 7:
          ((FrameworkElement) target).Initialized += new EventHandler(this.FeaturesStack_Initialized);
          break;
        case 8:
          ((FrameworkElement) target).Initialized += new EventHandler(this.FeaturesText_Initialized);
          break;
        case 9:
          ((FrameworkElement) target).Initialized += new EventHandler(this.DetailsStack_Initialized);
          break;
        case 10:
          ((FrameworkElement) target).Initialized += new EventHandler(this.DetailsText_Initialized);
          break;
        case 11:
          this.slideShow = (SlideShowControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class ExtensionPopupContext
    {
      [JsonProperty("Title", NullValueHandling = NullValueHandling.Ignore)]
      internal string Title;
      [JsonProperty("SubTitle", NullValueHandling = NullValueHandling.Ignore)]
      internal string SubTitle;
      [JsonProperty("TagLine", NullValueHandling = NullValueHandling.Ignore)]
      internal string TagLine;
      [JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
      internal string Description;
      [JsonProperty("FeaturesText", NullValueHandling = NullValueHandling.Ignore)]
      internal string FeaturesText;
      [JsonProperty("dummyFeatures", NullValueHandling = NullValueHandling.Ignore)]
      internal IEnumerable<string> features;
      [JsonProperty("ExtensionDetailText", NullValueHandling = NullValueHandling.Ignore)]
      internal string ExtensionDetailText;
      [JsonProperty("dummyExtensionDetails", NullValueHandling = NullValueHandling.Ignore)]
      internal IEnumerable<KeyValuePair<string, string>> ExtensionDetails;

      public static ExtensionPopupControl.ExtensionPopupContext ReadJson(
        JObject input)
      {
        ExtensionPopupControl.ExtensionPopupContext extensionPopupContext = input.ToObject<ExtensionPopupControl.ExtensionPopupContext>();
        extensionPopupContext.features = input["features"].ToIenumerableString();
        extensionPopupContext.ExtensionDetails = input["ExtensionDetails"].ToStringStringEnumerableKvp();
        return extensionPopupContext;
      }

      public void WriteJson(JObject writer)
      {
        writer.Add("Title", (JToken) this.Title);
        writer.Add("SubTitle", (JToken) this.SubTitle);
        writer.Add("TagLine", (JToken) this.TagLine);
        writer.Add("Description", (JToken) this.Description);
        writer.Add("FeaturesText", (JToken) this.FeaturesText);
        JObject jobject = writer;
        JArray jarray = new JArray();
        jarray.Add((object) this.features.ToList<string>());
        jobject.Add("features", (JToken) jarray);
        writer.Add("ExtensionDetailText", (JToken) this.ExtensionDetailText);
        writer.Add((object) "ExtensionDetails");
        foreach (KeyValuePair<string, string> extensionDetail in this.ExtensionDetails)
          writer.Add(extensionDetail.Key, (JToken) extensionDetail.Value);
      }
    }
  }
}
