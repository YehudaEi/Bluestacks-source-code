﻿#pragma checksum "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D7D0C117C7CAF9CFB9C0243E61936215C0D95BA83EB25E657DB190A46AFD88B0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using BlueStacks.Common;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace BlueStacks.BlueStacksUI {
    
    
    /// <summary>
    /// BackupRestoreSettingsControl
    /// </summary>
    public partial class BackupRestoreSettingsControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid mDiskCleanupGrid;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal BlueStacks.Common.CustomButton mDiskCleanupBtn;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Line mLineSeperator;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid mBackupRestoreGrid;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal BlueStacks.Common.CustomButton mRestoreBtn;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal BlueStacks.Common.CustomButton mBackupBtn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Bluestacks;component/controls/settingswindows/backuprestoresettingscontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.mDiskCleanupGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.mDiskCleanupBtn = ((BlueStacks.Common.CustomButton)(target));
            
            #line 33 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
            this.mDiskCleanupBtn.Click += new System.Windows.RoutedEventHandler(this.DiskCleanupBtn_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.mLineSeperator = ((System.Windows.Shapes.Line)(target));
            return;
            case 4:
            this.mBackupRestoreGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.mRestoreBtn = ((BlueStacks.Common.CustomButton)(target));
            
            #line 66 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
            this.mRestoreBtn.Click += new System.Windows.RoutedEventHandler(this.RestoreBtn_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.mBackupBtn = ((BlueStacks.Common.CustomButton)(target));
            
            #line 68 "..\..\..\..\controls\settingswindows\backuprestoresettingscontrol.xaml"
            this.mBackupBtn.Click += new System.Windows.RoutedEventHandler(this.BackupBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

