@echo off
REM Simple batch file to call Lithogen on Gitcheatsheet, helps with developing the --serve mode.
bin\Debug\Lithogen.exe -c --log=verbose --settings=..\Gitcheatsheet\bin\Debug\Lithogen.xml --serve --watch
