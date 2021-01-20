// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.MacroManager
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace BlueStacks.Player
{
  public class MacroManager
  {
    private static object syncRoot = new object();
    private static MacroManager sInstance = (MacroManager) null;
    private EventWaitHandle mMultiMacroEventHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
    private bool mWasMacroPlaybackStopped;

    public static MacroManager Instance
    {
      get
      {
        if (MacroManager.sInstance == null)
        {
          lock (MacroManager.syncRoot)
          {
            if (MacroManager.sInstance == null)
              MacroManager.sInstance = new MacroManager();
          }
        }
        return MacroManager.sInstance;
      }
    }

    public MacroRecording MacroToPlay { get; private set; }

    internal void InitMacroPlayback(string macroPath)
    {
      this.mWasMacroPlaybackStopped = false;
      MacroGraph.ReCreateMacroGraphInstance();
      string macroName = Path.GetFileNameWithoutExtension(macroPath);
      this.MacroToPlay = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro_ => string.Equals(string.IsNullOrEmpty(macro_.FileName) ? macro_.Name : macro_.FileName, macroName, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>();
      if (this.MacroToPlay.RecordingType == RecordingTypes.MultiRecording)
      {
        foreach (MacroRecording allLeaf in MacroGraph.Instance.GetAllLeaves(this.MacroToPlay))
        {
          MacroRecording leafMacro = allLeaf;
          MacroRecording macroRecording = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (marco_ => marco_.Equals(leafMacro))).FirstOrDefault<MacroRecording>();
          if (macroRecording.RecordingType == RecordingTypes.SingleRecording)
            InputMapper.Instance.InitMacroPlayback(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, (string.IsNullOrEmpty(macroRecording.FileName) ? macroRecording.Name.ToLower() : macroRecording.FileName) + ".json"));
        }
      }
      else
        InputMapper.Instance.InitMacroPlayback(macroPath);
    }

    private void StartMultiMacroInitAndPlaybackLoop(MacroRecording macro, double acceleration)
    {
      Logger.Info("Starting multi-macro playback loop");
      foreach (MergedMacroConfiguration macroConfiguration in (Collection<MergedMacroConfiguration>) macro.MergedMacroConfigurations)
      {
        for (int index = 0; index < macroConfiguration.LoopCount; ++index)
        {
          foreach (string str in (Collection<string>) macroConfiguration.MacrosToRun)
          {
            string macroToRun = str;
            if (this.mWasMacroPlaybackStopped)
              return;
            MacroRecording macro1 = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro_ => string.Equals(macro_.Name, macroToRun, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>();
            if (macro1 != null)
            {
              if (macro1.Childs.Count > 0)
              {
                this.StartMultiMacroInitAndPlaybackLoop(macro1, macroConfiguration.Acceleration * acceleration);
              }
              else
              {
                this.mMultiMacroEventHandle?.Reset();
                InputMapper.Instance.RunMacroUnit(macroToRun, macroConfiguration.Acceleration * acceleration);
                this.mMultiMacroEventHandle?.WaitOne();
                Logger.Info("inside StartMultiMacroInitAndPlaybackLoop: mMultiMacroEventHandle wait completed");
              }
            }
          }
          Thread.Sleep(macroConfiguration.LoopInterval * 1000);
        }
        Thread.Sleep(macroConfiguration.DelayNextScript * 1000);
      }
    }

    internal void PlaybackCompleteHandlerImpl()
    {
      Logger.Info("Inside PlaybackCompleteHandlerImpl");
      this.mMultiMacroEventHandle?.Set();
    }

    internal void RunMacroUnit()
    {
      Logger.Info("inside RunMacroUnit");
      if (this.MacroToPlay.RecordingType == RecordingTypes.MultiRecording)
      {
        this.StartMultiMacroInitAndPlaybackLoop(this.MacroToPlay, this.MacroToPlay.Acceleration);
        if (this.mWasMacroPlaybackStopped)
        {
          this.mWasMacroPlaybackStopped = true;
          return;
        }
      }
      else
      {
        this.mMultiMacroEventHandle?.Reset();
        InputMapper.Instance.RunMacroUnit(this.MacroToPlay.Name, this.MacroToPlay.Acceleration);
        this.mMultiMacroEventHandle?.WaitOne();
        Logger.Info("inside RunMacroUnit: mMultiMacroEventHandle wait completed");
      }
      HTTPUtils.SendRequestToClient("macroPlaybackComplete", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    internal void StopMacroPlayback()
    {
      Logger.Info("Inside StopMacroPlayback");
      this.mWasMacroPlaybackStopped = true;
      this.mMultiMacroEventHandle?.Set();
      InputMapper.Instance.StopMacroPlayback();
    }
  }
}
