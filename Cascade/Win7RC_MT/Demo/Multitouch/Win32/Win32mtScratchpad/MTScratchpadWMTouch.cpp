//-----------------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

// MTScratchpadWMTouch application.
// Description:
//  
// When users place fingers on the digitizer, within the sample's application window,  
// they draw on the digitizer by using multiple fingers at the same time. The application  
// traces each finger using a different color for each finger. The primary finger trace is always 
// drawn in black, and the remaining traces are drawn using rotating colors: 
// red, blue, green, magenta, cyan, and yellow.
//
// Purpose:
//  This sample demonstrates how to handle multi-touch input inside
//  a Win32 application by using the WM_TOUCH* group of window messages:
//  - Using RegisterTouchWindow and IsTouchWindow to register 
//    a window for multi-touch.
//  - Handling WM_TOUCHDOWN, WM_TOUCHUP, and WM_TOUCHMOVE messages and
//    unpacking their parameters by using GetTouchInputInfo and
//    CloseTouchInputHandle; reading contact info from the TOUCHINPUT
//    structure.
//  - Unregistering a window for multi-touch by using UnregisterTouchWindow.
//
//  In addition, the sample also shows how to store and draw strokes
//  entered by the user, through the helper classes CStroke and
//  CStrokeCollection.
//
// MTScratchpadWMTouch.cpp : Defines the entry point for the application.

// Windows header files
#include <windows.h>
#include <windowsx.h>

// C RunTime header files
#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <tchar.h>
#include <assert.h>
#define ASSERT assert

// Application header files
#include "resource.h"
#include "Stroke.h"
#include <msclr\gcroot.h>

#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE g_hInst;                              // Current module instance
WCHAR g_wszTitle[MAX_LOADSTRING];               // The title bar text
WCHAR g_wszWindowClass[MAX_LOADSTRING];         // The main window class name
CStrokeCollection g_StrkColFinished;            // The user finished entering strokes. The user lifted his or her finger.
CStrokeCollection g_StrkColDrawing;             // The Strokes collection the user is currently drawing.
msclr::gcroot<Windows7::Multitouch::TouchHandler ^> g_touchHandler;
HWND g_hWnd;

///////////////////////////////////////////////////////////////////////////////
// Drawing and WM_TOUCH* helpers

// Returns color for the newly started stroke.
// in:
//      bPrimaryContact     flag, whether the contact is the primary contact
// returns:
//      COLORREF, color of the stroke
COLORREF GetTouchColor(bool bPrimaryContact)
{
    static int g_iCurrColor = 0;    // Rotating secondary color index
    static COLORREF g_arrColor[] =  // Secondary colors array
    {
        RGB(255, 0, 0),             // Red
        RGB(0, 255, 0),             // Green
        RGB(0, 0, 255),             // Blue
        RGB(0, 255, 255),           // Cyan
        RGB(255, 0, 255),           // Magenta
        RGB(255, 255, 0)            // Yellow
    };

    COLORREF color;
    if(bPrimaryContact)
    {
        // The application renders the primary contact in black.
        color = RGB(0,0,0);         // Black
    }
    else
    {
        // Take the current secondary color.
        color = g_arrColor[g_iCurrColor];

        // Move to the next color in the array.
        g_iCurrColor = (g_iCurrColor + 1) % (sizeof(g_arrColor)/sizeof(g_arrColor[0]));
    }

    return color;
}

POINT ConvertToPOINT(System::Drawing::Point point)
{
	POINT p;
	p.x = point.X;
	p.y = point.Y;
	return p;
}

///////////////////////////////////////////////////////////////////////////////
// WM_TOUCH* message handlers

