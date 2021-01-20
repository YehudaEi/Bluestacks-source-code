// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DateTimeHelper
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
  internal class DateTimeHelper
  {
    internal static string GetReadableDateTimeString(DateTime yourDate)
    {
      DateTime dateTime1 = yourDate.ToLocalTime();
      DateTime date1 = dateTime1.Date;
      dateTime1 = DateTime.Now;
      DateTime date2 = dateTime1.Date;
      if (date1 == date2)
        return "Today at " + yourDate.ToLocalTime().ToString("HH:mm", (IFormatProvider) CultureInfo.InvariantCulture);
      DateTime dateTime2 = yourDate.ToLocalTime();
      dateTime2 = dateTime2.Date;
      int year1 = dateTime2.Year;
      dateTime2 = DateTime.Now;
      dateTime2 = dateTime2.Date;
      int year2 = dateTime2.Year;
      if (year1 == year2)
      {
        dateTime2 = yourDate.ToLocalTime();
        return dateTime2.ToString("%d MMM',' HH:mm", (IFormatProvider) CultureInfo.InvariantCulture);
      }
      dateTime2 = yourDate.ToLocalTime();
      return dateTime2.ToString("%d MMM yyyy',' HH:mm", (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
