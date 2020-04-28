@echo off

setlocal
set toolsPath=%~dp0..\tools

set bot1Path="""%~dp0Contenders\Bot_SD\Bot_SD1.exe"""
set bot2Path="""%~dp0Contenders\Bot_JdB\Bot_JdB.exe"""
set bot3Path="""%~dp0Contenders\Bot_pl\Bot_pl.exe"""
set Map="%toolsPath%\maps\maze\maze_p03_01.map"


if not exist %bot1Path% (echo %bot1Path% does not exist)
if not exist %bot2Path% (echo %bot2Path% does not exist)
if not exist %bot3Path% (echo %bot3Path% does not exist)

::set visualise=Yes!
set visualise=

IF DEFINED visualise (
echo Visualize
SET preViz=-So
SET postViz= | java -jar "%toolsPath%\visualizer.jar"
) else (
echo Do Not viz
SET preViz=
SET postViz=
)

python "%toolsPath%\playgame.py" %preViz% --engine_seed 42 --player_seed 42 --end_wait=11.25 --verbose --log_dir game_logs --turns 1000 --map_file %Map% %bot1Path% %bot2Path% %bot3Path% %postViz%


endlocal