Option Strict On
Option Infer On
Imports VbPixelGameEngine
Imports System.MathF

Public Class MultAddGame
    Inherits PixelGameEngine

    Public Sub New()
        AppName = "Mult-Add Game"
    End Sub

    Friend NotInheritable Class MultAddQuest
        ReadOnly a As Integer, b As Integer, c As Integer, sign As Char
        Friend Property Result As Single
        Friend Property HasAddend As Boolean

        Public Sub New(Optional hasAddend As Boolean = False)
            Dim rnd As New Random
            sign = If(rnd.NextDouble() > 0.5, "+"c, "-"c)
            Do
                a = rnd.Next(1, 9)
                b = rnd.Next(1, 9)
                c = rnd.Next(1, 9) * If(sign = "+"c, 1, -1)
                Result = If(hasAddend, FusedMultiplyAdd(a, b, c), a * b)
            Loop Until Result >= 0
            Me.HasAddend = hasAddend
        End Sub

        Public Overrides Function ToString() As String
            With New Text.StringBuilder($"{a} x {b} =")
                If HasAddend Then .Insert(.Length - 2, $" {sign}{Abs(c)}")
                Return .ToString()
            End With
        End Function
    End Class

    Friend Property UserInput As New Text.StringBuilder
    Private score As Integer = 0
    Private quest As New MultAddQuest
    Private sw As New Stopwatch
    Private prevAnswer As String
    Private prevQuest As String

    Protected Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean
        FillRect(New Vi2d, New Vi2d(ScreenWidth, ScreenHeight), Presets.Black)
        DrawString(New Vi2d, "Press ESC to exit anytime.", Presets.Gray)
        DrawString(New Vi2d(0, 25), $"SCORE: {score}", Presets.Apricot, 2)
        Dim wOffset As Integer = If(Not quest.HasAddend, 150, 200)
        Dim questionPos = New Vi2d(ScreenWidth \ 2 - wOffset, ScreenHeight \ 2 - 20)
        DrawString(questionPos + New Vi2d(1, 1), quest.ToString(), Presets.Snow, 3)
        DrawString(questionPos, quest.ToString(), Presets.Mint, 3)
        Dim inputPos = New Vi2d(ScreenWidth \ 2 + 50, ScreenHeight \ 2 - 20)
        DrawString(inputPos + New Vi2d(1, 1), UserInput.ToString(), Presets.Snow, 3)
        DrawString(inputPos, UserInput.ToString(), Presets.Mint, 3)
        DrawString(New Vi2d(80, 100), "Previous Answer: " & prevAnswer, Presets.Cyan, 2)

        Dim ResetInputAndTimer = Sub()
                                     UserInput.Clear()
                                     sw.Stop()
                                     sw.Reset()
                                 End Sub

        For k As Key = Key.K0 To Key.K9 Step CType(1, Key)
            If GetKey(k).Pressed Then UserInput.Append(CStr(k - 27))
        Next k
        If GetKey(Key.BACK).Pressed Then ResetInputAndTimer()

        Dim answer As New Single
        If Single.TryParse(UserInput.ToString(), answer) Then
            sw.Start()
            If answer = quest.Result Then
                prevAnswer = quest.Result.ToString()
                prevQuest = quest.ToString()
                Do
                    quest = New MultAddQuest(Rnd > 0.3)
                Loop Until quest.ToString() <> prevQuest
                score += 1
                ResetInputAndTimer()
            ElseIf sw.ElapsedMilliseconds > 1500 Then
                prevAnswer = String.Empty
                ResetInputAndTimer()
            End If
        End If

        Return Not GetKey(Key.ESCAPE).Pressed
    End Function

    Friend Shared Sub Main()
        With New MultAddGame
            If .Construct(500, 300, True) Then .Start()
        End With
    End Sub
End Class