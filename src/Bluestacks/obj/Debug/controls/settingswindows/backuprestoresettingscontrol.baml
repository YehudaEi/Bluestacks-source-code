   M S B A M L   `   `   `  ����I  EHD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=nullH2clr-namespace:BlueStacks.Common;assembly=HD-CommonBlueStacks.Common  5       
Bluestacks:   4BlueStacks.BlueStacksUI.BackupRestoreSettingsControl   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      CCommon2clr-namespace:BlueStacks.Common;assembly=HD-Common    5      .��5      � $��30,25,20,25q�6	     � RowDefinitions  5      �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   .+�5      � -   5         �Name$ mDiskCleanupGrid��5      $��0��6%    � ColumnDefinitions 5      �� 5   
   $
��Auto �6   �� 5   
   $
��Auto �6   .+�5      (   "BlueStacks.Common.CustomPictureBox  $��40��6    $��40��6,   $	��Top=�67   $
��Left=�6O      	ImageName$ disk_cleanup_settings��5       � 5      $��1��6   $	��440��6   $��20,0,0,0q�6$     5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��WrapWithOverflow=�6   $
��Left=�64   $��16(�6O   $��Normal"�6]   ��5      ��  �� Path$' !LocaleModel.[STRING_DISK_CLEANUP]� �� FallbackValue$ Disk cleanup����5      �� $< 6ColorModel.[SettingsWindowTabMenuItemLegendForeground]��� 5   
   $��WrapWithOverflow=�6   $��1��64   $��0,15,0,0q�6A   $
��Left=�6S   $��16(�5      ��5      �� $3 -LocaleModel.[STRING_DISK_CLEANUP_DESCRIPTION]�$Y SFree up space unused by BlueStacks so that other windows app on your PC may use it.����5       �� $3 -ColorModel.[SettingsWindowForegroundDimColor]�$   BlueStacks.Common.CustomButton  5!   
   -   5          Name$ mDiskCleanupBtn��5!   ;      ButtonColor$
 Blue=�6T   $��2��6g   $��0,10,0,0q�5"      $	��150��60   $��Right=�6?   ��5#      �� $( "LocaleModel.[STRING_FREE_UP_SPACE]�$ Free up Space��5$   	   5%      �� 5&      -   5         ��Name$ mLineSeperator��5&      $��1��6#   $��	0,15,0,15q�60   		 �� X1$	 60��6C   	
 �� Y1$
 0��6K   	 �� X2$	 500��6R   	 �� Y2$ 0��6[   $��1��5'      ��6   �� $&  ColorModel.[HorizontalSeparator]�� 5(      -   5        $ mBackupRestoreGrid��5(      $��2��6'    5)      �� 5*   
   $
��Auto �6   �� 5+   
   $
��Auto �6   .+�5-        $��40��6    $��40��6,   $	��Top=�67   $
��Left=�6O   $ backup_restore_settings��5.       � 5/      $��1��6   $	��440��6   $��20,0,0,0q�6$     50   
   �� 51      $
��Auto �6   �� 52      $
��Auto �6   �� 53      $
��Auto �6   �� 54      $
��Auto �6   .+�56   
   �� $��WrapWithOverflow=�6   $
��Left=�64   $��16(�6O   $��Normal"�6]   ��57      �� $0 *LocaleModel.[STRING_BACKUPRESTORE_SETTING]�$ Backup and restore����58      �� $< 6ColorModel.[SettingsWindowTabMenuItemLegendForeground]��� 59   
   $��WrapWithOverflow=�6   $��1��64   $��0,15,0,0q�6A   $
��Left=�6S   $��16(�5:      ��5;      �� $5 /LocaleModel.[STRING_BACKUP_RESTORE_DESCRIPTION]�$� �Create a backup of installed apps, game controls, user data and settings for all your instances. You can also restore an existing backup.����5<      �� $3 -ColorModel.[SettingsWindowForegroundDimColor]��� 5=   
   $��WrapWithOverflow=�6   $��2��64   $��0,10,0,0q�6A   $
��Left=�6S   $��14(�5>      ��5?      �� $> 8LocaleModel.[STRING_BACKUP_AND_RESTORE_SETTINGS_WARNING]�$X RBackups can only be successfully restored from the version they were created from.����5@      �� $1 +ColorModel.[OrangeMouseOutBorderBackground]��� 5A   
   $��3��6   $��0,10,0,0q�6"   $��Right=�64    �� Orientation$ 
Horizontal=�6P   .+�5B        -   5        $ mRestoreBtn��5B   9   $ White=�6N   $	��150��6b   $��0,0,20,0q�6q   ��5C       �� $) #LocaleModel.[STRING_RESTORE_BACKUP]�$ Restore backup��  5D      -   5        $ 
mBackupBtn��5D   8   $
 Blue=�6L   $	��150��6_   ��5E       �� $( "LocaleModel.[STRING_CREATE_BACKUP]�$ Create backup��5F      5G   	   5H      5I      5J      