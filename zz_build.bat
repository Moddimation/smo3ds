@echo off
echo Super Mario Odyssey 3DS demake, Scripts by Moddimation
echo ACTION: BUILD GAME
echo =------------------------------------------------------------=
echo Note: Output is printed once building has finished. Check LOG.txt

REM Start the file watcher in a separate CMD window
start /B cmd /C ":loop && if exist LOG.txt (copy /Y LOG.txt TEMP_LOG.txt) && timeout /t 1 >nul && goto loop"

"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -projectPath="C:\Users\user\Documents\smo3ds" -executeMethod "BuildCommand.PerformBuild" -buildTarget "N3DS" -logFile LOG.txt

REM Stop the CMD watcher when Unity finishes
taskkill /IM cmd.exe /F

powershell -Command "Get-Content TEMP_LOG.txt -Tail 1 -Wait"
