THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved.

MTScratchpadWMTouch application.
 
Description:
  
When users place fingers on the digitizer, within the sample's application window, they draw on the digitizer by using multiple fingers at the same time. The application traces each finger using a different color for each finger. The primary finger trace is always drawn in black, and the remaining traces are drawn using rotating colors: red, blue, green, magenta, cyan, and yellow.

Purpose:
  This sample demonstrates how to handle multi-touch input inside a Win32 application by using the WM_TOUCH* group of window messages:
  - Using RegisterTouchWindow and IsTouchWindow to register a window for multi-touch.
  - Handling WM_TOUCHDOWN, WM_TOUCHUP, and WM_TOUCHMOVE messages and unpacking their parameters by using etTouchInputInfo and CloseTouchInputHandle; reading contact info from the TOUCHINPUT structure.
  - Unregistering a window for multi-touch by using UnregisterTouchWindow.

In addition, the sample also shows how to store and draw strokes entered by the user, through the helper classes CStroke and CStrokeCollection.

Requirements:

A multi-touch digitizer 
Microsoft Windows 7
Windows Software Development Kit (SDK) for Windows 7 and .NET Framework 3.5 Service Pack 1: Pre-Beta distributed to PDC attendees