Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO


Public Class Form1
    Dim sggg_data As String = IO.Path.Combine(IO.Directory.GetCurrentDirectory, "sggg_data.txt")
    Dim sggg_bin As String = IO.Path.Combine(IO.Directory.GetCurrentDirectory, "1_SGGG.BIN")
    Dim sggg_patch_bin As String = IO.Path.Combine(IO.Directory.GetCurrentDirectory, "1_SGGG.PATCHED.BIN")


    Private myCache() As ListViewItem 'array to cache items for the virtual list
    Private firstItem As Integer 'stores the index of the first item in the cache


    Dim TextBox3Boolen As Boolean = False

    Public data As New List(Of String())



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        If Not IO.File.Exists(sggg_data) Then
            MsgBox("File sggg_data.txt is missing!")
            Me.Close()
            Exit Sub
        End If

        If Not IO.File.Exists(sggg_bin) Then
            MsgBox("File 1_SGGG.BIN is missing!")
            Me.Close()
            Exit Sub
        End If


        ComboBox1.SelectedIndex = 0



        ' Use TextFieldParser for proper CSV parsing
        Using parser As New TextFieldParser(sggg_data)
            parser.TextFieldType = FieldType.Delimited
            parser.SetDelimiters(",")
            parser.TrimWhiteSpace = False

            ' Enable quoted field handling
            parser.HasFieldsEnclosedInQuotes = True

            ' Read and skip the header row
            If Not parser.EndOfData Then
                parser.ReadFields()
            End If

            ' Read all rows
            While Not parser.EndOfData
                Dim fields As String() = parser.ReadFields()
                data.Add(fields)
            End While
        End Using



        ListView1.VirtualListSize = data.Count
        ListView1.BeginUpdate()
        'Now we need to rebuild the cache.
        firstItem = 0
        myCache = New ListViewItem(data.Count) {}
        Dim i As Integer = 0
        For Each row As String() In data
            myCache(i) = New ListViewItem({row(0), row(1), row(2), row(3), row(4)})
            i += 1
        Next
        ListView1.EndUpdate()

        Try
            ListView1.EnsureVisible(0)
        Catch
        End Try


        Label6.Text = "Total Lines: " + ListView1.Items.Count.ToString


    End Sub

    Function ReadFromBIN(ByVal File As String, ByVal Pos As String, ByVal Byte_Count As Integer)
        Pos = Convert.ToInt64(Pos, 16)

        Dim wFile As System.IO.FileStream = New FileStream(File, FileMode.Open, FileAccess.Read)
        Dim reader As New BinaryReader(wFile)
        wFile.Seek(Pos, SeekOrigin.Begin)
        Dim data(Byte_Count) As Byte
        data = reader.ReadBytes(data.Length)
        reader.Close()
        wFile.Close()
        Dim resault As String = System.Text.Encoding.GetEncoding("shift-jis").GetString(data)
        Return resault
    End Function


    Function WriteToBIN(ByVal File As String, ByVal Pos As String, ByVal text As String, ByVal Byte_Count As Integer)
        Pos = Convert.ToInt64(Pos, 16)
        Dim wFile As System.IO.FileStream = New FileStream(File, FileMode.Open, FileAccess.Write)
        Dim writer As New BinaryWriter(wFile)
        wFile.Seek(Pos, SeekOrigin.Begin)
        Dim data(Byte_Count) As Byte
        data = System.Text.Encoding.ASCII.GetBytes(text)
        If data.Length >= Byte_Count Then
            While data.Length > Byte_Count
                text = text.Remove(text.Length - 1)
                data = System.Text.Encoding.ASCII.GetBytes(text)
            End While
        ElseIf data.Length < Byte_Count Then
            While data.Length < Byte_Count
                text = text + vbNullChar
                data = System.Text.Encoding.ASCII.GetBytes(text)
            End While
        End If
        writer.Write(data)

        writer.Close()
        wFile.Close()
        Return text
    End Function

    Function WritePointerToBIN(ByVal File As String, ByVal Pos As String, ByVal text As String)
        Pos = Convert.ToInt64(Pos, 16)
        Dim wFile As System.IO.FileStream = New FileStream(File, FileMode.Open, FileAccess.Write)
        If text.Length Mod 2 <> 0 Then
            text = "0" & text ' Add leading zero if odd length
        End If
        Dim byteArray(text.Length / 2 - 1) As Byte
        For i As Integer = 0 To text.Length - 1 Step 2
            byteArray(i / 2) = Convert.ToByte(text.Substring(i, 2), 16)
        Next
        Dim writer As New BinaryWriter(wFile)
        wFile.Seek(Pos, SeekOrigin.Begin)
        writer.Write(byteArray, 0, byteArray.Length)
        writer.Close()
        wFile.Close()
        Return text
    End Function



    Function WriteDebugMenu(ByVal File As String)
        If ComboBox1.SelectedIndex >= 0 Then

            Dim Pos As String = Nothing

            Dim wFile As System.IO.FileStream = New FileStream(File, FileMode.Open, FileAccess.Write)
            Dim writer As New BinaryWriter(wFile)

            If ComboBox1.SelectedIndex = 1 Then

                Pos = Convert.ToInt64("F986", 16)
                wFile.Seek(Pos, SeekOrigin.Begin)
                Dim data() As Byte = {&H32, &H60}
                writer.Write(data)

                'Debug #1
                Pos = Convert.ToInt64("F98C", 16)
                wFile.Seek(Pos, SeekOrigin.Begin)
                Dim data2() As Byte = {&H18, &H0, &H14, &H8B}
                writer.Write(data2)

                Pos = Convert.ToInt64("129918", 16)
                wFile.Seek(Pos, SeekOrigin.Begin)
                Dim data3() As Byte = {&H1C, &HD8, &HA}
                writer.Write(data3)


            ElseIf ComboBox1.SelectedIndex = 2 Then
                'Debug #2
                Pos = Convert.ToInt64("F986", 16)
                wFile.Seek(Pos, SeekOrigin.Begin)
                Dim data() As Byte = {&H1, &HE0}
                writer.Write(data)

                Pos = Convert.ToInt64("F98C", 16)
                wFile.Seek(Pos, SeekOrigin.Begin)
                Dim data2() As Byte = {&H2, &H23, &H9, &H0}
                writer.Write(data2)

                Pos = Convert.ToInt64("129918", 16)
                wFile.Seek(Pos, SeekOrigin.Begin)
                Dim data3() As Byte = {&H68, &H25, &H2}
                writer.Write(data3)

            ElseIf ComboBox1.SelectedIndex = 0 Then

                'Original
                Pos = Convert.ToInt64("F986", 16)
                wFile.Seek(Pos, SeekOrigin.Begin)
                Dim data() As Byte = {&H32, &H60}
                writer.Write(data)

                Pos = Convert.ToInt64("F98C", 16)
                wFile.Seek(Pos, SeekOrigin.Begin)
                Dim data2() As Byte = {&H1, &H88, &H14, &H8B}
                writer.Write(data2)

                Pos = Convert.ToInt64("129918", 16)
                wFile.Seek(Pos, SeekOrigin.Begin)
                Dim data3() As Byte = {&H1C, &HD8, &HA}
                writer.Write(data3)
            End If


            writer.Close()
            wFile.Close()

        End If
        Return Nothing
    End Function



    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged

        Try
            Dim SelectedItems As ListView.SelectedIndexCollection = ListView1.SelectedIndices
            If SelectedItems.Count > 0 Then
                TextBox3Boolen = True
                TextBox1.Text = ListView1.Items(SelectedItems(0)).SubItems(0).Text
                TextBox2.Text = ListView1.Items(SelectedItems(0)).SubItems(2).Text
                TextBox3.Text = ListView1.Items(SelectedItems(0)).SubItems(1).Text
                If CheckBox1.Checked Then
                    TextBox4.Text = ReadFromBIN(sggg_bin, ListView1.Items(SelectedItems(0)).SubItems(0).Text, ListView1.Items(SelectedItems(0)).SubItems(2).Text).replace(ControlChars.NullChar, " ")
                Else
                    TextBox4.Text = ReadFromBIN(sggg_bin, ListView1.Items(SelectedItems(0)).SubItems(0).Text, ListView1.Items(SelectedItems(0)).SubItems(2).Text).replace(ControlChars.NullChar, "\0")
                End If
                TextBox5.Text = ListView1.Items(SelectedItems(0)).SubItems(4).Text
                Label7.Text = "Selected Line: " + (ListView1.Items(SelectedItems(0)).Index + 1).ToString
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try


    End Sub

    Function POStoDC(ByVal Location As String)
        Dim intA As Int64 = Convert.ToInt64("8C010000", 16)
        Dim intB As Int64 = Convert.ToInt64(Location, 16)
        Dim intC As Int64 = intA + intB
        Dim final As String = intC.ToString("X8")
        Return final
    End Function

    Function DCtoPOS(ByVal Location As String)
        Dim intA As Int64 = Convert.ToInt64("8C010000", 16)
        Dim intB As Int64 = Convert.ToInt64(Location, 16)
        Dim intC As Int64 = intB - intA
        Dim final As String = intC.ToString("X8")
        Return final
    End Function


    Function HexADD(ByVal LocationA As String, ByVal LocationB As String)
        Dim intA As Int64 = Convert.ToInt64(LocationA, 16)
        Dim intB As Int64 = Convert.ToInt64(LocationB, 16)
        Dim intC As Int64 = intA + intB
        Dim final As String = intC.ToString("X8")
        Return final
    End Function

    Public Function flip(ByVal value As String)
        Dim resault As String = Nothing
        For i = 8 To 1 Step -2
            resault += Mid(value, i - 1, 2)
        Next

        Return resault
    End Function

    Function SearchHexInFile(filePath As String, hexPattern As String) As List(Of String)
        Dim result As New List(Of String)()

        ' Convert hex string to byte array
        Dim patternBytes As Byte() = New Byte(hexPattern.Length \ 2 - 1) {}
        For i As Integer = 0 To patternBytes.Length - 1
            patternBytes(i) = Convert.ToByte(hexPattern.Substring(i * 2, 2), 16)
        Next

        ' File search optimization
        Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
            Dim bufferSize As Integer = 4096 ' Adjust buffer size for performance
            Dim buffer(bufferSize + patternBytes.Length - 1) As Byte ' Extra space for overlap
            Dim bytesRead As Integer
            Dim position As Long = 0

            Do
                ' Shift overlap bytes to the beginning
                If position > 0 Then
                    Array.Copy(buffer, bufferSize, buffer, 0, patternBytes.Length - 1)
                End If

                ' Read next chunk of data
                bytesRead = fs.Read(buffer, patternBytes.Length - 1, bufferSize)
                If bytesRead = 0 Then Exit Do

                ' Search for pattern in buffer
                Dim searchLimit As Integer = bytesRead + patternBytes.Length - 1
                For i As Integer = 0 To searchLimit - patternBytes.Length
                    Dim match As Boolean = True
                    For j As Integer = 0 To patternBytes.Length - 1
                        If buffer(i + j) <> patternBytes(j) Then
                            match = False
                            Exit For
                        End If
                    Next

                    ' If match found, store position in hex format
                    If match Then
                        result.Add((position + i - patternBytes.Length + 1).ToString("X"))
                    End If
                Next

                ' Move file position ahead
                position += bytesRead

            Loop While bytesRead > 0
        End Using

        Return result
    End Function

    Private Function CsvEscape(value As String) As String
        If value Is Nothing Then Return ""

        ' Escape double quotes
        value = value.Replace("""", """""")

        ' Always wrap with quotes for safety
        Return $"""{value}"""
    End Function

    Sub SaveData()
        Dim sb As New StringBuilder()

        ' Header
        sb.AppendLine("""Location"",""Pointers"",""Byte Count"",""String Value"",""New String Value""")

        For i As Integer = 0 To ListView1.VirtualListSize - 1
            Dim item As ListViewItem = ListView1.Items(i)

            sb.Append(CsvEscape(item.Text)).Append(",")
            sb.Append(CsvEscape(item.SubItems(1).Text)).Append(",")
            sb.Append(CsvEscape(item.SubItems(2).Text)).Append(",")
            sb.Append(CsvEscape(item.SubItems(3).Text)).Append(",")
            sb.Append(CsvEscape(item.SubItems(4).Text.Replace(vbLf, "")))
            sb.AppendLine()
        Next

        My.Computer.FileSystem.WriteAllText(sggg_data, sb.ToString(), False)
    End Sub


    Sub PatchBin()
        IO.File.Copy(sggg_bin, sggg_patch_bin, True)

        WriteDebugMenu(sggg_patch_bin)

        'sggg_patch_bin
        Dim NewLocation As String = "279F07" 'Start of Safe Area

        For i As Integer = 0 To ListView1.VirtualListSize - 1
            Dim item As ListViewItem = ListView1.Items(i)

            Dim DefaultLocation As String = item.Text
            Dim Bytes As Integer = item.SubItems(2).Text
            Dim NewText As String = item.SubItems(4).Text.Replace("\0", ControlChars.NullChar)
            Dim Pointers() As String

            If NewText.Length > 0 Then

                If NewText.Length > Bytes Then
                    If item.SubItems(1).Text <> Nothing Then 'check pointer presence
                        If InStr(item.SubItems(1).Text, "|") Then
                            Pointers = item.SubItems(1).Text.Split("|")
                        Else
                            Pointers = {item.SubItems(1).Text}
                        End If

                        Dim NewBytes As Integer = NewText.Length

                        '2F6537  END OF SAFE AREA
                        'Stop Patching if space is not enough
                        If (Convert.ToInt64(NewLocation, 16) + NewBytes + 2) > Convert.ToInt64("2F6537", 16) Then
                            MsgBox("Patch stoped no more space! " + Environment.NewLine + "Location: " + DefaultLocation)
                            Exit Sub
                        End If

                        'Write New Data
                        WriteToBIN(sggg_patch_bin, NewLocation, NewText, NewBytes)
                        'Write New Pointers
                        For Each Pointer In Pointers
                            Dim NewLocationDC As String = flip(POStoDC(NewLocation))
                            WritePointerToBIN(sggg_patch_bin, Pointer, NewLocationDC)
                        Next
                        'update NewLocation
                        NewLocation = HexADD(NewLocation, (NewBytes + 2).ToString("X8"))
                        ' MsgBox(NewLocation)
                    Else
                        MsgBox("No Pointer for: " + Environment.NewLine + Environment.NewLine + "DefaultLocation: " + DefaultLocation + Environment.NewLine + "Text: " + NewText)
                    End If
                Else
                    'Normal Write
                    WriteToBIN(sggg_patch_bin, DefaultLocation, NewText, Bytes)
            End If


            End If
        Next
        MsgBox("Patching Done!")


    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim SelectedItems As ListView.SelectedIndexCollection = ListView1.SelectedIndices
        If SelectedItems.Count > 0 Then
            Dim target As String = flip(POStoDC(ListView1.Items(SelectedItems(0)).SubItems(0).Text))
            Dim positions As List(Of String) = SearchHexInFile(sggg_bin, target)
            If positions.Count > 0 Then
                TextBox3.Text = ""
                For Each pos In positions
                    TextBox3.Text += pos + "|"
                Next
                TextBox3.Text = Mid(TextBox3.Text, 1, TextBox3.Text.Length - 1)
            Else
                MsgBox("No pointers found.")
            End If
        End If

    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        If TextBox3Boolen = False Then
            Try
                Dim SelectedItems As ListView.SelectedIndexCollection = ListView1.SelectedIndices
                If SelectedItems.Count > 0 Then
                    ListView1.BeginUpdate()
                    ListView1.Items(SelectedItems(0)).SubItems(1).Text = TextBox3.Text
                    ListView1.EndUpdate()
                End If
            Catch ex As Exception
            End Try
        Else
            TextBox3Boolen = False
        End If



    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        'If the Then New text length Is larger, change the BackColor To red.
        'This indicates that the text needs to be relocated because it doesn't fit in the original location.
        'To achieve that, we need to adjust the original pointers.
        Try
            Dim SelectedItems As ListView.SelectedIndexCollection = ListView1.SelectedIndices
            If SelectedItems.Count > 0 Then
                'ListView1.BeginUpdate()
                ListView1.Items(SelectedItems(0)).SubItems(4).Text = TextBox5.Text
                'ListView1.EndUpdate()

                If TextBox5.Text.Length > ListView1.Items(SelectedItems(0)).SubItems(2).Text Then
                    TextBox2.BackColor = Color.Red
                Else
                    TextBox2.BackColor = Control.DefaultBackColor
                End If
            End If
        Catch ex As Exception
        End Try


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SaveData()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        PatchBin()
    End Sub

    Private Sub ListView1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListView1.MouseDoubleClick
        Button1.PerformClick()
    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged

        For i = 0 To ListView1.Items.Count - 1
            Dim target As String = ListView1.Items.Item(i).SubItems(0).Text.ToString
            If target = TextBox6.Text.ToLower Then
                ListView1.Items(i).Selected = True
                ListView1.Select()
                ListView1.EnsureVisible(i)
            End If
            'ListView1.FocusedItem.Index
        Next

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form2.Show()
    End Sub

    Private Sub ListView1_RetrieveVirtualItem(sender As Object, e As RetrieveVirtualItemEventArgs) Handles ListView1.RetrieveVirtualItem
        If Not (myCache Is Nothing) AndAlso e.ItemIndex >= firstItem AndAlso e.ItemIndex < firstItem + myCache.Length Then
            e.Item = myCache((e.ItemIndex - firstItem))
        Else
            Dim x As Integer = e.ItemIndex
            e.Item = New ListViewItem(x.ToString())
        End If
    End Sub

    Private Sub TextBox5_Leave(sender As Object, e As EventArgs) Handles TextBox5.Leave
        ListView1.BeginUpdate()
        ListView1.EndUpdate()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        ListView1_SelectedIndexChanged(Nothing, Nothing)
    End Sub
End Class
