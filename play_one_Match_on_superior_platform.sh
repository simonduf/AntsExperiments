toolsPath=$PWD/tools

bot1Path="mono $PWD/Bot1/bin/x64/Debug/Bot1.exe"
bot2Path="python $toolsPath/sample_bots/python/LeftyBot.py"
bot3Path="mono $PWD/Bot_pl/bin/Debug/Bot_pl.exe"
bot4Path="mono $PWD/Bot_JdB/bin/Debug/Bot_JdB.exe"
Map=$toolsPath/maps/maze/maze_p04_11.map

echo $bot1Path
echo $Map

python "$toolsPath/playgame.py" $preViz --engine_seed 42 --player_seed 42 --end_wait=11.25 --verbose --log_dir game_logs --turns 100 --map_file $Map "$bot1Path" "$bot2Path" "$bot3Path" "$bot4Path" $postViz
