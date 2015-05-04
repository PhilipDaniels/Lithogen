@echo off
REM Simple batch file to call Lithogen on boilerplate, helps with developing the --serve mode.
Lithogen\bin\Debug\Lithogen.exe -c --log=verbose --settings=Lithogen.Boilerplate\bin\Lithogen.xml --serve --watch

