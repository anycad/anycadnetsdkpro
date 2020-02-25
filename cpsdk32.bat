
set BIN=..\anycore.sdk\bin\v141\x86
set OUT=.\bin\x86

for /f "delims=," %%i in (copylist.txt)  do (
	xcopy %BIN%\Release\%%i %OUT%\release\ /y
	xcopy %BIN%\Release\%%i %OUT%\debug\ /y
echo %%i)