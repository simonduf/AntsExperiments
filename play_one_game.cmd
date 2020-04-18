@echo off
cd tools

if exist C:\myprogram\sync\data.handler echo Now Exiting && Exit

python "%~dp0tools\playgame.py" --engine_seed 42 --player_seed 42 --end_wait=1.25 --verbose --log_dir game_logs --turns 100 --map_file "%~dp0tools\maps\maze\maze_p04_11.map" %* """%~dp0tools\..\Bot1\bin\Debug\ConsoleApplication.exe""" "python ""%~dp0tools\sample_bots\python\LeftyBot.py""" "python ""%~dp0tools\sample_bots\python\HunterBot.py""" "python ""%~dp0tools\sample_bots\python\RandomBot.py"""

