Imports System.IO
Imports System.IO.Compression
Imports Ionic.Zip

Public Class updatehelper
    Public Shared f As FtpWeb
    Public Shared updateok As Boolean = False
    Public Shared Sub updatemain()
        Dim t1 = New DirectoryInfo(System.Environment.CurrentDirectory)
        Dim downini = t1.Parent.FullName & "\tmp.ini"
        Dim localini = t1.Parent.FullName & "\version.ini"
        Dim rootpath = t1.Parent.FullName
        Dim curver = GetKeyValue("main", "ver", "", localini)
        SetKeyValue("main", "lastver", curver, localini)
        Try
            f = New FtpWeb("ftp://10.233.88.2", "user", "2003")
            f.Download("ftp://10.233.88.2/HandyDraw/ver.ini", downini)
            Dim newestver = GetKeyValue("main", "ver", "", downini)
            If newestver = "" Or curver = "" Then
                Exit Sub
            End If
            If curver = newestver Then
                Return
            End If
            f.Download("ftp://10.233.88.2/HandyDraw/" & newestver & ".zip", rootpath & "\" & newestver & ".zip")
            Console.WriteLine(rootpath & "\" & newestver & ".zip")
            UnPack(rootpath & "\" & newestver & ".zip", rootpath & "\" & newestver & "\")
            SetKeyValue("main", "ver", newestver, localini)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        updateok = True
    End Sub
    Public Shared Sub UnPack(ByVal PackPath As String, ByVal FolerPath As String)
        Dim zip As New ZipFile
        zip = ZipFile.Read(PackPath)
        'zip.Password = Psd  '注意密码一定要放在读取后
        zip.ExtractAll(FolerPath, ExtractExistingFileAction.OverwriteSilently)
        zip.Dispose()
    End Sub
End Class
