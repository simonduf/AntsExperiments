::@echo off

setlocal
set toolsPath=%~dp0..\tools

set bot1Path="""%~dp0Contenders\Bot_SD\Bot_SD.exe"""
set bot2Path="""%~dp0Contenders\Bot_JdB\Bot_JdB.exe"""
set Map="%toolsPath%\maps\maze\maze_p02_01.map"


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

python "%toolsPath%\playgame.py" %preViz% --engine_seed 42 --player_seed 42 --end_wait=11.25 --verbose --log_dir game_logs --turns 100 --map_file %Map% %bot1Path% %bot2Path% %postViz%


endlocal