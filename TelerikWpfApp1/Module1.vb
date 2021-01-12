Imports System.Runtime.InteropServices
Public Enum Edit_Mode_Enum As Integer
    Cursor = 1
    Pen = 2
    Marker = 4
    Eraser = 8
    Selectt = 16
End Enum
Public Enum App_Mode_Enum As Integer
    PPT = 1
    Board = 2
    Camera = 4
End Enum
Module Module1
    Public Structure RECT
        Dim Left As Integer
        Dim Top As Integer
        Dim Right As Integer
        Dim Bottom As Integer
    End Structure

    Public Class FlushMemory
        Public Declare Function SetProcessWorkingSetSize Lib "kernel32" Alias "SetProcessWorkingSetSize" (ByVal hProcess As Int32, ByVal dwMinimumWorkingSetSize As Int32, ByVal dwMaximumWorkingSetSize As Int32) As Int32
        Public Shared Sub Flush()
            GC.Collect()
            GC.WaitForPendingFinalizers()

            If Environment.OSVersion.Platform = PlatformID.Win32NT Then
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1)
            End If
        End Sub
    End Class

End Module