// Handler for touch-down message.
// in:
//      hWnd        window handle
//      ti          TOUCHINPUT structure (info about contact)
void OnTouchDownHandler(System::Object ^sender, Windows7::Multitouch::TouchEventArgs ^args)
{
    // Check for the stroke with the same ID in the collection of the strokes in drawing.
	int iStrk = g_StrkColDrawing.FindStrokeById(args->Id);

    // If a stroke exists with the same ID, finish it.
    if(iStrk >= 0)
    {
        // Add the finished stroke to the collection of finished strokes.
        g_StrkColFinished.AddStroke(g_StrkColDrawing[iStrk]);

        // Remove finished stroke from the collection of strokes currently being drawn.
        g_StrkColDrawing.RemoveStroke(iStrk);

        // Redraw the window.
        InvalidateRect(g_hWnd, NULL, FALSE);
        return;
    }

    // Create a new stroke, add a point, and assign a color to it.
    CStroke strkNew;
	strkNew.AddPoint(ConvertToPOINT(args->Location));
	strkNew.SetColor(GetTouchColor(args->IsPrimaryContact));
	strkNew.SetId(args->Id);

    // Add the new stroke to the collection of strokes being drawn.
    g_StrkColDrawing.AddStroke(strkNew);
}

// Handler for touch-mode message.
// in:
//      hWnd        window handle
//      ti          TOUCHINPUT structure (info about contact)
void OnTouchMoveHandler(System::Object ^sender, Windows7::Multitouch::TouchEventArgs ^args)
{

    // Find the stroke in the collection of the strokes being drawn.
	int iStrk = g_StrkColDrawing.FindStrokeById(args->Id);

    if(iStrk >= 0)
    {
        // Add the contact point to the stroke.
		g_StrkColDrawing[iStrk].AddPoint(ConvertToPOINT(args->Location));

        // Partial redraw: redraw only the last line segment.
        HDC hDC = GetDC(g_hWnd);
        g_StrkColDrawing[iStrk].DrawLast(hDC);
        ReleaseDC(g_hWnd, hDC);
    }
}

// Handler for touch-up message.
// in:
//      hWnd        window handle
//      ti          TOUCHINPUT structure (info about contact)
void OnTouchUpHandler(System::Object ^sender, Windows7::Multitouch::TouchEventArgs ^args)
{
    // Find the stroke in the collection of the strokes being drawn.
	int iStrk = g_StrkColDrawing.FindStrokeById(args->Id);

    if(iStrk >= 0)
    {
        // Add the finished stroke to the collection of finished strokes.
        g_StrkColFinished.AddStroke(g_StrkColDrawing[iStrk]);

        // Remove finished stroke from the collection of strokes being drawn.
        g_StrkColDrawing.RemoveStroke(iStrk);

        // Redraw the window.
        InvalidateRect(g_hWnd, NULL, FALSE);
    }
}

///////////////////////////////////////////////////////////////////////////////
// Application framework

// Forward declarations of functions included in this code module:
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

// Win32 application main entry point function.
// in:
//      hInstance       handle of the application instance
//      hPrevInstance   not used, always NULL
//      lpCmdLine       command line for the application, a null-terminated string
//      nCmdShow        how to show the window
int APIENTRY wWinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPWSTR    lpCmdLine,
                     int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    MSG msg;
    HACCEL hAccelTable;


	//Check for MultiTouch
	
	//BYTE digitizerStatus = (BYTE)GetSystemMetrics(SM_DIGITIZER);
	//if ((digitizerStatus & (0x80 + 0x40)) == 0) //Stack Ready + MultiTouch
	if (!Windows7::Multitouch::Handler::DigitizerCapabilities::IsMultiTouchReady)
	{
		MessageBox(0, L"No touch support is currently availible", L"Error", MB_OK);
		return 1;
	}


    // Initialize global strings.
    LoadString(hInstance, IDS_APP_TITLE, g_wszTitle, MAX_LOADSTRING);
    LoadString(hInstance, IDC_MTSCRATCHPADWMTOUCH, g_wszWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

	// Perform application initialization.
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_MTSCRATCHPADWMTOUCH));

    // Main message loop:
    while (GetMessage(&msg, NULL, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return (int) msg.wParam;
}

// Registers the window class of the application.
// This function and its usage are only necessary if you want this code
// to be compatible with Win32 systems prior to the 'RegisterClassEx'
// function that was added to Windows 95. It is important to call this function
// so that the application will get 'well formed' small icons associated
// with it.
// in:
//      hInstance       handle to the instance of the application
// returns:
//      class atom that uniquely identifies the window class
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEX wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style			= CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc	= WndProc;
    wcex.cbClsExtra		= 0;
    wcex.cbWndExtra		= 0;
    wcex.hInstance		= hInstance;
    wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_MTSCRATCHPADWMTOUCH));
    wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
    wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName	= MAKEINTRESOURCE(IDC_MTSCRATCHPADWMTOUCH);
    wcex.lpszClassName	= g_wszWindowClass;
    wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassEx(&wcex);
}

