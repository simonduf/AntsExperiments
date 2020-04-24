::@echo off

setlocal
set toolsPath=%~dp0tools

set bot1Path="""%~dp0Bot1\bin\x64\Debug\Bot1.exe"""
set bot2Path="python ""%toolsPath%\sample_bots\python\LeftyBot.py"""
set bot3Path="python ""%toolsPath%\sample_bots\python\HunterBot.py"""
set bot4Path="python ""%toolsPath%\sample_bots\python\RandomBot.py"""
set Map="%toolsPath%\maps\maze\maze_p04_11.map"


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

python "%toolsPath%\playgame.py" %preViz% --engine_seed 42 --player_seed 42 --end_wait=11.25 --verbose --log_dir game_logs --turns 1000 --map_file %Map% %bot1Path% %bot2Path% %bot3Path% %bot4Path% %postViz%


endlocal