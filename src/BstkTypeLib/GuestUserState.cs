// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.GuestUserState
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("B2A82B02-FD3D-4FC2-BA84-6BA5AC8BE198")]
  public enum GuestUserState
  {
    GuestUserState_Unknown,
    GuestUserState_LoggedIn,
    GuestUserState_LoggedOut,
    GuestUserState_Locked,
    GuestUserState_Unlocked,
    GuestUserState_Disabled,
    GuestUserState_Idle,
    GuestUserState_InUse,
    GuestUserState_Created,
    GuestUserState_Deleted,
    GuestUserState_SessionChanged,
    GuestUserState_CredentialsChanged,
    GuestUserState_RoleChanged,
    GuestUserState_GroupAdded,
    GuestUserState_GroupRemoved,
    GuestUserState_Elevated,
  }
}