// Saves instance handle and creates main window
// In this function, we save the instance handle in a global variable and
// create and display the main program window.
// in:
//      hInstance       handle to the instance of the application
//      nCmdShow        how to show the window
// returns:
//      flag, succeeded or failed to create the window
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
    HWND hWnd;

    g_hInst = hInstance; // Store the instance handle in our global variable.

    // Create the application window.
    hWnd = CreateWindow(g_wszWindowClass, g_wszTitle, WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);
    if (!hWnd)
    {
        return FALSE;
    }


    //// Register the application window for receiving multi-touch input.
    //if(!RegisterTouchWindow(hWnd, 0))
    //{
    //    MessageBox(hWnd, L"Cannot register application window for touch input", L"Error", MB_OK);
    //    return FALSE;
    //}
    //ASSERT(IsTouchWindow(hWnd, NULL));

	g_hWnd = hWnd;

	g_touchHandler = Windows7::Multitouch::Win32Helper::Factory::CreateHandler<Windows7::Multitouch::TouchHandler ^>(System::IntPtr(hWnd));
	g_touchHandler->TouchDown += gcnew System::EventHandler<Windows7::Multitouch::TouchEventArgs ^>(OnTouchDownHandler);
	g_touchHandler->TouchMove += gcnew System::EventHandler<Windows7::Multitouch::TouchEventArgs ^>(OnTouchMoveHandler);
	g_touchHandler->TouchUp += gcnew System::EventHandler<Windows7::Multitouch::TouchEventArgs ^>(OnTouchUpHandler);

    ShowWindow(hWnd, nCmdShow);
    UpdateWindow(hWnd);

    return TRUE;
}

// Processes messages for the main window:
//      WM_COMMAND  - process the application menu
//      WM_PAINT    - paint the main window
//      WM_TOUCH*   - multi-touch messages
//      WM_DESTROY  - post a quit message and return
// in:
//      hWnd        window handle
//      message     message code
//      wParam      message parameter (message-specific)
//      lParam      message parameter (message-specific)
// returns:
//      the result of the message processing and depends on the message sent
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    int wmId, wmEvent;
    PAINTSTRUCT ps;
    HDC hdc;

    switch (message)
    {
        case WM_COMMAND:
            wmId    = LOWORD(wParam);
            wmEvent = HIWORD(wParam);
            // Parse the menu selections:
            switch (wmId)
            {
                case IDM_ABOUT:
                    DialogBox(g_hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                    break;
                case IDM_EXIT:
                    DestroyWindow(hWnd);
                    break;
                default:
                    return DefWindowProc(hWnd, message, wParam, lParam);
            }
            break;

        case WM_PAINT:
            hdc = BeginPaint(hWnd, &ps);
            // Full redraw: draw complete collection of finished strokes and
            // also all the strokes that are currently in drawing.
            g_StrkColFinished.Draw(hdc);
            g_StrkColDrawing.Draw(hdc);
            EndPaint(hWnd, &ps);
            break;

        case WM_DESTROY:
            PostQuitMessage(0);
            break;

        default:
            return DefWindowProc(hWnd, message, wParam, lParam);
    }
	return 0;
}

// Message handler for about box.
// in:
//      hDlg        window handle
//      message     message code
//      wParam      message parameter (message-specific)
//      lParam      message parameter (message-specific)
// returns:
//      the result of the message processing and depends on the message sent
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
        case WM_INITDIALOG:
            return (INT_PTR)TRUE;

        case WM_COMMAND:
            if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
            {
                EndDialog(hDlg, LOWORD(wParam));
                return (INT_PTR)TRUE;
            }
            break;
    }
    return (INT_PTR)FALSE;
}