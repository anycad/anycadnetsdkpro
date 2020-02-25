
set BIN=..\anycore.sdk\bin\v141\x64

set OUT=.\bin\x64

for /f "delims=," %%i in (copylist.txt)  do (
	xcopy %BIN%\Release\%%i %OUT%\release\ /y
	xcopy %BIN%\Release\%%i %OUT%\debug\ /y
echo %%i)